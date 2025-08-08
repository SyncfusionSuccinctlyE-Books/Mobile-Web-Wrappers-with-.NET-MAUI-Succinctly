using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileWrapper.Model
{
    public class TextBoxValuesEventArgs : EventArgs
    {
        public string UsernameValue { get; }
        public string PwdValue { get; }

        public TextBoxValuesEventArgs(string userValue, string pwdValue)
        {
            UsernameValue = userValue;
            PwdValue = pwdValue;
        }
    }

    public class PdfEventArgs : EventArgs
    {
        public string PdfUrl { get; }
        public PdfEventArgs(string pdfUrl)
        {
            PdfUrl = pdfUrl;
        }
    }

    public class LocationEventArgs: EventArgs
    {
        public double Latitude { get; }
        public double Longitude { get; }
        public double? Accuracy { get; }

        public LocationEventArgs(double latitude, 
            double longitude, double? accuracy)
        {
            Latitude = latitude;
            Longitude = longitude;
            Accuracy = accuracy;
        }
    }
}
