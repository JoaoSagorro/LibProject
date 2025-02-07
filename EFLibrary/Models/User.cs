using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLibrary.Models
{
    public class User
    {
        
        public int UserId { get; set; }
        public Role Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Birthdate { get; set; }
        public DateTime RegisterDate { get; set; }
        public bool Suspended { get; set; } = false;
        public bool Active { get; set; } = true;
        public int Strikes { get; set; } = 0;
        public List<Order> Orders { get; set; } = [];

        public static User Copy(User user)
        {
            User copy = new()
            {
                UserId = user.UserId,
                Role = user.Role,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Email = user.Email,
                Password = user.Password,
                Birthdate = user.Birthdate,
                RegisterDate = user.RegisterDate,
                Suspended = user.Suspended,
                Active = user.Active,
                Strikes = user.Strikes,
            };
            return copy;
        }
    }
}
