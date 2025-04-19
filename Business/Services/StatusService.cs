using Business.Factories;
using Business.Handlers;
using Business.Models;
using Data.Repositories;
using Domain.Dtos;

namespace Business.Services
{
    public interface IStatusService
    {
        Task<ResponseResult<StatusModel>> GetStatusByNameAsync(string statusName);
        Task<ResponseResult<IEnumerable<StatusModel>>> GetStatusesAsync();
    }

    public class StatusService(IStatusRepository statusRepository, ICacheHandler<IEnumerable<StatusModel>> cacheHandler) : IStatusService
    {
        private readonly IStatusRepository _statusRepository = statusRepository;
        private readonly ICacheHandler<IEnumerable<StatusModel>> _cacheHandler = cacheHandler;

        private const string _cacheKey = "Statuses";

        public async Task<ResponseResult<IEnumerable<StatusModel>>> GetStatusesAsync()
        {
            try
            {
                var models = _cacheHandler.GetFromCache(_cacheKey) ?? await UpdateCacheAsync();
                return ResponseResult<IEnumerable<StatusModel>>.Ok(models);
            }
            catch (Exception ex)
            {
                return ResponseResult<IEnumerable<StatusModel>>.Failed($"An error occurred while retrieving the statuses: {ex.Message}");
            }
        }

        public async Task<ResponseResult<StatusModel>> GetStatusByNameAsync(string statusName)
        {
            try
            {
                var cached = _cacheHandler.GetFromCache(_cacheKey);

                var match = cached?.FirstOrDefault(x => x.StatusName == statusName);
                if (match != null)
                    return ResponseResult<StatusModel>.Ok(match);

                var models = await UpdateCacheAsync();
                if (models == null)
                    return ResponseResult<StatusModel>.NotFound("Statuses not found.");

                var model = models.FirstOrDefault(x => x.StatusName == statusName);
                if (model == null)
                    return ResponseResult<StatusModel>.NotFound("Status not found.");

                return ResponseResult<StatusModel>.Ok(model);
            }
            catch (Exception ex)
            {
                return ResponseResult<StatusModel>.Failed($"An error occurred while retrieving the status: {ex.Message}");
            }
        }

        public async Task<IEnumerable<StatusModel>> UpdateCacheAsync()
        {
            var entities = await _statusRepository.GetAllAsync(
                orderByDescending: false,
                sortBy: x => x.Id,
                filterBy: null
            );
            var models = entities.Select(StatusFactory.ToModel).ToList();

            _cacheHandler.SetCache(_cacheKey, models);
            return models;
        }
    }
}
