using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace EFLibrary
{
    public class LibraryContext : DbContext
    {
        private string CnString { get; set; } = "Server=LAPTOP-DKPO5APD\\MSSQLSERVER02;Database=upskill_fake_library;Trusted_Connection=True;TrustServerCertificate=True";  

        public LibraryContext()
        {

        }

        public LibraryContext(string cnString)
        {
            this.CnString = cnString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(CnString);
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
