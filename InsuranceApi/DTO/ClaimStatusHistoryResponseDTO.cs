using InsuranceApi.Models;

namespace InsuranceApi.DTO
{
    public class ClaimStatusHistoryResponseDTO
    {
        public int HistoryId { get; set; }

        public ClaimStatus PreviousStatus { get; set; }

        public ClaimStatus NewStatus { get; set; }

        public string Remarks { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public string ClaimNumber { get; set; } = string.Empty;
        public DateTime UpdatedDate { get; set; }
    }
}
