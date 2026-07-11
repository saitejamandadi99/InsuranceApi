using AutoMapper;
using InsuranceApi.Config;
using InsuranceApi.DTO;
using InsuranceApi.Middleware;
using InsuranceApi.Models;
using InsuranceApi.Repositiries;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace InsuranceApi.Services
{
    public class PremiumPaymentService : IPremiumPaymentService
    {
        private readonly IPremiumPaymentRepository _paymentRepo;
        private readonly IPolicyRepository _policyRepo;
        private readonly ICustomerRepository _cusRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<PremiumPaymentService> _logger;


        public PremiumPaymentService(IPremiumPaymentRepository paymentRepo,IPolicyRepository policyRepo,ICustomerRepository cusRepo, IMapper mapper, ILogger<PremiumPaymentService> logger)
        {
            _paymentRepo = paymentRepo;
            _policyRepo = policyRepo;
            _cusRepo = cusRepo;
            _mapper = mapper;
            _logger = logger;

        }

        public async Task<PaymentResponseDTO?> GetPaymentById(int paymentId)
        {
            var payment = await _paymentRepo.GetPaymentById(paymentId);
            if (payment == null)
                throw new Exception("Payment not found.");
            return _mapper.Map<PaymentResponseDTO>(payment);
        }

        public async Task<IEnumerable<PaymentResponseDTO>> GetPaymentsByCustomerId(int customerId)
        {
            var payments = await _paymentRepo.GetPaymentsByCustomerId(customerId);
            return _mapper.Map<IEnumerable<PaymentResponseDTO>>(payments);
        }

        public async Task<IEnumerable<PaymentResponseDTO>> GetPaymentsByPolicyId(int policyId)
        {
            var payments = await _paymentRepo.GetPaymentsByPolicyId(policyId);
            return _mapper.Map<IEnumerable<PaymentResponseDTO>>(payments);
        }

        public async Task<PaginatedResponseDTO<PaymentResponseDTO>> ListPayments(PaginationQueryDto query)
        {
            var payments = await _paymentRepo.ListPayments(query);
            return new PaginatedResponseDTO<PaymentResponseDTO>
            {
                Records = _mapper.Map<IEnumerable<PaymentResponseDTO>>(payments.Records),
                CurrentPage = payments.CurrentPage,
                PageSize = payments.PageSize,
                TotalRecords = payments.TotalRecords,
                TotalPages = payments.TotalPages,
                IsLastPage = payments.IsLastPage,
                SortBy = payments.SortBy,
                SortDirection = payments.SortDirection
            };
        }

        public async Task<PaymentResponseDTO> RecordOwnPayment(PaymentRequestDTO request, ClaimsPrincipal user)
        {
            var userId = user.GetUserId();
            var customer = await _cusRepo.GetCustomerByUserId(userId);
            if (customer == null)
            {
                throw new Exception("Customer does not exists");
            }
            if (customer.User.ActiveStatus != ActiveStatus.Active)
            {
                throw new Exception("Customer is not active");
            }

            var policy = await _policyRepo.GetPolicyById(request.PolicyId);
            if (policy == null)
            {
                throw new Exception("Policy does not exist.");
            }
            if (policy.CustomerId != customer.CustomerId)
            {

                throw new Exception("You can pay only for your own policies.");
            }
            if (policy.PolicyStatus == PolicyStatus.Cancelled)
            {
                throw new Exception("Cannot make payment for a cancelled policy.");
            }


            return await RecordPayment(request, policy);
        }

        public async Task<PaymentResponseDTO> RecordPaymentForCustomer(PaymentRequestDTO request)
        {
            var policy = await _policyRepo.GetPolicyById(request.PolicyId);
            if (policy == null)
            {
               throw new Exception("Policy does not exist.");
            }
            if (policy.PolicyStatus == PolicyStatus.Cancelled)
            {
                throw new Exception("Cannot make payment for a cancelled policy.");
            }

            return await RecordPayment(request, policy);
        }

        private async Task<PaymentResponseDTO> RecordPayment(PaymentRequestDTO request, Policy policy)
        {
            var existingTransaction =await _paymentRepo.GetPaymentByTransactionReference(request.TransactionReference);

            if (existingTransaction != null)
            {
                throw new Exception("Transaction reference already exists.");
            }
            if (request.Amount <= 0)
            {
                throw new Exception("Amount should be greater than 0.");
            }
            if (policy.TotalPremiumPaid + request.Amount > policy.PolicyPlan.PremiumAmount)
            {
                throw new Exception("Payment exceeds premium amount.");
            }

            var payment = _mapper.Map<PremiumPayment>(request);
            payment.PaymentDate = DateTime.Now;
            payment.PaymentStatus = PaymentStatus.Success;
            payment.CreatedDate = DateTime.Now;

            var createdPayment = await _paymentRepo.RecordPayment(payment);

            policy.TotalPremiumPaid += request.Amount;
            if (policy.TotalPremiumPaid >= policy.PolicyPlan.PremiumAmount)
            {
                policy.PolicyStatus = PolicyStatus.Active;
            }

            policy.UpdatedDate = DateTime.Now;

            await _policyRepo.UpdatePolicy(policy);

            createdPayment.Policy = policy;
            _logger.LogInformation("Premium payment of {Amount} recorded for policy '{PolicyNumber}'.",request.Amount,policy.PolicyNumber);
            return _mapper.Map<PaymentResponseDTO>(createdPayment);
        }
    }
}
