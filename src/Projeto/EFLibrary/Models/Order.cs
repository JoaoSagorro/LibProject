using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLibrary.Models
{
    public class Order
    {

        public int OrderId { get; set; }
        public User User { get; set; }
        public Library Library { get; set; }
        public Book Book { get; set; }
        public State State { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ReturnDate { get; set; }

    }
}
