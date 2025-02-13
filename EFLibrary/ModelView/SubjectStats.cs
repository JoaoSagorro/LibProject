using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFLibrary.Models;

namespace EFLibrary.ModelView
{
    public class SubjectStats
    {

        public List<List<Subject>> Subjects { get; set; }
        public List<int> SubjectsCount { get; set; }

    }
}
