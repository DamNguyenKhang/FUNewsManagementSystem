using AutoMapper;
using BusinessObject.Entities;
using FUNewsManagementSystemMVC.Models;

namespace FUNewsManagementSystemMVC.Mappers
{
    public class AccountMapper : Profile
    {
        public AccountMapper()
        {
            CreateMap<CreateAccountViewModel, SystemAccount>();
            CreateMap<EditAccountViewModel, SystemAccount>();
            CreateMap<SystemAccount, StaffProfileViewModel>().ReverseMap();
        }
    }
}
