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
    public class TakenBooksRepository : BaseRepository, IRepository<TakenBooks>
    {
        public TakenBooksRepository(BiblioDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(TakenBooks item)
        {
            await _context.TakenBooks.AddAsync(item);
        }

        public Task DeleteAsync(int id)
        {
            var takenBook = new TakenBooks() { TakenBooksId = id };
            _context.TakenBooks.Remove(takenBook);
            return Task.CompletedTask;
        }

        public async Task<TakenBooks?> GetAsync(int id)
        {
            return await _context.TakenBooks.FindAsync(id);
        }

        public async Task<IEnumerable<TakenBooks>> ListAsync()
        {
            return await _context.TakenBooks.ToListAsync();
        }

        public Task UpdateAsync(TakenBooks item)
        {
            _context.TakenBooks.Update(item);
            return Task.CompletedTask;
        }
    }
}
