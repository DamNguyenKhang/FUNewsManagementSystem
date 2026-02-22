using BusinessObject.Entities;
using BusinessObject.Enums;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class CategoryRepository : Repository<Category, short>, ICategoryRepository
    {
        public CategoryRepository(FunewsManagementContext context) : base(context)
        {
        }

        public async Task<List<Category>> GetAllAsync(bool? status = null, string? keyword = null)
        {
            var query = _context.Categories.AsQueryable();

            // Filter theo status
            if (status.HasValue)
            {
                query = query.Where(c => c.IsActive == status.Value);
            }

            // Search theo keyword (Name + Description)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(c =>
                    EF.Functions.Like(c.CategoryName, $"%{keyword}%") ||
                    EF.Functions.Like(c.CategoryDesciption, $"%{keyword}%"));
            }

            return await query
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }
    }
}
