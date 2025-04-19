using Business.Factories;
using Business.Handlers;
using Business.Helpers;
using Business.Models;
using Data.Repositories;
using Domain.Dtos;

namespace Business.Services
{
    public interface IClientService
    {
        Task<ResponseResult<ClientModel>> CreateClientAsync(AddClientForm form);
        Task<ResponseResult> DeleteClientAsync(string id);
        Task<ResponseResult<ClientModel>> GetClientByIdAsync(string id);
        Task<ResponseResult<IEnumerable<ClientModel>>> GetClientsAsync();
        Task<ResponseResult<ClientModel>> UpdateClientAsync(UpdateClientForm form);
    }

    public class ClientService(IClientRepository clientRepository, ICacheHandler<IEnumerable<ClientModel>> cacheHandler, IFormValidator formValidator, IFileHandler fileHandler) : IClientService
    {
        private readonly IClientRepository _clientRepository = clientRepository;
        private readonly IFormValidator _formValidator = formValidator;
        private readonly ICacheHandler<IEnumerable<ClientModel>> _cacheHandler = cacheHandler;
        private readonly IFileHandler _fileHandler = fileHandler;

        private const string _cacheKey = "Clients";

        public async Task<ResponseResult<ClientModel>> CreateClientAsync(AddClientForm form)
        {
            var validate = ValidateForm(form);
            if (!validate.Succeeded)
                return ResponseResult<ClientModel>.BadRequest(validate.Message);

            try
            {
                if (await _clientRepository.ExistsAsync(x => x.ClientName == form.ClientName))
                    return ResponseResult<ClientModel>.AlreadyExists("Client name already exists.");

                var entity = ClientFactory.ToEntity(form);
                entity.IsActive = true;

                var imageFileUri = await _fileHandler.UploadFileAsync(form.ImageFileName!);
                if (!string.IsNullOrEmpty(imageFileUri))
                    entity.ImageFileName = imageFileUri;

                var created = await _clientRepository.CreateAsync(entity);
                if (!created)
                    return ResponseResult<ClientModel>.Failed("Client creation failed.");

                var models = await UpdateCacheAsync();
                var model = models.FirstOrDefault(x => x.Id == entity.Id);

                return model != null
                            ? ResponseResult<ClientModel>.Created(model)
                            : ResponseResult<ClientModel>.Failed("Could not retrieve created client.");
            }
            catch (Exception ex)
            {
                return ResponseResult<ClientModel>.Failed($"An error occurred while trying to create client: {ex.Message}");
            }
        }

        public async Task<ResponseResult> DeleteClientAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return ResponseResult.BadRequest("Invalid ID.");

            try
            {
                var entity = await _clientRepository.ExistsAsync(x => x.Id == id);
                if (!entity)
                    return ResponseResult.NotFound("No client found with the given id.");

                var deleted = await _clientRepository.DeleteAsync(x => x.Id == id);
                if (!deleted)
                    return ResponseResult.Failed("Client deletion failed.");

                await UpdateCacheAsync();
                return ResponseResult.Ok();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed($"An error occurred while trying to delete client: {ex.Message}");
            }
        }

        public async Task<ResponseResult<ClientModel>> GetClientByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return ResponseResult<ClientModel>.BadRequest("Invalid ID.");

            try
            {
                var cached = _cacheHandler.GetFromCache(_cacheKey);

                var match = cached?.FirstOrDefault(x => x.Id == id);
                if (match != null)
                    return ResponseResult<ClientModel>.Ok(match);

                var models = await UpdateCacheAsync();

                var model = models.FirstOrDefault(x => x.Id == id);
                if (model != null)
                    return ResponseResult<ClientModel>.Ok(model);

                return ResponseResult<ClientModel>.NotFound("Client not found.");
            }
            catch (Exception ex)
            {
                return ResponseResult<ClientModel>.Failed($"An error occurred while trying to retrive client: {ex.Message}");
            }
        }

        public async Task<ResponseResult<IEnumerable<ClientModel>>> GetClientsAsync()
        {
            try
            {
                var models = _cacheHandler.GetFromCache(_cacheKey) ?? await UpdateCacheAsync();
                return ResponseResult<IEnumerable<ClientModel>>.Ok(models);
            }
            catch (Exception ex)
            {
                return ResponseResult<IEnumerable<ClientModel>>.Failed($"An error occurred while trying to retrive clients: {ex.Message}");
            }
        }

        public async Task<ResponseResult<ClientModel>> UpdateClientAsync(UpdateClientForm form)
        {
            var validate = ValidateForm(form);
            if (!validate.Succeeded)
                return ResponseResult<ClientModel>.BadRequest(validate.Message);

            try
            {
                var clientNameExists = await _clientRepository.ExistsAsync(x => x.ClientName == form.ClientName && x.Id != form.Id);
                if (clientNameExists)
                    return ResponseResult<ClientModel>.AlreadyExists($"Client with name {form.ClientName} already exists.");

                var entity = await _clientRepository.GetAsync(
                    x => x.Id == form.Id,
                    i => i.Information,
                    i => i.Address
                );
                if (entity == null)
                    return ResponseResult<ClientModel>.NotFound("Client not found.");

                ClientFactory.UpdateEntity(entity, form);

                if (form.NewImageFileName != null && form.NewImageFileName.Length > 0)
                {
                    var newImageFileUri = await _fileHandler.UploadFileAsync(form.NewImageFileName);
                    if (!string.IsNullOrEmpty(newImageFileUri))
                        entity.ImageFileName = newImageFileUri;
                }

                var updated = await _clientRepository.UpdateAsync(entity);
                if (!updated)
                    return ResponseResult<ClientModel>.Failed("Client update failed.");

                var models = await UpdateCacheAsync();
                var model = models.FirstOrDefault(x => x.Id == form.Id);

                return model != null
                            ? ResponseResult<ClientModel>.Ok(model)
                            : ResponseResult<ClientModel>.Failed("Could not retrieve updated client.");
            }
            catch (Exception ex)
            {
                return ResponseResult<ClientModel>.Failed($"An error occurred while trying to update client: {ex.Message}");
            }
        }

        public ResponseResult ValidateForm(UpdateClientForm form) => _formValidator.ValidateFormFields(
            form?.ClientName,
            form?.Email,
            form?.StreetName,
            form?.PostalCode,
            form?.City
        );

        public ResponseResult ValidateForm(AddClientForm form) => _formValidator.ValidateFormFields(
            form?.ClientName,
            form?.Email,
            form?.StreetName,
            form?.PostalCode,
            form?.City
        );

        public async Task<IEnumerable<ClientModel>> UpdateCacheAsync()
        {
            var entities = await _clientRepository.GetAllAsync(
                sortBy: x => x.ClientName,
                includes:
                [
                    x => x.Information,
                    x => x.Address
                ]
             );

            var models = entities.Select(ClientFactory.ToModel).ToList();

            _cacheHandler.SetCache(_cacheKey, models);
            return models;
        }
    }
}
