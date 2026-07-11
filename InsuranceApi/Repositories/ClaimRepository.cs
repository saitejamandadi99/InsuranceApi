using InsuranceApi.Data;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Repositiries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace InsuranceApi.Repositories
{
    public class ClaimRepository:IClaimRepository
    {
        private readonly DatabaseContext _context;
        public ClaimRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Claim> CreateClaim(Claim claim)
        {
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();
            return claim;
        }

        public async Task<Claim?> GetClaimByClaimNumber(string claimNumber)
        {
            return await _context.Claims.Include(c => c.Policy).ThenInclude(p => p.Customer).ThenInclude(c => c.User).Include(c => c.Policy).ThenInclude(p => p.PolicyPlan).FirstOrDefaultAsync(c => c.ClaimNumber == claimNumber);
        }

        public async Task<IEnumerable<Claim>> GetClaimByCustomerId(int customerId)
        {
            return await _context.Claims.Include(c => c.Policy).ThenInclude(p => p.Customer).ThenInclude(c => c.User).Include(c => c.Policy).ThenInclude(p => p.PolicyPlan).Where(c => c.Policy.CustomerId == customerId).ToListAsync();

        }

        public async Task<Claim?> GetClaimById(int claimId)
        {
            return await _context.Claims.Include(c => c.Policy).ThenInclude(p => p.Customer).ThenInclude(c => c.User).Include(c => c.Policy).ThenInclude(p => p.PolicyPlan).FirstOrDefaultAsync(c => c.ClaimId == claimId);

        }

        public async Task<IEnumerable<Claim>> GetClaimsByPolicyId(int policyId)
        {
            return await _context.Claims.Include(c => c.Policy).ThenInclude(p => p.Customer).ThenInclude(c => c.User).Include(c => c.Policy).ThenInclude(p => p.PolicyPlan).Where(c => c.PolicyId == policyId).ToListAsync();

        }

        public async Task<decimal> GetReserveAmountByPolicyId(int policyId)
        {


            return (await _context.Claims.Where(c => c.PolicyId == policyId && (c.ClaimStatus == ClaimStatus.Pending ||
             c.ClaimStatus == ClaimStatus.RecommendedForApproval ||
             c.ClaimStatus == ClaimStatus.RecommendedForRejection ||
             c.ClaimStatus == ClaimStatus.Approved)).SumAsync(c => (decimal?)c.ClaimAmount)) ?? 0; //?? because sumasync can return null
        }

        public async Task<PaginatedResponseDTO<Claim>> ListClaims(PaginationQueryDto query)
        {
            var claims =  _context.Claims.Include(c => c.Policy).ThenInclude(p => p.Customer).ThenInclude(c => c.User).Include(c => c.Policy).ThenInclude(p => p.PolicyPlan).AsQueryable();

            //searching 
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim().ToLower();
                claims = claims.Where(c =>
                c.ClaimNumber.ToLower().Contains(search) ||
                c.Policy.PolicyNumber.ToLower().Contains(search) ||
                c.Policy.Customer.User.FullName.ToLower().Contains(search));  
            }

            //sorting 
            claims = (query.SortBy?.ToLower(), query.SortDirection?.ToLower()) switch
            {
                ("claimnumber", "desc") => claims.OrderByDescending(c => c.ClaimNumber),
                ("claimnumber", _) => claims.OrderBy(c => c.ClaimNumber),

                ("amount", "desc") => claims.OrderByDescending(c => c.ClaimAmount),
                ("amount", _) => claims.OrderBy(c => c.ClaimAmount),


                ("incidentdate", "desc") => claims.OrderByDescending(c => c.CreatedDate),
                ("incidentdate", _) => claims.OrderBy(c => c.CreatedDate),

                _ => claims.OrderBy(c => c.ClaimId)
            };

            int totalRecords = await claims.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)query.PageSize);

            var pagedClaims = await claims.Skip((query.PageNumber - 1) * query.PageSize)
                                .Take(query.PageSize)
                                .ToListAsync();

            return new PaginatedResponseDTO<Claim>
            {
                Records = pagedClaims,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                IsLastPage = query.PageNumber >= totalPages,
                SortBy = query.SortBy,
                SortDirection = query.SortDirection
            }; 

        }

        public async Task UpdateClaim(Claim claim)
        {
            claim.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }
}
