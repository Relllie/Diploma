using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Domain.Models.Books
{
    public class Book
    {
        public Book()
        {
        }

        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string BookAuthor { get; set; } = string.Empty;
        public string? FullBiblioDescription { get; set; }
        public string? AbbreviatedNameOfDepartment { get; set; }
        public int? GenreId { get; set; }
        public Genre Genre { get; set; }
        public string BookCoverPath { get; set; } = string.Empty;
        public string BookPDFPath { get; set; } = string.Empty;
    }
}
