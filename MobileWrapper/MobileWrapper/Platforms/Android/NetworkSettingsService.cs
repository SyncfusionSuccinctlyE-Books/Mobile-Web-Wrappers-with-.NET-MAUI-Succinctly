using Android.Content;
using MobileWrapper.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileWrapper.Platforms.Android
{
    public class NetworkSettingsService : INetworkSettingsService
    {
        public void OpenNetworkSettings()
        {
            var intent = new Intent(global::Android.Provider.
                Settings.ActionWirelessSettings);
            intent.SetFlags(ActivityFlags.NewTask);
            global::Android.App.Application.
                Context.StartActivity(intent);
        }
    }
}
