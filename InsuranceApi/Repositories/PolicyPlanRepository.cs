using InsuranceApi.Data;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Repositiries;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Repositories
{
    public class PolicyPlanRepository:IPolicyPlanRepository
    {
        private readonly DatabaseContext _context;
        public PolicyPlanRepository(DatabaseContext context)
        {
            _context = context;            
        }

        public async Task<PolicyPlan?> ActivatePlan(int planId)
        {
            var existingPlan = await GetPlanById(planId);
            if (existingPlan != null)
            {
                existingPlan.ActiveStatus = ActiveStatus.Active;
                existingPlan.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return existingPlan;
            }
            return null;
        }

        public async Task<PolicyPlan> CreatePlan(PolicyPlan plan)
        {
            _context.PolicyPlans.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<PolicyPlan?> DeactivatePlan(int planId)
        {
            var existingPlan = await GetPlanById(planId);
            if (existingPlan != null)
            {
                existingPlan.ActiveStatus = ActiveStatus.Inactive;
                existingPlan.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return existingPlan;
            }
            return null;
        }

        public async Task<PolicyPlan?> GetPlanById(int planId)
        {
            return await _context.PolicyPlans.Include(p=>p.InsuranceProduct).FirstOrDefaultAsync(p=>p.PlanId == planId);
        }

        public async Task<PolicyPlan?> GetPlanByName(int productId, string planName)
        {
            return await _context.PolicyPlans.Include(p => p.InsuranceProduct).FirstOrDefaultAsync(p => p.PlanName == planName && p.ProductId== productId);
        }

        public async Task<IEnumerable<PolicyPlan>> GetPlansByProductId(int productId)
        {
            return await _context.PolicyPlans.Include(p => p.InsuranceProduct).Where(p => p.ProductId == productId).ToListAsync();

        }

        public async Task<PaginatedResponseDTO<PolicyPlan>> ListPlans(PaginationQueryDto query)
        {
            var plans = _context.PolicyPlans.Include(p => p.InsuranceProduct).AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim().ToLower();

                plans = plans.Where(p =>
                    p.PlanName.ToLower().Contains(search) ||
                    p.InsuranceProduct.ProductName.ToLower().Contains(search));
            }

            // Sorting
            plans = (query.SortBy?.ToLower(), query.SortDirection?.ToLower()) switch
            {
                ("planname", "desc") => plans.OrderByDescending(p => p.PlanName),
                ("planname", _) => plans.OrderBy(p => p.PlanName),

                ("premiumamount", "desc") => plans.OrderByDescending(p => p.PremiumAmount),
                ("premiumamount", _) => plans.OrderBy(p => p.PremiumAmount),

                ("coverageamount", "desc") => plans.OrderByDescending(p => p.CoverageAmount),
                ("coverageamount", _) => plans.OrderBy(p => p.CoverageAmount),

                ("duration", "desc") => plans.OrderByDescending(p => p.Duration),
                ("duration", _) => plans.OrderBy(p => p.Duration),

                ("createddate", "desc") => plans.OrderByDescending(p => p.CreatedDate),
                ("createddate", _) => plans.OrderBy(p => p.CreatedDate),

                _ => plans.OrderBy(p => p.PlanId)
            };

            int totalRecords = await plans.CountAsync();

            int totalPages = (int)Math.Ceiling(totalRecords / (double)query.PageSize);

            var pagedPlans = await plans.Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PaginatedResponseDTO<PolicyPlan>
            {
                Records = pagedPlans,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                IsLastPage = query.PageNumber >= totalPages,
                SortBy = query.SortBy,
                SortDirection = query.SortDirection
            };
        }

        public async Task<PolicyPlan?> UpdatePlan(int planId, PolicyPlan plan)
        {

            var existingPlan = await GetPlanById(planId);
            if(existingPlan != null)
            {
                existingPlan.ProductId = plan.ProductId;
                existingPlan.PlanName = plan.PlanName;
                existingPlan.CoverageAmount = plan.CoverageAmount;
                existingPlan.PremiumAmount = plan.PremiumAmount;
                existingPlan.Duration = plan.Duration;
                existingPlan.TermsAndConditions = plan.TermsAndConditions;
                existingPlan.ActiveStatus = plan.ActiveStatus;
                existingPlan.PremiumType = plan.PremiumType;
                existingPlan.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return existingPlan;
            }
            return null;
        }
    }
}
