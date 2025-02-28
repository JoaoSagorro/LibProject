using EFLibrary;
using EFLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibLibrary.Services
{
    public class LibSubjects
    {
        internal static bool SubjectExists(Subject sub, LibraryContext context) => context.Subjects.Any(s => s.SubjectName == sub.SubjectName);

        internal static void AddSubject(Subject sub, LibraryContext context)
        {
            context.Subjects.Add(sub);
            context.SaveChanges();
        }
    }
}
