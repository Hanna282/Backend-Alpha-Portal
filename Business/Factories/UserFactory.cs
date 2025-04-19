using Business.Models;
using Data.Entities;
using Domain.Dtos;

namespace Business.Factories
{
    public class UserFactory
    {
        public static UserEntity ToEntity(SignUpForm? form)
        {
            if (form == null)
                return null!;

            var entity = new UserEntity
            {
                UserName = form.Email,
                Email = form.Email,
                Information = new UserInformationEntity
                {
                    FirstName = form.FirstName,
                    LastName = form.LastName,
                },
                Address = new UserAddressEntity()
            };

            return entity;
        }

        public static UserModel ToModel(UserEntity? entity)
        {
            if (entity == null)
                return null!;

            var model = new UserModel
            {
                Id = entity.Id,
                ImageFileName = entity.ImageFileName,
                Created = entity.Created,
                Information = new UserInformationModel
                {
                    UserId = entity.Information.UserId,
                    FirstName = entity.Information.FirstName,
                    LastName = entity.Information.LastName,
                    Email = entity.Email, 
                    Phone = entity.Information.Phone,
                    JobTitle = entity.Information.JobTitle,
                    Role = entity.Information.Role,
                },
                Address = new UserAddressModel
                {
                    UserId = entity.Id,
                    StreetName = entity.Address.StreetName,
                    PostalCode = entity.Address.PostalCode,
                    City = entity.Address.City
                }
            };

            return model;
        }

        public static UserEntity ToEntity(AddUserForm? form, string? newImageFileName = null)
        {
            if (form == null)
                return null!;

            var entity = new UserEntity
            {
                UserName = form.Email,
                ImageFileName = newImageFileName,
                Email = form.Email,
                Information = new UserInformationEntity
                {
                    FirstName = form.FirstName,
                    LastName = form.LastName,
                    Phone = form.Phone,
                    JobTitle = form.JobTitle,
                    Role = form.Role,
                },
                Address = new UserAddressEntity
                {
                    StreetName = form.StreetName,
                    PostalCode = form.PostalCode,
                    City = form.City,
                }
            };

            return entity;
        }

        public static UserEntity UpdateEntity(UserEntity entity, UpdateUserForm form) 
        {
            if (form == null || entity == null)
                return null!;

            if (entity.ImageFileName != form.ExistingImageFileName)
                entity.ImageFileName = form.ExistingImageFileName;

            if (entity.Information.FirstName != form.FirstName)
                entity.Information.FirstName = form.FirstName;

            if (entity.Information.LastName != form.LastName)
                entity.Information.LastName = form.LastName;

            if (entity.Information.Phone != form.Phone)
                entity.Information.Phone = form.Phone;

            if (entity.Information.JobTitle != form.JobTitle)
                entity.Information.JobTitle = form.JobTitle;

            if (entity.Information.Role != form.Role)
                entity.Information.Role = form.Role;
               
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
