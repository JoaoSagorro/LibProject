using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;
using Microsoft.IdentityModel.Tokens;

namespace LibLibrary.AuthorServices
{
    public class LibAuthor
    {

        public static Author GetAuthorById(int authorId)
        {
            Author author;

            try
            {
                using(LibraryContext context = new LibraryContext())
                {
                    author = context.Authors.FirstOrDefault(atr => atr.AuthorId == authorId, null);

                    if (author is null) throw new Exception("Couldn't find author with the designated id."); 
                }

            } catch(Exception e)
            {
                throw e;
            }

            return author;
        }


        private static Author CopieAuthor(Author author)
        {
            Author newAuthor = new Author
            {
                AuthorId = author.AuthorId,
                AuthorName = author.AuthorName,
            };

            return newAuthor;
        }

        // Só para administradores
        // rever a função para com as verificações adequadas
        public static Author AddAuthor(Author author)
        {
            try
            {
                using (LibraryContext context = new LibraryContext())
                {
                    context.Add(author);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
                        
            return author;
        }

        // Author parameter must contain the id of the author to be updated and the new values;
        public static Author UpdateAuthor(Author author)
        {
            Author updatedAuthor;

            try
            {
                using(LibraryContext context = new LibraryContext())
                {
                    updatedAuthor = context
                        .Authors
                        .FirstOrDefault(atr => atr.AuthorId == author.AuthorId, null);

                    if (updatedAuthor is null) throw new Exception("Author not found.");
                    
                    string newName = author.AuthorName.ToString();

                    updatedAuthor.AuthorName = newName;
                    context.SaveChanges();
                }
            }
            catch(Exception e)
            {
                throw e;
            }

            return updatedAuthor;
        }

        public static Author DeleteAuthorById(int id)
        {
            Author delAuthor;

            try
            {
                using(LibraryContext context = new LibraryContext())
                {
                    Author author = GetAuthorById(id);
                    delAuthor = CopieAuthor(author);
                }
            }
            catch(Exception e)
            {
                throw e;
            }

            return delAuthor;
        }


        public static bool AuthorExists(string name)
        {
            using (LibraryContext context = new LibraryContext())
            {
                var authors = context.Authors.FirstOrDefault(a => a.AuthorName == name, null);

                return !(authors is null);
            }
        }

    }
}
