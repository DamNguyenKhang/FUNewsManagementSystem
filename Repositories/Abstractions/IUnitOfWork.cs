using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Abstractions
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
