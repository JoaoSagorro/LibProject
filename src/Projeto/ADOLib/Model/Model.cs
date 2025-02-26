using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADOLib.Model
{
    public class Model
    {

        public class Author
        {
            public int AuthorId { get; set; }
            public string AuthorName { get; set; }
        }

        public class Book
        {
            public int BookId { get; set; }
            public string Title { get; set; }
            public string Edition { get; set; }
            public int Year { get; set; }
            public int Quantity { get; set; }
            public int AuthorId { get; set; }
        }

        public class BookSubject
        {
            public int BookId { get; set; }
            public int SubjectId { get; set; }
        }

        public class Copie
        {
            public int BookId { get; set; }
            public int LibraryId { get; set; }
            public int NumberOfCopies { get; set; }
        }

        public class Cover
        {
            public int BookId { get; set; }
            public byte[] CoverImage { get; set; }
        }

        public class Library
        {
            public int LibraryId { get; set; }
            public string LibraryName { get; set; }
            public string LibraryAddress { get; set; }
            public string Email { get; set; }
            public string Contact { get; set; }
        }

        public class Order
        {
            public int OrderId { get; set; }
            public int UserId { get; set; }
            public int LibraryId { get; set; }
            public int BookId { get; set; }
            public int StateId { get; set; }
            public int RequestedCopiesQTY { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime? ReturnDate { get; set; } = null;
        }

        public class OrderHistory
        {
            public int OrderHistoryId { get; set; }
            public string UserName { get; set; }
            public string BookName { get; set; }
            public int BookYear { get; set; }
            public string BookEdition { get; set; }
            public string BookAuthor { get; set; }
            public string LibraryName { get; set; }
            public int OrderedCopies { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime ReturnDate { get; set; }
        }

        public class Role
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
        }

        public class State
        {
            public int StateId { get; set; }
            public string StateName { get; set; }
        }

        public class Subject
        {
            public int SubjectId { get; set; }
            public string SubjectName { get; set; }
        }

        public class User
        {
            public int UserId { get; set; }
            public int RoleId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Address { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public DateTime Birthdate { get; set; }
            public DateTime RegisterDate { get; set; }
            public bool Suspended { get; set; } = false;
            public bool Active { get; set; } = true;
            public int Strikes { get; set; } = 0;
        }
    }
}
