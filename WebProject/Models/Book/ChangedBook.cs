using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Models.Book
{
    public class ChangedBook
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string BookAuthor { get; set; } = string.Empty;
        public string Genre { get; set; }
        public IFormFile? BookCoverPath { get; set; }
        public IFormFile? BookPDFPath { get; set; }
    }
}
