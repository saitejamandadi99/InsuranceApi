using InsuranceApi.Models;

namespace InsuranceApi.DTO
{
    public class PaymentResponseDTO
    {
        public int PaymentId { get; set; }

        //public int PolicyId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentMode PaymentMode { get; set; }
        public string TransactionReference { get; set; } = string.Empty;

        public PaymentStatus PaymentStatus { get; set; }
    }
}
