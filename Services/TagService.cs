using BusinessObject.Entities;
using Repositories;
using Repositories.Abstractions;
using Services.Abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TagService(ITagRepository tagRepository, IUnitOfWork unitOfWork)
        {
            _tagRepository = tagRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Tag>> GetAllTags()
        {
            return await _tagRepository.GetAllAsync();
        }

        public async Task<Tag> AddTag(Tag tag)
        {
            var newTag = await _tagRepository.AddAsync(tag);
            await _unitOfWork.SaveChangesAsync();
            return newTag;
        }

        public async Task<Tag?> GetTagByName(string tagName)
        {
            return await _tagRepository.GetTagByName(tagName);
        }
        public async Task<Tag?> GetTagById(int id)
        {
            return await _tagRepository.GetByIdAsync(id);
        }
    }
}
