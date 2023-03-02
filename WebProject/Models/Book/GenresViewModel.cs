using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebProject.Domain.Models.Books;

namespace WebProject.Models.Book
{
    public class GenresViewModel
    {
        public Genre MainGenre { get; set; }
        public List<Genre> SubGenres { get; set; }
    }
}
