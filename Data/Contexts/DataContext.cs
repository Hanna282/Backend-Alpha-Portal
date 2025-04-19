using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts
{
    public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<UserEntity>(options)
    {
        public virtual DbSet<UserInformationEntity> UserInformations { get; set; }
        public virtual DbSet<UserAddressEntity> UserAddresses { get; set; }
        public virtual DbSet<ClientEntity> Clients { get; set; }
        public virtual DbSet<ClientInformationEntity> ClientInformations { get; set; }
        public virtual DbSet<ClientAddressEntity> ClientAddresses { get; set; }
        public virtual DbSet<ProjectEntity> Projects { get; set; }
        public virtual DbSet<StatusEntity> Statuses { get; set; }
    }
}
