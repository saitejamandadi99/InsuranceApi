using InsuranceApi.DTO;

namespace InsuranceApi.Services
{
    public interface IPolicyPlanService
    {
        Task<PaginatedResponseDTO<PlanResponseDTO>> ListPlans(PaginationQueryDto query);
        Task<IEnumerable<PlanResponseDTO>> GetPlansByProductId(int productId);
        Task<PlanResponseDTO?> GetPlanById(int planId);
        Task<PlanResponseDTO> CreatePlan(PlanRequestDTO request);
        Task<PlanResponseDTO?> UpdatePlan(int planId, PlanRequestDTO request);
        Task<PlanResponseDTO?> ActivatePlan(int planId);
        Task<PlanResponseDTO?> DeactivatePlan(int planId);
    }
}
