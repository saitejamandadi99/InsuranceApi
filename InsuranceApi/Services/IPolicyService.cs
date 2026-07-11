using InsuranceApi.DTO;
using System.Security.Claims;

namespace InsuranceApi.Services
{
    public interface IPolicyService
    {
        Task<PolicyResponseDTO> PurchasePolicy(CustomerPolicyPurchaseRequestDTO request, ClaimsPrincipal user);
        Task<PolicyResponseDTO> IssuePolicy(AgentOrAdminPolicyIssueRequestDTO request);
        Task<PolicyResponseDTO?> GetPolicyById(int policyId);
        Task<IEnumerable<PolicyResponseDTO>> GetPoliciesByCustomerId(int customerId);
        Task<IEnumerable<PolicyResponseDTO>> GetPoliciesByPlanId(int planId);
        Task<PaginatedResponseDTO<PolicyResponseDTO>> ListPolicies(PaginationQueryDto query);
        Task<PolicyResponseDTO?> CancelPolicy(int policyId);
        Task<string> GeneratePolicyNumber();
    }
}
