using InsuranceApi.DTO;
using System.Security.Claims;

namespace InsuranceApi.Services
{
    public interface IClaimDocumentService
    {
        Task<PaginatedResponseDTO<ClaimDocumentResponseDTO>> ListDocuments(PaginationQueryDto query); Task<ClaimDocumentResponseDTO?> GetDocumentById(int documentId);
        Task<IEnumerable<ClaimDocumentResponseDTO>> GetDocumentsByClaimId(int claimId);
        Task<IEnumerable<ClaimDocumentResponseDTO>> GetDocumentsByCustomerId(int customerId);

        Task<IEnumerable<ClaimDocumentResponseDTO>> AddDocument(ClaimDocumentRequestDTO request, ClaimsPrincipal user);

        Task DeleteDocument(int documentId, ClaimsPrincipal user);
    }
}
