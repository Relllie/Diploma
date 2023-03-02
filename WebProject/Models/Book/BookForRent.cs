using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Models.Book
{
    public class BookForRent
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string BookAuthor { get; set; } = string.Empty;
        public int BookOnShelfID { get; set; }
        public bool IsAvailable { get; set; }
    }
}
