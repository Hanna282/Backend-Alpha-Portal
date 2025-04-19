using Business.Models;
using Data.Entities;
using Domain.Dtos;

namespace Business.Factories
{
    public class ClientFactory
    {
        public static ClientModel ToModel(ClientEntity? entity)
        {
            return entity == null
                ? null!
                : new ClientModel
                {
                    Id = entity.Id,
                    ImageFileName = entity.ImageFileName,
                    ClientName = entity.ClientName,
                    Created = entity.Created,
                    IsActive = entity.IsActive,
                    Information = new ClientInformationModel
                    {
                        ClientId = entity.Information.ClientId,
                        Email = entity.Information.Email,
                        Phone = entity.Information.Phone,
                        Reference = entity.Information.Reference
                    },
                    Address = new ClientAddressModel
                    {
                        ClientId = entity.Address.ClientId,
                        StreetName = entity.Address.StreetName,
                        PostalCode = entity.Address.PostalCode,
                        City = entity.Address.City,
                    }
                };
        }

        public static ClientEntity ToEntity(AddClientForm? form, string? newImageFileName = null)
        {
            return form == null
                ? null!
                : new ClientEntity
                {
                    ImageFileName = newImageFileName,
                    ClientName = form.ClientName,
                    Information = new ClientInformationEntity
                    {
                        Email = form.Email,
                        Phone = form.Phone,
                        Reference = form.Reference
                    },
                    Address = new ClientAddressEntity
                    {
                        StreetName = form.StreetName,
                        PostalCode = form.PostalCode,
                        City = form.City,
                    },
                };
        }

        public static ClientEntity UpdateEntity(ClientEntity entity, UpdateClientForm form) 
        {
            if (form == null || entity == null)
                return null!;

            if (entity.ImageFileName != form.ExistingImageFileName)
                entity.ImageFileName = form.ExistingImageFileName;

            if (entity.ClientName != form.ClientName)
                entity.ClientName = form.ClientName;

            if (entity.Information.Email != form.Email)
                entity.Information.Email = form.Email;

            if (entity.Information.Phone != form.Phone)
                entity.Information.Phone = form.Phone;

            if (entity.Information.Reference != form.Reference)
                entity.Information.Reference = form.Reference;

            if (entity.Address.StreetName != form.StreetName)
                entity.Address.StreetName = form.StreetName;

            if (entity.Address.PostalCode != form.PostalCode)
                entity.Address.PostalCode = form.PostalCode;

            if (entity.Address.City != form.City)
                entity.Address.City = form.City;

            return entity;
        }
    }
}
