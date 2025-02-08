using EFLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Identity;
using Microsoft.AspNetCore.Identity;

namespace EFLibrary.Utils
{
    public class UserUtils
    {
        //public static (bool success, string message) AddUser(/*LibraryContext context,*/ string firstName, string lastName, string email, string password,  DateTime dataNascimento,string? morada = null, Role? role = null)
        //{
        //    try
        //    {
        //        using (var context = new LibraryContext())
        //        {
        //            //regex to check if email follows "word@word.word" else returns invalid email exception
        //            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        //            {
        //                return (false, "Invalid email format");
        //                //throw new Exception("Invalid email");
        //            }
        //            //check password Length, will do regex checks later
        //            if (password.Length < 8)
        //            {
        //                return (false, "Password should have 8 characters or more");
        //            //throw new Exception("Password too small");
        //            }
        //            //check for valid role or no role passed defaults to User
        //            if (!context.Roles.Any(r => r == role) && role != null)
        //            {
        //                return (false, "Role doesn't exist");
        //                //throw new Exception("Role doesn't exist");
        //            }
        //            else role ??= context.Roles.FirstOrDefault(r => r.RoleName == "User");
        //            PasswordHasher<User> pwHasher = new();
        //            User leitor = new User { FirstName = firstName, LastName = lastName, Email = email, Address = morada, Role = role, RegisterDate = DateTime.Now, Birthdate = dataNascimento };
        //            password = pwHasher.HashPassword(leitor, password);
        //            leitor.Password = password;
        //            context.Users.Add(leitor);
        //            context.SaveChanges();
        //            return (true, "Success creating user");
        //        }
        //    }catch(Exception e) {
        //        return (false, $"Error creating user: {e}");
        //        //throw new Exception($"Error adding user: {e}");
        //    }
        //}

        public static User AddUser( LibraryContext context,User user)
        {
            try
            {
                context.Users.Add(user);
                    context.SaveChanges();
                    return user;
                
            }catch(Exception e)
            {
                throw e;
            }

        }

        public static (bool success,string message) Login(/*LibraryContext context,*/ string email, string password)
        {
            try
            {
                using var context = new LibraryContext();
                var user = context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null) return (false, "Invalid user");//throw new Exception("Invalid user");
                else
                {
                    var pwHasher = new PasswordHasher<User>();
                    var result = pwHasher.VerifyHashedPassword(user, user.Password, password);
                    if (result == PasswordVerificationResult.Success) return (true, "Logged in successfully");
                    return (false, "Incorrect password");
                }
            }
            catch(Exception e) {
                return (false, $"Error logging in: {e}");
                //throw new Exception($"Error logging in: {e}");
            }
        }

        public static User? FindUser(string Email) 
        {
            using var context = new LibraryContext();
            //return context.Users.Include(u => u.Orders).FirstOrDefault(u => u.Email == Email);
            return context.Users.FirstOrDefault(u => u.Email == Email);
        }

        public static User? FindUser(string Email, LibraryContext context) => context.Users.FirstOrDefault(u => u.Email == Email, null);

        //public static (bool success, string message) SuspendUser(string email)
        //{
        //    using (LibraryContext context = new())
        //    {
        //        var user = FindUser(email);
        //        if (user != null)
        //        {
        //            //if already suspended return false
        //            if (user.Suspended) return (false, $"User {user.Email} is already suspended");
        //            user.Suspended = true;
        //            user.Active = false;
        //            context.SaveChanges();
        //            return (true, $"User {user.Email} is suspended");
        //        }
        //        return (false, "Error suspending user: user is null");
        //    }
        //}

        public static (bool success, string message) SuspendUser(string email)
        {
            using (LibraryContext context = new())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == email);

                if (user == null)
                {
                    return (false, $"User with email {email} not found");
                }

                if (user.Suspended)
                {
                    return (false, $"User {user.Email} is already suspended");
                }

                try
                {
                    user.Suspended = true;
                    user.Active = false;
                    context.SaveChanges();
                    return (true, $"User {user.Email} is now suspended");
                }
                catch (Exception ex)
                {
                    return (false, $"Error suspending user: {ex.Message}");
                }
            }
        }

        public static (bool success, string message) ReactivateUser(string email)
        {
            using (LibraryContext context = new())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    if (user.Active && !user.Suspended) return (false, $"User {email} is already active");
                    user.Active = true;
                    user.Suspended = false;
                    context.SaveChanges();
                    return (true, $"User {email} is active now");
                }
                return (false, "Error suspending user: user is null");
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
