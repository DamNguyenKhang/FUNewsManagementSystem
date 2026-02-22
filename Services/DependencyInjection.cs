using Microsoft.Extensions.DependencyInjection;
using Services.Abstractions;

namespace Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IFileStorageService, CloudinaryStorageService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISystemAccountService, SystemAccountService>();
            services.AddScoped<INewsArticleService, NewsArticleService>();
            services.AddScoped<ITagService, TagService>();
            return services;
        }
    }
}
