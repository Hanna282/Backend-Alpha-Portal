using Business.Models;
using Business.Services;
using Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Presentation.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [UseAdminApiKey]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(
            Summary = "Create a new User",
            Description = "Only Admins can create users. This will require a API-key 'X-ADM-API-KEY' in the header request."
        )]
        [SwaggerResponse(200, "User successfully created", typeof(UserModel))]
        public async Task<IActionResult> Create([FromForm] AddUserForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.CreateUserAsync(form);

            return result.StatusCode switch
            {
                201 => CreatedAtAction(nameof(Get), new { id = result.Result!.Id }, result.Result),
                400 => BadRequest(result),
                409 => Conflict(result),
                500 => StatusCode(500, result),
                _ => Problem(detail: result.Message, statusCode: result.StatusCode),
            };
        }

        [UseAdminApiKey]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Delete a User",
            Description = "Only Admins can delete users. This will require a API-key 'X-ADM-API-KEY' in the header request."
        )]
        [SwaggerResponse(200, "User successfully deleted")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.DeleteUserAsync(id);

            return result.StatusCode switch
            {
                200 => Ok(),
                400 => BadRequest(result),
                404 => NotFound(result),
                409 => Conflict(result),
                500 => StatusCode(500, result),
                _ => Problem(detail: result.Message, statusCode: result.StatusCode),
            };
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all Users")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetUsersAsync();
            return result.Succeeded ? Ok(result.Result) : BadRequest(result);
        }

        [UseAdminApiKey]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a User")]
        [SwaggerResponse(200, "Returns a user by ID", typeof(UserModel))]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return result.StatusCode switch
            {
                200 => Ok(result.Result),
                400 => BadRequest(result),
                404 => NotFound(result),
                409 => Conflict(result),
                500 => StatusCode(500, result),
                _ => Problem(detail: result.Message, statusCode: result.StatusCode),
            };
        }

        [HttpGet("me")]
        [SwaggerOperation(
            Summary = "Get user info based on access token",
            Description = "Returns the user data for the currently signed-in user by extracting their email from the JWT access token."
        )]
        [SwaggerResponse(200, "Returns the signed-in user", typeof(UserModel))]
        [SwaggerResponse(401, "Unauthorized - token missing or invalid")]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Tagit hjälp av AI för att använda Claims för denna request
            var email = User.FindFirstValue(ClaimTypes.Name); 
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { message = "Unauthorized - token missing or invalid" });

            var result = await _userService.GetUserByEmailAsync(email);
            return result.Succeeded ? Ok(result.Result) : NotFound(result);
        }

        [UseAdminApiKey]
        [HttpPut]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(
            Summary = "Update a User",
            Description = "Only Admins can update users. This will require a API-key 'X-ADM-API-KEY' in the header request."
        )]
        [SwaggerResponse(200, "User successfully updated", typeof(UserModel))]
        public async Task<IActionResult> Update([FromForm] UpdateUserForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.UpdateUserAsync(form);

            return result.StatusCode switch
            {
                200 => Ok(result),
                400 => BadRequest(result),
                404 => NotFound(result),
                409 => Conflict(result),
                500 => StatusCode(500, result),
                _ => Problem(detail: result.Message, statusCode: result.StatusCode),
            };
        }
    }
}
