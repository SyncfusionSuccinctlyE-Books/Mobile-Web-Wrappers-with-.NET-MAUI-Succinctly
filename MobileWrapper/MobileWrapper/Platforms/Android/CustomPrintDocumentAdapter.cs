using Android.OS;
using Android.Print;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Print.PrintDocumentAdapter;

namespace MobileWrapper.Platforms.Android
{
    //[Register("com.companyname.mobilewrapper")]
    public class CustomPrintDocumentAdapter : PrintDocumentAdapter
    {
        private global::Android.Webkit.WebView webView;
        private PrintDocumentAdapter originalAdapter;
        private string jobName;

        public CustomPrintDocumentAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public CustomPrintDocumentAdapter(global::Android.Webkit.WebView webView, PrintDocumentAdapter originalAdapter, string documentName)
        {
            this.webView = webView;
            this.originalAdapter = originalAdapter;
            this.jobName = documentName;
        }

        public override void OnLayout(PrintAttributes oldAttributes, PrintAttributes newAttributes, CancellationSignal cancellationSignal, LayoutResultCallback callback, Bundle extras)
        {
            originalAdapter.OnLayout(oldAttributes, newAttributes, cancellationSignal, callback, extras);
        }

        public override void OnWrite(PageRange[] pages, ParcelFileDescriptor destination, CancellationSignal cancellationSignal, WriteResultCallback callback)
        {
            originalAdapter.OnWrite(pages, destination, cancellationSignal, callback);
        }

        public override void OnFinish()
        {
            base.OnFinish();
            originalAdapter.OnFinish();
        }
    }
}
