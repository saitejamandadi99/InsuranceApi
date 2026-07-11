using InsuranceApi.Data;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Repositories
{
    public class ClaimStatusHistoryRepository : IClaimStatusHistoryRepository
    {
        private readonly DatabaseContext _context;
        public ClaimStatusHistoryRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ClaimStatusHistory> CreateClaimStatusHistory(ClaimStatusHistory history)
        {
            _context.ClaimStatusHistories.Add(history);
            await _context.SaveChangesAsync();
            return history;
        }

        public async Task<IEnumerable<ClaimStatusHistory>> GetClaimStatusHistoryByClaimId(int claimId)
        {
            return await _context.ClaimStatusHistories.Include(c => c.Claim).ThenInclude(c => c.Policy).ThenInclude(c => c.Customer).Where(c => c.ClaimId == claimId).ToListAsync();
        }

        public async Task<IEnumerable<ClaimStatusHistory>> GetClaimStatusHistoryByCustomerId(int customerId)
        {
            return await _context.ClaimStatusHistories.Include(c => c.Claim).ThenInclude(c => c.Policy).ThenInclude(c => c.Customer).Where(c => c.Claim.Policy.CustomerId == customerId).ToListAsync();

        }

        public async Task<PaginatedResponseDTO<ClaimStatusHistory>> ListClaimStatusHistories(PaginationQueryDto query)
        {
            var histories = _context.ClaimStatusHistories.Include(c=>c.User).Include(c => c.Claim)
                    .ThenInclude(c => c.Policy).ThenInclude(c => c.Customer).ThenInclude(c => c.User).AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim().ToLower();

                histories = histories.Where(h =>
                    h.Claim.ClaimNumber.ToLower().Contains(search) ||
                    h.User.FullName.ToLower().Contains(search) ||
                    h.Remarks.ToLower().Contains(search));
            }

            // Sorting
            histories = (query.SortBy?.ToLower(), query.SortDirection?.ToLower()) switch
            {
                ("newstatus", "desc") => histories.OrderByDescending(h => h.NewStatus),
                ("newstatus", _) => histories.OrderBy(h => h.NewStatus),

                ("previousstatus", "desc") => histories.OrderByDescending(h => h.PreviousStatus),
                ("previousstatus", _) => histories.OrderBy(h => h.PreviousStatus),

                ("updateddate", "desc") => histories.OrderByDescending(h => h.UpdatedDate),
                ("updateddate", _) => histories.OrderBy(h => h.UpdatedDate),

                ("updatedby", "desc") => histories.OrderByDescending(h => h.User.FullName),
                ("updatedby", _) => histories.OrderBy(h => h.User.FullName),

                _ => histories.OrderBy(h => h.HistoryId)
            };

            int totalRecords = await histories.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)query.PageSize);

            var pagedHistories = await histories
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PaginatedResponseDTO<ClaimStatusHistory>
            {
                Records = pagedHistories,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                IsLastPage = query.PageNumber >= totalPages,
                SortBy = query.SortBy,
                SortDirection = query.SortDirection
            };
        }
    }
}
