using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Session;
using System.ComponentModel.DataAnnotations;
using LibLibrary.Services;
using EFLibrary.ModelView;

public class IndexModel : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Email is required.")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public LibraryStats lib { get; set; } = new LibraryStats();

    public IActionResult OnGet()
    {
        if(HttpContext.Session.GetString("User") != null)

        {
            return RedirectToPage("/Admin/Index");
        }
        return Page();
    }

    public IActionResult OnPost()
    {
        if (ModelState.IsValid)
        {
            // Replace this with your actual authentication logic
            if (Email == "admin@xpto.com" && Password == "password")
            {
                //lib = LibStatistics.GetLibraryWithLessOrders()[0];
                // Redirect to a secure page upon successful login
                HttpContext.Session.SetString("User", Email);
                return RedirectToPage("/Admin/Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
        }
        return Page();
    }
}

