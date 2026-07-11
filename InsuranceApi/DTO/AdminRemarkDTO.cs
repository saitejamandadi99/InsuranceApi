using InsuranceApi.Models;
using System.ComponentModel.DataAnnotations;

namespace InsuranceApi.DTO
{
    public class AdminRemarkDTO
    {
        [Range(4,5,ErrorMessage ="Admin can only approve or reject.")]
        public ClaimStatus ClaimStatus { get; set; }
        [Required]
        [StringLength(300, MinimumLength = 5)]
        public string AdminRemarks { get; set; } = string.Empty;
    }
}
