using BusinessObject.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategories(bool? isActive = null, string? search = null);
        Task<Category?> GetCategoryById(short id);
        Task AddCategory(Category category);
        Task UpdateCategory(Category category);
        Task DeleteCategory(short id);
    }
}
