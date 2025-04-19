using Business.Factories;
using Business.Handlers;
using Business.Helpers;
using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Business.Services
{
    public interface IAuthService
    {
        Task<ResponseResult<AuthResult>> SignInAsync(SignInForm form);
        Task<ResponseResult> SignUpAsync(SignUpForm form);
    }

    public class AuthService(
            SignInManager<UserEntity> signInManager, UserManager<UserEntity> userManager, IJwtTokenHandler jwtTokenHandler, IRoleService roleService, 
            ICacheHandler<IEnumerable<UserModel>> cacheHandler, IUserRepository userRepository, IFormValidator formValidator, IConfiguration configuration) : IAuthService
    {
        private readonly SignInManager<UserEntity> _signInManager = signInManager;
        private readonly UserManager<UserEntity> _userManager = userManager;
        private readonly IJwtTokenHandler _jwtTokenHandler = jwtTokenHandler;
        private readonly IFormValidator _formValidator = formValidator;
        private readonly IRoleService _roleService = roleService;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICacheHandler<IEnumerable<UserModel>> _cacheHandler = cacheHandler;
        private readonly IConfiguration _configuration = configuration;

        private const string _cacheKey = "Users";

        public async Task<ResponseResult> SignUpAsync(SignUpForm form)
        {
            var validate = ValidateForm(form);
            if (!validate.Succeeded)
                return validate;

            try
            {
                var existingEntity = await _userManager.FindByEmailAsync(form.Email);
                if (existingEntity != null)
                {
                    if (existingEntity.PasswordHash != null)
                        return ResponseResult.AlreadyExists("User already has an account.");

                    var password = await _userManager.AddPasswordAsync(existingEntity, form.Password);
                    if (!password.Succeeded)
                        return ResponseResult.Failed("Password could not be saved.");

                    return ResponseResult.Created("User updated successfully.");
                }

                var entity = UserFactory.ToEntity(form);

                var created = await _userManager.CreateAsync(entity, form.Password);
                if (!created.Succeeded)
                    return ResponseResult.Failed("Failed to create user.");

                var result = await _roleService.AssignDefaultRoleAsync(entity);
                if (!result.Succeeded)
                    return result;

                await UpdateCacheAsync();

                return ResponseResult.Created("User was successfully created.");
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed($"An error occurred while trying to create user: {ex.Message}"); 
            }
        }

        public async Task<ResponseResult<AuthResult>> SignInAsync(SignInForm form)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(form.Email);
                if (user == null)
                    return ResponseResult<AuthResult>.BadRequest("Invalid email.");

                var result = await _signInManager.PasswordSignInAsync(form.Email, form.Password, false, false);
                if (!result.Succeeded)
                    return ResponseResult<AuthResult>.BadRequest("Invalid email or password.");

                var response = new AuthResult();

                var role = await _roleService.GetUserRoleAsync(user);
                var token = _jwtTokenHandler.GenerateJwtToken(user, role);
                if (string.IsNullOrEmpty(token))
                    return ResponseResult<AuthResult>.Failed("Failed to generate JWT token.");

                response.AccessToken = token;

                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (isAdmin)
                {
                    response.IsAdmin = true;
                    var adminApiKey = _configuration["SecretKeys:Admin"];
                    response.ApiKey = adminApiKey;
                }

                return ResponseResult<AuthResult>.Ok(response);
            }
            catch (Exception ex)
            {
                return ResponseResult<AuthResult>.Failed($"An error occurred while trying to sign in: {ex.Message}"); 
            }
        }

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

        public ResponseResult ValidateForm(SignUpForm form)
        {
            var fieldValidation = _formValidator.ValidateFormFields(
                form?.Email,
                form?.Password,
                form?.ConfirmPassword,
                form?.FirstName,
                form?.LastName
            );

            if (!fieldValidation.Succeeded)
                return fieldValidation;

            var confirmationValidation = _formValidator.ValidatePasswordMatchAndTerms(form!);
            if (!confirmationValidation.Succeeded)
                return confirmationValidation;

            return ResponseResult.Ok();
        }
    }
}
