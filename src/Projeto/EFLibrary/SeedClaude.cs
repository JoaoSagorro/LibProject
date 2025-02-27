using EFLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFLibrary
{
    public class SeedClaude
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
                var adminRole = context.Roles.FirstOrDefault(r => r.RoleName == "Admin");
                var funcionarioRole = context.Roles.FirstOrDefault(r => r.RoleName == "Funcionário");
                var usuarioRole = context.Roles.FirstOrDefault(r => r.RoleName == "Usuário");

                // Data base para cálculo de datas de registro (alguns usuários mais antigos, outros mais recentes)
                var baseDate = DateTime.Now.AddYears(-2);

                context.Users.AddRange(new List<User>{
                    new User {
                        FirstName = "Ana",
                        LastName = "Santos",
                        Address = "Rua das Acácias, 100, Lisboa",
                        Active = true,
                        Birthdate = DateTime.Parse("1990-05-01"),
                        RegisterDate = baseDate.AddMonths(2),
                        Email = "ana.santos@email.com",
                        Password = "Password123", // Senhas mais seguras
                        Suspended = false,
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "Bruno",
                        LastName = "Pereira",
                        Address = "Av. Paulista, 200, Porto",
                        Active = true,
                        Birthdate = DateTime.Parse("1989-12-25"),
                        RegisterDate = baseDate.AddMonths(5),
                        Email = "bruno.pereira@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "Carlos",
                        LastName = "Souza",
                        Address = "Rua do Comércio, 300, Coimbra",
                        Active = true,
                        Birthdate = DateTime.Parse("1985-03-15"),
                        RegisterDate = baseDate.AddMonths(8),
                        Email = "carlos.souza@email.com", // Corrigido para padrão consistente
                        Password = "Password123",
                        Suspended = false,
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "Mariana",
                        LastName = "Lima",
                        Address = "Rua das Flores, 400, Aveiro",
                        Active = true,
                        Birthdate = DateTime.Parse("1992-07-22"),
                        RegisterDate = baseDate.AddMonths(10),
                        Email = "mariana.lima@email.com", // Corrigido para padrão consistente
                        Password = "Password123",
                        Suspended = false,
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "João",
                        LastName = "Oliveira",
                        Address = "Rua Comercial, 55, Faro",
                        Active = true,
                        Birthdate = DateTime.Parse("1988-09-10"),
                        RegisterDate = baseDate.AddMonths(12),
                        Email = "joao.oliveira@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "Rita",
                        LastName = "Ferreira",
                        Address = "Av. Central, 876, Lisboa",
                        Active = true,
                        Birthdate = DateTime.Parse("1995-02-28"),
                        RegisterDate = baseDate.AddMonths(15),
                        Email = "rita.ferreira@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "Miguel",
                        LastName = "Rodrigues",
                        Address = "Praça da República, 20, Porto",
                        Active = false, // Usuário inativo para diversidade de dados
                        Birthdate = DateTime.Parse("1982-11-15"),
                        RegisterDate = baseDate.AddMonths(4),
                        Email = "miguel.rodrigues@email.com",
                        Password = "Password123",
                        Suspended = true, // Usuário suspenso
                        Role = usuarioRole
                    },
                    new User {
                        FirstName = "Teresa",
                        LastName = "Almeida",
                        Address = "Av. da Liberdade, 150, Lisboa",
                        Active = true,
                        Birthdate = DateTime.Parse("1991-08-05"),
                        RegisterDate = baseDate.AddMonths(7),
                        Email = "teresa.almeida@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = funcionarioRole // Funcionário da biblioteca
                    },
                    new User {
                        FirstName = "Pedro",
                        LastName = "Costa",
                        Address = "Rua da Biblioteca, 45, Porto",
                        Active = true,
                        Birthdate = DateTime.Parse("1986-04-12"),
                        RegisterDate = baseDate,
                        Email = "pedro.costa@email.com",
                        Password = "Password123",
                        Suspended = false,
                        Role = funcionarioRole // Funcionário da biblioteca
                    },
                    new User {
                        FirstName = "Admin",
                        LastName = "Sistema",
                        Address = "Rua Central, 1, Lisboa",
                        Active = true,
                        Birthdate = DateTime.Parse("1980-01-01"),
                        RegisterDate = baseDate.AddMonths(-6), // Admin é o mais antigo
                        Email = "admin@biblioteca.com", // Email mais apropriado para sistema
                        Password = "AdminPass123", // Senha diferente para admin
                        Suspended = false,
                        Role = adminRole
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
                    new Subject { SubjectName = "Histórico" },
                    new Subject { SubjectName = "Biografia" },
                    new Subject { SubjectName = "Poesia" },
                    new Subject { SubjectName = "Drama" },
                    new Subject { SubjectName = "Thriller" },
                    new Subject { SubjectName = "Ensaio" },
                    new Subject { SubjectName = "Literatura Infantil" }
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
                    new Author { AuthorName = "Guimarães Rosa" },
                    new Author { AuthorName = "Fernando Pessoa" },
                    new Author { AuthorName = "Eça de Queirós" },
                    new Author { AuthorName = "Lygia Fagundes Telles" },
                    new Author { AuthorName = "Cecília Meireles" },
                    new Author { AuthorName = "Mia Couto" },
                    new Author { AuthorName = "José Saramago" },
                    new Author { AuthorName = "Sophia de Mello Breyner" },
                    new Author { AuthorName = "Luís de Camões" }
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
                var poesia = context.Subjects.FirstOrDefault(s => s.SubjectName == "Poesia");
                var misterio = context.Subjects.FirstOrDefault(s => s.SubjectName == "Mistério");
                var ficcao = context.Subjects.FirstOrDefault(s => s.SubjectName == "Ficção Científica");
                var fantasia = context.Subjects.FirstOrDefault(s => s.SubjectName == "Fantasia");
                var drama = context.Subjects.FirstOrDefault(s => s.SubjectName == "Drama");

                // Obter autores
                var machado = context.Authors.FirstOrDefault(a => a.AuthorName == "Machado de Assis");
                var clarice = context.Authors.FirstOrDefault(a => a.AuthorName == "Clarice Lispector");
                var jorgeAmado = context.Authors.FirstOrDefault(a => a.AuthorName == "Jorge Amado");
                var pauloCoelho = context.Authors.FirstOrDefault(a => a.AuthorName == "Paulo Coelho");
                var joseAlencar = context.Authors.FirstOrDefault(a => a.AuthorName == "José de Alencar");
                var guimaraesRosa = context.Authors.FirstOrDefault(a => a.AuthorName == "Guimarães Rosa");
                var pessoa = context.Authors.FirstOrDefault(a => a.AuthorName == "Fernando Pessoa");
                var eca = context.Authors.FirstOrDefault(a => a.AuthorName == "Eça de Queirós");
                var saramago = context.Authors.FirstOrDefault(a => a.AuthorName == "José Saramago");
                var camoes = context.Authors.FirstOrDefault(a => a.AuthorName == "Luís de Camões");
                var sophia = context.Authors.FirstOrDefault(a => a.AuthorName == "Sophia de Mello Breyner");

                context.Books.AddRange(new List<Book>
                {
                    new Book {
                        Title = "Dom Casmurro",
                        Year = 1899,
                        Edition = "1ª Edição",
                        Quantity = 15,
                        Author = machado,
                        Subjects = new List<Subject> { romance, drama }
                    },
                    new Book {
                        Title = "Memórias Póstumas de Brás Cubas",
                        Year = 1881,
                        Edition = "Edição Comemorativa",
                        Quantity = 12,
                        Author = machado,
                        Subjects = new List<Subject> { romance, drama }
                    },
                    new Book {
                        Title = "O Alquimista",
                        Year = 1988,
                        Edition = "Edição Especial",
                        Quantity = 20,
                        Author = pauloCoelho,
                        Subjects = new List<Subject> { aventura, fantasia }
                    },
                    new Book {
                        Title = "Capitães da Areia",
                        Year = 1937,
                        Edition = "Edição Revista",
                        Quantity = 18,
                        Author = jorgeAmado,
                        Subjects = new List<Subject> { aventura, drama }
                    },
                    new Book {
                        Title = "Grande Sertão: Veredas",
                        Year = 1956,
                        Edition = "Edição Crítica",
                        Quantity = 10,
                        Author = guimaraesRosa,
                        Subjects = new List<Subject> { aventura, romance }
                    },
                    new Book {
                        Title = "A Hora da Estrela",
                        Year = 1977,
                        Edition = "Edição Bolso",
                        Quantity = 14,
                        Author = clarice,
                        Subjects = new List<Subject> { romance, drama }
                    },
                    new Book {
                        Title = "Iracema",
                        Year = 1865,
                        Edition = "Edição Escolar",
                        Quantity = 25,
                        Author = joseAlencar,
                        Subjects = new List<Subject> { historico, romance }
                    },
                    new Book {
                        Title = "Os Lusíadas",
                        Year = 1572,
                        Edition = "Edição Comentada",
                        Quantity = 30,
                        Author = camoes,
                        Subjects = new List<Subject> { poesia, historico }
                    },
                    new Book {
                        Title = "Livro do Desassossego",
                        Year = 1982,
                        Edition = "Edição Definitiva",
                        Quantity = 8,
                        Author = pessoa,
                        Subjects = new List<Subject> { poesia, drama }
                    },
                    new Book {
                        Title = "O Crime do Padre Amaro",
                        Year = 1875,
                        Edition = "Edição Anotada",
                        Quantity = 12,
                        Author = eca,
                        Subjects = new List<Subject> { romance, drama }
                    },
                    new Book {
                        Title = "Ensaio Sobre a Cegueira",
                        Year = 1995,
                        Edition = "1ª Edição",
                        Quantity = 15,
                        Author = saramago,
                        Subjects = new List<Subject> { romance, ficcao }
                    },
                    new Book {
                        Title = "A Menina do Mar",
                        Year = 1958,
                        Edition = "Edição Ilustrada",
                        Quantity = 22,
                        Author = sophia,
                        Subjects = new List<Subject> { fantasia }
                    },
                    new Book {
                        Title = "Quincas Borba",
                        Year = 1891,
                        Edition = "Edição Acadêmica",
                        Quantity = 10,
                        Author = machado,
                        Subjects = new List<Subject> { romance }
                    },
                    new Book {
                        Title = "Gabriela, Cravo e Canela",
                        Year = 1958,
                        Edition = "Edição de Luxo",
                        Quantity = 12,
                        Author = jorgeAmado,
                        Subjects = new List<Subject> { romance }
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
                    //new State { StateName = "Solicitado" },
                    //new State { StateName = "Em Processamento" },
                    new State { StateName = "Requisitado" },
                    new State { StateName = "ATRASADO" },
                    new State { StateName = "Devolução URGENTE" },
                    new State { StateName = "Devolver em breve" },
                    new State { StateName = "Devolvido" },
                    //new State { StateName = "Cancelado" }
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
                    new Library {
                        LibraryName = "Biblioteca Central de Lisboa",
                        LibraryAddress = "Rua Augusta, 150, Lisboa",
                        Contact = "211823746",
                        Email = "central.lisboa@biblioteca.pt"
                    },
                    new Library {
                        LibraryName = "Biblioteca Pública do Porto",
                        LibraryAddress = "Avenida dos Aliados, 45, Porto",
                        Contact = "222823744",
                        Email = "publica.porto@biblioteca.pt"
                    },
                    new Library {
                        LibraryName = "Biblioteca Municipal de Aveiro",
                        LibraryAddress = "Praça da República, 30, Aveiro",
                        Contact = "234823743",
                        Email = "municipal.aveiro@biblioteca.pt"
                    },
                    new Library {
                        LibraryName = "Biblioteca Regional de Faro",
                        LibraryAddress = "Rua do Mar, 120, Faro",
                        Contact = "289823742",
                        Email = "regional.faro@biblioteca.pt"
                    },
                    new Library {
                        LibraryName = "Biblioteca Universitária de Coimbra",
                        LibraryAddress = "Rua Larga, 10, Coimbra",
                        Contact = "239823741",
                        Email = "universitaria.coimbra@biblioteca.pt"
                    }
                });
                context.SaveChanges();
            }
        }

        public static void Copies(LibraryContext context)
        {
            if (!context.Copies.Any())
            {
                // Obter todos os livros
                var books = context.Books.ToList();

                // Obter todas as bibliotecas
                var libraries = context.Libraries.ToList();

                // Lista para armazenar as novas cópias
                var copies = new List<Copie>();

                // Distribuir cópias de cada livro pelas bibliotecas de forma mais realista
                foreach (var book in books)
                {
                    // Determinar quantas bibliotecas terão este livro (entre 2 e 5)
                    int numLibraries = new Random().Next(2, libraries.Count + 1);

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
                            int copiesForLibrary = new Random().Next(1, (remainingCopies - distributedCopies) / 2 + 1);
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
        }

        private static void Orders(LibraryContext context)
        {
            if (!context.Orders.Any())
            {
                // Obter usuários (excluindo admin e funcionários)
                var usuarios = context.Users
                    .Where(u => u.Role.RoleName == "Usuário" && u.Active)
                    .ToList();

                // Obter livros
                var livros = context.Books.ToList();

                // Obter bibliotecas
                var bibliotecas = context.Libraries.ToList();

                // Obter estados
                //var solicitado = context.States.FirstOrDefault(s => s.StateName == "Solicitado");
                var emprestado = context.States.FirstOrDefault(s => s.StateName == "Requisitado");
                var emAtraso = context.States.FirstOrDefault(s => s.StateName == "ATRASO");
                var devolucaoUrgente = context.States.FirstOrDefault(s => s.StateName == "Devolução URGENTE");
                var devolucaoEmBreve = context.States.FirstOrDefault(s => s.StateName == "Devolver em breve");
                var devolvido = context.States.FirstOrDefault(s => s.StateName == "Devolvido");
                //var cancelado = context.States.FirstOrDefault(s => s.StateName == "Cancelado");

                // Configuração para datas
                var agora = DateTime.Now;
                var random = new Random();

                // Lista para armazenar os novos pedidos
                var orders = new List<Order>();

                // Criar pedidos para cada usuário (2-4 pedidos por usuário)
                foreach (var usuario in usuarios)
                {
                    int numPedidos = random.Next(2, 5);

                    for (int i = 0; i < numPedidos; i++)
                    {
                        // Selecionar livro e biblioteca aleatoriamente
                        var livro = livros[random.Next(livros.Count)];
                        var biblioteca = bibliotecas[random.Next(bibliotecas.Count)];

                        // Verificar se existe cópia deste livro na biblioteca
                        var copiasNaBiblioteca = context.Copies
                            .FirstOrDefault(c => c.Book.BookId == livro.BookId && c.Library.LibraryId == biblioteca.LibraryId);

                        if (copiasNaBiblioteca == null || copiasNaBiblioteca.NumberOfCopies <= 0)
                            continue;

                        // Determinar estado do pedido aleatoriamente
                        State estado;
                        DateTime dataPedido;
                        DateTime? dataRetorno;

                        // Distribuição realista de estados
                        int estadoRandom = random.Next(100);

                        if (estadoRandom < 15) // 15% solicitados
                        {
                            estado = emprestado;
                            dataPedido = agora.AddDays(-random.Next(1, 3));
                            dataRetorno = null; // 2 semanas para devolução
                        }
                        else if (estadoRandom < 40) // 25% emprestados
                        {
                            estado = emprestado;
                            dataPedido = agora.AddDays(-random.Next(3, 10));
                            dataRetorno = null;
                        }
                        else if (estadoRandom < 50) // 10% em atraso
                        {
                            estado = emAtraso;
                            dataPedido = agora.AddDays(-random.Next(20, 30));
                            dataRetorno = null; // Data de retorno já passou
                        }
                        else if (estadoRandom < 60) // 10% devolução urgente
                        {
                            estado = devolucaoUrgente;
                            dataPedido = agora.AddDays(-random.Next(13, 15));
                            dataRetorno = null; // Data de retorno está muito próxima
                        }
                        else if (estadoRandom < 75) // 15% devolução em breve
                        {
                            estado = devolucaoEmBreve;
                            dataPedido = agora.AddDays(-random.Next(10, 13));
                            dataRetorno = null;
                        }
                        else if (estadoRandom < 95) // 20% devolvidos
                        {
                            estado = devolvido;
                            dataPedido = agora.AddDays(-random.Next(15, 45));
                            dataRetorno = dataPedido.AddDays(14);
                        }
                        else // 5% cancelados
                        {
                            estado = emAtraso;
                            dataPedido = agora.AddDays(-random.Next(20, 30));
                            dataRetorno = null; // Data de retorno já passou

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
        }

        public static void SeedAll()
        {
            try
            {
                using var context = new LibraryContext();
                Roles(context);
                Authors(context);
                Users(context);
                Subjects(context);
                States(context);
                Books(context);
                Libraries(context);
                Copies(context);
                Orders(context);

                Console.WriteLine("Banco de dados populado com sucesso.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Erro ao popular o banco de dados: {e.Message}");
                // Adicionar mais detalhes do erro para facilitar a depuração
                if (e.InnerException != null)
                {
                    Console.WriteLine($"Detalhes adicionais: {e.InnerException.Message}");
                }
                Console.WriteLine($"Stack trace: {e.StackTrace}");
            }
        }
    }
}
