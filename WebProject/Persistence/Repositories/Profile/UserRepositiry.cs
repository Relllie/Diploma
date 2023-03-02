using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebProject.Domain.Models;
using WebProject.Persistence.Contexts;
using System.Threading.Tasks;
using WebProject.Domain.Repositories;
using System;

namespace WebProject.Persistence.Repositories
{
    public class UserRepository : BaseRepository, IRepository<User>
    {
        public UserRepository(BiblioDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(User item)
        {
            await _context.Users.AddAsync(item);
        }

        public Task DeleteAsync(int id)
        {
            var user = new User() { UserId = id };
            _context.Users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task<User?> GetAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<User>> ListAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public Task UpdateAsync(User item)
        {
            _context.Users.Update(item);
            return Task.CompletedTask;
        }
    }
}
