using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileWrapper.ViewModels
{
    public class MainPageViewModel : ObservableObject
    {
        private WebViewSource source;

        public WebViewSource Source
        {
            get => source;
            set
            {
                if (!object.Equals(source, value))
                {
                    source = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
