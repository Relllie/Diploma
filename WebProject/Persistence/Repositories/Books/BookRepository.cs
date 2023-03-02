using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebProject.Domain.Models.Books;
using WebProject.Domain.Repositories;
using WebProject.Persistence.Contexts;

namespace WebProject.Persistence.Repositories
{
    public class BookRepository : BaseRepository, IRepository<Book>
    {
        public BookRepository(BiblioDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(Book item)
        {
            await _context.Books.AddAsync(item);
        }

        public Task DeleteAsync(int id)
        {
            var book = new Book() { BookId = id };
            _context.Books.Remove(book);
            return Task.CompletedTask;
        }

        public async Task<Book?> GetAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<IEnumerable<Book>> ListAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public Task UpdateAsync(Book item)
        {
            _context.Books.Update(item);
            return Task.CompletedTask;
        }
    }
}
