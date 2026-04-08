using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesiPortal.Models
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(UserName), IsUnique = true)]

    public class UserAccount
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "Max 50 characters is allowed.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Max 50 characters is allowed.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [MaxLength(50, ErrorMessage = "Max 50 characters is allowed.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [MaxLength(50, ErrorMessage = "Max 50 characters is allowed.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(50, ErrorMessage = "Max 50 characters is allowed.")]
        public string UserName { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
