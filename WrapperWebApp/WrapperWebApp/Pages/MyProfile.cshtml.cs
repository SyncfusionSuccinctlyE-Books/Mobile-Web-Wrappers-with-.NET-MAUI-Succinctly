using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace WrapperWebApp.Pages
{
    [Authorize]
    public class MyProfileModel : PageModel
    {
        [BindProperty]
        [Required]
        public string FullName { get; set; }

        [BindProperty]
        [DataType(DataType.Date)]
        [Required]
        public DateTime? DateOfBirth { get; set; }

        [BindProperty]
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public void OnGet()
        {
            // Optionally load preexisting user data
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Save data (e.g., to DB or user claims)
            TempData["Message"] = "Profile saved successfully!";
            return RedirectToPage(); // Or stay on the page
        }
    }
}
