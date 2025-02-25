using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibLibrary.Services;
using EFLibrary.Models;
using EFLibrary.ModelView;
using Microsoft.Win32;
using static System.Formats.Asn1.AsnWriter;
using System.Threading;
using System;

namespace AdminMPA.Pages.Admin
{
    public class StatisticsModel : PageModel
    {
        public List<SubjectStats> MostRequestedSubjects { get; set;}
        public int RequestedNumber { get; set; } = 0;
        public List<LibraryStats> LibsByOrders { get; set; }
        public int OrdersNumber { get; set; } = 0;

        public List<LibraryStats> LibsByOldestRequest { get; set; }
        public int OldestNumber { get; set; } = 0;


        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("User") != null)
            {
                MostRequestedSubjects = LibStatistics.GetMostRequestedSubjects();
                LibsByOrders = LibStatistics.GetLibraryWithLessOrders();
                LibsByOldestRequest = LibStatistics.GetLibraryWithOldestRequest();

                return Page();
            }
            return RedirectToPage("../Index");
        }
        
    }
}
