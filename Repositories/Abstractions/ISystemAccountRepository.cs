using BusinessObject.Entities;
using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Abstractions
{
    public interface ISystemAccountRepository : IRepository<SystemAccount, short>
    {
        Task<SystemAccount?> GetAccountByEmail(string email);
        Task<SystemAccount?> GetAccountByEmailAndPassword(string email, string password);
        Task<bool> EmailExists(string email);

        Task<List<SystemAccount>> GetAllAsync(AccountRole? role = null, string? keyword = null);
    }
}
