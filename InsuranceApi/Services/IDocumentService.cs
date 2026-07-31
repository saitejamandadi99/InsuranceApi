namespace InsuranceApi.Services
{
    public interface IDocumentService
    {
        Task<string> UploadClaimDocumentAsync(IFormFile file);
        Task DeleteDocumentAsync(string documentReference); 
    }
}
