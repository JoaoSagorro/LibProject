using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOLib.DTOs
{
    public class LibraryByNumberOfCopiesDTO
    {
        public int LibraryId { get; set; }
        public string LibraryName { get; set; }
        public int NumberOfCopies { get; set; }
    }
}
