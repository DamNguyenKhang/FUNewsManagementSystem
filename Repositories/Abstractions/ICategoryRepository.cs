using BusinessObject.Entities;

namespace Repositories.Abstractions
{
    public interface ICategoryRepository : IRepository<Category, short>
    {
        Task<List<Category>> GetAllAsync(bool? status = null, string? keyword = null);
    }
}
