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
        public DateTime OrderDate { get; set; }
        public DateTime ReturnDate { get; set; }

        // Read-only property to calculate the state dynamically
        public string State
        {
            get
            {
                var today = DateTime.UtcNow;
                var daysUntilReturn = (ReturnDate - today).TotalDays;

                if (daysUntilReturn < 0)
                {
                    return "ATRASO"; // Late
                }
                if (daysUntilReturn == 0)
                {
                    return "Devolução URGENTE"; // Due today
                }
                if (daysUntilReturn <= 3)
                {
                    return "Devolução em breve"; // Due soon
                }
                return "Requested"; // Default state
            }
        }

    }
}
