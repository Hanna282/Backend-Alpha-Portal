using Domain.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Documentation.ClientEndPoint
{
    public class UpdateClientDataExample : IExamplesProvider<UpdateClientForm>
    {
        public UpdateClientForm GetExamples()
        {
            return new UpdateClientForm
            {
                Id = "ae5f645a-9537-40c0-9016-2ffe881s1s5d",
                ExistingImageFileName = "logo.png",
                NewImageFileName = null,
                ClientName = "Nackademin AB",
                Email = "info@nackademin.se",
                Phone = "+46084666000",
                StreetName = "Tomtebodavägen 3A",
                PostalCode = "17165",
                City = "Solna",
                Reference = "Gunilla Simonsson"
            };
        }
    }
}
