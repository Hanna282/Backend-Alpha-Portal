using Business.Services;
using Domain.Dtos;
using Microsoft.AspNetCore.Mvc;
using Presentation.Documentation.AuthEndPoint;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Controllers
{
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost]
        [Route("SignIn")]
        [SwaggerOperation(Summary = "Sign In with a existing User")]
        [SwaggerRequestExample(typeof(SignInForm), typeof(SignInDataExample))]
        [SwaggerResponse(200, "User successfully authenticated")]
        public async Task<IActionResult> SignIn(SignInForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.SignInAsync(form);
            if (result.Succeeded)
                return Ok(result);
            else
                return Unauthorized(result.Message);
        }

        [HttpPost]
        [Route("SignUp")]
        [SwaggerOperation(Summary = "Sign Up as a new User")]
        [SwaggerRequestExample(typeof(SignUpForm), typeof(SignUpDataExample))]
        [SwaggerResponse(201, "User was successfully created.")]
        public async Task<IActionResult> SignUp(SignUpForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); 

            var result = await _authService.SignUpAsync(form);
            return result.StatusCode switch
            {
                201 => Created("", new { ok = true, message = result.Message }),
                400 => BadRequest(result.Message), 
                409 => Conflict(result.Message), 
                500 => StatusCode(500, result.Message),
                _ => Problem(detail: result.Message, statusCode: result.StatusCode),
            };
        }
    }
}
