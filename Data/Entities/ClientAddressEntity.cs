using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class ClientAddressEntity
    {
        [Key, ForeignKey(nameof(Client))]
        public string ClientId { get; set; } = null!;
        public virtual ClientEntity Client { get; set; } = null!;
        public string StreetName { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string City { get; set; } = null!;
    }
}
