using AutoMapper;
using BusinessObject.Entities;
using FUNewsManagementSystemMVC.Models;

namespace FUNewsManagementSystemMVC.Mappers
{
    public class CategoryMapper : Profile
    {
        public CategoryMapper()
        {
            CreateMap<CreateCategoryViewModel, Category>();
            CreateMap<UpdateCategoryViewModel, Category>();
        }
    }
}
