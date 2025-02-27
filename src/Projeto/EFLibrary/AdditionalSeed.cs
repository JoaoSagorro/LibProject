using EFLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLibrary
{
    public class AdditionalSeed
    {
        public static void AdditionalBooks(LibraryContext context)
        {
            // Verificar se já existem os livros adicionais (usando um livro específico como marcador)
            if (!context.Books.Any(b => b.Title == "Memorial do Convento"))
            {
                // Obter os assuntos
                var romance = context.Subjects.FirstOrDefault(s => s.SubjectName == "Romance");
                var aventura = context.Subjects.FirstOrDefault(s => s.SubjectName == "Aventura");
                var historico = context.Subjects.FirstOrDefault(s => s.SubjectName == "Histórico");
                var poesia = context.Subjects.FirstOrDefault(s => s.SubjectName == "Poesia");
                var misterio = context.Subjects.FirstOrDefault(s => s.SubjectName == "Mistério");
                var ficcao = context.Subjects.FirstOrDefault(s => s.SubjectName == "Ficção Científica");
                var fantasia = context.Subjects.FirstOrDefault(s => s.SubjectName == "Fantasia");
                var drama = context.Subjects.FirstOrDefault(s => s.SubjectName == "Drama");
                var biografia = context.Subjects.FirstOrDefault(s => s.SubjectName == "Biografia");
                var thriller = context.Subjects.FirstOrDefault(s => s.SubjectName == "Thriller");

                // Obter autores
                var saramago = context.Authors.FirstOrDefault(a => a.AuthorName == "José Saramago");
                var camoes = context.Authors.FirstOrDefault(a => a.AuthorName == "Luís de Camões");
                var miaCouto = context.Authors.FirstOrDefault(a => a.AuthorName == "Mia Couto");
                var clarice = context.Authors.FirstOrDefault(a => a.AuthorName == "Clarice Lispector");
                var jorgeAmado = context.Authors.FirstOrDefault(a => a.AuthorName == "Jorge Amado");
                var eca = context.Authors.FirstOrDefault(a => a.AuthorName == "Eça de Queirós");
                var pessoa = context.Authors.FirstOrDefault(a => a.AuthorName == "Fernando Pessoa");
                var sophiaMB = context.Authors.FirstOrDefault(a => a.AuthorName == "Sophia de Mello Breyner");
                var lygia = context.Authors.FirstOrDefault(a => a.AuthorName == "Lygia Fagundes Telles");
                var cecilia = context.Authors.FirstOrDefault(a => a.AuthorName == "Cecília Meireles");

                // Adicionar 10 novos livros
                var newBooks = new List<Book>
                {
                    new Book {
                        Title = "Memorial do Convento",
                        Year = 1982,
                        Edition = "Edição Crítica",
                        Quantity = 15,
                        Author = saramago,
                        Subjects = new List<Subject> { romance, historico }
                    },
                    new Book {
                        Title = "Terra Sonâmbula",
                        Year = 1992,
                        Edition = "Edição Revista",
                        Quantity = 12,
                        Author = miaCouto,
                        Subjects = new List<Subject> { romance, aventura }
                    },
                    new Book {
                        Title = "A Paixão Segundo G.H.",
                        Year = 1964,
                        Edition = "Edição Comemorativa",
                        Quantity = 10,
                        Author = clarice,
                        Subjects = new List<Subject> { romance, drama }
                    },
                    new Book {
                        Title = "Dona Flor e Seus Dois Maridos",
                        Year = 1966,
                        Edition = "Edição Ilustrada",
                        Quantity = 18,
                        Author = jorgeAmado,
                        Subjects = new List<Subject> { romance, fantasia }
                    },
                    new Book {
                        Title = "Os Maias",
                        Year = 1888,
                        Edition = "Edição Anotada",
                        Quantity = 20,
                        Author = eca,
                        Subjects = new List<Subject> { romance, drama, historico }
                    },
                    new Book {
                        Title = "Mensagem",
                        Year = 1934,
                        Edition = "Edição Especial",
                        Quantity = 25,
                        Author = pessoa,
                        Subjects = new List<Subject> { poesia, historico }
                    },
                    new Book {
                        Title = "O Conto da Ilha Desconhecida",
                        Year = 1997,
                        Edition = "1ª Edição",
                        Quantity = 15,
                        Author = saramago,
                        Subjects = new List<Subject> { fantasia, romance }
                    },
                    new Book {
                        Title = "A Noite Escura e Mais Eu",
                        Year = 1995,
                        Edition = "Edição Bolso",
                        Quantity = 12,
                        Author = lygia,
                        Subjects = new List<Subject> { romance, misterio }
                    },
                    new Book {
                        Title = "Romanceiro da Inconfidência",
                        Year = 1953,
                        Edition = "Edição Histórica",
                        Quantity = 10,
                        Author = cecilia,
                        Subjects = new List<Subject> { poesia, historico }
                    },
                    new Book {
                        Title = "O Lagarto",
                        Year = 1977,
                        Edition = "Edição Ilustrada",
                        Quantity = 14,
                        Author = sophiaMB,
                        Subjects = new List<Subject> { aventura, fantasia }
                    }
                };

                context.Books.AddRange(newBooks);
                context.SaveChanges();

                // Após adicionar os livros, distribuir cópias pelas bibliotecas
                DistributeBookCopies(context, newBooks);

                Console.WriteLine("10 novos livros adicionados com sucesso.");
            }
            else
            {
                Console.WriteLine("Livros adicionais já existem no banco de dados.");
            }
        }

        private static void DistributeBookCopies(LibraryContext context, List<Book> books)
        {
            // Obter todas as bibliotecas
            var libraries = context.Libraries.ToList();
            var random = new Random();

            // Lista para armazenar as novas cópias
            var copies = new List<Copie>();

            // Distribuir cópias de cada livro pelas bibliotecas
            foreach (var book in books)
            {
                // Determinar quantas bibliotecas terão este livro (entre 2 e 5)
                int numLibraries = random.Next(2, libraries.Count + 1);

                // Escolher bibliotecas aleatoriamente
                var selectedLibraries = libraries.OrderBy(l => Guid.NewGuid()).Take(numLibraries).ToList();

                // Distribuir as cópias entre as bibliotecas selecionadas
                int remainingCopies = book.Quantity;
                int distributedCopies = 0;

                foreach (var library in selectedLibraries)
                {
                    // A última biblioteca recebe todas as cópias restantes
                    if (library == selectedLibraries.Last())
                    {
                        copies.Add(new Copie
                        {
                            Book = book,
                            Library = library,
                            NumberOfCopies = remainingCopies - distributedCopies
                        });
                    }
                    else
                    {
                        // Calcular número aleatório de cópias para esta biblioteca
                        int copiesForLibrary = random.Next(1, (remainingCopies - distributedCopies) / 2 + 1);
                        copies.Add(new Copie
                        {
                            Book = book,
                            Library = library,
                            NumberOfCopies = copiesForLibrary
                        });
                        distributedCopies += copiesForLibrary;
                    }
                }
            }

            context.Copies.AddRange(copies);
            context.SaveChanges();
        }

        public static void AdditionalUsers(LibraryContext context)
        {
            // Verificar se já existem os usuários adicionais (usando um email específico como marcador)
            if (!context.Users.Any(u => u.Email == "sofia.martins@email.com"))
            {
                var usuarioRole = context.Roles.FirstOrDefault(r => r.RoleName == "Usuário");
                var funcionarioRole = context.Roles.FirstOrDefault(r => r.RoleName == "Funcionário");

                // Data base para cálculo de datas de registro (usuários mais recentes)
                var baseDate = DateTime.Now.AddMonths(-6);
                var random = new Random();

                var newUsers = new List<User>{
                    new User {
                        FirstName = "Sofia",
                        LastName = "Martins",
                        Address = "Rua das Palmeiras, 78, Lisboa",
                        Active = true,
                        Birthdate = DateTime.Parse("1993-10-15"),
                        RegisterDate = baseDate.AddDays(random.Next(30)),
                        Email = "sofia.martins@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "Diogo",
                        LastName = "Silva",
                        Address = "Avenida Central, 45, Porto",
                        Active = true,
                        Birthdate = DateTime.Parse("1987-03-22"),
                        RegisterDate = baseDate.AddDays(random.Next(40)),
                        Email = "diogo.silva@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "Inês",
                        LastName = "Ferreira",
                        Address = "Rua da Liberdade, 120, Coimbra",
                        Active = true,
                        Birthdate = DateTime.Parse("1995-07-12"),
                        RegisterDate = baseDate.AddDays(random.Next(50)),
                        Email = "ines.ferreira@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "Tiago",
                        LastName = "Mendes",
                        Address = "Praça da República, 34, Aveiro",
                        Active = true,
                        Birthdate = DateTime.Parse("1990-11-30"),
                        RegisterDate = baseDate.AddDays(random.Next(60)),
                        Email = "tiago.mendes@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "Marta",
                        LastName = "Ribeiro",
                        Address = "Rua do Comércio, 67, Faro",
                        Active = true,
                        Birthdate = DateTime.Parse("1994-04-18"),
                        RegisterDate = baseDate.AddDays(random.Next(70)),
                        Email = "marta.ribeiro@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "Ricardo",
                        LastName = "Costa",
                        Address = "Avenida dos Aliados, 89, Porto",
                        Active = true,
                        Birthdate = DateTime.Parse("1988-09-05"),
                        RegisterDate = baseDate.AddDays(random.Next(80)),
                        Email = "ricardo.costa@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "Filipa",
                        LastName = "Santos",
                        Address = "Rua Augusta, 111, Lisboa",
                        Active = false, // Um usuário inativo
                        Birthdate = DateTime.Parse("1992-05-25"),
                        RegisterDate = baseDate.AddDays(random.Next(90)),
                        Email = "filipa.santos@email.com",
                        Password = "Password123",
                        Suspended = true, // Suspenso
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "André",
                        LastName = "Oliveira",
                        Address = "Praça do Giraldo, 22, Évora",
                        Active = true,
                        Birthdate = DateTime.Parse("1991-01-13"),
                        RegisterDate = baseDate.AddDays(random.Next(100)),
                        Email = "andre.oliveira@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "Catarina",
                        LastName = "Rodrigues",
                        Address = "Rua da Sé, 45, Braga",
                        Active = true,
                        Birthdate = DateTime.Parse("1989-08-08"),
                        RegisterDate = baseDate.AddDays(random.Next(110)),
                        Email = "catarina.rodrigues@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = funcionarioRole // Um funcionário
                    },
                    new User {
                        FirstName = "Gonçalo",
                        LastName = "Almeida",
                        Address = "Rua dos Clérigos, 76, Viseu",
                        Active = true,
                        Birthdate = DateTime.Parse("1986-12-19"),
                        RegisterDate = baseDate.AddDays(random.Next(120)),
                        Email = "goncalo.almeida@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = usuarioRole
                    }
                };

                context.Users.AddRange(newUsers);
                context.SaveChanges();

                // Após adicionar os usuários, criar pedidos para eles
                CreateOrdersForNewUsers(context, newUsers);

                Console.WriteLine("10 novos usuários adicionados com sucesso.");
            }
            else
            {
                Console.WriteLine("Usuários adicionais já existem no banco de dados.");
            }
        }

        private static void CreateOrdersForNewUsers(LibraryContext context, List<User> users)
        {
            // Obter apenas usuários ativos e que não sejam funcionários
            var activeUsers = users.Where(u => u.Active && u.Role.RoleName == "Usuário").ToList();

            // Obter livros - priorizando os 10 mais recentes adicionados
            var livros = context.Books.OrderByDescending(b => b.BookId).Take(15).ToList();

            // Obter bibliotecas
            var bibliotecas = context.Libraries.ToList();

            // Obter estados
            var atraso = context.States.FirstOrDefault(s => s.StateName == "ATRASO");
            var emprestado = context.States.FirstOrDefault(s => s.StateName == "Requisitado");
            var urgente = context.States.FirstOrDefault(s => s.StateName == "Devolução URGENTE");
            var devolucaoEmBreve = context.States.FirstOrDefault(s => s.StateName == "Devolução em breve");

            // Configuração para datas
            var agora = DateTime.Now;
            var random = new Random();

            // Lista para armazenar os novos pedidos
            var orders = new List<Order>();

            // Criar pedidos para cada usuário (2-3 pedidos por usuário)
            foreach (var usuario in activeUsers)
            {
                int numPedidos = random.Next(2, 4);

                // Lista para controlar quais livros já foram pedidos por este usuário
                var livrosPedidos = new List<int>();

                for (int i = 0; i < numPedidos; i++)
                {
                    // Selecionar livro aleatoriamente (sem repetir para o mesmo usuário)
                    Book livro;
                    do
                    {
                        livro = livros[random.Next(livros.Count)];
                    } while (livrosPedidos.Contains(livro.BookId));

                    livrosPedidos.Add(livro.BookId);

                    // Selecionar biblioteca aleatoriamente
                    var biblioteca = bibliotecas[random.Next(bibliotecas.Count)];

                    // Verificar se existe cópia deste livro na biblioteca
                    var copiasNaBiblioteca = context.Copies
                        .FirstOrDefault(c => c.Book.BookId == livro.BookId && c.Library.LibraryId == biblioteca.LibraryId);

                    if (copiasNaBiblioteca == null || copiasNaBiblioteca.NumberOfCopies <= 0)
                        continue;

                    // Determinar estado do pedido - para novos usuários, focar em pedidos mais recentes
                    State estado;
                    DateTime dataPedido;
                    DateTime? dataRetorno;

                    int estadoRandom = random.Next(100);

                    if (estadoRandom < 35) // 35% solicitados (maior probabilidade de pedidos recentes)
                    {
                        estado = urgente;
                        dataPedido = agora.AddDays(-random.Next(13, 15));
                        dataRetorno = null; // 2 semanas para devolução
                    }
                    else if (estadoRandom < 60) // 25% em processamento
                    {
                        estado = atraso;
                        dataPedido = agora.AddDays(-random.Next(15,20));
                        dataRetorno = null;
                    }
                    else if (estadoRandom < 90) // 30% emprestados
                    {
                        estado = emprestado;
                        dataPedido = agora.AddDays(-random.Next(3, 10));
                        dataRetorno = null;
                    }
                    else // 10% devolução em breve
                    {
                        estado = devolucaoEmBreve;
                        dataPedido = agora.AddDays(-random.Next(8, 12));
                        dataRetorno = null;
                    }

                    orders.Add(new Order
                    {
                        User = usuario,
                        Book = livro,
                        Library = biblioteca,
                        OrderDate = dataPedido,
                        ReturnDate = dataRetorno,
                        RequestedCopiesQTY = 1,
                        State = estado
                    });
                }
            }

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }

        public static void SeedAdditional()
        {
            try
            {
                using var context = new LibraryContext();
                AdditionalBooks(context);
                AdditionalUsers(context);

                Console.WriteLine("Adição de dados complementares concluída com sucesso.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao adicionar dados complementares: {e.Message}");
                if (e.InnerException != null)
                {
                    Console.WriteLine($"Detalhes adicionais: {e.InnerException.Message}");
                }
                Console.WriteLine($"Stack trace: {e.StackTrace}");
            }
        }
    }
}
