using InsuranceApi.DTO;
using InsuranceApi.Models;

namespace InsuranceApi.Repositories
{
    public interface IClaimDocumentRepository
    {
        Task<ClaimDocument> AddClaimDocument(ClaimDocument document);
        Task<IEnumerable<ClaimDocument>> GetDocumentsByClaimId(int claimId);

        Task<ClaimDocument?> GetDocumentById(int documentId);

        Task<IEnumerable<ClaimDocument>> GetDocumentsByCustomerId(int customerId);

        Task<PaginatedResponseDTO<ClaimDocument>> ListDocuments(PaginationQueryDto query);

        Task DeleteDocument(ClaimDocument document);
    }
}
