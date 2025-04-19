using Business.Models;
using Swashbuckle.AspNetCore.Filters;

namespace Presentation.Documentation.ClientEndPoint
{
    public class ClientExample : IExamplesProvider<ClientModel>
    {
        public ClientModel GetExamples()
        {
            return new ClientModel
            {
                Id = "ae5f645a-9537-40c0-9016-2ffe881s1s5d",                            
                ImageFileName = "npicture.svg",          
                ClientName = "Nackademin AB",              
                Created = DateTime.UtcNow.Date,               
                IsActive = true,                           
                Information = new ClientInformationModel  
                {
                    ClientId = "ae5f645a-9537-40c0-9016-2ffe881s1s5d",                  
                    Email = "info@nackademin.se",              
                    Phone = "+46084666000",                 
                    Reference = "Gunilla Simonsson"
                },
                Address = new ClientAddressModel          
                {
                    ClientId = "ae5f645a-9537-40c0-9016-2ffe881s1s5d",                
                    StreetName = "Tomtebodavägen 3A",              
                    PostalCode = "17165",                
                    City = "Solna"
                }
            };
        }
    }
}
