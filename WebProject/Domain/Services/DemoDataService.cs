using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using WebProject.Domain.Models;
using WebProject.Domain.Models.Books;
using WebProject.Domain.Repositories;

namespace WebProject.Domain.Services
{
    public class DemoDataService
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<Book> _booksRepository;
        private readonly IRepository<BooksOnShelves> _booksOnShelvesRepository;
        private readonly IRepository<TakenBooks> _takenBooksRepository;

        public DemoDataService(IRepository<Role> roleRepository,
            IRepository<User> userRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<Genre> genreRepository,
            IRepository<Book> booksRepository,
            IRepository<BooksOnShelves> booksOnShelvesRepository,
            IRepository<TakenBooks> takenBooksRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _genreRepository = genreRepository;
            _booksRepository = booksRepository;
            _booksOnShelvesRepository = booksOnShelvesRepository;
            _takenBooksRepository = takenBooksRepository;
        }

        private async Task GenerateRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                   RoleID = 1,
                   RoleName = "Пользователь",
                   RoleDescription = "Роль стандартного пользователя системы с доступам только к основным частям системы или же студент без подтверждённого студенческого билета."
                },

                new Role()
                {
                   RoleID = 2,
                   RoleName = "Администратор",
                   RoleDescription = "Роль главного администратора с доступом ко всей системе."
                },
                new Role()
                {
                   RoleID = 3,
                   RoleName = "Ученик",
                   RoleDescription = "Роль ученика, с подтверждённым студенческим билетом, с возможностью бранировать книги."
                },
                new Role()
                {
                   RoleID = 4,
                   RoleName = "Библиотекарь",
                   RoleDescription = "Роль библиотекаря, выдающего и принимающего книги."
                }

            };

            foreach (var role in roles)
            {
                await _roleRepository.CreateAsync(role);
            }
            await _roleRepository.SaveAsync();
        }

        private async Task GenerateUsers()
        {
            string adminPassword = "Biblio_ADmin123";
            byte[] adminPasswordSalt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(adminPasswordSalt);
            }
            string hashedAdmin = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: adminPassword, salt: adminPasswordSalt, prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));

            string testPassword = "Biblio_TEst456";
            byte[] testPasswordSalt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(testPasswordSalt);
            }
            string hashedTest = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: testPassword, salt: testPasswordSalt, prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));

            string student1Password = "Biblio_ST1";
            byte[] student1PasswordSalt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(student1PasswordSalt);
            }
            string hashedStudent1 = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: student1Password, salt: student1PasswordSalt, prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));

            string student2Password = "Biblio_ST2";
            byte[] student2PasswordSalt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(student2PasswordSalt);
            }
            string hashedStudent2 = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: student2Password, salt: student2PasswordSalt, prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));

            string librarianPassword = "Biblio_LB";
            byte[] librarianPasswordSalt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(librarianPasswordSalt);
            }
            string hashedTeacher = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: librarianPassword, salt: librarianPasswordSalt, prf: KeyDerivationPrf.HMACSHA1, iterationCount: 10000, numBytesRequested: 256 / 8));

            var users = new List<User>()
            {
                new User()
                {
                    UserId = 1,
                    RegistrationDate = DateTime.Now,
                    Email = "admin@mail.ru",
                    PasswordSalt = adminPasswordSalt,
                    PasswordHash = hashedAdmin,
                    LastSiteAccessDate = DateTime.Now,
                    Name = "Main",
                    SurName = "Administrator",
                    StudentIDNumber= "1231241",
                    ConfirmedStudentId = true
                },

                new User()
                {
                    UserId = 2,
                    RegistrationDate = DateTime.Now,
                    Email = "test@mail.ru",
                    PasswordSalt = testPasswordSalt,
                    PasswordHash = hashedTest,
                    LastSiteAccessDate = DateTime.Now,
                    Name = "Test",
                    SurName = "User"
                },

                new User()
                {
                    UserId = 3,
                    RegistrationDate = DateTime.Now,
                    Email = "student1@mail.ru",
                    PasswordSalt = student1PasswordSalt,
                    PasswordHash = hashedStudent1,
                    LastSiteAccessDate = DateTime.Now,
                    Name = "Stundent",
                    SurName = "One"
                },

                new User()
                {
                    UserId = 4,
                    RegistrationDate = DateTime.Now,
                    Email = "student2@mail.ru",
                    PasswordSalt = student2PasswordSalt,
                    PasswordHash = hashedStudent2,
                    LastSiteAccessDate = DateTime.Now,
                    Name = "Stundent",
                    SurName = "Two"
                },

                new User()
                {
                    UserId = 5,
                    RegistrationDate = DateTime.Now,
                    Email = "librarian@mail.ru",
                    PasswordSalt = librarianPasswordSalt,
                    PasswordHash = hashedTeacher,
                    LastSiteAccessDate = DateTime.Now,
                    Name = "Test",
                    SurName = "Librarian"
                },
            };

            foreach (var user in users)
            {
                await _userRepository.CreateAsync(user);
            }
            await _userRepository.SaveAsync();
        }
        private async Task GenerateUserRoles()
        {
            Role User = await _roleRepository.GetAsync(1);
            Role Admin = await _roleRepository.GetAsync(2);
            Role Student = await _roleRepository.GetAsync(3);
            Role Librarian = await _roleRepository.GetAsync(4);

            var userRoles = new List<UserRole>()
            {
                new UserRole()
                {
                    UserRoleID = 1,
                    RoleID = Admin.RoleID,
                    Role = Admin,
                    UserID = 1
                },
                new UserRole()
                {
                    UserRoleID = 2,
                    RoleID = User.RoleID,
                    Role = User,
                    UserID = 1
                },
                new UserRole()
                {
                    UserRoleID = 3,
                    RoleID = User.RoleID,
                    Role = User,
                    UserID = 2
                },
                new UserRole()
                {
                    UserRoleID = 4,
                    RoleID = User.RoleID,
                    Role = User,
                    UserID = 3
                },
                new UserRole()
                {
                    UserRoleID = 5,
                    RoleID = Student.RoleID,
                    Role = Student,
                    UserID = 3
                },
                new UserRole()
                {
                    UserRoleID = 6,
                    RoleID = User.RoleID,
                    Role = User,
                    UserID = 4
                },
                new UserRole()
                {
                    UserRoleID = 7,
                    RoleID = Student.RoleID,
                    Role = Student,
                    UserID = 4
                },
                new UserRole()
                {
                    UserRoleID = 8,
                    RoleID = User.RoleID,
                    Role = User,
                    UserID = 5
                },
                new UserRole()
                {
                    UserRoleID = 9,
                    RoleID = Librarian.RoleID,
                    Role = Librarian,
                    UserID = 5
                }
            };

            foreach (var userRole in userRoles)
            {
                await _userRoleRepository.CreateAsync(userRole);
            }
            await _userRoleRepository.SaveAsync();
        }

        private async Task GenerateGenres()
        {
            var genres = new List<Genre>()
            {
                new Genre()
                {
                    GenreID = 1,
                    GenreName = "История",
                    IsSubGenre = false
                },
                new Genre()
                {
                    GenreID = 2,
                    GenreName = "Техника",
                    IsSubGenre = false
                },
                new Genre()
                {
                    GenreID = 3,
                    GenreName = "Проза",
                    IsSubGenre = false
                }

            };

            foreach (var genre in genres)
            {
                await _genreRepository.CreateAsync(genre);
            }
            await _genreRepository.SaveAsync();

            Genre History = await _genreRepository.GetAsync(1);
            Genre Tech = await _genreRepository.GetAsync(2);
            Genre Prose = await _genreRepository.GetAsync(3);

            var subGenres = new List<Genre>()
            {
                new Genre()
                {
                    GenreID = 4,
                    GenreName = "Документальная литература",
                    IsSubGenre = true,
                    MainGenre = History
                },
                new Genre()
                {
                    GenreID = 5,
                    GenreName = "Металлургия",
                    IsSubGenre = true,
                    MainGenre = Tech
                },
                new Genre()
                {
                    GenreID = 6,
                    GenreName = "Роман",
                    IsSubGenre = true,
                    MainGenre = Prose
                }

            };

            foreach (var genre in subGenres)
            {
                await _genreRepository.CreateAsync(genre);
            }
            await _genreRepository.SaveAsync();
        }

        private async Task GenerateBooks()
        {
            Genre History = await _genreRepository.GetAsync(1);
            Genre Tech = await _genreRepository.GetAsync(2);
            Genre Prose = await _genreRepository.GetAsync(3);
            Genre DocLit = await _genreRepository.GetAsync(4);
            Genre Metall = await _genreRepository.GetAsync(5);
            Genre Novel = await _genreRepository.GetAsync(6);
            var books = new List<Book>()
            {
                new Book()
                {
                    BookId = 1,
                    BookAuthor = "Толстой Алексей Николаевич",
                    BookCoverPath = "/files/bookCover/1.jpg",
                    BookPDFPath = "/files/bookPDF/1.pdf",
                    BookTitle = "Петр Первый",
                    Genre = History,
                    GenreId = 1
                },
                new Book()
                {
                    BookId = 2,
                    BookAuthor = "Шепелев Александр Михайлович",
                    BookCoverPath = "/files/bookCover/2.jpg",
                    BookPDFPath = "/files/bookPDF/2.pdf",
                    BookTitle = "Как построить сельский дом",
                    Genre = Tech,
                    GenreId = 2
                },
                new Book()
                {
                    BookId = 3,
                    BookAuthor = "Паустовский Константин Георгиевич",
                    BookCoverPath = "/files/bookCover/3.jpg",
                    BookPDFPath = "/files/bookPDF/3.pdf",
                    BookTitle = "Телеграмма",
                    Genre = Prose,
                    GenreId = 3
                },
                new Book()
                {
                    BookId = 4,
                    BookAuthor = "Толстой Алексей Николаевич",
                    BookCoverPath = "/files/bookCover/1.jpg",
                    BookPDFPath = "/files/bookPDF/1.pdf",
                    BookTitle = "Петр Первый",
                    Genre = History,
                    GenreId = 1
                },
                new Book()
                {
                    BookId = 5,
                    BookAuthor = "Толстой Алексей Николаевич",
                    BookCoverPath = "/files/bookCover/1.jpg",
                    BookPDFPath = "/files/bookPDF/1.pdf",
                    BookTitle = "Петр Первый",
                    Genre = History,
                    GenreId = 1
                },
                new Book()
                {
                    BookId = 6,
                    BookAuthor = "Толстой Алексей Николаевич",
                    BookCoverPath = "/files/bookCover/1.jpg",
                    BookPDFPath = "/files/bookPDF/1.pdf",
                    BookTitle = "Петр Первый",
                    Genre = History,
                    GenreId = 1
                }

            };

            foreach (var book in books)
            {
                await _booksRepository.CreateAsync(book);
            }
            await _booksRepository.SaveAsync();
        }

        private async Task GenerateBooksOnShelves()
        {
            var booksOnShelves = new List<BooksOnShelves>()
            {
                new BooksOnShelves()
                {
                    BookOnShelfID = 1,
                    BookId = 1,
                    ShelfNumber = 1,
                    IsAvailable = false
                },
                new BooksOnShelves()
                {
                    BookOnShelfID = 2,
                    BookId = 2,
                    ShelfNumber = 2,
                    IsAvailable = true
                },
                new BooksOnShelves()
                {
                    BookOnShelfID = 3,
                    BookId = 3,
                    ShelfNumber = 3,
                    IsAvailable = false
                },
                new BooksOnShelves()
                {
                    BookOnShelfID = 4,
                    BookId = 4,
                    ShelfNumber = 2,
                    IsAvailable = true
                },
                new BooksOnShelves()
                {
                    BookOnShelfID = 5,
                    BookId = 5,
                    ShelfNumber = 2,
                    IsAvailable = true
                },
                new BooksOnShelves()
                {
                    BookOnShelfID = 6,
                    BookId = 6,
                    ShelfNumber = 1,
                    IsAvailable = true
                }
            };
            foreach(var bookOnShelfe in booksOnShelves)
            {
                await _booksOnShelvesRepository.CreateAsync(bookOnShelfe);
            }
            await _booksOnShelvesRepository.SaveAsync();
        }

        private async Task GenerateTakenBooks()
        {
            Book firstBook = await _booksRepository.GetAsync(1);
            Book secondtBook = await _booksRepository.GetAsync(4);
            var takenBooks = new List<TakenBooks>()
            {
                new TakenBooks()
                {
                    TakenBooksId = 1,
                    BookOnShelfID = 1,
                    DateOfTaking = DateTime.Now,
                    UserId = 1,
                    Returned = false,
                    Book = firstBook
                },
                new TakenBooks()
                {
                    TakenBooksId = 2,
                    BookOnShelfID = 4,
                    DateOfTaking = DateTime.Now,
                    UserId = 1,
                    Returned = false,
                    Book =  secondtBook
                }
            };
            foreach(var takenBook in takenBooks)
            {
                await _takenBooksRepository.CreateAsync(takenBook);
            }
            await _takenBooksRepository.SaveAsync();
        }

        public async Task Generate()
        {
            await GenerateRoles();
            await GenerateUsers();
            await GenerateUserRoles();
            await GenerateGenres();
            await GenerateBooks();
            await GenerateBooksOnShelves();
            await GenerateTakenBooks();
        }
    }
}
