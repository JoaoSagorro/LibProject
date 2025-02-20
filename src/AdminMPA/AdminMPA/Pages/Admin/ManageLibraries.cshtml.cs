using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminMPA.Pages.Admin
{
    public class ManageLibrariesModel : PageModel
    {
        
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
