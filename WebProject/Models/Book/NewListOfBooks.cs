using Microsoft.AspNetCore.Http;

namespace WebProject.Models.Book
{
    public class NewListOfBooks
    {
        public IFormFile excelFile { get; set; }
    }
}
