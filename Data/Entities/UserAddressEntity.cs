using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class UserAddressEntity
    {
        [PersonalData]
        [Key, ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public virtual UserEntity User { get; set; } = null!;

        [PersonalData]
        public string? StreetName { get; set; } 

        [PersonalData]
        public string? PostalCode { get; set; } 

        [PersonalData]
        public string? City { get; set; } 
    }
}
