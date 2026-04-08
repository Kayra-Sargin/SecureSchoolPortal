using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BitirmeProjesiPortal.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username/Email is required.")]
        [MaxLength(50, ErrorMessage = "Max 50 characters is allowed.")]
        [DisplayName("UserName/Email")]
        public string UserNameOrEmail { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MaxLength(20, ErrorMessage = "Max 20 characters is allowed.")]
        public string Password { get; set; }
    }
}
