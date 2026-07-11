using InsuranceApi.DTO;
using InsuranceApi.Models;

namespace InsuranceApi.Repositiries
{
    public interface IClaimRepository
    {
        Task<PaginatedResponseDTO<Claim>> ListClaims(PaginationQueryDto query);
        Task<IEnumerable<Claim>> GetClaimsByPolicyId(int policyId);
        Task<IEnumerable<Claim>> GetClaimByCustomerId(int customerId);
        Task<Claim?> GetClaimById(int claimId);
        Task<Claim?> GetClaimByClaimNumber(string claimNumber);
        Task<Claim> CreateClaim(Claim claim);
        Task UpdateClaim(Claim claim);

        Task<decimal> GetReserveAmountByPolicyId(int policyId);
    }
}
