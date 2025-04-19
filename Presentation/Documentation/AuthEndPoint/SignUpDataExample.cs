using Domain.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Documentation.AuthEndPoint
{
    public class SignUpDataExample : IExamplesProvider<SignUpForm>
    {
        public SignUpForm GetExamples()
        {
            return new SignUpForm
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@domain.com",
                Password = "BytMig123!",
                ConfirmPassword = "BytMig123!",
                TermsAndConditions = true
            };
        }
    }
}
