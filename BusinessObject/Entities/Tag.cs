using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class Tag : IEntity<int>
{
    public int Id { get; set; }

    public string? TagName { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
