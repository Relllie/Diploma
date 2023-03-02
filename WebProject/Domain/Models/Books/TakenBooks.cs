using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Domain.Models.Books
{
    public class TakenBooks
    {
        public TakenBooks()
        { }
        public int TakenBooksId { get; set; }
        public int BookOnShelfID { get; set; }
        public Book Book { get; set; }
        public int UserId { get; set; }
        public DateTime DateOfTaking { get; set; }
        public bool Returned { get; set; }
    }
}
