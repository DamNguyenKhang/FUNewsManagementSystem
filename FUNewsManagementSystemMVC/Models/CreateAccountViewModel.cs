using System.ComponentModel.DataAnnotations;
using BusinessObject.Enums;

namespace FUNewsManagementSystemMVC.Models
{
    public class CreateAccountViewModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name")]
        public string AccountName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string AccountEmail { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [Display(Name = "Password")]
        public string AccountPassword { get; set; } = null!;

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public int AccountRole { get; set; } // Using int to map to Enum

        public bool IsActive { get; set; } = true;
    }
}
