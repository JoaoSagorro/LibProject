using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLibrary.Models
{
    public class OrderHistory
    {
        [Key]
        public int OrderHistoryId { get; set; }
        public string UserName { get; set; }
        public string BookName { get; set; }
        public int BookYear { get; set; }
        public string BookEdition { get; set; }
        public string BookAuthor { get; set; }
        public string LibraryName { get; set; }
        public int OrderedCopies { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ReturnDate { get; set; }

    }
}
