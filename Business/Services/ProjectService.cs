using Business.Factories;
using Business.Handlers;
using Business.Helpers;
using Business.Models;
using Data.Repositories;
using Domain.Dtos;

namespace Business.Services
{
    public interface IProjectService
    {
        Task<ResponseResult<ProjectModel>> CreateProjectAsync(AddProjectForm form, string defaultStatus = "STARTED");
        Task<ResponseResult> DeleteProjectAsync(string id);
        Task<ResponseResult<ProjectModel>> GetProjectByIdAsync(string id);
        Task<ResponseResult<IEnumerable<ProjectModel>>> GetProjectsAsync();
        Task<ResponseResult<ProjectModel>> UpdateProjectAsync(UpdateProjectForm form);
    }

    public class ProjectService(
        IProjectRepository projectRepository, IStatusService statusService, ICacheHandler<IEnumerable<ProjectModel>> cacheHandler, 
        IFormValidator formValidator, IFileHandler fileHandler) : IProjectService
    {
        private readonly IProjectRepository _projectRepository = projectRepository;
        private readonly IStatusService _statusService = statusService;
        private readonly IFormValidator _formValidator = formValidator;
        private readonly ICacheHandler<IEnumerable<ProjectModel>> _cacheHandler = cacheHandler;
        private readonly IFileHandler _fileHandler = fileHandler;

        private const string _cacheKey = "Projects";

        public async Task<ResponseResult<ProjectModel>> CreateProjectAsync(AddProjectForm form, string defaultStatus = "STARTED")
        {
            var validate = ValidateAddForm(form);
            if (!validate.Succeeded)
                return ResponseResult<ProjectModel>.BadRequest(validate.Message);

            try
            {
                var entity = ProjectFactory.ToEntity(form);

                var status = await _statusService.GetStatusByNameAsync(defaultStatus);
                if (status.Succeeded && status.Result != null)
                    entity.StatusId = status.Result.Id;
                else
                    throw new Exception(status.Message);

                var imageFileUri = await _fileHandler.UploadFileAsync(form.ImageFileName!);
                if (!string.IsNullOrEmpty(imageFileUri))
                    entity.ImageFileName = imageFileUri;

                var created = await _projectRepository.CreateAsync(entity);
                if (!created)
                    return ResponseResult<ProjectModel>.Failed("Project creation failed.");

                var models = await UpdateCacheAsync();
                var model = models.FirstOrDefault(x => x.Id == entity.Id);

                return model != null
                            ? ResponseResult<ProjectModel>.Created(model)
                            : ResponseResult<ProjectModel>.Failed("Could not retrieve created project.");
            }
            catch (Exception ex)
            {
                return ResponseResult<ProjectModel>.Failed($"An error occurred while trying to create project: {ex.Message}");
            }
        }

        public async Task<ResponseResult> DeleteProjectAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return ResponseResult.BadRequest("Invalid ID.");

            try
            {
                var exists = await _projectRepository.ExistsAsync(x => x.Id == id);
                if (!exists)
                    return ResponseResult.NotFound("No project found with the given ID.");

                var deleted = await _projectRepository.DeleteAsync(x => x.Id == id);
                if (!deleted)
                    return ResponseResult.Failed("Project deletion failed.");

                await UpdateCacheAsync();
                return ResponseResult.Ok();
            }
            catch (Exception ex)
            {
                return ResponseResult.Failed($"An error occurred while trying to delete project: {ex.Message}");
            }
        }

        public async Task<ResponseResult<IEnumerable<ProjectModel>>> GetProjectsAsync()
        {
            try
            {
                var models = _cacheHandler.GetFromCache(_cacheKey) ?? await UpdateCacheAsync();
                return ResponseResult<IEnumerable<ProjectModel>>.Ok(models);
            }
            catch (Exception ex)
            {
                return ResponseResult<IEnumerable<ProjectModel>>.Failed($"An error occurred while trying to retrieve projects: {ex.Message}");
            }
        }

        public async Task<ResponseResult<ProjectModel>> GetProjectByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return ResponseResult<ProjectModel>.BadRequest("Invalid ID.");

            try
            {
                var cached = _cacheHandler.GetFromCache(_cacheKey);

                var match = cached?.FirstOrDefault(x => x.Id == id);
                if (match != null)
                    return ResponseResult<ProjectModel>.Ok(match);

                var models = await UpdateCacheAsync();

                var model = models.FirstOrDefault(x => x.Id == id);
                if (model != null)
                    return ResponseResult<ProjectModel>.Ok(model);

                return ResponseResult<ProjectModel>.NotFound("Project not found.");
            }
            catch (Exception ex)
            {
                return ResponseResult<ProjectModel>.Failed($"An error occurred while retrieving the project: {ex.Message}");
            }
        }

        public async Task<ResponseResult<ProjectModel>> UpdateProjectAsync(UpdateProjectForm form)
        {
            var validate = ValidateUpdateForm(form);
            if (!validate.Succeeded)
                return ResponseResult<ProjectModel>.BadRequest(validate.Message);

            try
            {
                var entity = await _projectRepository.GetAsync(x => x.Id == form.Id);
                if (entity == null)
                    return ResponseResult<ProjectModel>.NotFound("Project not found.");

                entity = ProjectFactory.UpdateEntity(entity, form);

                if (form.NewImageFileName != null && form.NewImageFileName.Length > 0)
                {
                    var newImageFileUri = await _fileHandler.UploadFileAsync(form.NewImageFileName);
                    if (!string.IsNullOrEmpty(newImageFileUri))
                        entity.ImageFileName = newImageFileUri;
                }

                var updated = await _projectRepository.UpdateAsync(entity);
                if (!updated)
                    return ResponseResult<ProjectModel>.Failed("Project update failed.");

                var models = await UpdateCacheAsync();
                var model = models.FirstOrDefault(x => x.Id == form.Id);

                return model != null
                            ? ResponseResult<ProjectModel>.Ok(model)
                            : ResponseResult<ProjectModel>.Failed("Could not retrieve updated project.");
            }
            catch (Exception ex)
            {
                return ResponseResult<ProjectModel>.Failed($"An error occurred while trying to update project: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ProjectModel>> UpdateCacheAsync()
        {
            var entities = await _projectRepository.GetAllAsync(
                orderByDescending: true,
                sortBy: x => x.Created,
                filterBy: null,
                i => i.User,
                i => i.User.Address,
                i => i.User.Information,
                i => i.Client,
                i => i.Client.Address,
                i => i.Client.Information,
                i => i.Status
            );

            var models = entities.Select(ProjectFactory.ToModel).ToList();

            _cacheHandler.SetCache(_cacheKey, models);
            return models;
        }

        public ResponseResult ValidateAddForm(AddProjectForm form)
        {
            var fieldValidation = _formValidator.ValidateFormFields(
                form?.ProjectName,
                form?.ClientId,
                form?.UserId
            );

            if (!fieldValidation.Succeeded)
                return fieldValidation;

            var dateValidation = _formValidator.ValidateDateFields(form!.StartDate, form.EndDate);
            if (!dateValidation.Succeeded)
                return dateValidation;

            return ResponseResult.Ok();
        }

        public ResponseResult ValidateUpdateForm(UpdateProjectForm form)
        {
            var fieldValidation = _formValidator.ValidateFormFields(
                form?.ProjectName,
                form?.ClientId,
                form?.UserId
            );

            if (!fieldValidation.Succeeded)
                return fieldValidation;

            var dateValidation = _formValidator.ValidateDateFields(form!.StartDate, form.EndDate);
            if (!dateValidation.Succeeded)
                return dateValidation;

            var statusValidation = _formValidator.ValidateNumberFields(form!.StatusId);
            if (!statusValidation.Succeeded)
                return statusValidation;

            return ResponseResult.Ok();
        }
    }
}
