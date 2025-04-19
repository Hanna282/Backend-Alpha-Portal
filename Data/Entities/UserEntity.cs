using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class UserEntity : IdentityUser
    {
        [PersonalData]
        public string? ImageFileName { get; set; }

        [Column(TypeName = "date")]
        public DateTime Created { get; set; } = DateTime.Now;

        public virtual UserInformationEntity Information { get; set; } = null!;
        public virtual UserAddressEntity Address { get; set; } = null!;
        public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
    }
}
