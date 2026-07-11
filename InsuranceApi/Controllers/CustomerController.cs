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
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _cusService;
        public CustomerController(ICustomerService cusService)
        {
            _cusService = cusService;
        }

        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]

        [HttpGet]
        public async Task<IActionResult> ListCustomers([FromQuery] PaginationQueryDto query)
        {
            var customers = await _cusService.ListCustomers(query);
            return Success(customers, "Retrieved customers successfully");
        }

        
        [Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Officer)}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById([FromRoute] int id)
        {
            var customer = await _cusService.GetCustomerById(id);
            return Success(customer, "Retrieved customer successfully");

        }


        [Authorize(Roles = nameof(Role.Customer))]

        [HttpGet("/profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var customer = await _cusService.GetCustomerByUserId(User.GetUserId());
            return Success(customer, "Retrieved Profile successfully");

        }


        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCustomerByUserId([FromRoute] int userId)
        {
            var customer = await _cusService.GetCustomerByUserId(userId);
            return Success(customer, "Retrieved customer successfully");

        }

        [Authorize(Roles =nameof(Role.Customer))]
        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromBody] CustomerRequestDTO request)
        {
            var customer = await _cusService.CreateProfile(request, User);
            return CreatedAtSuccess(customer, "customer profile created successfully", nameof(GetCustomerById), new { id = customer.CustomerId});
        }

        [Authorize(Roles = nameof(Role.Customer))]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] CustomerRequestDTO request)
        {
            var customer = await _cusService.UpdateProfile(User.GetUserId(), request);
            return Success(customer, "Updated customer successfully");


        }

    }
}
