using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    [Index(nameof(ClientName), IsUnique = true)]
    public class ClientEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? ImageFileName { get; set; }
        public string ClientName { get; set; } = null!;

        [Column(TypeName = "date")]
        public DateTime Created { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
        public virtual ClientInformationEntity Information { get; set; } = null!;
        public virtual ClientAddressEntity Address { get; set; } = null!;
        public virtual ICollection<ProjectEntity> Projects { get; set; } = []; 
    }
}
