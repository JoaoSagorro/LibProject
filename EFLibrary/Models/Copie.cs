using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLibrary.Models
{
    public class Copie
    {
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int LibraryId { get; set; }
        public Library Library { get; set; }

        public int NumberOfCopies { get; set; }

    }
}
