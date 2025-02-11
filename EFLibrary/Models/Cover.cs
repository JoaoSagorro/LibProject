using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLibrary.Models
{
    public class Cover
    {
        [Key, ForeignKey("Book")]
        // Change CoverId to BookId(?)
        public int CoverId { get; set; }
        public byte[] CoverImage { get; set; }

        public virtual Book Book { get; set; }
    }
}
