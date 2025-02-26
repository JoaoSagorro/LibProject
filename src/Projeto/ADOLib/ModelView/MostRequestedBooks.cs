using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOLib.ModelView
{
    public class MostRequestedBooks
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int OrderId { get; set; }
        public int RequestedCopiesQTY { get; set; }
        public int TotalRequests { get; set; }
    }
}
