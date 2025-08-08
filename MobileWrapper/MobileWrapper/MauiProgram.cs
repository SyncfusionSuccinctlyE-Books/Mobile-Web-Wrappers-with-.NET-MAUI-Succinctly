using Microsoft.Extensions.Logging;
using MobileWrapper.Services;
using MobileWrapper.Views;

namespace MobileWrapper
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
#if ANDROID
            builder.Services.AddSingleton<INetworkSettingsService, Platforms.Android.NetworkSettingsService>();
#elif IOS
            builder.Services.AddSingleton<INetworkSettingsService, Platforms.iOS.NetworkSettingsService>();
#endif

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                }).ConfigureMauiHandlers(handlers =>
                {
#if ANDROID
                    Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping(
                    "WebViewNavigationCustomization",
                    (handler, view) => {
                        if (view is CustomWebView)
                        {
                            var platformView = handler.PlatformView;
                            platformView.Settings.SetSupportMultipleWindows(false);
                            platformView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
                            platformView.Settings.JavaScriptEnabled = true;
                            platformView.AddJavascriptInterface(
                            new Platforms.Android.JSBridge(handler), "jsBridge");
                            platformView.Settings.SetSupportZoom(true);
                            platformView.Settings.MediaPlaybackRequiresUserGesture = false;
                            platformView.Settings.AllowFileAccessFromFileURLs = true;
                            platformView.Settings.AllowUniversalAccessFromFileURLs = true;
                            platformView.SetWebChromeClient(new Platforms.Android.
                            CustomWebViewClient(Platform.CurrentActivity));
                        }
                    });

#elif IOS
    handlers.AddHandler<CustomWebView, 
    MobileWrapper.Platforms.iOS.CustomWebViewHandler>();

                    Microsoft.Maui.Handlers.WebViewHandler.Mapper.
                    AppendToMapping("WebViewNavigationCustomization", 
                    (handler, view) =>
                    {
                        if (view is CustomWebView)
                        {
                            CustomWebView customWebView = view as CustomWebView;
                            try
                            {
                                handler.PlatformView.NavigationDelegate = 
                                new CustomWebViewNavigationDelegate(customWebView);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error at WebViewHandler: " 
                                + ex.Message);
                            }
                        }
                    });
#endif
                }); ;

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }

#if IOS

    public class CustomWebViewNavigationDelegate : WebKit.WKNavigationDelegate
    {
        private CustomWebView _customWebView;

        public CustomWebViewNavigationDelegate(CustomWebView customWebView)
        {
            _customWebView=customWebView;
        }
        public override async void DidCommitNavigation(WebKit.WKWebView webView, 
        WebKit.WKNavigation navigation)
        {
            WebNavigationEvent navigationEvent = new WebNavigationEvent();
            WebViewSource source = _customWebView.Source;

            string url = webView.Url.ToString().ToLower();
            WebNavigationResult result = new WebNavigationResult();

            _customWebView.OnWebNavigating(navigationEvent, source, url);
        }
        public override async void DidFinishNavigation(WebKit.WKWebView webView, 
        WebKit.WKNavigation navigation)
        {
            WebNavigationEvent navigationEvent = new WebNavigationEvent();
            WebViewSource source = _customWebView.Source;

            string url = webView.Url.ToString().ToLower();
            WebNavigationResult result = new WebNavigationResult();

            _customWebView.OnWebNavigated(navigationEvent, source, url, result);

        }
    }

#endif
}
