using EFLibrary.Models;
using LibLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AdminMPA.Pages.Admin
{
    public class ManageOrdersModel : PageModel
    {
        public int Number { get; set; }
        public List<Order> Orders { get; set; }

        public IActionResult OnGet()
        {
            Number = 0;
            if (HttpContext.Session.GetString("User") != null)
            {
                LibOrders libOrders = new LibOrders();
                Orders = libOrders.GetAllOrders();
                return Page();
            }
            return RedirectToPage("../Index");
        }
    }
}
