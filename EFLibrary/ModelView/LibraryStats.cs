using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLibrary.ModelView
{
    public class LibraryStats
    {

        public string LibraryName { get; set; }
        public int UsersCount { get; set; }
        public int OrdersCount { get; set; }
        public DateTime OrderDate { get; set; }


    }
}
