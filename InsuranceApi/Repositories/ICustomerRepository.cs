using InsuranceApi.DTO;
using InsuranceApi.Models;

namespace InsuranceApi.Repositiries
{
    public interface ICustomerRepository
    {
        Task<Customer> CreateProfile(Customer customer);

        Task<Customer?> UpdateProfile(int id, Customer customer);

        Task<Customer?> GetCustomerById(int id);
        Task<PaginatedResponseDTO<Customer>> ListCustomers(PaginationQueryDto query);

        Task<Customer?> GetCustomerByEmail(string email);
        Task<Customer?> GetCustomerByUserId(int userId);
    }
}
