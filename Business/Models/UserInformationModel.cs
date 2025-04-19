using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class UserInformationModel
    {
        [Required]
        public string UserId { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? JobTitle { get; set; }
        public string? Role { get; set; }
    }
}
