using System.ComponentModel.DataAnnotations;

namespace FUNewsManagementSystemMVC.Models
{
    public class StaffProfileViewModel
    {
        public short Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        public string AccountName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string AccountEmail { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Department { get; set; }

        public string? Bio { get; set; }

        public string? Status { get; set; } = "Active";
        public DateTime? JoinedDate { get; set; }
        public DateTime? LastLogin { get; set; }

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "New password must be at least 8 characters")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string? ConfirmNewPassword { get; set; }
    }
}
