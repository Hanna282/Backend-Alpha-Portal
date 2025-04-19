using Microsoft.AspNetCore.Http;

namespace Business.Handlers
{
    public interface IFileHandler
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
