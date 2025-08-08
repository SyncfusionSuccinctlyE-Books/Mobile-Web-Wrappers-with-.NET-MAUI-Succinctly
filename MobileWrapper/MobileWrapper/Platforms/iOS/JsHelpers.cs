using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileWrapper.Platforms.iOS
{
    public class JavascriptData
    {
        public string function { get; set; }
        public string data { get; set; }
    }

    public class JavaScriptActionEventArgs : EventArgs
    {
        public string Payload { get; private set; }

        public JavaScriptActionEventArgs(string payload)
        {
            Payload = payload;
        }

    }
}

