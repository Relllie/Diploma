using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Domain.Repositories
{
    interface IUnitOfWork
    {
        Task SaveAsync();
    }
}
