using Microsoft.EntityFrameworkCore;
using EFLibrary.Models;

namespace EFLibrary
{
    public class LibraryContext : DbContext
    {
        private string CnString { get; set; } = Environment.GetEnvironmentVariable("CONNECTION_STRING");


        // Constructor for dependency injection (used in production)
        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }

        public LibraryContext()
        {

        }

        // Constructor for manual instantiation (used in testing or other scenarios)
        public LibraryContext(string cnString)
        {
            this.CnString = cnString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Only configure the connection if no options were provided (e.g., in testing)
            if (!optionsBuilder.IsConfigured)
            {
                if (string.IsNullOrEmpty(CnString))
                {
                    // Use in-memory database for testing
                    optionsBuilder.UseInMemoryDatabase("InMemoryDatabase");
                }
                else
                {
                    // Use a real database for production
                    optionsBuilder.UseSqlServer(CnString);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Copie>().HasKey(ck => new { ck.BookId, ck.LibraryId });
            modelBuilder.Entity<Copie>().HasIndex(e => new { e.BookId, e.LibraryId }).IsUnique();

            modelBuilder.Entity<Role>().HasIndex(r => r.RoleName).IsUnique();
            modelBuilder.Entity<Book>().HasIndex(o => new { o.Title, o.Edition }).IsUnique();
            modelBuilder.Entity<Subject>().HasIndex(a => a.SubjectName).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Author>().HasIndex(a => a.AuthorName).IsUnique();
            modelBuilder.Entity<State>().HasIndex(e => e.StateName).IsUnique();
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Copie> Copies { get; set; }
        public DbSet<Cover> Covers { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<User> Users { get; set; }
    }
}