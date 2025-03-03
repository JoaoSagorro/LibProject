using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOLib.ModelView
{
    public class BookSearchResult
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int Quantity { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public int BooksBookId { get; set; }
        public byte[] CoverImage { get; set; }
        public List<string> SubjectNames { get; set; }
    }
}
