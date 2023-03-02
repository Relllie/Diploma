using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebProject.Domain.Repositories;
using WebProject.Persistence.Contexts;

namespace WebProject.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BiblioDbContext _context;

        public UnitOfWork(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
