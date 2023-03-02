using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Domain.Models.Books
{
    public class BooksOnShelves
    {
        public BooksOnShelves()
        { }
        [Key]
        public int BookOnShelfID { get; set; }
        public int BookId { get; set; }
        public int? InventoryNumber { get; set; }
        public int BookCaseNumber { get; set; }
        public int ShelfNumber { get; set; }
        public bool IsAvailable { get; set; }
        public byte[]? QRCodePath { get; set; } = null;
    }
}
