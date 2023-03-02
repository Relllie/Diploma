using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using WebProject.Domain.Models.Books;
using WebProject.Models;
using WebProject.Persistence.Contexts;

namespace WebProject.Controllers
{
    public class HomeController : Controller
    {
        private BiblioDbContext _db;
        IEnumerable<Book> BooksList = new List<Book>();

        public HomeController(BiblioDbContext db)
        {
            _db = db;
            
        }

        public IActionResult Index()
        {
            BooksList = _db.Books.ToList<Book>();
            List<Book> bl = BooksList.Reverse().Take(6).ToList();
            return View(bl);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ErrorPage()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

