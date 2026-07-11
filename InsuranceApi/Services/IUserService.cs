using InsuranceApi.DTO;
using InsuranceApi.Models;
using System.Security.Claims;

namespace InsuranceApi.Services
{
    public interface IUserService
    {
        Task<PaginatedResponseDTO<UserResponseDTO>> GetAllUsers(PaginationQueryDto query);
        Task<UserResponseDTO?> GetUserById(int id);
        Task<UserResponseDTO> AddUser(RegisterRequestDTO dto);

        Task<UserResponseDTO?> ActivateUser(int id);
        Task<UserResponseDTO?> DeactivateUser(int id);
        Task<User> ValidateCustomerUser(int userId);

    }
}
