using EFLibrary;
using EFLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace EFLibrary
//{
//    public class Seed
//    {
//        private static void Users(LibraryContext context)
//        {

//            if (!context.Users.Any())
//            {
//                var role = context.Roles.FirstOrDefault(r => r.RoleName == "User");
//                context.Users.AddRange(new List<User>{
//                    new() {FirstName="Ana",LastName="Santos", Address="Rua 1", Active= true, Birthdate = DateTime.Parse("1990-05-01"),RegisterDate= DateTime.Now, Email = "ana@email.com", Password = "password", Suspended = false, Role = role},
//                    new() {FirstName="Bruno",LastName="Pereira", Address="Rua 2", Active= true, Birthdate = DateTime.Parse("1989-12-25"),RegisterDate= DateTime.Now, Email = "bruno@email.com", Password = "password", Suspended = false,  Role = role},
//                    });
//                context.SaveChanges();
//            }

//        }

//        private static void Subjects(LibraryContext context)
//        {

//            if (!context.Subjects.Any())
//            {
//                context.Subjects.AddRange(new List<Subject>{
//                new() { SubjectName = "Misterio"},
//                new() { SubjectName = "Romance"},
//                new() { SubjectName = "Aventura"},
//                new() { SubjectName = "Sci-fi"}
//                });
//                context.SaveChanges();
//            }
//        }

//        private static void Authors(LibraryContext context)
//        {

//            if (!context.Authors.Any())
//            {
//                context.Authors.AddRange(new List<Author>{
//                new() { AuthorName= "Autor1"},
//                new() { AuthorName = "Autor2"},
//                new() { AuthorName = "Autor3"},
//                new() { AuthorName = "Autor4"},
//                new() { AuthorName = "Autor5"}
//                });
//                context.SaveChanges();
//            }
//        }

//        private static void Roles(LibraryContext context)
//        {

//            if (!context.Roles.Any())
//            {
//                context.Roles.AddRange(new List<Role>{
//                new() {RoleName = "Admin"},
//                new() {RoleName = "Employee"},
//                new() {RoleName = "User"},
//                });
//                context.SaveChanges();
//            }

//        }

//        private static void Books(LibraryContext context)
//        {
//            var assunto = context.Subjects.FirstOrDefault(a => a.SubjectName == "Misterio");
//            if (!context.Books.Any())
//            {
//                var autor = context.Authors.FirstOrDefault(a => a.AuthorId == 1);
//                context.Books.AddRange(new List<Book>
//                {
//                    new() {Year = 1990, Subjects = new List<Subject>{assunto}, Edition = "Primeira", Quantity = 5, Title = "Test Book", Author = autor},
//                    new() {Year = 1991, Subjects = new List<Subject>{assunto}, Edition = "Primeira", Quantity = 5, Title = "Test Book 2 : Electric Boogaloo",Author = autor},
//                    new() {Year = 1992, Subjects = new List<Subject>{assunto}, Edition = "Segunda", Quantity = 5, Title = "Test Book 3",Author = autor},
//                });
//                context.SaveChanges();
//            }
//        }

//        private static void States(LibraryContext context)
//        {
//            if (!context.States.Any())
//            {
//                context.States.AddRange(new List<State>
//        {
//            new() { StateName = "Requested" },
//            new() { StateName = "ATRASO" },
//            new() { StateName = "Devolução URGENTE" },
//            new() { StateName = "Devolução em breve" }
//        });
//                context.SaveChanges();
//            }
//        }

//        private static void Libraries(LibraryContext context)
//        {
//            if (!context.Libraries.Any())
//            {
//                context.Libraries.AddRange(new List<Library>
//                {
//                    new() { Contact= "219823746", Email="lisboa@biblioteca.com", LibraryAddress= "Lisboa",LibraryName = "Lisboa XPTO" },
//                    new() { Contact= "219823744", Email="porto@biblioteca.com", LibraryAddress= "Porto",  LibraryName = "Porto XPTO" },
//                    new() { Contact= "219823743", Email="aveiro@biblioteca.com",LibraryAddress= "Aveiro", LibraryName = "Aveiro XPTO" },
//                    new() { Contact= "219823742", Email="faro@biblioteca.com", LibraryAddress= "Faro",    LibraryName = "Faro XPTO" },
//                });
//                context.SaveChanges();
//            }
//        }

//        public static void Copies(LibraryContext context)
//        {
//            var book = context.Books.FirstOrDefault(b => b.Title == "Test Book");
//            var library1 = context.Libraries.FirstOrDefault(l => l.LibraryName == "Lisboa XPTO");
//            var library2 = context.Libraries.FirstOrDefault(l => l.LibraryName == "Porto XPTO");

//            if (!context.Copies.Any()) 
//            {
//            context.Copies.AddRange(new List<Copie>
//            {
//                new() { Book = book, Library = library1, NumberOfCopies = 5 },
//                new() { Book = book, Library = library2, NumberOfCopies = 5 },
//            }); }

//            context.SaveChanges();
//        }

//        private static void Orders(LibraryContext context)
//        {
//            var user = context.Users.FirstOrDefault(u => u.Email == "ana@email.com");
//            var book = context.Books.FirstOrDefault(b => b.Title == "Test Book");
//            var library = context.Libraries.FirstOrDefault(l => l.LibraryName == "Lisboa XPTO");

//            if (user != null && book != null && library != null)
//            {
//                if (!context.Orders.Any())
//                {
//                    context.Orders.AddRange(new List<Order>
//            {
//                new Order
//                {
//                    User = user,
//                    Book = book,
//                    Library = library,
//                    OrderDate = DateTime.Now,
//                    //State = context.States.FirstOrDefault(s => s.StateName == "Devolução URGENTE"),
//                    ReturnDate = DateTime.Now.AddDays(7)
//                },
//                new Order
//                {
//                    User = user,
//                    Book = book,
//                    Library = library,
//                    OrderDate = DateTime.Now,
//                    //State = context.States.FirstOrDefault(s => s.StateName == "Devolver em breve"),
//                    ReturnDate = DateTime.Now.AddDays(3)
//                }
//            });
//                    context.SaveChanges();
//                }
//            }
//        }

//        public static void SeedAll()
//        {
//            using LibraryContext context = new();
//            try
//            {
//                Roles(context);
//                Authors(context);
//                Users(context);
//                Subjects(context);
//                States(context);
//                Books(context);
//                Libraries(context);
//                Copies(context);
//                Orders(context);
//            }
//            catch (Exception e) { Console.WriteLine($"Error seeding database {e}"); };
//        }
//    }
//}

namespace EFLibrary
{
    public class Seed
    {
        private static void Roles(LibraryContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(new List<Role>{
                new Role { RoleName = "Admin" },
                new Role { RoleName = "Funcionário" },
                new Role { RoleName = "Usuário" }
            });
                context.SaveChanges();
            }
        }

        private static void Users(LibraryContext context)
        {
            if (!context.Users.Any())
            {
                var adminR = context.Roles.FirstOrDefault(r => r.RoleName == "Admin");
                var role = context.Roles.FirstOrDefault(r => r.RoleName == "Usuário");
                context.Users.AddRange(new List<User>{
                new User {
                    FirstName = "Ana",
                    LastName = "Santos",
                    Address = "Rua das Acácias, 100",
                    Active = true,
                    Birthdate = DateTime.Parse("1990-05-01"),
                    RegisterDate = DateTime.Now,
                    Email = "ana.santos@email.com",
                    Password = "password",
                    Suspended = false,
                    Role = role
                },
                new User {
                    FirstName = "Bruno",
                    LastName = "Pereira",
                    Address = "Av. Paulista, 200",
                    Active = true,
                    Birthdate = DateTime.Parse("1989-12-25"),
                    RegisterDate = DateTime.Now,
                    Email = "bruno.pereira@email.com",
                    Password = "password",
                    Suspended = false,
                    Role = role
                },
                new User {
                    FirstName = "Carlos",
                    LastName = "Souza",
                    Address = "Rua do Comércio, 300",
                    Active = true,
                    Birthdate = DateTime.Parse("1985-03-15"),
                    RegisterDate = DateTime.Now,
                    Email = "carlos.souza@example.com",
                    Password = "password",
                    Suspended = false,
                    Role = role
                },
                new User {
                    FirstName = "Mariana",
                    LastName = "Lima",
                    Address = "Rua das Flores, 400",
                    Active = true,
                    Birthdate = DateTime.Parse("1992-07-22"),
                    RegisterDate = DateTime.Now,
                    Email = "mariana.lima@example.com",
                    Password = "password",
                    Suspended = false,
                    Role = role
                },
                new User {
                    FirstName = "Admin",
                    LastName = "Admin",
                    Address = "Rua das Flores, 400",
                    Active = true,
                    Birthdate = DateTime.Parse("1992-07-22"),
                    RegisterDate = DateTime.Now,
                    Email = "admin@xpto.com",
                    Password = "password",
                    Suspended = false,
                    Role = adminR
                }
            });
                context.SaveChanges();
            }
        }

        private static void Subjects(LibraryContext context)
        {
            if (!context.Subjects.Any())
            {
                context.Subjects.AddRange(new List<Subject>{
                new Subject { SubjectName = "Mistério" },
                new Subject { SubjectName = "Romance" },
                new Subject { SubjectName = "Aventura" },
                new Subject { SubjectName = "Ficção Científica" },
                new Subject { SubjectName = "Fantasia" },
                new Subject { SubjectName = "Histórico" }
            });
                context.SaveChanges();
            }
        }

        private static void Authors(LibraryContext context)
        {
            if (!context.Authors.Any())
            {
                context.Authors.AddRange(new List<Author>{
                new Author { AuthorName = "Machado de Assis" },
                new Author { AuthorName = "Clarice Lispector" },
                new Author { AuthorName = "Jorge Amado" },
                new Author { AuthorName = "Paulo Coelho" },
                new Author { AuthorName = "José de Alencar" },
                new Author { AuthorName = "Guimarães Rosa" }
            });
                context.SaveChanges();
            }
        }

        private static void Books(LibraryContext context)
        {
            if (!context.Books.Any())
            {
                // Obter os assuntos
                var romance = context.Subjects.FirstOrDefault(s => s.SubjectName == "Romance");
                var aventura = context.Subjects.FirstOrDefault(s => s.SubjectName == "Aventura");
                var historico = context.Subjects.FirstOrDefault(s => s.SubjectName == "Histórico");

                // Obter autores
                var machado = context.Authors.FirstOrDefault(a => a.AuthorName == "Machado de Assis");
                var clarice = context.Authors.FirstOrDefault(a => a.AuthorName == "Clarice Lispector");
                var jorgeAmado = context.Authors.FirstOrDefault(a => a.AuthorName == "Jorge Amado");
                var pauloCoelho = context.Authors.FirstOrDefault(a => a.AuthorName == "Paulo Coelho");
                var joseAlencar = context.Authors.FirstOrDefault(a => a.AuthorName == "José de Alencar");
                var guimaraesRosa = context.Authors.FirstOrDefault(a => a.AuthorName == "Guimarães Rosa");

                context.Books.AddRange(new List<Book>
            {
                new Book {
                    Title = "Dom Casmurro",
                    Year = 1900,
                    Edition = "1ª Edição",
                    Quantity = 5,
                    Author = machado,
                    Subjects = new List<Subject> { romance }
                },
                new Book {
                    Title = "Memórias Póstumas de Brás Cubas",
                    Year = 1881,
                    Edition = "1ª Edição",
                    Quantity = 4,
                    Author = machado,
                    Subjects = new List<Subject> { romance }
                },
                new Book {
                    Title = "O Alquimista",
                    Year = 1988,
                    Edition = "1ª Edição",
                    Quantity = 10,
                    Author = pauloCoelho,
                    Subjects = new List<Subject> { aventura }
                },
                new Book {
                    Title = "Capitães da Areia",
                    Year = 1937,
                    Edition = "1ª Edição",
                    Quantity = 6,
                    Author = jorgeAmado,
                    Subjects = new List<Subject> { aventura }
                },
                new Book {
                    Title = "Grande Sertão: Veredas",
                    Year = 1956,
                    Edition = "1ª Edição",
                    Quantity = 3,
                    Author = guimaraesRosa,
                    Subjects = new List<Subject> { aventura, romance }
                },
                new Book {
                    Title = "A Hora da Estrela",
                    Year = 1977,
                    Edition = "1ª Edição",
                    Quantity = 7,
                    Author = clarice,
                    Subjects = new List<Subject> { romance }
                },
                new Book {
                    Title = "Iracema",
                    Year = 1865,
                    Edition = "1ª Edição",
                    Quantity = 8,
                    Author = joseAlencar,
                    Subjects = new List<Subject> { historico, romance }
                }
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
                new State { StateName = "Requisitado" },
                new State { StateName = "Em atraso" },
                new State { StateName = "Devolução URGENTE" },
                new State { StateName = "Devolver em breve" },
                new State { StateName = "Devolvido" }
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
                new Library { LibraryName = "Biblioteca Central de Lisboa", LibraryAddress = "Lisboa", Contact = "219823746", Email = "lisboa@biblioteca.com" },
                new Library { LibraryName = "Biblioteca Pública do Porto", LibraryAddress = "Porto", Contact = "219823744", Email = "porto@biblioteca.com" },
                new Library { LibraryName = "Biblioteca Municipal de Aveiro", LibraryAddress = "Aveiro", Contact = "219823743", Email = "aveiro@biblioteca.com" },
                new Library { LibraryName = "Biblioteca Regional de Faro", LibraryAddress = "Faro", Contact = "219823742", Email = "faro@biblioteca.com" },
                new Library { LibraryName = "Biblioteca Universitária de Coimbra", LibraryAddress = "Coimbra", Contact = "219823741", Email = "coimbra@biblioteca.com" }
            });
                context.SaveChanges();
            }
        }

        public static void Copies(LibraryContext context)
        {
            if (!context.Copies.Any())
            {
                // Obter alguns livros
                var domCasmurro = context.Books.FirstOrDefault(b => b.Title == "Dom Casmurro");
                var oAlquimista = context.Books.FirstOrDefault(b => b.Title == "O Alquimista");
                var capitães = context.Books.FirstOrDefault(b => b.Title == "Capitães da Areia");

                // Obter bibliotecas
                var lisboa = context.Libraries.FirstOrDefault(l => l.LibraryName == "Biblioteca Central de Lisboa");
                var porto = context.Libraries.FirstOrDefault(l => l.LibraryName == "Biblioteca Pública do Porto");
                var aveiro = context.Libraries.FirstOrDefault(l => l.LibraryName == "Biblioteca Municipal de Aveiro");

                context.Copies.AddRange(new List<Copie>
            {
                new Copie { Book = domCasmurro, Library = lisboa, NumberOfCopies = 3 },
                new Copie { Book = domCasmurro, Library = porto, NumberOfCopies = 2 },
                new Copie { Book = oAlquimista, Library = lisboa, NumberOfCopies = 5 },
                new Copie { Book = oAlquimista, Library = aveiro, NumberOfCopies = 4 },
                new Copie { Book = capitães, Library = porto, NumberOfCopies = 3 },
                new Copie { Book = capitães, Library = aveiro, NumberOfCopies = 2 }
            });
                context.SaveChanges();
            }
        }

        private static void Orders(LibraryContext context)
        {
            if (!context.Orders.Any())
            {
                var ana = context.Users.FirstOrDefault(u => u.Email.Contains("ana@"));
                var bruno = context.Users.FirstOrDefault(u => u.Email.Contains("bruno@"));
                var carlos = context.Users.FirstOrDefault(u => u.Email.Contains("carlos"));
                var mariana = context.Users.FirstOrDefault(u => u.Email.Contains("mariana"));

                var domCasmurro = context.Books.FirstOrDefault(b => b.Title == "Dom Casmurro");
                var oAlquimista = context.Books.FirstOrDefault(b => b.Title == "O Alquimista");

                var lisboa = context.Libraries.FirstOrDefault(l => l.LibraryName == "Biblioteca Central de Lisboa");
                var porto = context.Libraries.FirstOrDefault(l => l.LibraryName == "Biblioteca Pública do Porto");

                // Obter estados
                var solicitado = context.States.FirstOrDefault(s => s.StateName == "Requisitado");
                var devolUrgente = context.States.FirstOrDefault(s => s.StateName == "Devolução URGENTE");

                context.Orders.AddRange(new List<Order>
            {
                new Order
                {
                    User = ana,
                    Book = domCasmurro,
                    Library = lisboa,
                    OrderDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(7),
                    State = solicitado
                },
                new Order
                {
                    User = bruno,
                    Book = oAlquimista,
                    Library = porto,
                    OrderDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(5),
                    State = devolUrgente
                },
                new Order
                {
                    User = carlos,
                    Book = domCasmurro,
                    Library = lisboa,
                    OrderDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(10),
                    State = solicitado
                },
                new Order
                {
                    User = mariana,
                    Book = oAlquimista,
                    Library = porto,
                    OrderDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(3),
                    State = solicitado
                }
            });
                context.SaveChanges();
            }
        }

        //private static void Orders(LibraryContext context)
        //{
        //    var user = context.Users.FirstOrDefault(u => u.Email.Contains("ana@"));
        //    var book = context.Books.FirstOrDefault(b => b.Title == "Test Book");
        //    var library = context.Libraries.FirstOrDefault(l => l.LibraryName == "Lisboa XPTO");

        //    if (user != null && book != null && library != null)
        //    {
        //        if (!context.Orders.Any())
        //        {
        //            // Buscando estados válidos (assegure-se de que os nomes correspondam exatamente aos inseridos em States)
        //            var stateDevolucaoUrgente = context.States.FirstOrDefault(s => s.StateName == "Devolução URGENTE");
        //            var stateDevolucaoEmBreve = context.States.FirstOrDefault(s => s.StateName == "Devolução em breve");

        //            context.Orders.AddRange(new List<Order>
        //    {
        //        new Order
        //        {
        //            User = user,
        //            Book = book,
        //            Library = library,
        //            OrderDate = DateTime.Now,
        //            State = stateDevolucaoUrgente,
        //            ReturnDate = DateTime.Now.AddDays(7)
        //        },
        //        new Order
        //        {
        //            User = user,
        //            Book = book,
        //            Library = library,
        //            OrderDate = DateTime.Now,
        //            State = stateDevolucaoEmBreve,
        //            ReturnDate = DateTime.Now.AddDays(3)
        //        }
        //    });
        //            context.SaveChanges();
        //        }
        //    }
        //}



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
                Orders(context);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao popular o banco de dados: {e.Message}");
            }
        }
    }
}
