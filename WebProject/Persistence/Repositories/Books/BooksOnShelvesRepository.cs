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
    public class BooksOnShelvesRepository : BaseRepository, IRepository<BooksOnShelves>
    {
        public BooksOnShelvesRepository(BiblioDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(BooksOnShelves item)
        {
            await _context.BooksOnShelves.AddAsync(item);
        }

        public Task DeleteAsync(int id)
        {
            var bookOnShelf = new BooksOnShelves() { BookOnShelfID = id };
            _context.BooksOnShelves.Remove(bookOnShelf);
            return Task.CompletedTask;
        }

        public async Task<BooksOnShelves?> GetAsync(int id)
        {
            return await _context.BooksOnShelves.FindAsync(id);
        }

        public async Task<IEnumerable<BooksOnShelves>> ListAsync()
        {
            return await _context.BooksOnShelves.ToListAsync();
        }

        public Task UpdateAsync(BooksOnShelves item)
        {
            _context.BooksOnShelves.Update(item);
            return Task.CompletedTask;
        }
    }
}
