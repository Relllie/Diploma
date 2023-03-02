using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Domain.Repositories
{
    public interface IRepository<T>
        where T: class
    {
        Task<IEnumerable<T>> ListAsync();

        Task<T?> GetAsync(int id);

        Task CreateAsync(T item);

        Task UpdateAsync(T item);

        Task DeleteAsync(int id);

        Task SaveAsync();
    }
}
