using Business.Services;
using Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController(IProjectService projectService) : ControllerBase
    {
        private readonly IProjectService _projectService = projectService;

        [HttpPost]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Create a new Project")]
        public async Task<IActionResult> Create([FromForm] AddProjectForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectService.CreateProjectAsync(form);

            return result.StatusCode switch
            {
                201 => CreatedAtAction(nameof(Get), new { id = result.Result!.Id }, result.Result),
                400 => BadRequest(result), 
                409 => Conflict(result),
                500 => StatusCode(500, result),
                _ => Problem(detail: result.Message, statusCode: result.StatusCode),
            };
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a Project")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _projectService.DeleteProjectAsync(id);

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

        //[Authorize]
        [HttpGet]
        [SwaggerOperation(Summary = "Get all Projects")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _projectService.GetProjectsAsync();
            return result.Succeeded ? Ok(result.Result) : BadRequest(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a Project")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _projectService.GetProjectByIdAsync(id);
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

        [HttpPut]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Update a Project")]
        public async Task<IActionResult> Update([FromForm] UpdateProjectForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _projectService.UpdateProjectAsync(form);

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
