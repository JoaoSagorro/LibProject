using EFLibrary.Models;
using EFLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibLibrary.Services
{
    public class LibRole
    {
        public static Role GetRole(string roleName)
        {
            using (LibraryContext context = new())
            {
                try
                {
                    var role = context.Roles.FirstOrDefault(r => r.RoleName == roleName);
                    return role;
                }
                catch (Exception e) { throw new Exception("Error getting role: ", e); }
            }
        }

        public static IEnumerable<Role> GetRoles()
        {
            using (LibraryContext context = new())
            {
                try
                {
                    return context.Roles.Select(r => r).ToList();
                }
                catch (Exception e) { throw new Exception("Error getting roles: ", e); }
            }
        }
    }
}
