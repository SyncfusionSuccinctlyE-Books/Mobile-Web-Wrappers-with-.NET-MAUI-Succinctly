using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Webkit;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace MobileWrapper.Platforms.Android
{
    public class CustomWebViewClient : WebChromeClient
    {
        private readonly Activity _activity;

        public CustomWebViewClient(Activity activity)
        {
            _activity = activity;
        }

        public override void OnPermissionRequest(PermissionRequest? request)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop && request != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                         request.Grant(request.GetResources());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"OnPermissionRequest Exception: {ex}");
                        request.Deny();
                    }
                });
            }
        }
    }
}
