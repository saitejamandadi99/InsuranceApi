using InsuranceApi.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApi.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message)
        {
            return Ok(new SuccessResponseDTO<T>
            {
                Success = true,
                Message = message,
                Data = data
            });
        }

        protected IActionResult CreatedAtSuccess<T>(T data, string message, string actionName, object routeValues)
        {
            return CreatedAtAction(actionName, routeValues, (new SuccessResponseDTO<T>
            {
                Success = true,
                Message = message,
                Data = data
            }));
        }
    }
}
