using Maui.Biometric;
using MobileWrapper.Model;
using MobileWrapper.Services;
using MobileWrapper.ViewModels;
using System.Threading.Tasks;

namespace MobileWrapper
{
    public partial class MainPage : ContentPage
    {
        private string userName;
        private string pwd;
        private MainPageViewModel _viewModel;
        private readonly INetworkSettingsService? 
            _networkSettingsService; 
        private readonly string baseUrl = 
            "https://your-domain-name.azurewebsites.net/";
        public MainPageViewModel ViewModel
        { 
            get => _viewModel; 
            set { _viewModel = value; 
                OnPropertyChanged(); } 
        }

        public MainPage()
        {
            InitializeComponent();
            ViewModel = new MainPageViewModel();

            this.BindingContext = ViewModel;
            ViewModel.Source = 
                new UrlWebViewSource() 
                { Url = $"{baseUrl}Identity/Account/Login" };

            _networkSettingsService = 
                App.Services.GetService<INetworkSettingsService>();
            Connectivity.Current.
                ConnectivityChanged += Current_ConnectivityChanged;
        }

        private void Current_ConnectivityChanged(object? sender, 
            ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess != NetworkAccess.Internet)
                DisableUIForNoInternet();
            else
                EnableUIForInternet();
        }

        // Internet is available: hide the "no network" popup
        private void EnableUIForInternet()
        {
            Dispatcher.Dispatch(() =>
            {
                RootWebView.IsEnabled = true;
                LayoutRoot.IsEnabled = true;
                LayoutRoot.Opacity = 1;
                NoNetworkGrid.IsVisible = false;
            });
        }

        // Internet is not available: show the "no network" popup
        private void DisableUIForNoInternet()
        {
            Dispatcher.Dispatch(() =>
            {
                RootWebView.IsEnabled = false;
                LayoutRoot.IsEnabled = false;
                LayoutRoot.Opacity = 0.3;
                NoNetworkGrid.IsVisible = true;
            });
        }

        private async void RootWebView_TextBoxValuesRetrieved(object sender, 
            Model.TextBoxValuesEventArgs e)
        {
            userName = e.UsernameValue;
            pwd = e.PwdValue;
            if (userName != null && pwd != null)
            {
                await SecureStorage.SetAsync("username", userName);
                await SecureStorage.SetAsync("pwd", pwd);
                await TryEnableBiometricLoginAsync();
            }
        }

        public async Task<bool> IsBiometricLoginEnabledAsync()
        {
            var biometricEnabled = 
                await SecureStorage.Default.GetAsync("biometric_enabled");
            try
            {
                bool result = bool.Parse(biometricEnabled);
                return result;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> TryEnableBiometricLoginAsync()
        {
            var availability = 
                await BiometricAuthentication.Current.
                CheckAvailabilityAsync();

            if (!availability.IsAvailable)
                return false;

            var authResult = 
                await BiometricAuthentication.Current.AuthenticateAsync(
                    new AuthenticationRequest("Enable biometric login",
                    "Use biometrics to log in faster"));

            if (authResult.IsSuccessful)
            {
                await SecureStorage.Default.SetAsync("biometric_enabled", 
                    "true");
                return true;
            }
            else
            {
                await SecureStorage.Default.SetAsync("biometric_enabled", 
                    "false");
                return false;
            }
        }



        private async void RootWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            RootWebView.Opacity = 1; // full page visibility
            SpinnerGrid.IsVisible = false;

            if (e.Url.ToLower().Contains("/identity/account/login"))
            {
                var checkBiometricEnabled = await IsBiometricLoginEnabledAsync();
                if (!checkBiometricEnabled)
                    await RootWebView.InjectCredentialsManagementToLoginPageAsync();
                else
                {
                    await RootWebView.TryAutoLoginWithBiometricsAsync();
                }
            }
            else if (e.Url.ToLower() == "https://wrapperwebapp20250529074211.azurewebsites.net/")
            {
                // Navbar visible
                NavigationBar.IsVisible = true;
            }
            else if (e.Url.ToLower().Contains("/myprofile"))
                await RootWebView.InjectTelephoneKeyboardAsync();
            else if (e.Url.ToLower().Contains("/brochure"))
                await RootWebView.HandlePdfAsync();
            else if (e.Url.ToLower().Contains("privacy"))
                await RootWebView.PrintPageAsync();
            else if (e.Url.ToLower().Contains("/geolocation"))
                await RootWebView.InjectLocationScriptAsync();

        }

        private async void RootWebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            RootWebView.Opacity = 0.3; // Add some transparency while the spinner is shown
            SpinnerGrid.IsVisible = true;
            bool geoPermission = false;
            bool photoPermission = false;
            if (e.Url.ToLower().Contains("/geolocation"))
            {
                geoPermission = await RequestGeolocationPermissionAsync();
                if (!geoPermission) 
                    e.Cancel = true;
            }
        }

        private void OnHomeTapped(object sender, TappedEventArgs e)
        {
            RootWebView.Source = baseUrl;
#if IOS
                           ((WebKit.WKWebView)RootWebView.Handler.PlatformView).LoadRequest(new Foundation.NSUrlRequest(new Foundation.NSUrl(baseUrl)));
#endif
        }

        private void OnBrochureTapped(object sender, TappedEventArgs e)
        {
            string url = $"{baseUrl}brochure";
            RootWebView.Source = url;
#if IOS
                           ((WebKit.WKWebView)RootWebView.Handler.PlatformView).LoadRequest(new Foundation.NSUrlRequest(new Foundation.NSUrl(url)));
#endif
        }

        private void OnMyProfileTapped(object sender, TappedEventArgs e)
        {
            string url = $"{baseUrl}myprofile";
            RootWebView.Source = url;
#if IOS
                           ((WebKit.WKWebView)RootWebView.Handler.PlatformView).LoadRequest(new Foundation.NSUrlRequest(new Foundation.NSUrl(url)));
#endif
        }

        private void OnGeolocationTapped(object sender, TappedEventArgs e)
        {
            string url = $"{baseUrl}geolocation";
#if IOS
                           ((WebKit.WKWebView)RootWebView.Handler.PlatformView).LoadRequest(new Foundation.NSUrlRequest(new Foundation.NSUrl(url)));
#endif
        }

        private async void RootWebView_LocationRetrieved(object sender, 
            LocationEventArgs e)
        {
            string location = $"Latitude: {e.Latitude}, Longitude " +
                $"{e.Longitude}, Accuracy: {e.Accuracy}";
            await DisplayAlert("Location", location, "OK");
        }

        public async Task<bool> RequestGeolocationPermissionAsync()
        {
            var statusWhenInUse = 
                await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (statusWhenInUse != PermissionStatus.Granted)
            {
                statusWhenInUse = 
                    await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            var statusAlways = 
                await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            if (statusAlways != PermissionStatus.Granted)
            {
                statusAlways = 
                    await Permissions.RequestAsync<Permissions.LocationAlways>();
            }

            return statusWhenInUse == 
                PermissionStatus.Granted && 
                statusAlways == PermissionStatus.Granted;
        }

        private void NetworkSettingsButton_Clicked(object sender, EventArgs e)
        {
            _networkSettingsService?.OpenNetworkSettings();
        }

        private async void RootWebView_PdfRequested(object sender, PdfEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.PdfUrl))
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Uri = e.PdfUrl,
                    Title = "Open PDF"
                });
            }
        }
    }
}
