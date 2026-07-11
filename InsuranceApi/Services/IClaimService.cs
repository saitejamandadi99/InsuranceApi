using InsuranceApi.DTO;
using System.Security.Claims;

namespace InsuranceApi.Services
{
    public interface IClaimService
    {
        Task<PaginatedResponseDTO<ClaimResponseDTO>> ListClaims(PaginationQueryDto query);

        Task<IEnumerable<ClaimResponseDTO>> GetClaimsByCustomerId(int customerId);

        Task<IEnumerable<ClaimResponseDTO>> GetClaimsByPolicyId(int policyId);

        Task<ClaimResponseDTO?> GetClaimById(int claimId);

        Task<ClaimResponseDTO> RaiseClaim(ClaimRequestDTO request, ClaimsPrincipal user);

        Task<ClaimResponseDTO> OfficerReview(int claimId, OfficerRemarkDTO request, ClaimsPrincipal user);

        Task<ClaimResponseDTO> AdminReview(int claimId, AdminRemarkDTO request, ClaimsPrincipal user);
        Task<string> GenerateClaimNumber();
    }
}
