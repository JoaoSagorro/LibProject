using EFLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLibrary
{
    public class Seed
    {
        private static void Users(LibraryContext context)
        {

            if (!context.Users.Any())
            {
                var role = context.Roles.FirstOrDefault(r => r.RoleName == "User");
                context.Users.AddRange(new List<User>{
                    new() {FirstName="Ana",LastName="Santos", Address="Rua 1", Active= true, Birthdate = DateTime.Parse("1990-05-01"),RegisterDate= DateTime.Now, Email = "ana@email.com", Password = "password", Suspended = false, Role = role},
                    new() {FirstName="Bruno",LastName="Pereira", Address="Rua 2", Active= true, Birthdate = DateTime.Parse("1989-12-25"),RegisterDate= DateTime.Now, Email = "bruno@email.com", Password = "password", Suspended = false,  Role = role},
                    });
                context.SaveChanges();
            }

        }

        private static void Subjects(LibraryContext context)
        {

            if (!context.Subjects.Any())
            {
                context.Subjects.AddRange(new List<Subject>{
                new() { SubjectName = "Misterio"},
                new() { SubjectName = "Romance"},
                new() { SubjectName = "Aventura"},
                new() { SubjectName = "Sci-fi"}
                });
                context.SaveChanges();
            }
        }

        private static void Authors(LibraryContext context)
        {

            if (!context.Authors.Any())
            {
                context.Authors.AddRange(new List<Author>{
                new() { AuthorName= "Autor1"},
                new() { AuthorName = "Autor2"},
                new() { AuthorName = "Autor3"},
                new() { AuthorName = "Autor4"},
                new() { AuthorName = "Autor5"}
                });
                context.SaveChanges();
            }
        }

        private static void Roles(LibraryContext context)
        {

            if (!context.Roles.Any())
            {
                context.Roles.AddRange(new List<Role>{
                new() {RoleName = "Admin"},
                new() {RoleName = "Employee"},
                new() {RoleName = "User"},
                });
                context.SaveChanges();
            }

        }

        private static void Books(LibraryContext context)
        {
            var assunto = context.Subjects.FirstOrDefault(a => a.SubjectName == "Misterio");
            if (!context.Books.Any())
            {
                var autor = context.Authors.FirstOrDefault(a => a.AuthorId == 1);
                context.Books.AddRange(new List<Book>
                {
                    new() {Year = 1990, Subjects = new List<Subject>{assunto}, Edition = "Primeira", Quantity = 5, Title = "Test Book", Author = autor},
                    new() {Year = 1991, Subjects = new List<Subject>{assunto}, Edition = "Primeira", Quantity = 5, Title = "Test Book 2 : Electric Boogaloo",Author = autor},
                    new() {Year = 1992, Subjects = new List<Subject>{assunto}, Edition = "Segunda", Quantity = 5, Title = "Test Book 3",Author = autor},
                });
                context.SaveChanges();
            }
        }

        private static void States(LibraryContext context)
        {
            if (!context.States.Any())
            {
                context.States.AddRange(new List<State>
                {
                    new() { StateName = "ATRASO"},
                    new() { StateName = "Devolução URGENTE"},
                    new() { StateName = "Devolver em breve"},
                });
                context.SaveChanges();
            }
        }

        private static void Libraries(LibraryContext context)
        {
            if (!context.Libraries.Any())
            {
                context.Libraries.AddRange(new List<Library>
                {
                    new() { Contact= "219823746", Email="lisboa@biblioteca.com", LibraryAddress= "Lisboa",LibraryName = "Lisboa XPTO" },
                    new() { Contact= "219823744", Email="porto@biblioteca.com", LibraryAddress= "Porto",  LibraryName = "Porto XPTO" },
                    new() { Contact= "219823743", Email="aveiro@biblioteca.com",LibraryAddress= "Aveiro", LibraryName = "Aveiro XPTO" },
                    new() { Contact= "219823742", Email="faro@biblioteca.com", LibraryAddress= "Faro",    LibraryName = "Faro XPTO" },
                });
                context.SaveChanges();
            }
        }

        public static void Copies(LibraryContext context)
        {
            var book = context.Books.FirstOrDefault(b => b.Title == "Test Book");
            var library1 = context.Libraries.FirstOrDefault(l => l.LibraryName == "Lisboa XPTO");
            var library2 = context.Libraries.FirstOrDefault(l => l.LibraryName == "Porto XPTO");

            context.Copies.AddRange(new List<Copie>
            {
                new() { Book = book, Library = library1, NumberOfCopies = 5 },
                new() { Book = book, Library = library2, NumberOfCopies = 5 },
            });

            context.SaveChanges();
        }

        public static void SeedAll(LibraryContext context)
        {
            try
            {
                Roles(context);
                Authors(context);
                Users(context);
                Subjects(context);
                States(context);
                Books(context);
                Libraries(context);
                Copies(context);
            }
            catch (Exception e) { Console.WriteLine($"Error seeding database {e}"); };
        }
    }
}

