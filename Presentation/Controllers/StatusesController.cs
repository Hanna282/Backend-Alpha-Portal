using Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class StatusesController(IStatusService statusService) : ControllerBase
    {
        private readonly IStatusService _statusService = statusService;

        [HttpGet]
        [SwaggerOperation(Summary = "Get all Statuses")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _statusService.GetStatusesAsync();
            return Ok(result.Result);
        }
    }
}
