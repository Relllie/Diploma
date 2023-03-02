using WebProject.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Persistence.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly BiblioDbContext _context;

        protected BaseRepository(BiblioDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
