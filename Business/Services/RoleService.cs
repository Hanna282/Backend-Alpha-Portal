using Data.Entities;
using Domain.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Business.Services
{
    public interface IRoleService
    {
        Task<ResponseResult> AssignDefaultRoleAsync(UserEntity entity);
        Task<ResponseResult> AssignRoleAsync(UserEntity entity);
        Task<ResponseResult> UpdateRoleAsync(UserEntity entity, UpdateUserForm form, string role);
        Task<string?> GetUserRoleAsync(UserEntity user);
    }

    public class RoleService(UserManager<UserEntity> userManager) : IRoleService
    {
        private readonly UserManager<UserEntity> _userManager = userManager;

        public async Task<ResponseResult> AssignDefaultRoleAsync(UserEntity entity)
        {
            var role = await DetermineDefaultRoleAsync();

            var added = await _userManager.AddToRoleAsync(entity, role);
            if (!added.Succeeded)
                return ResponseResult.Failed("Unable to add default role to user.");

            entity.Information.Role = role;

            var updated = await _userManager.UpdateAsync(entity);
            if (updated.Succeeded)
                return ResponseResult.Ok();

            return ResponseResult.Failed("Unable to uppdate user with default role.");
        }

        public async Task<string> DetermineDefaultRoleAsync()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            return admins.Any() ? "User" : "Admin";
        }

        public async Task<string?> GetUserRoleAsync(UserEntity user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "User";

            return userRole;
        }

        public async Task<ResponseResult> AssignRoleAsync(UserEntity entity)
        {
            var role = entity.Information.Role;
            if (role != null)
            {
                var result = await _userManager.AddToRoleAsync(entity, role);
                if (result.Succeeded)
                    return ResponseResult.Ok();
            }

            return ResponseResult.Failed("Unable to add role to user.");
        }

        public async Task<ResponseResult> UpdateRoleAsync(UserEntity entity, UpdateUserForm form, string role)
        {
            var removed = await _userManager.RemoveFromRoleAsync(entity, role);
            if (!removed.Succeeded)
                return ResponseResult.Failed();

            var added = await _userManager.AddToRoleAsync(entity, form.Role);
            if (!added.Succeeded)
                return ResponseResult.Failed();

            return ResponseResult.Ok();
        }
    }
}
