using Data.Contexts;
using Data.Entities;

namespace Data.Repositories
{
    public interface IStatusRepository : IBaseRepository<StatusEntity>
    {
    }

    public class StatusRepository(DataContext context) : BaseRepository<StatusEntity>(context), IStatusRepository
    {
    }
}
