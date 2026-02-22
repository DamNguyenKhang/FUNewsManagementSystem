using BusinessObject.Enums;
using System;
using System.Collections.Generic;

namespace BusinessObject.Entities;

public partial class SystemAccount : IEntity<short>
{
    public short Id { get; set; }

    public string? AccountName { get; set; }

    public string? AccountEmail { get; set; }

    public AccountRole? AccountRole { get; set; }

    public string? AccountPassword { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}
