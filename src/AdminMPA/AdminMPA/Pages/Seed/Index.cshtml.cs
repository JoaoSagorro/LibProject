using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminMPA.Pages.Seed
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            EFLibrary.Seed.SeedAll();
        }
    }
}
