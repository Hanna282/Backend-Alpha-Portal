using Business.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Documentation.ClientEndPoint
{
    public class ClientNotFoundExample : IExamplesProvider<ErrorMessageModel>
    {
        public ErrorMessageModel GetExamples()
        {
            return new ErrorMessageModel
            {
                Message = "Client not found. Please check the provided ID."
            };
        }
    }
}
