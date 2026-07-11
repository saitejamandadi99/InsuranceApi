using InsuranceApi.Data;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Repositiries;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Repositories
{
    public class PolicyRepository :IPolicyRepository
    {
        private readonly DatabaseContext _context;
        public PolicyRepository(DatabaseContext  context)
        {
            _context = context;
        }

        public async Task<Policy?> CancelPolicy(int policyId)
        {
            var existingPolicy = await GetPolicyById(policyId);
            if(existingPolicy != null)
            {
                existingPolicy.PolicyStatus = PolicyStatus.Cancelled;
                existingPolicy.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return existingPolicy;
            }
            return null;
        }

        public async Task<Policy?> CreatePolicy(Policy policy) //works for both issue policy and purchasae policy
        {
            _context.Policies.Add(policy);
            await _context.SaveChangesAsync();
            return policy;
        }

        public async Task<IEnumerable<Policy>> GetPoliciesByCustomerId(int customerId)
        {
            return await _context.Policies.Include(p => p.PolicyPlan).Include(p => p.Customer).ThenInclude(p=>p.User).Where(p=>p.CustomerId  == customerId).ToListAsync();
        }

        public async Task<IEnumerable<Policy>> GetPoliciesByPlanId(int planId)
        {
            return await _context.Policies.Include(p => p.PolicyPlan).Include(p => p.Customer).ThenInclude(p => p.User).Where(p => p.PlanId == planId).ToListAsync();

        }

        public async Task<Policy?> GetPolicyById(int policyId)
        {
            return await _context.Policies.Include(p => p.PolicyPlan).Include(p => p.Customer).ThenInclude(p => p.User).FirstOrDefaultAsync(p => p.PolicyId == policyId);
        }

        public async Task<Policy?> GetPolicyByPolicyNumber(string policyNumber)
        {
            return await _context.Policies.Include(p => p.PolicyPlan).Include(p => p.Customer).ThenInclude(p => p.User).FirstOrDefaultAsync(p => p.PolicyNumber == policyNumber);
        }

        public async Task<PaginatedResponseDTO<Policy>> ListPolicies(PaginationQueryDto query)
        {
            var policies =  _context.Policies.Include(p => p.PolicyPlan).Include(p => p.Customer).ThenInclude(p => p.User).AsQueryable();
            //search 
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim().ToLower();
                policies = policies.Where(p =>
                p.PolicyNumber.ToLower().Contains(search) ||
                p.Customer.User.FullName.ToLower().Contains(search) ||
                p.PolicyPlan.PlanName.ToLower().Contains(search)
                );
            }

            policies = (query.SortBy?.ToLower(), query.SortDirection?.ToLower()) switch
            {
                ("policynumber", "desc") => policies.OrderByDescending(p => p.PolicyNumber),
                ("policynumber", _) => policies.OrderBy(p => p.PolicyNumber),
                ("planname", "desc") => policies.OrderByDescending(p => p.PolicyPlan.PlanName),
                ("planname", _) => policies.OrderBy(p => p.PolicyPlan.PlanName),


                ("startdate", "desc") => policies.OrderByDescending(p => p.StartDate),
                ("startdate", _) => policies.OrderBy(p => p.StartDate),

                ("enddate", "desc") => policies.OrderByDescending(p => p.EndDate),
                ("enddate", _) => policies.OrderBy(p => p.EndDate),

                ("totalpremiumpaid", "desc") => policies.OrderByDescending(p => p.TotalPremiumPaid),
                ("totalpremiumpaid", _) => policies.OrderBy(p => p.TotalPremiumPaid),

                ("policystatus", "desc") => policies.OrderByDescending(p => p.PolicyStatus),
                ("policystatus", _) => policies.OrderBy(p => p.PolicyStatus),

                ("createddate", "desc") => policies.OrderByDescending(p => p.CreatedDate),
                ("createddate", _) => policies.OrderBy(p => p.CreatedDate),

                _ => policies.OrderBy(p => p.PolicyId)
            };

            int totalRecords = await policies.CountAsync();

            int totalPages = (int)Math.Ceiling(totalRecords / (double)query.PageSize);

            var pagedPolicies = await policies.Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize).ToListAsync();

            return new PaginatedResponseDTO<Policy>
            {
                Records = pagedPolicies,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                IsLastPage = query.PageNumber >= totalPages,
                SortBy = query.SortBy,
                SortDirection = query.SortDirection
            };
        }

        public async Task UpdatePolicy(Policy policy)
        {
            policy.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }
}
