using InsuranceApi.DTO;
using System.Security.Claims;

namespace InsuranceApi.Services
{
    public interface IPremiumPaymentService
    {
        Task<PaymentResponseDTO> RecordOwnPayment(PaymentRequestDTO request, ClaimsPrincipal user);
        Task<PaymentResponseDTO> RecordPaymentForCustomer(PaymentRequestDTO request);
        Task<PaymentResponseDTO?> GetPaymentById(int paymentId);
        Task<IEnumerable<PaymentResponseDTO>> GetPaymentsByCustomerId(int customerId);
        Task<IEnumerable<PaymentResponseDTO>> GetPaymentsByPolicyId(int policyId);
        Task<PaginatedResponseDTO<PaymentResponseDTO>> ListPayments(PaginationQueryDto query);

    }
}
