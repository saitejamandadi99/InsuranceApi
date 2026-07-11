using InsuranceApi.Config;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimStatusHistoryController : BaseController
    {
        private readonly IClaimStatusHistoryService _historyService;
        private readonly ICustomerService _cusService;


        public ClaimStatusHistoryController(IClaimStatusHistoryService historyService, ICustomerService cusService)
        {
            _historyService = historyService;
            _cusService = cusService;

        }

        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        public async Task<IActionResult> ListClaimStatusHistories([FromQuery] PaginationQueryDto query)
        {
            var histories = await _historyService.ListClaimStatusHistories(query);

            return Success(histories, "Claim status histories retrieved successfully.");
        }

        [HttpGet("claim/{claimId}")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        public async Task<IActionResult> GetClaimStatusHistoryByClaimId(int claimId)
        {
            var histories = await _historyService.GetClaimStatusHistoryByClaimId(claimId);
            return Success(histories, "Claim status histories retrieved successfully.");

        }

        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        public async Task<IActionResult> GetClaimStatusHistoryByCustomerId(int customerId)
        {
            var histories = await _historyService.GetClaimStatusHistoryByCustomerId(customerId);
            return Success(histories, "Claim status histories retrieved successfully.");

        }

        [HttpGet("my-history")]
        [Authorize(Roles = nameof(Role.Customer))]
        public async Task<IActionResult> GetMyHistory()
        {
            var customer = await _cusService.GetCustomerByUserId(User.GetUserId());

            var histories = await _historyService.GetClaimStatusHistoryByCustomerId(customer.CustomerId);

            return Success(histories, "Claim status histories retrieved successfully.");

        }
    }
}
