using Business.Factories;
using Business.Handlers;
using Business.Helpers;
using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Business.Services
{
    public interface IUserService
    {
        Task<ResponseResult<UserModel>> CreateUserAsync(AddUserForm form);
        Task<ResponseResult> DeleteUserAsync(string id);
        Task<ResponseResult<UserModel>> GetUserByIdAsync(string id);
        Task<ResponseResult<UserModel>> GetUserByEmailAsync(string email);
        Task<ResponseResult<IEnumerable<UserModel>>> GetUsersAsync();
        Task<ResponseResult<UserModel>> UpdateUserAsync(UpdateUserForm form);
    }

    public class UserService(
        IUserRepository userRepository, UserManager<UserEntity> userManager, IRoleService roleservice, 
        ICacheHandler<IEnumerable<UserModel>> cacheHandler, IFormValidator formValidator, ImageHandler imageHandler) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly UserManager<UserEntity> _userManager = userManager;
        private readonly IRoleService _roleService = roleservice;
        private readonly IFormValidator _formValidator = formValidator;
        private readonly ICacheHandler<IEnumerable<UserModel>> _cacheHandler = cacheHandler;
        private readonly ImageHandler _imageHandler = imageHandler;

        private const string _cacheKey = "Users";

        public async Task<ResponseResult<UserModel>> CreateUserAsync(AddUserForm form)
        {
            var validate = ValidateForm(form);
            if (!validate.Succeeded)
                return ResponseResult<UserModel>.BadRequest(validate.Message);

            try
            {
                var exists = await _userRepository.ExistsAsync(x => x.Email == form.Email);
                if (exists)
                    return ResponseResult<UserModel>.AlreadyExists($"User with email {form.Email} already exists.");

                var entity = UserFactory.ToEntity(form);

                var fileName = await _imageHandler.UploadImageAsync(form.ImageFileName!);
                if (!string.IsNullOrEmpty(fileName))
                    entity.ImageFileName = fileName;

                var created = await _userManager.CreateAsync(entity);
                if (!created.Succeeded)
                    return ResponseResult<UserModel>.Failed("User creation failed.");

                var result = await _roleService.AssignRoleAsync(entity);
                if (!result.Succeeded)
                    return ResponseResult<UserModel>.Failed(result.Message);

                var models = await UpdateCacheAsync();
                var model = models.FirstOrDefault(x => x.Id == entity.Id);

                return model != null
                            ? ResponseResult<UserModel>.Created(model)
                            : ResponseResult<UserModel>.Failed("Could not retrieve created user.");
            }
            catch (Exception ex)
            {
                return ResponseResult<UserModel>.Failed($"An error occurred while trying to create client: {ex.Message}");
            }
        }

        public async Task<ResponseResult> DeleteUserAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return ResponseResult.BadRequest();

            try
            {
                var entity = await _userRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                    return ResponseResult.NotFound("No user found with the given ID.");

                var deleted = await _userManager.DeleteAsync(entity);
                if (!deleted.Succeeded)
                    return ResponseResult.Failed("User deletion failed.");

                await UpdateCacheAsync();
                return ResponseResult.Ok();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed($"An error occurred while trying to delete user: {ex.Message}");
            }
        }

        public async Task<ResponseResult<IEnumerable<UserModel>>> GetUsersAsync()
        {
            try
            {
                var models = _cacheHandler.GetFromCache(_cacheKey) ?? await UpdateCacheAsync();
                return ResponseResult<IEnumerable<UserModel>>.Ok(models);
            }
            catch (Exception ex)
            {
                return ResponseResult<IEnumerable<UserModel>>.Failed($"An error occurred while retrieving the users: {ex.Message}");
            }
        }

        public async Task<ResponseResult<UserModel>> GetUserByIdAsync(string id)
        {
            try
            {
                var cached = _cacheHandler.GetFromCache(_cacheKey);

                var match = cached?.FirstOrDefault(x => x.Id == id);
                if (match != null)
                    return ResponseResult<UserModel>.Ok(match);

                var models = await UpdateCacheAsync();

                var model = models.FirstOrDefault(x => x.Id == id);
                if (model != null)
                    return ResponseResult<UserModel>.Ok(model);

                return ResponseResult<UserModel>.NotFound("User not found.");
            }
            catch (Exception ex)
            {
                return ResponseResult<UserModel>.Failed($"An error occurred while retrieving the user: {ex.Message}");
            }
        }

        public async Task<ResponseResult<UserModel>> GetUserByEmailAsync(string email)
        {
            try
            {
                var cached = _cacheHandler.GetFromCache(_cacheKey);

                var match = cached?.FirstOrDefault(x => x.Information.Email == email);
                if (match != null)
                    return ResponseResult<UserModel>.Ok(match);

                var models = await UpdateCacheAsync();

                var model = models.FirstOrDefault(x => x.Information.Email == email);
                if (model != null)
                    return ResponseResult<UserModel>.Ok(model);

                return ResponseResult<UserModel>.NotFound("User not found.");
            }
            catch (Exception ex)
            {
                return ResponseResult<UserModel>.Failed($"An error occurred while retrieving the user: {ex.Message}");
            }
        }

        public async Task<ResponseResult<UserModel>> UpdateUserAsync(UpdateUserForm form)
        {
            var validate = ValidateForm(form);
            if (!validate.Succeeded)
                return ResponseResult<UserModel>.BadRequest(validate.Message);

            try
            {
                var entity = await _userRepository.GetAsync(x => x.Id == form.Id, i => i.Information, i => i.Address);
                if (entity == null)
                    return ResponseResult<UserModel>.NotFound("User not found.");

                var role = entity.Information.Role;

                entity = UserFactory.UpdateEntity(entity, form);

                if (role != form.Role && role != null)
                {
                    var updatedRole = await _roleService.UpdateRoleAsync(entity, form, role);
                    if (!updatedRole.Succeeded)
                        return ResponseResult<UserModel>.Failed(updatedRole.Message);
                }

                if (form.NewImageFileName != null && form.NewImageFileName.Length > 0)
                {
                    var newFileName = await _imageHandler.UploadImageAsync(form.NewImageFileName);
                    if (!string.IsNullOrEmpty(newFileName))
                        entity.ImageFileName = newFileName;
                }

                await _userManager.UpdateAsync(entity);

                var models = await UpdateCacheAsync();
                var model = models.FirstOrDefault(x => x.Id == form.Id);

                return model != null
                            ? ResponseResult<UserModel>.Ok(model)
                            : ResponseResult<UserModel>.Failed("Could not retrieve updated user.");
            }
            catch (Exception ex)
            {
                return ResponseResult<UserModel>.Failed($"An error occurred while trying to update user: {ex.Message}");
            }
        }

        public ResponseResult ValidateForm(AddUserForm form) => _formValidator.ValidateFormFields(
            form?.FirstName,
            form?.LastName,
            form?.Email,
            form?.JobTitle,
            form?.Role,
            form?.StreetName,
            form?.PostalCode,
            form?.City
        );

        public ResponseResult ValidateForm(UpdateUserForm form) => _formValidator.ValidateFormFields(
           form?.FirstName,
           form?.LastName,
           form?.Email,
           form?.JobTitle,
           form?.Role,
           form?.StreetName,
           form?.PostalCode,
           form?.City
       );

        public async Task<IEnumerable<UserModel>> UpdateCacheAsync()
        {
            var entities = await _userRepository.GetAllAsync(
                orderByDescending: false,
                sortBy: x => x.Created,
                filterBy: null,
                i => i.Information,
                i => i.Address
            );
            var models = entities.Select(UserFactory.ToModel).ToList();

            _cacheHandler.SetCache(_cacheKey, models);
            return models;
        }
    }
}
