using Microsoft.AspNetCore.Http;

namespace Core.Infrastructures.CloudinaryServices;

public interface ICloudinaryService
{
    Task<string> UploadImage(IFormFile formFile, string imageDirectory);
}
