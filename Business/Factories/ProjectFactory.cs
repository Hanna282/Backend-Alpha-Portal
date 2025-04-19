using Business.Models;
using Data.Entities;
using Domain.Dtos;

namespace Business.Factories
{
    public class ProjectFactory
    {
        public static ProjectModel ToModel(ProjectEntity? entity)
        {
            return entity == null
                ? null!
                : new ProjectModel
                {
                    Id = entity.Id,
                    ImageFileName = entity.ImageFileName,
                    ProjectName = entity.ProjectName,
                    Description = entity.Description,
                    StartDate = entity.StartDate,
                    EndDate = entity.EndDate,
                    Budget = entity.Budget,
                    Created = entity.Created,
                    Client = ClientFactory.ToModel(entity.Client),
                    User = UserFactory.ToModel(entity.User),
                    Status = new StatusModel
                    {
                        Id = entity.Status.Id,
                        StatusName = entity.Status.StatusName,
                    }
                };
        }

        public static ProjectEntity ToEntity(AddProjectForm? form, string? newImageFileName = null)
        {
            return form == null
                ? null!
                : new ProjectEntity
                {
                    ImageFileName = newImageFileName,
                    ProjectName = form.ProjectName,
                    Description = form.Description,
                    StartDate = form.StartDate,
                    EndDate = form.EndDate,
                    Budget = form.Budget,
                    ClientId = form.ClientId,
                    UserId = form.UserId,
                };
        }

        public static ProjectEntity UpdateEntity(ProjectEntity entity, UpdateProjectForm form) 
        {
            if (form == null || entity == null)
                return null!;

            if (entity.ImageFileName != form.ExistingImageFileName)
                entity.ImageFileName = form.ExistingImageFileName;

            if (entity.ProjectName != form.ProjectName)
                entity.ProjectName = form.ProjectName;

            if (entity.Description != form.Description)
                entity.Description = form.Description;

            if (entity.StartDate != form.StartDate)
                entity.StartDate = form.StartDate;

            if (entity.EndDate != form.EndDate)
                entity.EndDate = form.EndDate;

            if (entity.Budget != form.Budget)
                entity.Budget = form.Budget;

            if (entity.ClientId != form.ClientId)
                entity.ClientId = form.ClientId;

            if (entity.UserId != form.UserId)
                entity.UserId = form.UserId;

            if (entity.StatusId != form.StatusId)
                entity.StatusId = form.StatusId;

            return entity;
        }
    }
}
