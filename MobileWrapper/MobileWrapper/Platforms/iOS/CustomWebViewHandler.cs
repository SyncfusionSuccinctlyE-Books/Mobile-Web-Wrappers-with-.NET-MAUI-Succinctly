using CoreGraphics;
using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using MobileWrapper.Platforms.iOS;
using UIKit;
using WebKit;
namespace MobileWrapper.Platforms.iOS;
public class CustomWebViewHandler : WebViewHandler
{
    const string JavaScriptFunction = "function invokeCSharpAction(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";
    private WKUserContentController userController;
    private JSBridge jsBridgeHandler;
    static SynchronizationContext sync;

    public CustomWebViewHandler()
    {
        sync = SynchronizationContext.Current;
    }

    protected override WKWebView CreatePlatformView()
    {
        sync = sync ?? SynchronizationContext.Current;
        jsBridgeHandler = new JSBridge(this);
        userController = new WKUserContentController();

        var script = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);

        userController.AddUserScript(script);
        userController.AddScriptMessageHandler(jsBridgeHandler, "invokeAction");

        var config = new WKWebViewConfiguration
        {
            UserContentController = userController,
            MediaTypesRequiringUserActionForPlayback = WKAudiovisualMediaTypes.None,
            AllowsInlineMediaPlayback = true,
            
        };
        config.Preferences.JavaScriptEnabled = true;
        var webView = new WKWebView(CGRect.Empty, config);
        return webView;
    }

    protected override void ConnectHandler(WKWebView platformView)
    {
        if (VirtualView.Source != null)
        {
            LoadSource(((WebViewSource)VirtualView.Source), PlatformView);
        }

        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(WKWebView platformView)
    {
        base.DisconnectHandler(platformView);

        userController.RemoveAllUserScripts();
        userController.RemoveScriptMessageHandler("invokeAction");

        jsBridgeHandler?.Dispose();
        jsBridgeHandler = null;
    }
    public static void LoadSource(WebViewSource source, WKWebView control)
    {
        if (source is HtmlWebViewSource html)
        {
            control.LoadHtmlString(html.Html, new NSUrl(html.BaseUrl ?? "http://localhost", true));
        }
        else if (source is UrlWebViewSource url)
        {
            control.LoadRequest(new NSUrlRequest(new NSUrl(url.Url)));
        }
    }
}
