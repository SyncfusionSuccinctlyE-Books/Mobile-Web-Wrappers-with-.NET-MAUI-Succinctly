using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WrapperWebApp.Pages
{
    [Authorize]
    public class GeoLocationModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
