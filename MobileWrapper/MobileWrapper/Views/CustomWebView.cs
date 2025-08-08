using Maui.Biometric;
using MobileWrapper.Model;

namespace MobileWrapper.Views
{
    public class CustomWebView : WebView
    {
        public event EventHandler<PdfEventArgs> 
            PdfRequested;
        // Event triggered when username and password values 
        // are retrieved from the web page.
        public event EventHandler<TextBoxValuesEventArgs> 
            TextBoxValuesRetrieved;
        public event EventHandler<LocationEventArgs> 
            LocationRetrieved;
        public event EventHandler<WebNavigatedEventArgs> 
            WebNavigated;
        public event EventHandler<WebNavigatingEventArgs> 
            WebNavigating;

        public void OnTextBoxValuesRetrieved(string userValue, string pwdValue)
        {
            TextBoxValuesRetrieved?.Invoke(this, 
                new TextBoxValuesEventArgs(userValue, pwdValue));
        }

        public void OnLocationRetrieved(double latitude, 
            double longitude, double? accuracy)
        {
            LocationRetrieved?.Invoke(this, 
                new LocationEventArgs(latitude, longitude, accuracy));
        }

        public void OnPdfRequested(string pdfUrl)
        {
            PdfRequested?.Invoke(this, new PdfEventArgs(pdfUrl));
        }

        public void OnWebNavigated(WebNavigationEvent navigationEvent, 
            WebViewSource source, string url, WebNavigationResult result)
        {
            WebNavigated?.Invoke(this, 
                new WebNavigatedEventArgs(navigationEvent, 
                source, url, result));
        }
        public void OnWebNavigating(WebNavigationEvent navigationEvent, 
            WebViewSource source, string url)
        {
            WebNavigating?.Invoke(this, 
                new WebNavigatingEventArgs(navigationEvent, 
                source, url));
        }
    }
}
