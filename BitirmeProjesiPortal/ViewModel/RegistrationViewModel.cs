using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesiPortal.ViewModel
{
    public class RegistrationViewModel
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
        [DataType(DataType.EmailAddress)]
        [MaxLength(50, ErrorMessage = "Max 50 characters is allowed.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MaxLength(20, ErrorMessage = "Max 20 characters is allowed.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(50, ErrorMessage = "Max 50 characters is allowed.")]
        public string UserName { get; set; }
        [MaxLength(20, ErrorMessage = "Max 20 characters is allowed.")]
        [Compare("Password", ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
