using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLibrary.Models
{
    public class Book
    {

        public Book()
        {
            this.Subjects = new HashSet<Subject>();
        }

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
}
