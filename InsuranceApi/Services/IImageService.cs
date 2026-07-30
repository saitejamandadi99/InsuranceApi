namespace InsuranceApi.Services
{
    public interface IImageService
    {
        Task<string> UploadImageToCloudAsync(IFormFile file);
    }
}
