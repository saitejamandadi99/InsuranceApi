using InsuranceApi.DTO;
using System.Security.Claims;

namespace InsuranceApi.Services
{
    public interface ICustomerService
    {
        Task<PaginatedResponseDTO<CustomerResponseDTO>> ListCustomers(PaginationQueryDto query);
        Task<CustomerResponseDTO?> GetCustomerById(int id);
        Task<CustomerResponseDTO?> GetCustomerByUserId(int userId);
        Task<CustomerResponseDTO> CreateProfile(CustomerRequestDTO request, ClaimsPrincipal user);
        Task<CustomerResponseDTO?> UpdateProfile(int id, CustomerRequestDTO request);


    }
}
