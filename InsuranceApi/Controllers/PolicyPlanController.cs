using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace InsuranceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolicyPlanController : BaseController
    {
        private readonly IPolicyPlanService _planService;

        public PolicyPlanController(IPolicyPlanService planService)
        {
            _planService = planService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListPlans([FromQuery] PaginationQueryDto query)
        {
            var plans = await _planService.ListPlans(query);
            return Success(plans, "Retrieved plans successfully");

        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanById([FromRoute] int id)
        {
            var plan = await _planService.GetPlanById(id);
            return Success(plan, "Retrieved plan successfully");


        }

        [Authorize]
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetPlansByProductId([FromRoute] int productId)
        {
            var plans = await _planService.GetPlansByProductId(productId);
            return Success(plans, "Retrieved plans successfully");
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPost]
        public async Task<IActionResult> CreatePlan([FromBody] PlanRequestDTO request)
        {
            var plan = await _planService.CreatePlan(request);

            return CreatedAtSuccess(plan, "Created plan successfully", nameof(GetPlanById), new { id = plan.PlanId });
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlan([FromRoute] int id,[FromBody] PlanRequestDTO request)
        {
            var plan = await _planService.UpdatePlan(id, request);
            return Success(plan, "Updated plan successfully");

        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPut("activate/{id}")]
        public async Task<IActionResult> ActivatePlan([FromRoute] int id)
        {
            var plan = await _planService.ActivatePlan(id);
            return Success(plan, "Activated plan successfully");

        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivatePlan([FromRoute] int id)
        {
            var plan = await _planService.DeactivatePlan(id);
            return Success(plan, "Deactivated plan successfully");

        }
    }
}
