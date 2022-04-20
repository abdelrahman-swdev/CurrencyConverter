using System.ComponentModel.DataAnnotations;

namespace CurrencyConverter.Service.DTOs
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression("(?=^.{8,10}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\\s).*$",
        ErrorMessage = "Password must have 1 uppercase, 1 lowercase, 1 number, 1 nonalphanumeric and atleast 8 characters")]
        public string Password { get; set; }

        public string PhoneNumber { get; set; }
    }
}
