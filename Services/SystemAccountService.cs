using BusinessObject.Entities;
using BusinessObject.Enums;
using Repositories.Abstractions;
using Services.Abstractions;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class SystemAccountService : ISystemAccountService
    {
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SystemAccountService(ISystemAccountRepository systemAccountRepository, IUnitOfWork unitOfWork)
        {
            _systemAccountRepository = systemAccountRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SystemAccount>> GetAllAccounts(AccountRole? role = null, string? search = null)
        {
            return await _systemAccountRepository.GetAllAsync(role, search);
        }

        public async Task<bool> EmailExists(string email)
        {
            return await _systemAccountRepository.EmailExists(email);
        }

        public async Task<SystemAccount?> GetAccountByEmail(string email)
        {
            return await _systemAccountRepository.GetAccountByEmail(email);
        }

        public async Task<SystemAccount?> GetAccountById(short id)
        {
            return await _systemAccountRepository.GetByIdAsync(id);
        }

        public async Task AddAccount(SystemAccount account)
        {
            await _systemAccountRepository.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAccount(SystemAccount account)
        {
            await _systemAccountRepository.UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAccount(short id)
        {
            var account = await _systemAccountRepository.GetByIdAsync(id);
            if (account != null)
            {
                await _systemAccountRepository.DeleteAsync(account);
                await _unitOfWork.SaveChangesAsync();
            }
        }

    }
}
