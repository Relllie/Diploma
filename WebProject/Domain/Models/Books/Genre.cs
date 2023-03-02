using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Domain.Models.Books
{
    public class Genre
    {
        public Genre()
        { }

        public int GenreID { get; set; }
        public string GenreName { get; set; } = string.Empty;
        public bool IsSubGenre { get; set; }
        public Genre? MainGenre { get; set; }
    }
}

