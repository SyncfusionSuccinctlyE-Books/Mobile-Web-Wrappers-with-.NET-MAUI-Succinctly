using Maui.Biometric;
using MobileWrapper.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MobileWrapper.Scripts.AndroidScripts;
using static MobileWrapper.Scripts.IOSScripts;

namespace MobileWrapper.Model
{
    public static class ExtensionMethods
    {
        public static async Task 
            InjectCredentialsManagementToLoginPageAsync(
            this CustomWebView webView)
        {
#if ANDROID
            MainThread.BeginInvokeOnMainThread(async () => 
            { await webView.
            EvaluateJavaScriptAsync(AndroidGetCredentialsScript); });

#elif IOS
            await webView.EvaluateJavaScriptAsync(iOSGetCredentialsScript);
#endif
        }

        public async static Task 
            TryAutoLoginWithBiometricsAsync(
            this CustomWebView webView)
        {
            var biometricEnabled = 
                await SecureStorage.Default.GetAsync("biometric_enabled");
            if (biometricEnabled != "true")
                return; // Biometric login not enabled

            var authResult = await BiometricAuthentication.Current.
                AuthenticateAsync(new AuthenticationRequest(
                    "Login with biometrics", "Biometric login"));

            if (!authResult.IsSuccessful)
                return; // User canceled or failed biometric auth

            var email = await SecureStorage.Default.GetAsync("username");
            var password = await SecureStorage.Default.GetAsync("pwd");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return; // No credentials saved

            // Inject JS to fill and submit form
            string script = $@"
(function () {{
  function getElementByXPath(xpath) {{
    return document.evaluate(xpath, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
  }}
  const interval = setInterval(() => {{
    const emailInput = getElementByXPath('//input[@id=""Input_Email""]');
    const passwordInput = getElementByXPath('//input[@id=""Input_Password""]');
    const loginButton = getElementByXPath('//button[@id=""login-submit""]');

    if (emailInput && passwordInput && loginButton) {{
      clearInterval(interval);
      emailInput.value = '{email}';
      passwordInput.value = '{password}';
      loginButton.click();
    }}
  }}, 500);
}})();";

            await webView.EvaluateJavaScriptAsync(script);
        }

        public async static Task PrintPageAsync(this CustomWebView webView)
        {
#if ANDROID
            await webView.EvaluateJavaScriptAsync(AndroidPrintPageScript);
#elif IOS
            await webView.EvaluateJavaScriptAsync(iOSPrintPageScript);
#endif
        }

        public async static Task InjectTelephoneKeyboardAsync(this CustomWebView webView)
        {
#if ANDROID
            await webView.EvaluateJavaScriptAsync(AndroidReinforceTelephoneScript);
#elif IOS
            await webView.EvaluateJavaScriptAsync(iOSReinforceTelephoneScript);
#endif
        }

        public async static Task InjectLocationScriptAsync(this CustomWebView webView)
        {
#if ANDROID
            await webView.EvaluateJavaScriptAsync(AndroidGetLocationScript);
#elif IOS
            await webView.EvaluateJavaScriptAsync(iOSGetLocationScript);
#endif
        }

        public static async Task HandlePdfAsync(this CustomWebView webView)
        {
#if ANDROID
            MainThread.BeginInvokeOnMainThread(async () => { 
                await webView.EvaluateJavaScriptAsync(AndroidPdfShareScript); });
#elif IOS
            await webView.EvaluateJavaScriptAsync(iOSPdfShareScript);
#endif
        }
    }
}
