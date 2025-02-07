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
        private string CnString { get; set; } = "";  

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
