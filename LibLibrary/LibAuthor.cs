using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;
using Microsoft.IdentityModel.Tokens;

namespace LibLibrary
{
    public class LibAuthor
    {

        // Só para administradores
        // rever a função para com as verificações adequadas
        public static Author AddAuthor(string author)
        {
            using (LibraryContext context = new LibraryContext())
            {
                Author newAuthor = new Author
                {
                    AuthorName = author,
                };

                return newAuthor;
            }
        }

        public static bool AuthorExists(string name)
        {
            using (LibraryContext context = new LibraryContext())
            {
                var authors = context.Authors.Where(a => a.AuthorName == name);

                return !authors.IsNullOrEmpty();
            }
        }

        // Só para administradores
        public static Author? AuthorFinder(string name)
        {
            using (LibraryContext context = new LibraryContext())
            {

                Author? authorsList = context.Authors.FirstOrDefault(e => e.AuthorName == name);

                return authorsList;
            }
        }

    }
}
