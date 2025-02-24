using AdminMPA.RazorModels;
using EFLibrary.Models;
using LibLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminMPA.Pages.Admin
{
    public class CreateUserModel : PageModel
    {
            [BindProperty]
            public CreateUserDTO NewUser { get; set; }
            public IEnumerable<SelectListItem> roles { get; set; }

            public void OnGet()
            {
                roles = LibRole.GetRoles().Select(r => new SelectListItem { Value = r.RoleName, Text = r.RoleName }).ToList();
            }

            public IActionResult OnPost()
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                NewUser.CreateDate = DateTime.Now;
                User user = new User
                {
                    FirstName = NewUser.FirstName,
                    LastName = NewUser.LastName,
                    Email = NewUser.Email,
                    Password = NewUser.Password,
                    Address = NewUser.Address,
                    Birthdate = NewUser.Birth,
                    RegisterDate = NewUser.CreateDate,
                    Active = true,
                    Role = LibRole.GetRole(NewUser.Role),
                    Strikes = 0,
                    Suspended = false
                };
                LibUser.AddUser(user);
                return RedirectToPage("/ManageUsers");
            }
    }
}
