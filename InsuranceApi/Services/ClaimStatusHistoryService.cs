using AutoMapper;
using InsuranceApi.DTO;
using InsuranceApi.Repositories;

namespace InsuranceApi.Services
{
    public class ClaimStatusHistoryService : IClaimStatusHistoryService
    {
        private readonly IClaimStatusHistoryRepository _historyRepo;
        private readonly IMapper _mapper;

        public ClaimStatusHistoryService(IClaimStatusHistoryRepository historyRepo, IMapper mapper)
        {
            _historyRepo = historyRepo;
            _mapper = mapper;
        }

        public async Task<PaginatedResponseDTO<ClaimStatusHistoryResponseDTO>> ListClaimStatusHistories(PaginationQueryDto query)
        {
            var histories = await _historyRepo.ListClaimStatusHistories(query);

            return new PaginatedResponseDTO<ClaimStatusHistoryResponseDTO>
            {
                Records = _mapper.Map<IEnumerable<ClaimStatusHistoryResponseDTO>>(histories.Records),
                CurrentPage = histories.CurrentPage,
                PageSize = histories.PageSize,
                TotalRecords = histories.TotalRecords,
                TotalPages = histories.TotalPages,
                IsLastPage = histories.IsLastPage,
                SortBy = histories.SortBy,
                SortDirection = histories.SortDirection
            };
        }

        public async Task<IEnumerable<ClaimStatusHistoryResponseDTO>> GetClaimStatusHistoryByClaimId(int claimId)
        {
            var histories = await _historyRepo.GetClaimStatusHistoryByClaimId(claimId);
            return _mapper.Map<IEnumerable<ClaimStatusHistoryResponseDTO>>(histories);
        }

        public async Task<IEnumerable<ClaimStatusHistoryResponseDTO>> GetClaimStatusHistoryByCustomerId(int customerId)
        {
            var histories = await _historyRepo.GetClaimStatusHistoryByCustomerId(customerId);
            return _mapper.Map<IEnumerable<ClaimStatusHistoryResponseDTO>>(histories);
        }
    }
}
