using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace InsuranceApi.DTO
{
    public class AgentOrAdminPolicyIssueRequestDTO
    {

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Customer.")]
        public int CustomerId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid plan.")]
        public int PlanId { get; set; }

        public DateTime StartDate { get; set; }
    }
}
