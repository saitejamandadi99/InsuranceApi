using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles =nameof(Role.Admin))]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] PaginationQueryDto query)
        {
            var users = await _userService.GetAllUsers(query);
            return Success(users, "Retrieved users successfully");
        }
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute]int id)
        {
            var user = await _userService.GetUserById(id);
            return Success(user, "Retrieved user successfully");
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody]RegisterRequestDTO request)
        {
            var user = await _userService.AddUser(request);
            return CreatedAtSuccess(user,"User(officer) created successfully", nameof(GetUserById), new { id = user.UserId });
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPut("activate/{id}")]
        public async Task<IActionResult> ActivateUser([FromRoute] int id)
        {
            var user = await _userService.ActivateUser(id);
            return Success(user, "Activated User successfully");
        }

        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateUser([FromRoute] int id)
        {
            var user = await _userService.DeactivateUser(id);
            return Success(user, "Deactivated user successfully");
        }
    }
}
