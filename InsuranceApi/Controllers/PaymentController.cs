using InsuranceApi.Config;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly IPremiumPaymentService _paymentService;
        private readonly ICustomerService _cusService;

        public PaymentController(IPremiumPaymentService paymentService, ICustomerService cusService)
        {
            _paymentService = paymentService;
            _cusService = cusService;
        }

        // Admin & Officer - View all payments
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        [HttpGet]
        public async Task<IActionResult> ListPayments([FromQuery] PaginationQueryDto query)
        {
            var payments = await _paymentService.ListPayments(query);
            return Success(payments, "Payments retrieved successfully");
        }

        // Admin & Officer - View payment by Id
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(int paymentId)
        {
            var payment = await _paymentService.GetPaymentById(paymentId);
            return Success(payment, "Payment retrieved successfully");
        }

        // Admin & Officer - View payments by customer
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetPaymentsByCustomerId(int customerId)
        {
            var payments = await _paymentService.GetPaymentsByCustomerId(customerId);
            return Success(payments, "Payments retrieved successfully");
        }

        [HttpGet("my-payments")]
        [Authorize(Roles = nameof(Role.Customer))]
        public async Task<IActionResult> GetMyPayments()
        {
            var customer = await _cusService.GetCustomerByUserId(User.GetUserId());

            var payments = await _paymentService.GetPaymentsByCustomerId(customer.CustomerId);

            return Success(payments, "Payments retrieved successfully");
        }

        // Admin & Officer - View payments by policy
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        [HttpGet("policy/{policyId}")]
        public async Task<IActionResult> GetPaymentsByPolicyId(int policyId)
        {
            var payments = await _paymentService.GetPaymentsByPolicyId(policyId);
            return Success(payments, "Payments retrieved successfully");
        }

        // Customer - Record payment for own policy
        [Authorize(Roles = nameof(Role.Customer))]
        [HttpPost("my-payment")]
        public async Task<IActionResult> RecordOwnPayment([FromBody] PaymentRequestDTO request)
        {
            var payment = await _paymentService.RecordOwnPayment(request, User);

            return CreatedAtSuccess(payment, "Payment recorded successfully", nameof(GetPaymentById), new { paymentId = payment.PaymentId });
        }

        // Admin & Officer - Record payment for any customer's policy
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        [HttpPost("officer-payment")]
        public async Task<IActionResult> RecordPaymentForCustomer([FromBody] PaymentRequestDTO request)
        {
            var payment = await _paymentService.RecordPaymentForCustomer(request);

            return CreatedAtSuccess(payment, "Payment recorded successfully", nameof(GetPaymentById), new { paymentId = payment.PaymentId });

        }
    }
}
