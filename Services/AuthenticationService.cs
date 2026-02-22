using BusinessObject.Entities;
using Microsoft.Extensions.Configuration;
using Repositories.Abstractions;
using Services.Abstractions;

namespace Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ISystemAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationService(ISystemAccountRepository accountRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<SystemAccount?> Login(string email, string password)
        {
            var account = await _accountRepository.GetAccountByEmail(email);
            if (account != null && password == account.AccountPassword)
            {
                return account;
            }
            return null;
        }
    }
}
