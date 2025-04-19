using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos
{
    public class AddUserForm
    {
        public IFormFile? ImageFileName { get; set; }
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
        public string? Phone { get; set; }
        [Required]
        public string JobTitle { get; set; } = null!;
        [Required]
        public string Role { get; set; } = null!;
        [Required]
        [MinLength(2, ErrorMessage = "Length must be at least 2 characters.")]
        public string StreetName { get; set; } = null!;
        [Required]
        [MinLength(2, ErrorMessage = "Length must be at least 2 characters.")]
        public string PostalCode { get; set; } = null!;
        [Required]
        [MinLength(2, ErrorMessage = "Length must be at least 2 characters.")]
        public string City { get; set; } = null!;
    }
}
