using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace FUNewsManagementSystemMVC.Models
{
    public class PreviewNewsViewModel
    {
        public string? NewsArticleId { get; set; }
        public string NewsTitle { get; set; } = null!;
        public string Headline { get; set; } = null!;
        public string NewsContent { get; set; } = null!;
        public string? NewsSource { get; set; }
        public short CategoryId { get; set; }
        public string? NewsArticleImage { get; set; }
        public IFormFile? ImageFile { get; set; }
        public List<int> TagIds { get; set; } = new List<int>();
    }
}
