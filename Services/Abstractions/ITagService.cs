using BusinessObject.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface ITagService
    {
        Task<List<Tag>> GetAllTags();
        Task<Tag> AddTag(Tag tag);
        Task<Tag?> GetTagByName(string tagName);
        Task<Tag?> GetTagById(int id);
    }
}
