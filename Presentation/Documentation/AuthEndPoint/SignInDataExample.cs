using Domain.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Documentation.AuthEndPoint
{
    public class SignInDataExample : IExamplesProvider<SignInForm>
    {
        public SignInForm GetExamples() => new()
        {
            Email = "john.doe@domain.com",
            Password = "BytMig123!"
        };
    }
}
