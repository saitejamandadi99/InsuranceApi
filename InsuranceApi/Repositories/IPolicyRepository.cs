using InsuranceApi.DTO;
using InsuranceApi.Models;

namespace InsuranceApi.Repositiries
{
    public interface IPolicyRepository
    {
        Task<PaginatedResponseDTO<Policy>> ListPolicies(PaginationQueryDto query);
        Task<Policy?> CreatePolicy(Policy policy);
        Task<Policy?> GetPolicyByPolicyNumber(string policyNumber);
        Task<Policy?> CancelPolicy(int policyId);
        Task<Policy?> GetPolicyById(int policyId);
        Task<IEnumerable<Policy>> GetPoliciesByCustomerId(int customerId);
        Task<IEnumerable<Policy>> GetPoliciesByPlanId(int planId);
        Task UpdatePolicy(Policy policy);

    }
}
