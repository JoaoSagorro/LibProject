using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibLibrary.Services;
using EFLibrary.Models;
using static System.Formats.Asn1.AsnWriter;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;
using Azure;
using System;

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

        public IActionResult DeleteInactiveUsers()
        {
            deletedUsers = LibUser.DeleteInactiveUsers();
            if(deletedUsers == null || !deletedUsers.Any())
            {
                return NotFound("Nenhum usuario inativo encontrado para deletar.");
            }
            return Page();
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
                if (Users is null || !Users.Any())
                {
                    html = AllUsers;
                }
                else if (Users != null && Users.Any())
                {
                    html = Users;
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
