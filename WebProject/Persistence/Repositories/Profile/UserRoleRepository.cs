using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebProject.Domain.Models;
using WebProject.Persistence.Contexts;
using WebProject.Domain.Repositories;

namespace WebProject.Persistence.Repositories
{
    public class UserRoleRepository : BaseRepository, IRepository<UserRole>
    {
        public UserRoleRepository(BiblioDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(UserRole item)
        {
            await _context.UserRoles.AddAsync(item);
        }

        public Task DeleteAsync(int id)
        {
            var userRole = new UserRole() { UserRoleID = id };
            _context.UserRoles.Remove(userRole);
            return Task.CompletedTask;
        }

        public async Task<UserRole?> GetAsync(int id)
        {
            return await _context.UserRoles.FindAsync(id);
        }

        public async Task<IEnumerable<UserRole>> ListAsync()
        {
            return await _context.UserRoles.ToListAsync();
        }

        public Task UpdateAsync(UserRole item)
        {
            _context.UserRoles.Update(item);
            return Task.CompletedTask;
        }
    }
}
