using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<SystemAccount?> Login(string email, string password);
    }
}
