using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class ClientModel
    {
        [Required]
        public string Id { get; set; } = null!;
        public string? ImageFileName { get; set; }
        [Required]
        public string ClientName { get; set; } = null!;
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public ClientInformationModel Information { get; set; } = null!;
        [Required]
        public ClientAddressModel Address { get; set; } = null!;
    }
}
