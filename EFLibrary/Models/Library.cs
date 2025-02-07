using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLibrary.Models
{
    public class Library
    {

        public int LibraryId { get; set; }
        public string LibraryName { get; set; }
        public string LibraryAddress { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }

        public virtual ICollection<Copie> Exemplares { get; set; }

    }
}
