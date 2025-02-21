using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EFLibrary.Models;
using LibLibrary.Services;

namespace AdminMPA.Pages.Admin
{
    public class EditUserModel : PageModel
    {
        public User? user { get; set; }
        public IActionResult OnGet(string email)
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                if (string.IsNullOrEmpty(email))
                {
                    // Optionally, handle the case when no email is provided.
                    return RedirectToPage("../Index");
                }

                user = LibUser.GetUserByEmail(email);
                if (user == null)
                {
                    // User not found, you might want to redirect or show an error.
                    return RedirectToPage("../Index");
                }

                return Page();
            }
            return RedirectToPage("../Index");
        }


        public IActionResult OnPostSuspendUser(string email)
        {
            if (HttpContext.Session.GetString("User") != null && !string.IsNullOrEmpty(email))
            {
                var user = LibUser.GetUserByEmail(email);
                if (user != null)
                {
                    LibUser.SuspendUser(user.Email);
                    return RedirectToPage(new { email = user.Email });
                }
            }
            return RedirectToPage("../Index");
        }

        public IActionResult OnPostReactivateUser(string email)
        {
            if (HttpContext.Session.GetString("User") != null && !string.IsNullOrEmpty(email))
            {
                var user = LibUser.GetUserByEmail(email);
                if (user != null)
                {
                    LibUser.ReactivateUser(user.Email);
                    return RedirectToPage(new { email = user.Email });
                }
            }
            return RedirectToPage("../Index");
        }

    }
}
