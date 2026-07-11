using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceApi.Models
{
    public class Policy
    {
        [Key]
        public int PolicyId { get; set; }

        [Required(ErrorMessage ="Policy Number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage ="Policy number should be 10 digits")]
        public string PolicyNumber { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [ForeignKey("PolicyPlan")]
        public int PlanId { get; set; }

        [Required(ErrorMessage = "Start Date Number is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date Number is required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Policy Status is required")]
        public PolicyStatus PolicyStatus { get; set; }

        [Range(0, double.MaxValue, ErrorMessage ="Amount should be greater than 0")]
        public decimal TotalPremiumPaid { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public virtual Customer? Customer { get; set; }

        public virtual PolicyPlan? PolicyPlan { get; set; }


        public virtual ICollection<PremiumPayment>? PremiumPayments { get; set; }
        public virtual ICollection<Claim>? Claims { get; set; }

    }
}
