using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Core.Infrastructures.CloudinaryServices;

public sealed class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;
    private readonly Account _account;
    private readonly CloudinarySettings _cloudinarySettings;
    public CloudinaryService(IOptions<CloudinarySettings> cloudOptions)
    {
        _cloudinarySettings = cloudOptions.Value;
        _account = new Account(_cloudinarySettings.CloudName, _cloudinarySettings.ApiKey, _cloudinarySettings.ApiSecret);
        _cloudinary = new CloudinaryDotNet.Cloudinary(_account);
    }
    public async Task<string> UploadImage(IFormFile formFile, string imageDirectory)
    {
        if (formFile.Length > 0)
        {
            using var stream = formFile.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(formFile.FileName, stream),
                Folder = imageDirectory
            };
            var imageUploadResult = await _cloudinary.UploadAsync(uploadParams);
            EnsureSuccess(imageUploadResult);
            return imageUploadResult.SecureUrl.ToString();
        }
        return string.Empty;
    }

    public async Task<string> UploadVideo(IFormFile formFile, string videoDirectory)
    {
        if (formFile.Length > 0)
        {
            using var stream = formFile.OpenReadStream();

            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(formFile.FileName, stream),
                Folder = videoDirectory
            };

            var videoUploadResult = await _cloudinary.UploadAsync(uploadParams);
            EnsureSuccess(videoUploadResult);

            return videoUploadResult.SecureUrl.ToString();
        }

        return string.Empty; 
    }

    private static void EnsureSuccess(UploadResult result)
    {
        if (result.Error != null || result.SecureUrl == null)
        {
            throw new InvalidOperationException($"Cloudinary yüklemesi başarısız: {result.Error?.Message ?? "bilinmeyen hata"}");
        }
    }
}
