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
    public class PolicyController : BaseController
    {
        private readonly IPolicyService _policyService;
        private readonly ICustomerService _customerService;

        public PolicyController(IPolicyService policyService, ICustomerService customerService)
        {
            _policyService = policyService;
            _customerService = customerService;
        }

        // Customer purchases policy
        [Authorize(Roles = nameof(Role.Customer))]
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchasePolicy([FromBody] CustomerPolicyPurchaseRequestDTO request)
        {
            var policy = await _policyService.PurchasePolicy(request, User);
            return CreatedAtSuccess(policy, "policy purchased successfully", nameof(GetPolicyById), new { id = policy.PolicyId });
        }

        // Admin/Officer issues policy
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        [HttpPost("issue")]
        public async Task<IActionResult> IssuePolicy([FromBody] AgentOrAdminPolicyIssueRequestDTO request)
        {
            var policy = await _policyService.IssuePolicy(request);
            return CreatedAtSuccess(policy, "policy issued successfully", nameof(GetPolicyById), new { id = policy.PolicyId });
        }


        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPolicyById(int id)
        {
            var policy = await _policyService.GetPolicyById(id);
            return Success(policy, "Policy retrieved  successfully");
        }


        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        [HttpGet]
        public async Task<IActionResult> ListPolicies([FromQuery] PaginationQueryDto query)
        {
            var policies = await _policyService.ListPolicies(query);
            return Success(policies, "Policies retrieved  successfully");

        }



        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetPoliciesByCustomerId(int customerId)
        {
            var policies = await _policyService.GetPoliciesByCustomerId(customerId);
            return Success(policies, "Policies retrieved  successfully");

        }

        [Authorize(Roles = nameof(Role.Customer))]
        [HttpGet("my-policies")]
        public async Task<IActionResult> GetMyPolicies()
        {
            var userId = User.GetUserId();
            var customer = await _customerService.GetCustomerByUserId(userId);
            var policies = await _policyService.GetPoliciesByCustomerId(customer.CustomerId);
            return Success(policies, "Policies retrieved  successfully");

        }

        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        [HttpGet("plan/{planId}")]
        public async Task<IActionResult> GetPoliciesByPlanId(int planId)
        {
            var policies = await _policyService.GetPoliciesByPlanId(planId);
            return Success(policies, "Policies retrieved  successfully");

        }

        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelPolicy(int id)
        {
            var policy = await _policyService.CancelPolicy(id);
            return Success(policy, "Policy cancelled successfully");

        }
    }
}
