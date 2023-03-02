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
    public class RoleRepository : BaseRepository, IRepository<Role>
    {
        public RoleRepository(BiblioDbContext context) : base(context)
        {
        }

        public async Task CreateAsync(Role item)
        {
            await _context.Roles.AddAsync(item);
        }

        public Task DeleteAsync(int id)
        {
            var role = new Role() { RoleID = id };
            _context.Roles.Remove(role);
            return Task.CompletedTask;
        }

        public async Task<Role?> GetAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<IEnumerable<Role>> ListAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public Task UpdateAsync(Role item)
        {
            _context.Roles.Update(item);
            return Task.CompletedTask;
        }
    }
}
