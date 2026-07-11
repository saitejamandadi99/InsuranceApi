using System.ComponentModel.DataAnnotations;

namespace InsuranceApi.DTO
{
    public class CustomerPolicyPurchaseRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid plan.")]
        public int PlanId { get; set; }

        [Required(ErrorMessage ="Start date is required")]
        public DateTime StartDate { get; set; }
    }
}
