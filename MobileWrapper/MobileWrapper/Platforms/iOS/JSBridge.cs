using Foundation;
using MobileWrapper.Views;
using Newtonsoft.Json;
using System.Diagnostics;
using UIKit;
using WebKit;
namespace MobileWrapper.Platforms.iOS;

public class JSBridge : NSObject, IWKScriptMessageHandler
{
    private string documentName = string.Empty;
    readonly CustomWebViewHandler _handler;
    WKWebView? webView;

    public JSBridge(CustomWebViewHandler handler)
    {
        _handler = handler;
    }

    public void PrintWebViewContent(string jobName)
    {
        documentName = jobName;
        webView = _handler.PlatformView;
        if (webView == null)
            return;

        var printFormatter = 
            webView.ViewPrintFormatter;
        var printInfo = UIPrintInfo.PrintInfo;
        printInfo.OutputType = 
            UIPrintInfoOutputType.General;
        printInfo.JobName = jobName;

        var printController = 
            UIPrintInteractionController.
            SharedPrintController;
        printController.PrintInfo = printInfo;
        printController.PrintFormatter = printFormatter;
        string script = string.Empty;

        printController.Present(true, (controller, 
            completed, error) =>
        {
            if (!completed && error != null)
            {
                Console.WriteLine($"Error printing: " +
                    $"{error.LocalizedDescription}");
            }
        });
    }

    public void WebViewNavigationBack()
    {
        var webView = _handler.PlatformView;
        if (webView == null)
            return;

        Foundation.NSUrl url = new NSUrl("");

        WKBackForwardList HistoryList = webView.BackForwardList;
        var lastItem = HistoryList.BackList.Count();
        if (lastItem > 0)
        {
            url = HistoryList.BackList[lastItem - 1].Url;
        }
        else
        {
            url = HistoryList.BackList[0].Url;
        }

        webView.LoadRequest(new Foundation.NSUrlRequest(url));

    }
    public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
    {
        try
        {
            var body = message.Body.ToString();

            JavascriptData _data = JsonConvert.DeserializeObject<JavascriptData>(body);

            if (_data.function == "retrieveCredentials")
            {
                var values = _data.data.Split(',');
                ((CustomWebView)_handler.VirtualView)?.OnTextBoxValuesRetrieved(values[0], values[1]);
            }
            else if (_data.function == "openPdf")
                ((CustomWebView)_handler.VirtualView)?.OnPdfRequested(_data.data);
            else if (_data.function == "printPrivacy")
            {
                PrintWebViewContent("Privacy Policy");
            }
            else if(_data.function == "getLocation")
            {
                try
                {
                    MainThread.BeginInvokeOnMainThread(async () => 
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
                        OnLocationRetrieved(location.Latitude, location.Longitude, location.Accuracy);
                    });
                }
                catch (Exception ex)
                {
                    // Handle exceptions here
                }
            }
            else if (_data.function == "invokeCSharpMSNavigationBack")
            {
                WebViewNavigationBack();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}
