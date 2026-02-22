using BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using Repositories.Abstractions;
using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Category>> GetAllCategories(bool? isActive = null, string? search = null)
        {
            return await _categoryRepository.GetAllAsync(isActive, search);
        }

        public async Task<Category?> GetCategoryById(short id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task AddCategory(Category category)
        {
            await _categoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateCategory(Category category)
        {
            await _categoryRepository.UpdateAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategory(short id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
            {
                await _categoryRepository.DeleteAsync(category);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
