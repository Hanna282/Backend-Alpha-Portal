using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos
{
    public class SignUpForm
    {
        [Required]
        [MinLength(2, ErrorMessage = "Length must be at least 2 characters.")]
        public string FirstName { get; set; } = null!;

        [Required]
        [MinLength(2, ErrorMessage = "Length must be at least 2 characters.")]
        public string LastName { get; set; } = null!;

        [Required]
        [MinLength(5, ErrorMessage = "Length must be at least 5 characters.")]
        [RegularExpression("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(8, ErrorMessage = "Length must be at least 8 characters.")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[\\W_]).{8,}$", ErrorMessage = "Invalid password")]
        public string Password { get; set; } = null!;

        [Required]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        public bool TermsAndConditions { get; set; }
    }
}
