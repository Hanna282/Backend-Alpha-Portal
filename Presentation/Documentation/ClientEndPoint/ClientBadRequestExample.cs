using Business.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Documentation.ClientEndPoint
{
    public class ClientBadRequestExample : IExamplesProvider<ErrorMessageModel>
    {
        public ErrorMessageModel GetExamples()
        {
            return new ErrorMessageModel
            {
                Message = "Client request contains invalid data. Please review your input."
            };
        }
    }
}
