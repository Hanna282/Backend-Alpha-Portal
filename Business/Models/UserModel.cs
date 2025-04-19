using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class UserModel
    {
        [Required]
        public string Id { get; set; } = null!;
        public string? ImageFileName { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public UserInformationModel Information { get; set; } = null!;
        [Required]
        public UserAddressModel Address { get; set; } = null!;
    }
}
