using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystemMVC.Models
{
    public class CreateTagViewModel
    {
        [Required(ErrorMessage = "Tag Name is required")]
        [StringLength(50, ErrorMessage = "Tag Name cannot exceed 50 characters")]
        public string TagName { get; set; } = null!;

        [StringLength(400, ErrorMessage = "Note cannot exceed 400 characters")]
        public string? Note { get; set; }
    }
}
