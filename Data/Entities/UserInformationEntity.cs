using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class UserInformationEntity
    {
        [PersonalData]
        [Key, ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public virtual UserEntity User { get; set; } = null!;

        [ProtectedPersonalData]
        public string FirstName { get; set; } = null!;

        [ProtectedPersonalData]
        public string LastName { get; set; } = null!;

        [ProtectedPersonalData]
        public string? Phone { get; set; }

        [ProtectedPersonalData]
        public string? JobTitle { get; set; }

        [ProtectedPersonalData]
        public string? Role { get; set; }
    }
}
