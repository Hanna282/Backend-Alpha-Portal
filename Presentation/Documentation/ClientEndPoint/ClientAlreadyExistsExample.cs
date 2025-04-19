using Business.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Documentation.ClientEndPoint
{
    public class ClientAlreadyExistsExample : IExamplesProvider<ErrorMessageModel>
    {
        public ErrorMessageModel GetExamples()
        {
            return new ErrorMessageModel
            {
                Message = "Client name already exists."
            };
        }
    }
}
