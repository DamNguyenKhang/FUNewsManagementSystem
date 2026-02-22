using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Category : IEntity<short>
{
    public short Id { get; set; }

    public string CategoryName { get; set; } = null!;

    public string CategoryDesciption { get; set; } = null!;

    public short? ParentId { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();

    public virtual Category? Parent { get; set; }
}
