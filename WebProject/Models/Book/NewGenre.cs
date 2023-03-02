using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Models.Book
{
    public class NewGenre
    {
        public NewGenre()
        { }
        public string GenreName { get; set; } = string.Empty;
        public bool IsSubGenre { get; set; }
        public string? MainGenre { get; set; }
    }
}
