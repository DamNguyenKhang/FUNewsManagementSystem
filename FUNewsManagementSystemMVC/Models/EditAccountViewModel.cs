using System.ComponentModel.DataAnnotations;
using BusinessObject.Enums;

namespace FUNewsManagementSystemMVC.Models
{
    public class EditAccountViewModel
    {
        [Required]
        public short Id { get; set; }

        [Required]
        [StringLength(100)]
        public string AccountName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string AccountEmail { get; set; } = null!;

        [StringLength(100, MinimumLength = 6)]
        public string? AccountPassword { get; set; }

        [Required]
        public AccountRole AccountRole { get; set; }
    }
}
