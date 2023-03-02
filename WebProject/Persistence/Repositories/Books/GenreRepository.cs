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
    public class GenreRepository : BaseRepository, IRepository<Genre>
    {
        public GenreRepository(BiblioDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(Genre item)
        {
            await _context.Genres.AddAsync(item);
        }

        public Task DeleteAsync(int id)
        {
            var genre = new Genre() { GenreID = id };
            _context.Genres.Remove(genre);
            return Task.CompletedTask;
        }

        public async Task<Genre?> GetAsync(int id)
        {
            return await _context.Genres.FindAsync(id);
        }

        public async Task<IEnumerable<Genre>> ListAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public Task UpdateAsync(Genre item)
        {
            _context.Genres.Update(item);
            return Task.CompletedTask;
        }
    }
}
