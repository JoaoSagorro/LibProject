using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOLib.ModelView
{
    public class BooksWithSubjects
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Edition { get; set; }
        public int Year { get; set; }
        public int Quantity { get; set; }
        public int AuthorId { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }

    }
}
