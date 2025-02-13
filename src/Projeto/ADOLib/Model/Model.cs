using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOLib.Model
{
    public class Model
    {

        public class Author
        {
            public int AuthorId { get; set; }
            public string AuthorName { get; set; }
        }

        public class Book
        {
            public int BookId { get; set; }
            public string Title { get; set; }
            public string Edition { get; set; }
            public int Year { get; set; }
            public int Quantity { get; set; }
            public Author Author { get; set; }


            public virtual Cover Cover { get; set; }
            public virtual ICollection<Subject> Subjects { get; set; }
            public virtual ICollection<Copie> Copies { get; set; }
        }

        public class Copie
        {
            public int BookId { get; set; }
            public Book Book { get; set; }

            public int LibraryId { get; set; }
            public Library Library { get; set; }

            public int NumberOfCopies { get; set; }
        }

        public class Cover
        {
            public int CoverId { get; set; }
            public byte[] CoverImage { get; set; }

            public virtual Book Book { get; set; }
        }

    }
}
