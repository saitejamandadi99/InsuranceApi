using InsuranceApi.DTO;
using InsuranceApi.Models;

namespace InsuranceApi.Repositiries
{
    public interface IPremiumPaymentRepository
    {
        Task<PaginatedResponseDTO<PremiumPayment>> ListPayments(PaginationQueryDto query);
        Task<IEnumerable<PremiumPayment>> GetPaymentsByCustomerId(int customerId);
        Task<IEnumerable<PremiumPayment>> GetPaymentsByPolicyId(int policyId);
        Task<PremiumPayment> RecordPayment(PremiumPayment payment);
        Task<PremiumPayment?> GetPaymentById(int paymentId);
        Task<PremiumPayment?> GetPaymentByTransactionReference(string transactionReference);

    }
}
