using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminMPA.Pages.Admin
{
    public class EditBookModel : PageModel
    {
        public int BookId { get; set; }

        public void OnGet(int id)
        {
            BookId = id; // Aqui você pode buscar o livro pelo ID no banco de dados
        }
    }
}
