using InsuranceApi.DTO;
using InsuranceApi.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace InsuranceApi.Repositories
{
    public interface IUserRepository
    {
        Task<PaginatedResponseDTO<User>> GetAllUsers(PaginationQueryDto query);

        Task<User?> GetUserById(int id);
        Task<User?> GetUserByEmail(string email);

        Task<User> AddUser(User user);
        Task<User?> ActivateUser(int id);
        Task<User?> DeactivateUser(int id);

        Task<bool> EmailExists(string email);


    }
}
