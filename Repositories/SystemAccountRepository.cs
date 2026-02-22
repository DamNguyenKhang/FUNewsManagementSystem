using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class SystemAccountRepository : Repository<SystemAccount, short>, ISystemAccountRepository
    {
        public SystemAccountRepository(FunewsManagementContext context) : base(context)
        {
        }

        public async Task<SystemAccount?> GetAccountByEmail(string email)
        {
            return await _dbSet.SingleOrDefaultAsync(a => a.AccountEmail == email);
        }

        public async Task<SystemAccount?> GetAccountByEmailAndPassword(string email, string password)
        {
            return await _dbSet.SingleOrDefaultAsync(a => a.AccountEmail == email && a.AccountPassword == password);
        }

        public async Task<bool> EmailExists(string email)
        {
            return await _dbSet.AnyAsync(a => a.AccountEmail == email);
        }

        public async Task<List<SystemAccount>> GetAllAsync(AccountRole? role = null, string? keyword = null)
        {
            IQueryable<SystemAccount> query = _context.SystemAccounts;

            if (role.HasValue)
            {
                query = query.Where(x => x.AccountRole == role.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim().ToLower();

                query = query.Where(x =>
                    x.AccountName.ToLower().Contains(keyword) ||
                    x.AccountEmail.ToLower().Contains(keyword));
            }

            return await query
                .OrderBy(x => x.AccountName)
                .ToListAsync();
        }
    }
}
