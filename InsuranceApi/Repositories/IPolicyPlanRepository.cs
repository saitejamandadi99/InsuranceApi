using InsuranceApi.DTO;
using InsuranceApi.Models;

namespace InsuranceApi.Repositiries
{
    public interface IPolicyPlanRepository
    {
        Task<PolicyPlan> CreatePlan(PolicyPlan plan); 
        Task<PolicyPlan?> UpdatePlan(int planId, PolicyPlan plan); 
        Task<PolicyPlan?> DeactivatePlan(int planId); 
        Task<PolicyPlan?> ActivatePlan(int planId);
        Task<PolicyPlan?> GetPlanById(int planId);
        Task<IEnumerable<PolicyPlan>> GetPlansByProductId(int productId);
        Task<PolicyPlan?> GetPlanByName(int productId,string planName);
        Task<PaginatedResponseDTO<PolicyPlan>> ListPlans(PaginationQueryDto query);

    }
}
