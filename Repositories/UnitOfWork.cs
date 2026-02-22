using DataAccessObject;
using Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FunewsManagementContext _dbContext;

        public UnitOfWork(FunewsManagementContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _dbContext.SaveChangesAsync(ct);
    }
}
