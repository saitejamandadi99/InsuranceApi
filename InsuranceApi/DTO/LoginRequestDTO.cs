using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace InsuranceApi.DTO
{
    public class LoginRequestDTO
    {

        [Required(ErrorMessage = "Please enter the Email Address")]
        [EmailAddress(ErrorMessage = "Email address is invalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the password")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "password must be between 8 to 20 characters")]
        public string Password { get; set; } = string.Empty;
    }
}
