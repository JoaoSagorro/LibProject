using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary.Models;
using EFLibrary.Utils;
using EFLibrary;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibLibrary
{
    public class UserServices
    {
        //getUsers for readonly purposes
        public static List<User> GetUsers()
        {
            try
            {
                using(var context = new LibraryContext())
                {
                   return  context.Users.AsNoTracking().ToList(); 
                }
            } catch (Exception e){ throw new Exception($"Error getting users", e); } 
        }

        public static User AddUser(string firstName, string lastName, string email, string password, DateTime dataNascimento, string? morada = null, Role? role = null)
        {
            try
            {
                using (var context = new LibraryContext())
                {
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
                    if (!context.Roles.Any(r => r == role) && role != null)
                    {
                        throw new Exception("Role doesn't exist");
                    }
                    else role ??= context.Roles.FirstOrDefault(r => r.RoleName == "User");
                    PasswordHasher<User> pwHasher = new();
                    User leitor = new User { FirstName = firstName, LastName = lastName, Email = email, Address = morada, Role = role, RegisterDate = DateTime.Now, Birthdate = dataNascimento };
                    password = pwHasher.HashPassword(leitor, password);
                    leitor.Password = password;
                    UserUtils.AddUser(leitor);
                    return leitor;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error adding user: {e}");
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
                    var pwHasher = new PasswordHasher<User>();
                    var result = pwHasher.VerifyHashedPassword(user, user.Password, password);
                    return result == PasswordVerificationResult.Success;
                }
            }
            catch(Exception e) {
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
            } catch(Exception e) { throw e; }    
        }

        public static User SuspendUser(string email)
        {
            using (LibraryContext context = new())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    throw new Exception($"User with {email} not found");
                }

                if (user.Suspended)
                {
                    throw new Exception($"User {email} is already suspended");
                }

                try
                {
                    user.Suspended = true;
                    user.Active = false;
                    context.SaveChanges();
                    return user;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error suspending user", ex);
                }
            }
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
            }catch(Exception e) { throw e; };
        }
    }
}
