using Data.Contexts;
using Data.Entities;

namespace Data.Repositories
{
    public interface IClientRepository : IBaseRepository<ClientEntity>
    {
    }

    public class ClientRepository(DataContext context) : BaseRepository<ClientEntity>(context), IClientRepository
    {
    }
}
