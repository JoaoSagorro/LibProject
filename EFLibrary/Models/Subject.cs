using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLibrary.Models
{
    public class Subject
    {

        public Subject()
        {
            this.Books = new HashSet<Book>();
        }

        [Key]
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }

        public virtual ICollection<Book> Books { get; set; }

    }
}
