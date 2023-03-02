using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Models.Book
{
    public class NewBook
    {
        [Required(ErrorMessage = "Не указано название книги")]
        public string BookTitle { get; set; } = string.Empty;
        [Required(ErrorMessage = "Не указан автор книги")]
        public string BookAuthor { get; set; } = string.Empty;
        [Required(ErrorMessage = "Не указан жанр книги")]
        public string GenreName { get; set; }
        public IFormFile? BookCoverPath { get; set; }
        public IFormFile? BookPDFPath { get; set; }
    }
}
