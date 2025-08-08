using Android.Content;
using Android.Print;
using Android.Webkit;
using Java.Interop;
using Java.IO;
using Microsoft.Maui.Handlers;
using MobileWrapper.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Android.Media.MediaRouter;

namespace MobileWrapper.Platforms.Android
{
    public class JSBridge : Java.Lang.Object
    {
        readonly IWebViewHandler _handler;

        public JSBridge(IWebViewHandler handler)
        {
            _handler = handler;
        }

        [JavascriptInterface]
        [Export("retrieveCredentials")]
        public void RetrieveCredentials(string userValue, string pwdValue)
        {
            MainThread.BeginInvokeOnMainThread(() => 
            {
                ((CustomWebView)_handler.VirtualView)?.OnTextBoxValuesRetrieved(userValue, pwdValue);
            });
        }

        [JavascriptInterface]
        [Export("printPrivacy")]
        public void PrintPrivacy()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                PrintWebViewContent("PrivacyPolicy");
            });
        }


        [JavascriptInterface]
        [Export("openPdf")]
        public void OpenPdf(string pdfUrl)
        {
            ((CustomWebView)_handler.VirtualView)?.OnPdfRequested(pdfUrl);
        }

        [JavascriptInterface] 
        [Export("getLocation")] 
        public void GetLocation()
        {
            MainThread.BeginInvokeOnMainThread(async () => 
            {
                try
                {
                    var status = 
                    await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted)
                        return;
                    var statusAlways = 
                    await Permissions.RequestAsync<Permissions.LocationAlways>();
                    if (status != PermissionStatus.Granted)
                        return;

                    var location = 
                    await Geolocation.GetLastKnownLocationAsync()
                    ?? await Geolocation.GetLocationAsync(
                        new GeolocationRequest(GeolocationAccuracy.Medium));

                    if (location == null)
                        return;

                    ((CustomWebView)_handler.VirtualView)?.
                    OnLocationRetrieved(location.Latitude, 
                    location.Longitude, location.Accuracy);

                }
                catch (Exception ex) 
                {

                }
            });
        }

        public void PrintWebViewContent(string jobName)
        {
            var activity = Platform.CurrentActivity;
            if (activity == null)
                return;

            // Get the current WebView instance
            var currentWebView = _handler.PlatformView;
            if (currentWebView == null)
                return;

            currentWebView.EvaluateJavascript("console.log('print');", 
                new ValueCallback((result) =>
            {
                var printManager = 
                    (PrintManager)activity.
                    GetSystemService(
                        global::Android.Content.Context.PrintService);
                var originalAdapter = 
                currentWebView.CreatePrintDocumentAdapter(jobName);
                var customAdapter = new CustomPrintDocumentAdapter(
                    currentWebView, originalAdapter, jobName);

                printManager.Print(jobName, customAdapter, 
                    new PrintAttributes.Builder().Build());
            }));
        }
    }

    public class ValueCallback : Java.Lang.Object, global::Android.Webkit.IValueCallback
    {
        private readonly Action<string> _callback;

        public ValueCallback(Action<string> callback)
        {
            _callback = callback;
        }

        public void OnReceiveValue(Java.Lang.Object value)
        {
            _callback?.Invoke(value.ToString());
        }
    }
}