using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminMPA.Pages.Admin
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if(HttpContext.Session.GetString("User") != null)
            {
                return Page();
            }
            return RedirectToPage("../Index");

        }
    }
}
