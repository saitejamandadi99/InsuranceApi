using InsuranceApi.DTO;

namespace InsuranceApi.Services
{
    public interface IClaimStatusHistoryService
    {
        Task<PaginatedResponseDTO<ClaimStatusHistoryResponseDTO>> ListClaimStatusHistories(PaginationQueryDto query);
        Task<IEnumerable<ClaimStatusHistoryResponseDTO>> GetClaimStatusHistoryByClaimId(int claimId);
        Task<IEnumerable<ClaimStatusHistoryResponseDTO>> GetClaimStatusHistoryByCustomerId(int customerId);
    }
}
