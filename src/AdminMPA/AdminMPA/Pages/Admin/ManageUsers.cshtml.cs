using EFLibrary.Models;
using LibLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace AdminMPA.Pages.Admin
{
    public class ManageUsersModel : PageModel
    {
        public List<User> deletedUsers { get; set; } = [];
        public List<User> Users { get; set; } = [];
        public List<User> AllUsers { get; set; } = [];
        public int Number { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        [BindProperty]
        public string Email { get; set; }


        public IActionResult OnGet()
        {
            Number = 0;
            if (HttpContext.Session.GetString("User") != null)
            {
                AllUsers = LibUser.GetUsers();
                if (!string.IsNullOrEmpty(SearchTerm))
                {
                    Users = AllUsers.Where(u => u.Email.ToLower().Contains(SearchTerm.ToLower()) || u.FirstName.ToLower().Contains(SearchTerm.ToLower()) || u.LastName.ToLower().Contains(SearchTerm.ToLower())).ToList();
                    //Users = query.ToList();
                }
                return Page();
            }
            return RedirectToPage("../Index");
        }

        public IActionResult OnPostCheckInactiveUsers()
        {
            deletedUsers = LibUser.DeleteInactiveUsers();
            string jsonDeletedUsers = JsonSerializer.Serialize(deletedUsers);
            HttpContext.Session.SetString("DeletedUsers", jsonDeletedUsers);
            if(deletedUsers == null || !deletedUsers.Any())
            {
                return NotFound("Nenhum usuario inativo encontrado para deletar.");
            }
            return RedirectToPage("/Admin/DeletedUsers");
        }

        public IActionResult OnPostDelete()
        {
            LibUser.DeleteUser(Email);
            AllUsers = LibUser.GetUsers();

            return Page();
        }

        public List<User> TableContent()
        {
            List<User> html = null;
            try
            {
                if (Users is null && deletedUsers is null || !Users.Any() && !deletedUsers.Any())
                {
                    html = AllUsers;
                }
                else if (Users != null && Users.Any())
                {
                    html = Users;
                }
                else if (deletedUsers != null && deletedUsers.Any())
                {
                    html = deletedUsers;
                }
                return html;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
