using InsuranceApi.Config;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InsuranceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimController : BaseController
    {
        private readonly IClaimService _claimService;
        private readonly ICustomerService _cusService;


        public ClaimController(IClaimService claimService, ICustomerService cusService)
        {
            _claimService = claimService;
            _cusService = cusService;
        }

        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        public async Task<IActionResult> ListClaims([FromQuery] PaginationQueryDto query)
        {
            var claims = await _claimService.ListClaims(query);
            return Success(claims, "Claims retrieved successfully");
        }

        [HttpGet("{claimId}")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)},{nameof(Role.Customer)}")]
        public async Task<IActionResult> GetClaimById(int claimId)
        {
            var claim = await _claimService.GetClaimById(claimId);
            return Success(claim, "Claim retrieved successfully");

        }

        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        public async Task<IActionResult> GetClaimsByCustomerId(int customerId)
        {
            var claims = await _claimService.GetClaimsByCustomerId(customerId);
            return Success(claims, "Claims retrieved successfully");

        }

        [HttpGet("my-claims")]
        [Authorize(Roles = nameof(Role.Customer))]
        public async Task<IActionResult> GetMyClaims()
        {
            var customer = await _cusService.GetCustomerByUserId(User.GetUserId());

            var claims = await _claimService.GetClaimsByCustomerId(customer.CustomerId);

            return Success(claims, "Claims retrieved successfully");

        }

        [HttpGet("policy/{policyId}")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        public async Task<IActionResult> GetClaimsByPolicyId(int policyId)
        {
            var claims = await _claimService.GetClaimsByPolicyId(policyId);
            return Success(claims, "Claims retrieved successfully");

        }

        [HttpPost("raise")]
        [Authorize(Roles = nameof(Role.Customer))]
        public async Task<IActionResult> RaiseClaim(ClaimRequestDTO request)
        {
            var claim = await _claimService.RaiseClaim(request, User);
            return CreatedAtSuccess(claim, "Claim raised successfully", nameof(GetClaimById), new { claimId = claim.ClaimId });
        }


        [HttpPut("officer-review/{claimId}")]
        [Authorize(Roles = nameof(Role.Officer))]
        public async Task<IActionResult> OfficerReview(int claimId, OfficerRemarkDTO request)
        {
            var claim = await _claimService.OfficerReview(claimId, request, User);
            return Success(claim, "Claim reviewed successfully");

        }

        [HttpPut("admin-review/{claimId}")]
        [Authorize(Roles = nameof(Role.Admin))]
        public async Task<IActionResult> AdminReview(int claimId, AdminRemarkDTO request)
        {
            var claim = await _claimService.AdminReview(claimId, request, User);
            return Success(claim, "Claim reviewed successfully");

        }

    }
}
