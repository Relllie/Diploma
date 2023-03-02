using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using OfficeOpenXml;

using QRCoder;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using WebProject.Domain.Models;
using WebProject.Domain.Models.Books;
using WebProject.Models.Book;
using WebProject.Persistence.Contexts;
using WebProject.Persistence.Repositories;

namespace WebProject.Controllers
{
    public class BooksController : Controller
    {
        private BiblioDbContext _context;
        IWebHostEnvironment _appEnvironment;
        BookRepository bookRepository;
        GenreRepository genreRepository;
        BooksOnShelvesRepository booksOnShelvesRepository;
        TakenBooksRepository takenBooksRepository;
        List<Book> BooksList = new List<Book>();
        List<Genre> GenresList = new List<Genre>();

        public BooksController(BiblioDbContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            bookRepository = new BookRepository(_context);
            genreRepository = new GenreRepository(_context);
            booksOnShelvesRepository = new BooksOnShelvesRepository(_context);
            takenBooksRepository = new TakenBooksRepository(_context);
            BooksList = _context.Books.ToList();
            GenresList = _context.Genres.ToList();
        }

        [HttpGet]
        public IActionResult Genres()
        {
            List<GenresViewModel> lists = new List<GenresViewModel>(); 
            foreach(Genre main in GenresList)
            {
                if (!main.IsSubGenre)
                {
                    GenresViewModel genresView = new GenresViewModel();
                    List<Genre> SubGenres = new List<Genre>();
                    genresView.MainGenre = main;
                    foreach (Genre sub in GenresList)
                    {
                        if(sub.IsSubGenre && sub.MainGenre==main)
                        {
                            SubGenres.Add(sub);
                        }
                    }
                    genresView.SubGenres = SubGenres;
                    lists.Add(genresView);
                }
            }
            ViewData["Title"] = "Список жанров";
            return View(lists);
        }

        [HttpGet]
        public IActionResult ListOfBooks(int? genreID, int pageNumber)
        {
            ViewData["Title"] = "Список книг";
            int pageNumberCount = 4 * (pageNumber - 1);
            IEnumerable<Book> booksList;
            double booksListCount;
            if (genreID != null)
            {
                booksList = BooksList.Where(b => b.Genre != null);
                booksList = booksList.Where(b => b.Genre.GenreID == genreID).Reverse().Skip(pageNumberCount).Take(4);
                booksListCount = booksList.Count();
                ViewBag.MaxNumber = Math.Ceiling(booksListCount / 4.0);
            }
            else
            {
                booksList = BooksList.Where(b => b.BookAuthor != "").Reverse().Skip(pageNumberCount).Take(4);
                booksListCount = BooksList.Count;
                ViewBag.MaxNumber = Math.Ceiling(booksListCount / 4.0);
            }
            ViewBag.PageNumber = pageNumber;
            if ((pageNumber < 1 || pageNumber > Math.Ceiling(booksListCount /4)) && Math.Ceiling(booksListCount / 4)!=0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(booksList);
        }

        [HttpGet]
        public async Task<IActionResult> ListOfTakenBooks(string email)
        {
            ViewData["Title"] = "Взятые книги";
            if (User.Identity.IsAuthenticated)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                IEnumerable<TakenBooks> takenBooksList = _context.TakenBooks.Where(t => t.UserId == user.UserId);
                return View(takenBooksList);
            }
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult NewBook()
        {
            ViewData["Title"] = "Новая книга";
            if (User.IsInRole("Администратор"))
            {
                NewBook NB = new NewBook();
                ViewBag.Genres = _context.Genres.ToList();
                return View(NB);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewBook(NewBook model)
        {
            if (ModelState.IsValid)
            {
                Book NB = await _context.Books.FirstOrDefaultAsync(b => b.BookTitle == model.BookTitle);
                if (NB == null)
                {
                    NB = new Book() { BookTitle = model.BookTitle, BookAuthor = model.BookAuthor };

                    Genre bookGenre = await _context.Genres.FirstOrDefaultAsync(g => g.GenreName == model.GenreName);
                    NB.Genre = bookGenre;
                    NB.GenreId = bookGenre.GenreID;

                    await bookRepository.CreateAsync(NB);
                    await bookRepository.SaveAsync();

                    NB = await _context.Books.FirstOrDefaultAsync(b => b.BookTitle == model.BookTitle);

                    if (model.BookPDFPath != null)
                    {
                        string path = "/files/bookPDF/" + NB.BookId + ".pdf";
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await model.BookPDFPath.CopyToAsync(fileStream);
                        }
                        NB.BookPDFPath = path;
                    }

                    if (model.BookCoverPath != null)
                    {
                        string path = "/files/bookCover/" + NB.BookId + ".jpg";
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                        {
                            await model.BookCoverPath.CopyToAsync(fileStream);
                        }
                        NB.BookCoverPath = path;
                    }
                    else
                    {
                        NB.BookCoverPath = "/files/bookCover/standartBook.png";
                    }

                    await bookRepository.UpdateAsync(NB);
                    await bookRepository.SaveAsync();


                    return RedirectToAction("ChangeBookOnShelves", "Books",new { bookId = NB.BookId});
                }
                else
                    ModelState.AddModelError("", "Такая книга уже существует");
            }
            ViewBag.Genres = _context.Genres.ToList();
            return View(model);
        }

        [HttpGet]
        public IActionResult NewGenre()
        {
            ViewData["Title"] = "Новый жанр";
            if (User.IsInRole("Администратор"))
            {
                NewGenre NG = new NewGenre();
                ViewBag.Genres = _context.Genres.Where(g=>!g.IsSubGenre).ToList();
                return View(NG);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewGenre(NewGenre model)
        {
            if (ModelState.IsValid)
            {
                Genre NG = await _context.Genres.FirstOrDefaultAsync(g => g.GenreName == model.GenreName);
                if (NG == null)
                {
                    NG = new Genre() { GenreName = model.GenreName, IsSubGenre = model.IsSubGenre};

                    if (model.IsSubGenre != false)
                    {
                        Genre mainGenre = await _context.Genres.FirstOrDefaultAsync(g => g.GenreName == model.MainGenre);
                        NG.MainGenre = mainGenre;
                    }

                    await genreRepository.UpdateAsync(NG);
                    await genreRepository.SaveAsync();

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("ErrorPage","Home");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeBook(int bookID)
        {
            if (User.IsInRole("Администратор"))
            {
                Book oldBook = await _context.Books.FirstOrDefaultAsync(b => b.BookId == bookID);
                ChangedBook model = new ChangedBook() { BookId = oldBook.BookId, BookAuthor = oldBook.BookAuthor, BookTitle = oldBook.BookTitle};
                if (oldBook.Genre != null)
                    model.Genre = oldBook.Genre.GenreName;
                ViewBag.Genres = _context.Genres.ToList();
                ViewData["Title"] = model.BookTitle;
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeBook(ChangedBook model)
        {
            if (ModelState.IsValid)
            {
                Book changedBook = await _context.Books.FirstOrDefaultAsync(b => b.BookId == model.BookId);
                if (changedBook != null)
                {
                    if (model.BookTitle != "")
                        changedBook.BookTitle = model.BookTitle;
                    if (model.BookAuthor != "")
                        changedBook.BookAuthor = model.BookAuthor;
                    if (model.BookPDFPath != null)
                        changedBook.BookTitle = model.BookTitle;
                    if (model.BookTitle != "")
                        changedBook.BookTitle = model.BookTitle;
                    Genre newGenre = _context.Genres.FirstOrDefault(g => g.GenreName == model.Genre);
                    changedBook.Genre = newGenre;
                    changedBook.GenreId = newGenre.GenreID;

                    await bookRepository.UpdateAsync(changedBook);
                    await bookRepository.SaveAsync();

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Такой книги не существует");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeBookOnShelves(int bookID)
        {
            ViewData["Title"] = "Расположение книги";
            if (User.IsInRole("Администратор"))
            {
                BooksOnShelves BOS = await _context.BooksOnShelves.FirstOrDefaultAsync(b => b.BookId == bookID);
                if (BOS == null)
                {
                    BOS = new BooksOnShelves() { BookId = bookID};
                }
                return View(BOS);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeBookOnShelves(BooksOnShelves model)
        {
            if (ModelState.IsValid)
            {
                BooksOnShelves changedBooksOnShelves = await _context.BooksOnShelves.FirstOrDefaultAsync(b => b.BookOnShelfID == model.BookOnShelfID);
                Book book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == model.BookId);
                if (changedBooksOnShelves != null)
                {
                    if (model.BookCaseNumber != changedBooksOnShelves.BookCaseNumber)
                        changedBooksOnShelves.BookCaseNumber = model.BookCaseNumber;
                    if (model.ShelfNumber != changedBooksOnShelves.ShelfNumber)
                        changedBooksOnShelves.ShelfNumber = model.ShelfNumber;
                    if (model.IsAvailable != changedBooksOnShelves.IsAvailable)
                        changedBooksOnShelves.IsAvailable = model.IsAvailable;
                    changedBooksOnShelves.QRCodePath = QRCodeGeneration(QRCodeText(changedBooksOnShelves, book));
                    await booksOnShelvesRepository.UpdateAsync(changedBooksOnShelves);
                }
                else
                {
                    changedBooksOnShelves = new BooksOnShelves() { BookId = model.BookId, BookCaseNumber = model.BookCaseNumber, ShelfNumber = model.ShelfNumber, IsAvailable = model.IsAvailable, QRCodePath = QRCodeGeneration(QRCodeText(model, book)) };
                    await booksOnShelvesRepository.CreateAsync(changedBooksOnShelves);
                }
                await bookRepository.SaveAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult BooksSearch()
        {
            ViewData["Title"] = "Поиск книг";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BooksSearch(string searchText)
        {
            if (searchText != null)
            {
                List<Book> serchbooksList = new();
                foreach (var b in BooksList)
                {
                    string bookText = (b.BookAuthor + b.BookTitle + b.Genre.GenreName).ToLower().Replace(" ","");
                    if (bookText.Contains(searchText.ToLower().Replace(" ", "")))
                        serchbooksList.Add(b);
                }
                return View(serchbooksList);
            }
            else
                return View();
        }

        [HttpGet]
        public async Task<IActionResult> RentBook(int bookId)
        {
            if (User.Identity.IsAuthenticated)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                if(user.ConfirmedStudentId)
                {
                    Book book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == bookId);
                    BooksOnShelves bookOnShelfe = await _context.BooksOnShelves.FirstOrDefaultAsync(s => s.BookId == bookId);
                    ViewData["Title"] = book.BookTitle;
                    if (bookOnShelfe != null)
                    {
                        BookForRent rent = new BookForRent() { BookId = bookId, BookAuthor = book.BookAuthor, BookTitle = book.BookTitle, BookOnShelfID = bookOnShelfe.BookOnShelfID, IsAvailable = bookOnShelfe.IsAvailable };
                        return View(rent);
                    }
                }
            }
            return RedirectToAction("ListOfBooks", "Books");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RentBook(BookForRent model)
        {
            if (ModelState.IsValid)
            {
                BooksOnShelves bookOnShelves = await _context.BooksOnShelves.FirstOrDefaultAsync(b => b.BookOnShelfID == model.BookOnShelfID);
                if (bookOnShelves.IsAvailable)
                {
                    User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                    Book book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == model.BookId);
                    TakenBooks takenBook = new TakenBooks() { BookOnShelfID = model.BookOnShelfID, Returned = false, UserId = user.UserId, DateOfTaking = DateTime.Today, Book = book };
                    bookOnShelves.IsAvailable = false;
                    await takenBooksRepository.CreateAsync(takenBook);
                    await takenBooksRepository.SaveAsync();
                    await booksOnShelvesRepository.SaveAsync();
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet]
        public IActionResult ListOfBooksForRent()
        {
            ViewData["Title"] = "Книги на выдачу";
            if (User.IsInRole("Администратор"))
            {
                IEnumerable<TakenBooks> booksList = _context.TakenBooks.Where(b => b.Returned == false).ToList();
                return View(booksList);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ListOfBooksForRent(int bookId)
        {
            TakenBooks takenBook = await _context.TakenBooks.FirstOrDefaultAsync(t => t.TakenBooksId == bookId);
            BooksOnShelves bookOnShelves = await _context.BooksOnShelves.FirstOrDefaultAsync(b => b.BookId == takenBook.Book.BookId);
            if (!takenBook.Returned)
            {
                takenBook.Returned = true;
                bookOnShelves.IsAvailable = true;
                await takenBooksRepository.SaveAsync();
                await booksOnShelvesRepository.SaveAsync();
            }
            IEnumerable<TakenBooks> booksList = _context.TakenBooks.Where(b => b.Returned == false).ToList();
            return View(booksList);
        }

        [HttpGet]
        public IActionResult NewListOfBooks()
        {
            if (User.IsInRole("Администратор"))
            {
                ViewData["Title"] = "Добавление новых книг";
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewListOfBooks(NewListOfBooks model)
        {
            string path = "/files/tempFiles/excelFiles.xlsx";
            if (model.excelFile != null)
            {
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await model.excelFile.CopyToAsync(fileStream);
                }
            }
            List<NewBook> newBooks = readXLS(_appEnvironment.WebRootPath + path);
            if(newBooks==null)
            {
                return RedirectToAction("ErrorPage","Home");
            }
            else
            {
                foreach(var e in newBooks)
                {
                    Book NB = await _context.Books.FirstOrDefaultAsync(b => b.BookTitle == e.BookTitle);
                    if (NB == null)
                    {
                        NB = new Book() { BookTitle = e.BookTitle, BookAuthor = e.BookAuthor , BookCoverPath = "/files/bookCover/standartBook.png"
                    };

                        await bookRepository.CreateAsync(NB);
                    }
                }
                await bookRepository.SaveAsync();
            }
            return View();
        }

        private static string QRCodeText(BooksOnShelves shelfeInfo, Book bookInfo)
        {
            return ("Номер шкафа: " + shelfeInfo.BookCaseNumber + ", номер полки: " + shelfeInfo.ShelfNumber + ", название книги: " + bookInfo.BookTitle + ", " + bookInfo.BookAuthor + ".\nВ случае нахождения вернуть в библиотеку БФ БашГУ!!!");
        }

        private static Byte[] QRCodeGeneration(string qrText)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return BitmapToBytes(qrCodeImage);
        }
        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Book(int bookID)
        {
            Book model = await _context.Books.FirstOrDefaultAsync(b => b.BookId == bookID);
            BooksOnShelves BookOnShelves = await _context.BooksOnShelves.FirstOrDefaultAsync(b => b.BookId == bookID);
            if(!System.IO.File.Exists(_appEnvironment.WebRootPath + "/files/bookPDF/" + bookID + ".pdf"))
            {
                model.BookPDFPath = string.Empty;
            }
            if(BookOnShelves!=null)
            if (BookOnShelves.QRCodePath != null)
                ViewBag.QRCode = BookOnShelves.QRCodePath;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteGenre(int genreID)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g=>g.GenreID == genreID);
            _context.Genres.Remove(genre);
            await genreRepository.SaveAsync();
            return RedirectToAction("Genres","Books");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteBook(int bookID)
        {
            var book = await _context.Books.FirstOrDefaultAsync(g => g.BookId == bookID);
            _context.Books.Remove(book);
            await bookRepository.SaveAsync();
            return RedirectToAction("ListOfBooks", "Books", new { pageNumber = 1 });
        }

        public VirtualFileResult GetBook(int bookID)
        {
            var filepath = Path.Combine("~/files/bookPDF", bookID + ".pdf");
            return File(filepath, "application/pdf", bookID + ".pdf");
        }

        public List<NewBook> readXLS(string FilePath)
        {
            FileInfo existingFile = new FileInfo(FilePath);
            List<NewBook> newBooks;
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                int colCount = worksheet.Dimension.Columns;
                int rowCount = worksheet.Dimension.Rows;
                newBooks = new List<NewBook>();
                for (int row = 1; row <= rowCount; row++)
                {
                    if(worksheet.Cells[row, 1].Value == null)
                    {
                        break;
                    }
                    NewBook book = new NewBook() { BookTitle = worksheet.Cells[row, 1].Value.ToString().Trim(), BookAuthor = worksheet.Cells[row, 2].Value.ToString().Trim(), GenreName = worksheet.Cells[row, 3].Value.ToString().Trim() };
                    newBooks.Add(book);
                }
                
            }
            return newBooks;
        }
    }
}
