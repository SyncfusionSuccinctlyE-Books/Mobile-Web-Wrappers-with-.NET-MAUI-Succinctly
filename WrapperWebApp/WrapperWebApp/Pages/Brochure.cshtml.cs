using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WrapperWebApp.Pages
{
    [Authorize]
    public class BrochureModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
