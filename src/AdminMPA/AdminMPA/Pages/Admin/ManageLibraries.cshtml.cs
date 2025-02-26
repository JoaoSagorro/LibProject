using EFLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminMPA.Pages.Admin
{
    public class ManageLibrariesModel : PageModel
    {
        public List<Library> Libraries { get; set; } = [];

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                return Page();
            }
            return RedirectToPage("../Index");
        }
    }
}
