using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibLibrary.Services;
using EFLibrary.Models;

namespace AdminMPA.Pages.Admin
{
    public class ManageUsersModel : PageModel
    {
        public List<User> deletedUsers { get; set; } = [];
        public List<User> Users { get; set; } = [];
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                var query = LibUser.GetUsers();
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    Users = query.Where(u => u.Email.ToLower().Contains(SearchTerm.ToLower()) || u.FirstName.ToLower().Contains(SearchTerm.ToLower()) || u.LastName.ToLower().Contains(SearchTerm.ToLower())).ToList();
                    //Users = query.ToList();
                }
                return Page();
            }
            return RedirectToPage("../Index");
        }
        public IActionResult DeleteInactiveUsers()
        {
            deletedUsers = LibUser.DeleteInactiveUsers();
            if(deletedUsers == null || !deletedUsers.Any())
            {
                return NotFound("Nenhum usuario inativo encontrado para deletar.");
            }
            return Page();
        }
    }
}
