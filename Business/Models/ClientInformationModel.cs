using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class ClientInformationModel
    {
        [Required]
        public string ClientId { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Reference { get; set; }
    }
}
