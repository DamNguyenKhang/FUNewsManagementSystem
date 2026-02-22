using BusinessObject.Entities;

namespace Services.Abstractions
{
    public interface ISystemAccountService
    {
        Task<List<SystemAccount>> GetAllAccounts(BusinessObject.Enums.AccountRole? role = null, string? search = null);
        Task<bool> EmailExists(string email);
        Task<SystemAccount?> GetAccountByEmail(string email);
        Task<SystemAccount?> GetAccountById(short id);
        Task AddAccount(SystemAccount account);
        Task UpdateAccount(SystemAccount account);
        Task DeleteAccount(short id);
    }
}
