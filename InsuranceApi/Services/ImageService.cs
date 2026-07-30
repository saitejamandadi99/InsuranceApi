using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace InsuranceApi.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        public ImageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadImageToCloudAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("Profile image is required.");
            }
            if (!file.ContentType.StartsWith("image/"))
            {
                throw new Exception("Only image files are allowed.");
            }

            const long maxFileSize = 5 * 1024 * 1024; // 5MB

            if (file.Length > maxFileSize)
            {
                throw new Exception("Image size cannot exceed 5 MB.");
            }

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "insurance/customer-profiles"
            };


            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if(uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }
            if (uploadResult.SecureUrl == null)
            {
                throw new Exception("Image upload failed.");
            }
            return uploadResult.SecureUrl.AbsoluteUri;
        }
    }
}
