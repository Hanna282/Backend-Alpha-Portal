using Business.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Documentation.ClientEndPoint
{
    public class ClientInternalServerErrorExample : IExamplesProvider<ErrorMessageModel>
    {
        public ErrorMessageModel GetExamples()
        {
            return new ErrorMessageModel
            {
                Message = "An unexpected error occurred while processing the client request. Please try again later."
            };
        }
    }
}
