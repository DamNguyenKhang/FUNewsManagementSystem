using AutoMapper;
using BusinessObject.Entities;
using FUNewsManagementSystemMVC.Models;

namespace FUNewsManagementSystemMVC.Mappers
{
    public class NewsArticleMapper : Profile
    {
        public NewsArticleMapper()
        {
            CreateMap<CreateNewsArticleViewModel, NewsArticle>()
                .ForMember(dest => dest.NewsArticleImage, opt => opt.Ignore()) // Handled manually for file/url logic
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())    // Handled manually from session
                .ForMember(dest => dest.Tags, opt => opt.Ignore());           // Handled manually via tagIds

            CreateMap<UpdateNewsArticleViewModel, NewsArticle>()
                .ForMember(dest => dest.NewsArticleImage, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore());

            CreateMap<NewsArticle, UpdateNewsArticleViewModel>()
                .ForMember(dest => dest.TagIds, opt => opt.MapFrom(src => src.Tags.Select(t => t.Id).ToList()))
                .ForMember(dest => dest.NewsArticleId, opt => opt.MapFrom(src => src.Id));

            CreateMap<NewsArticle, NewsArticle>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
        }
    }
}
