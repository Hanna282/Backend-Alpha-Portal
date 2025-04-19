using Business.Models;
using Business.Services;
using Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Documentation.ClientEndPoint;
using Presentation.Extensions.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController(IClientService clientService) : ControllerBase
    {
        private readonly IClientService _clientService = clientService;

        [UseAdminApiKey]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(
            Summary = "Create a new Client",
            Description = "Only Admins can create clients. This will require a API-key 'X-ADM-API-KEY' in the header request."
        )]
        [SwaggerRequestExample(typeof(AddClientForm), typeof(AddClientDataExample))]
        [SwaggerResponseExample(201, typeof(ClientExample))]
        [SwaggerResponseExample(400, typeof(ClientBadRequestExample))]
        [SwaggerResponseExample(409, typeof(ClientAlreadyExistsExample))]
        [SwaggerResponseExample(500, typeof(ClientInternalServerErrorExample))]
        [SwaggerResponse(201, "Client was successfully created.", typeof(ClientModel))]
        [SwaggerResponse(400, "Client request contains invalid data.", typeof(ErrorMessageModel))]
        [SwaggerResponse(409, "Client already exists.", typeof(ErrorMessageModel))]
        [SwaggerResponse(500, "Unexpected error occurred while creating the client.", typeof(ErrorMessageModel))]
        public async Task<IActionResult> Create([FromForm] AddClientForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _clientService.CreateClientAsync(form);
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
            Summary = "Delete a Client",
            Description = "Only Admins can delete clients. This will require a API-key 'X-ADM-API-KEY' in the header request."
        )]
        [SwaggerResponseExample(400, typeof(ClientBadRequestExample))]
        [SwaggerResponseExample(404, typeof(ClientNotFoundExample))]
        [SwaggerResponseExample(500, typeof(ClientInternalServerErrorExample))]
        [SwaggerResponse(200, "Client was successfully deleted.")]
        [SwaggerResponse(400, "Client request contains invalid data.", typeof(ErrorMessageModel))]
        [SwaggerResponse(404, "Client not found.", typeof(ErrorMessageModel))]
        [SwaggerResponse(500, "Unexpected error occurred while deleting the client.", typeof(ErrorMessageModel))]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _clientService.DeleteClientAsync(id);

            return result.StatusCode switch
            {
                200 => Ok(),
                400 => BadRequest(result),
                404 => NotFound(result),
                500 => StatusCode(500, result),
                _ => Problem(detail: result.Message, statusCode: result.StatusCode),
            };
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all Clients")]
        [SwaggerResponse(200, "Returns all clients.", typeof(IEnumerable<ClientModel>))]
        [SwaggerResponseExample(200, typeof(ClientExample))]
        public async Task<IActionResult> GetAll()
        {
            var result = await _clientService.GetClientsAsync();
            return Ok(result.Result);
        }

        [UseAdminApiKey]
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a Client")]
        [SwaggerResponseExample(200, typeof(ClientExample))]
        [SwaggerResponseExample(404, typeof(ClientNotFoundExample))]
        [SwaggerResponse(404, "Client not found.", typeof(ErrorMessageModel))]
        [SwaggerResponse(200, "Returns a client by ID.", typeof(ClientModel))]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _clientService.GetClientByIdAsync(id);
            return result.Succeeded ? Ok(result.Result) : NotFound(result);
        }

        [UseAdminApiKey]
        [HttpPut]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(
            Summary = "Update a Client",
            Description = "Only Admins can update clients. This will require a API-key 'X-ADM-API-KEY' in the header request."
        )]
        [SwaggerRequestExample(typeof(UpdateClientForm), typeof(UpdateClientDataExample))]
        [SwaggerResponseExample(200, typeof(ClientExample))]
        [SwaggerResponseExample(400, typeof(ClientBadRequestExample))]
        [SwaggerResponseExample(404, typeof(ClientNotFoundExample))]
        [SwaggerResponseExample(409, typeof(ClientAlreadyExistsExample))]
        [SwaggerResponseExample(500, typeof(ClientInternalServerErrorExample))]
        [SwaggerResponse(200, "Client was successfully updated.", typeof(ClientModel))]
        [SwaggerResponse(400, "Client request contains invalid data.", typeof(ErrorMessageModel))]
        [SwaggerResponse(404, "Client not found.", typeof(ErrorMessageModel))]
        [SwaggerResponse(409, "Client already exists.", typeof(ErrorMessageModel))]
        [SwaggerResponse(500, "Unexpected error occurred while updating the client.", typeof(ErrorMessageModel))]
        public async Task<IActionResult> Update([FromForm] UpdateClientForm form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _clientService.UpdateClientAsync(form);

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
