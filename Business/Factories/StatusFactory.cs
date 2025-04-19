using Business.Models;
using Data.Entities;

namespace Business.Factories
{
    public class StatusFactory
    {
        public static StatusModel ToModel(StatusEntity? entity)
        {
            return entity == null
                ? null!
                : new StatusModel()
                {
                    Id = entity.Id,
                    StatusName = entity.StatusName,
                };
        }
    }
}
