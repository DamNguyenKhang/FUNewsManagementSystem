using BusinessObject.Entities;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using Repositories.Abstractions;

namespace Repositories
{
    public class TagRepository : Repository<Tag, int>, ITagRepository
    {
        public TagRepository(FunewsManagementContext context) : base(context)
        {
        }

        public async Task<Tag?> GetTagByName(string tagName)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.TagName == tagName);
        }
    }
}
