using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class UserAddressModel
    {
        [Required]
        public string UserId { get; set; } = null!;
        public string? StreetName { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
    }
}
