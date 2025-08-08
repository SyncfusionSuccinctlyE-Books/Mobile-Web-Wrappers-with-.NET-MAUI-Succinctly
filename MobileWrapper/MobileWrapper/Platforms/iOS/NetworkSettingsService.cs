using Foundation;
using MobileWrapper.Services;
using UIKit;

namespace MobileWrapper.Platforms.iOS
{
    public class NetworkSettingsService : INetworkSettingsService
    {
        public void OpenNetworkSettings()
        {
            var wifiUrl = new NSUrl("prefs:root=WIFI");
            var app = UIApplication.SharedApplication;

            if (app.CanOpenUrl(wifiUrl))
            {
                app.OpenUrl(wifiUrl);
            }
            else
            {
                var appPrefsUrl = new NSUrl("App-Prefs:root=WIFI");
                app.OpenUrl(appPrefsUrl);
            }
        }
    }
}
