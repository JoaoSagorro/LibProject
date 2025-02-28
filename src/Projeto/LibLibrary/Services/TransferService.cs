using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary;
using EFLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace LibLibrary.Services
{
    public class TransferService
    {
        public static void TransferCopies(Copie currentCopy, int destinationLibraryId, int quantity)
        {
            try
            {
                using(LibraryContext context = new LibraryContext())
                {
                    var targetCopie = context.Copies.FirstOrDefault(c => c.BookId == currentCopy.BookId && c.LibraryId == destinationLibraryId);

                    if(targetCopie is null)
                    {
                        var targetLib = context.Libraries.FirstOrDefault(l => l.LibraryId == destinationLibraryId);
                        var newCopie = new Copie { BookId = currentCopy.BookId, LibraryId = targetLib.LibraryId, NumberOfCopies = quantity };
                        currentCopy.NumberOfCopies -= quantity;
                        context.Copies.Add(newCopie);
                    }
                    else
                    {
                        currentCopy.NumberOfCopies -= quantity;
                        targetCopie.NumberOfCopies += quantity;
                    }
                    context.Update(currentCopy);
                    context.SaveChanges();
                }
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }

}

