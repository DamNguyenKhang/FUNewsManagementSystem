using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FUNewsManagementSystemMVC.Models
{
    public class UpdateNewsArticleViewModel
    {
        [Required]
        public string NewsArticleId { get; set; } = null!;

        [Required(ErrorMessage = "News Title is required")]
        [MaxLength(400, ErrorMessage = "News Title cannot exceed 400 characters")]
        public string NewsTitle { get; set; } = null!;

        [Required(ErrorMessage = "Headline is required")]
        [MaxLength(150, ErrorMessage = "Headline cannot exceed 150 characters")]
        public string Headline { get; set; } = null!;

        [Required(ErrorMessage = "News Content is required")]
        [MaxLength(4000, ErrorMessage = "News Content cannot exceed 4000 characters")]
        public string NewsContent { get; set; } = null!;

        [MaxLength(400, ErrorMessage = "News Source cannot exceed 400 characters")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public short CategoryId { get; set; }

        public bool NewsStatus { get; set; }

        [MaxLength(400, ErrorMessage = "Image URL cannot exceed 400 characters")]
        public string? NewsArticleImage { get; set; }

        public IFormFile? ImageFile { get; set; }

        public List<int> TagIds { get; set; } = new List<int>();
    }
}
