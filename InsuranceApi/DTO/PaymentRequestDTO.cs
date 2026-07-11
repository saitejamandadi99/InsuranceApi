using InsuranceApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceApi.DTO
{
    public class PaymentRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Policy.")]
        public int PolicyId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, double.MaxValue, ErrorMessage = "amount  should be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment mode is required")]

        public PaymentMode PaymentMode { get; set; }
        [Required(ErrorMessage = "Transaction Reference is required")]
        [StringLength(50)]
        public string TransactionReference { get; set; } = string.Empty;

    }
}
