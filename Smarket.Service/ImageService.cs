using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Smarket.DataAccess.Repository.IRepository;
using Smarket.Models;
using Smarket.Services.IServices;


namespace Smarket.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IUnitOfWork _unitOfWork;


        public ImageService(IOptions<CloudinarySettings> config, IUnitOfWork unitOfWork)
        {
            var acc = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);
            _unitOfWork = unitOfWork;
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {


            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),

                    //Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Transformation = new Transformation(),
                    Folder = "da-net7"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<VideoUploadResult> AddVideoAsync(IFormFile file)
        {
            var uploadResult = new VideoUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "da-net7",
                    EagerAsync = true,
                    EagerTransforms = new List<Transformation>()
                    {
                        new EagerTransformation().Width(300).Height(300).Crop("fill").AudioCodec("none"),
                        new EagerTransformation().Width(160).Height(100).Crop("fill").Gravity("south").AudioCodec("none"),
                    }
                };
                uploadResult = await _cloudinary.UploadLargeAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}