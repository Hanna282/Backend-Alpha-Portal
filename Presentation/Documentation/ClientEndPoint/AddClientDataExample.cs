using Domain.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Documentation.ClientEndPoint
{
    public class AddClientDataExample : IExamplesProvider<AddClientForm>
    {
        public AddClientForm GetExamples()
        {
            return new AddClientForm()
            {
                ImageFileName = null, 
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
