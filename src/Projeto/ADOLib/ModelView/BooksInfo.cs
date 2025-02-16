using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOLib.ModelView
{
    public class BooksInfo
    {
            public int AuthorId { get; set; }
            public string AuthorName { get; set; }
            public int BookId { get; set; }
            public string Title { get; set; }
            public string Edition { get; set; }
            public int Year { get; set; }
            public int Quantity { get; set; }
            public int LibraryId { get; set; }
            public int NumberOfCopies { get; set; }
            public byte[] CoverImage { get; set; }
            public string LibraryName { get; set; }
            public string LibraryAddress { get; set; }
            public string Email { get; set; }
            public string Contact { get; set; }
            public int SubjectId { get; set; }
            public List<string> SubjectNames { get; set; }

    }
}
