using BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Abstractions
{
    public interface ITagRepository : IRepository<Tag, int>
    {
        Task<Tag?> GetTagByName(string tagName);
    }
}
