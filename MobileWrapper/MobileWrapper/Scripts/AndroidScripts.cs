namespace MobileWrapper.Scripts
{
    public static class AndroidScripts
    {
        public static string AndroidGetCredentialsScript = @"(function(){
    function getElementByXPath(xpath){
        return document.evaluate(xpath,document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue;
    }
    const interval=setInterval(()=>{
        const emailInput=getElementByXPath('//input[@id=\""Input_Email\""]');
        const passwordInput=getElementByXPath('//input[@id=\""Input_Password\""]');
        const loginButton=getElementByXPath('//button[@id=\""login-submit\""]');
        if(emailInput&&passwordInput&&loginButton){
            clearInterval(interval);
            loginButton.addEventListener('click',(e)=>{
                jsBridge.retrieveCredentials(emailInput.value,passwordInput.value);
            });
            console.log('Login button handler attached!');
        }else{
            console.log('Waiting for login elements...');
        }
    },500);
})();";


        public static string AndroidPdfShareScript = @"(function() {
  function getElementByXPath(xpath) {
    return document.evaluate(xpath, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
  }

  const interval = setInterval(() => {
    const pdfLink = getElementByXPath('//a[contains(@href, "".pdf"")]');

    if (pdfLink) {
      clearInterval(interval);

      pdfLink.addEventListener('click', function(e) {
        e.preventDefault();

        const pdfUrl = new URL(pdfLink.getAttribute('href'), window.location.origin).href;

        jsBridge.openPdf(pdfUrl);
      });

      console.log('PDF link click interception attached!');
    } else {
      console.log('Waiting for PDF link...');
    }
  }, 500);
})();";

        public static string AndroidPrintPageScript = @"(function() {
  function getElementByXPath(xpath) {
    return document.evaluate(xpath, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;
  }

  const interval = setInterval(() => {
    const printButton = getElementByXPath('//button[@id=""printButton""]');

    if (printButton) {
      clearInterval(interval);

      // Remove the inline onclick handler
      printButton.removeAttribute('onclick');

      // Attach our custom listener
      printButton.addEventListener('click', function(e) {
        e.preventDefault();
        e.stopPropagation(); // Prevent other handlers
        jsBridge.printPrivacy(); // Call your MAUI-native code
      });

      console.log('Print button intercepted and inline handler removed.');
    } else {
      console.log('Waiting for print button...');
    }
  }, 500);
})();";

        public static string AndroidReinforceTelephoneScript = @"var interval = setInterval(() => {
  var phoneInput = document.getElementById('user-phone-number');
  if (phoneInput) {
    clearInterval(interval);
    phoneInput.type = 'tel';
    phoneInput.setAttribute('inputmode', 'tel');
  }
}, 500);
";

        public static string AndroidGetLocationScript = @"
setInterval(() => {
    if (document.readyState === 'complete') {
        navigator.geolocation.getCurrentPosition = function(successCallback, errorCallback) {
            jsBridge.getLocation()
                .then(function(json) {
                    const pos = JSON.parse(json);
                    successCallback({
                        coords: {
                            latitude: pos.latitude,
                            longitude: pos.longitude,
                            accuracy: pos.accuracy
                        },
                        timestamp: Date.now()
                    });
                })
                .catch(function(error) {
                    errorCallback({ code: 1, message: error });
                });
        };
    }
}, 300);";
    }
}
//public static string AndroidGetCredentialsScript = @"(function(){function getElementByXPath(xpath){return document.evaluate(xpath,document,null,XPathResult.FIRST_ORDERED_NODE_TYPE,null).singleNodeValue;}const interval=setInterval(()=>{const emailInput=getElementByXPath('//input[@id=\""Input_Email\""]');const passwordInput=getElementByXPath('//input[@id=\""Input_Password\""]');const loginButton=getElementByXPath('//button[@id=\""login-submit\""]');if(emailInput&&passwordInput&&loginButton){clearInterval(interval);loginButton.addEventListener('click',(e)=>{jsBridge.retrieveCredentials(emailInput.value,passwordInput.value);});console.log('Login button handler attached!');}else{console.log('Waiting for login elements...');}},500);})();";
