using Microsoft.AspNetCore.Http;

namespace Business.Handlers
{
    public class ImageHandler(string imagePath) 
    {
        private readonly string _imagePath = imagePath;

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null!;

            var fileExtension = Path.GetExtension(file.FileName);

            var fileName = $"n{Guid.NewGuid()}{fileExtension}";

            if (!Directory.Exists(_imagePath))
                Directory.CreateDirectory(_imagePath);

            var filePath = Path.Combine(_imagePath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }
    }
}
