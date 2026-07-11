using InsuranceApi.DTO;
using InsuranceApi.Models;

namespace InsuranceApi.Repositories
{
    public interface IClaimStatusHistoryRepository
    {
        Task<PaginatedResponseDTO<ClaimStatusHistory>> ListClaimStatusHistories(PaginationQueryDto query);
        Task<IEnumerable<ClaimStatusHistory>> GetClaimStatusHistoryByClaimId(int claimId);
        Task<IEnumerable<ClaimStatusHistory>> GetClaimStatusHistoryByCustomerId(int customerId);

        Task<ClaimStatusHistory> CreateClaimStatusHistory(ClaimStatusHistory history);
    }
}
