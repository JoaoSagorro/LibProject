using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibLibrary.Services;
using EFLibrary.Models;
using EFLibrary.ModelView;

namespace AdminMPA.Pages.Admin
{
    public class StatisticsModel : PageModel
    {
        public List<SubjectStats> MostRequestedSubjects { get; set;}
        public List<LibraryStats> LibsByOrders { get; set; }
        public void OnGet()
        {
            MostRequestedSubjects = LibStatistics.GetMostRequestedSubjects();
            LibsByOrders = LibStatistics.GetLibraryWithLessOrders();
        }
    }
}
