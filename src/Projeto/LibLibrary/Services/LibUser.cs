using EFLibrary.Models;
using EFLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LibLibrary.Services
{
    public class LibUser
    {
       public static List<User> GetUsers()
        {
            try
            {
                using (var context = new LibraryContext())
                {
                    return context.Users.Include(u => u.Role).ToList();
                }
            }
            catch (Exception e) { throw new Exception($"Error getting users", e); }
        }

        public static User GetUserByEmail(string email)
        {
            try
            {
                using var context = new LibraryContext();
                return context.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == email);
            }
            catch (Exception e) { throw new Exception("Error getting user from Database", e); }
        }

        //deixar aqui por agora pode ser que seja util no webapi ou assim
        public static User AddUser(string firstName, string lastName, string email, string password, DateTime dataNascimento, string? morada = null, string role = "User")
        {
            try
            {
                using (var context = new LibraryContext())
                {
                    Role roleToAdd;
                    //regex to check if email follows "word@word.word" else returns invalid email exception
                    if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        throw new Exception("Invalid email");
                    }
                    //check password Length, will do regex checks later
                    if (password.Length < 8)
                    {
                        throw new Exception("Password too small");
                    }
                    //check for valid role or no role passed defaults to User
                    if (!context.Roles.Any(r => r.RoleName == role))
                    {
                        throw new Exception("Role doesn't exist");
                    }
                    else roleToAdd = context.Roles.FirstOrDefault(r => r.RoleName == role);
                    User leitor = new User { FirstName = firstName, LastName = lastName, Password = password, Email = email, Address = morada, Role = roleToAdd, RegisterDate = DateTime.Now, Birthdate = dataNascimento };
                    var leitor1 = InsertUser(context, leitor);
                    return leitor1;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error adding user: {e}");
            }
        }
        public static User AddUser(User user)
        {
            try
            {
                using (var context = new LibraryContext())
                {
                    Role roleToAdd;
                    //regex to check if email follows "word@word.word" else returns invalid email exception
                    if (!Regex.IsMatch(user.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        throw new Exception("Invalid email");
                    }
                    //check password Length, will do regex checks later
                    if (user.Password.Length < 8)
                    {
                        throw new Exception("Password too small");
                    }
                    //check for valid role or no role passed defaults to User
                    if (!context.Roles.Any(r => r.RoleName == user.Role.RoleName))
                    {
                        throw new Exception("Role doesn't exist");
                    }
                    else roleToAdd = context.Roles.FirstOrDefault(r => r.RoleName == user.Role.RoleName);
                    user.Role = roleToAdd;
                    var leitor1 = InsertUser(context, user);
                    return leitor1;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error adding user: {e}");
            }
        }

        private static User InsertUser(LibraryContext context, User user)
        {
            try
            {
                context.Add(user);
                context.SaveChanges();
                return context.Users.FirstOrDefault(u => u.Email == user.Email);
            }
            catch (Exception e)
            {
                throw new Exception("Error Adding user to Database", e);
            }
        }

        public static bool Login(string email, string password)
        {
            try
            {
                using var context = new LibraryContext();
                var user = context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null) throw new Exception("Invalid user");
                else
                {
                    return user.Password == password;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error logging in: {e}");
            }
        }

        public static User ReactivateUser(string email)
        {
            try
            {
                using (LibraryContext context = new())
                {
                    var user = context.Users.FirstOrDefault(u => u.Email == email);
                    if (user == null)
                    {
                        throw new Exception("Error suspending user: user not found");
                    }
                    if (user.Active && !user.Suspended) throw new Exception("User already active");
                    user.Active = true;
                    user.Suspended = false;
                    context.SaveChanges();
                    return user;
                }
            }
            catch (Exception e) { throw e; }
        }


        public static User SuspendUser(string email)
        {
            using (LibraryContext context = new())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == email);


                try
                {
                    if (user == null)
                    {
                        throw new Exception($"User with {email} not found");
                    }

                    if (user.Suspended)
                    {
                        throw new Exception($"User {email} is already suspended");
                    }
                    user.Suspended = true;
                    user.Active = false;
                    context.SaveChanges();
                    return user;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Suspending user: {ex}");
                    return user;
                    //throw new Exception($"Error suspending user", ex);
                }
            }
        }

        //private static List<Order> ReturnAllOrders(LibraryContext context,User user)
        //{

        //}
        public static List<User> DeleteInactiveUsers()
        {
            List<User> deletedUsers = [];
            using (LibraryContext context = new())
            {
                var users = GetUsers().Where(u => !UserHasActiveOrders(context, u) && !HasRecentOrders(context, u) && u.Role.RoleId == 3).ToList();
                foreach (var user in users)
                {
                    deletedUsers.Add(DeleteUser(user.Email));
                }
            }
            return deletedUsers;

        }

        public static User DeleteUser(string email)
        {
            try
            {
                using (LibraryContext context = new())
                {
                    var user = context.Users.FirstOrDefault(u => u.Email == email);
                    //var user = context.Users.Include(u => u.Orders).FirstOrDefault(u => u.Email == email);
                    if (user != null)
                    {
                        /*foreach(Order order in user.Orders.ToList())
                        {
                           remove Order function
                        }
                        ou
                        var orders = context.Orders.Where(o => o.userID == user.UserID);
                        foreach(Order order in orders){
                            remove Order function
                        }
                         */
                        context.Users.Remove(user);
                        context.SaveChanges();
                        return user;
                    }
                    throw new Exception($"User with email: {email} not found");
                }
            }
            catch (Exception e) { throw  new Exception("Error deleting user: ",e); };
        }

        public static User UpdateUser(User user)
        {
            try
            {
                using var context = new LibraryContext();
                context.Users.Update(user);
                context.SaveChanges();
                return user;
            }
            catch (Exception e) { throw new Exception("Error updating user", e); }
        }

        private static bool UserHasActiveOrders(LibraryContext context, User user)
        {
                try
                {
                    return context.Orders.Include(o => o.User).Any(o => o.User.UserId == user.UserId && o.ReturnDate == null);
                } catch (Exception e) { throw new Exception("Can't check valid Orders: ", e); }
        }

        private static bool HasRecentOrders(LibraryContext context, User user)
        {
            try
            {
                //return context.Orders.Include(o => o.User).Any(o => o.User.UserId == user.UserId && o.ReturnDate.HasValue && o.ReturnDate.Year - DateTime.Now.Year <1);
                return context.Orders
    .Include(o => o.User)
    .Any(o => o.User.UserId == user.UserId
           && o.ReturnDate.HasValue
           && (o.ReturnDate.Value > DateTime.Now.AddYears(-1) ));
            } catch (Exception e) { throw new Exception("Error Checking recent Orders: ", e); }
        }

        public static void AddStrikeToUser(User user)
        {
            using (LibraryContext context = new())
            {
                user.Strikes++;
                context.Update(user);
                context.SaveChanges();
            }
            if(user.Strikes > 3)
            {
                SuspendUser(user.Email);
            }
        }
    }
}
