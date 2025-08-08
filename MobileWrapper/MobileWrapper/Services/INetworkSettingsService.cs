using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileWrapper.Services
{
    public interface INetworkSettingsService
    {
        /// <summary>
        /// Open the platform-specific OS settings page
        /// to restore Internet connection
        /// </summary>
        void OpenNetworkSettings();
    }
}
