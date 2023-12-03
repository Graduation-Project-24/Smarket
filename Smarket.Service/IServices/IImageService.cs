using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;


namespace Smarket.Services.IServices
{
    public interface IImageService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);

        Task<VideoUploadResult> AddVideoAsync(IFormFile file);

        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}