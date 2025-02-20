using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminMPA.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }
    }
}
