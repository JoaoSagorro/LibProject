using EFLibrary.Models;
using LibLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace AdminMPA.Pages.Admin
{
    public class DeletedUsersModel : PageModel
    {
        public List<User> deletedUsers { get; set; } = [];
        public int Number { get; set; }


        public IActionResult OnGet()
        {
            Number = 0;
            if (HttpContext.Session.GetString("User") != null)
            {
                deletedUsers = JsonSerializer.Deserialize<List<User>>(HttpContext.Session.GetString("DeletedUsers"));
                // apagar da session
                HttpContext.Session.Remove("DeletedUsers");
                return Page();
            }
            return RedirectToPage("../Index");
        }

        
    }
}
