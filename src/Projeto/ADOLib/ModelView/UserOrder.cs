using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOLib.ModelView
{
    public class UserOrder
    {
        public int OrderId { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string LibraryName { get; set; }
        public int RequestedCopiesQTY { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string StateName { get; set; }
    }
}
