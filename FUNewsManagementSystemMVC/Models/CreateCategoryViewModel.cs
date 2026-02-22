using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystemMVC.Models
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100, ErrorMessage = "Category Name cannot exceed 100 characters")]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = null!;

        [Required(ErrorMessage = "Category Description is required")]
        [StringLength(250, ErrorMessage = "Category Description cannot exceed 250 characters")]
        [Display(Name = "Description")]
        public string CategoryDesciption { get; set; } = null!;

        public short? ParentId { get; set; }

        [Display(Name = "Active and Visible")]
        public bool IsActive { get; set; } = true;
    }
}
