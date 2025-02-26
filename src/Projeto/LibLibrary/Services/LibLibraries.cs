using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary.Models;
using EFLibrary;

namespace LibLibrary.Services
{
    public class LibLibraries
    {
        public static List<Library> GetLibraries()
        {
            using var context = new LibraryContext();
            return [.. context.Libraries];
        }
    }
}
