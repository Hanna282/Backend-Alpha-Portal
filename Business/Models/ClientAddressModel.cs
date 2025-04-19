using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class ClientAddressModel
    {
        [Required]
        public string ClientId { get; set; } = null!;
        [Required]
        public string StreetName { get; set; } = null!;
        [Required]
        public string PostalCode { get; set; } = null!;
        [Required]
        public string City { get; set; } = null!;
    }
}
