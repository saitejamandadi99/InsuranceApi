using AutoMapper;
using InsuranceApi.Config;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Repositiries;
using InsuranceApi.Repositories;
using System.Security.Claims;

namespace InsuranceApi.Services
{
    public class CustomerService:ICustomerService
    {
        private readonly IUserRepository _userRepo;
        private readonly ICustomerRepository _cusRepo;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public CustomerService(IUserRepository userRepo, ICustomerRepository cusRepo, IUserService userService, IMapper mapper)
        {
            _userRepo = userRepo;
            _cusRepo = cusRepo;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<CustomerResponseDTO> CreateProfile(CustomerRequestDTO request, ClaimsPrincipal user)
        {
            int userId = user.GetUserId();
            await _userService.ValidateCustomerUser(userId);

            var existingCustomer = await _cusRepo.GetCustomerByUserId(userId);
            if(existingCustomer != null)
            {
                throw new Exception("Customer for this user already exists");
            }

            var customer = _mapper.Map<Customer>(request);
            customer.UserId = userId;
            var createdCustomer = await _cusRepo.CreateProfile(customer);
            return _mapper.Map<CustomerResponseDTO>(createdCustomer);
        }

        public async Task<CustomerResponseDTO?> GetCustomerById(int id)
        {
            var customer = await _cusRepo.GetCustomerById(id);
            if(customer == null)
            {
                throw new Exception("Customr does not exists");
            }
            return _mapper.Map<CustomerResponseDTO>(customer);
        }

        public async Task<CustomerResponseDTO?> GetCustomerByUserId(int userId)
        {
            var customer = await _cusRepo.GetCustomerByUserId(userId);
            if (customer == null)
            {
                throw new Exception("Customr does not exists");
            }
            return _mapper.Map<CustomerResponseDTO>(customer);
        }

        public async Task<PaginatedResponseDTO<CustomerResponseDTO>> ListCustomers(PaginationQueryDto query)
        {
            var customers = await _cusRepo.ListCustomers(query);

            return new PaginatedResponseDTO<CustomerResponseDTO>
            {
                Records = _mapper.Map<IEnumerable<CustomerResponseDTO>>(customers.Records),
                CurrentPage = customers.CurrentPage,
                PageSize = customers.PageSize,
                TotalRecords = customers.TotalRecords,
                TotalPages = customers.TotalPages,
                IsLastPage = customers.IsLastPage,
                SortBy = customers.SortBy,
                SortDirection = customers.SortDirection
            };
        }

        public async Task<CustomerResponseDTO?> UpdateProfile(int id, CustomerRequestDTO request)
        {
            var existingCustomer = await _cusRepo.GetCustomerByUserId(id);
            if (existingCustomer == null)
            {
                throw new Exception("Customer does not exists");
            }
            _mapper.Map(request, existingCustomer);
            var updatedCustomer = await _cusRepo.UpdateProfile(existingCustomer.CustomerId, existingCustomer);
            return _mapper.Map<CustomerResponseDTO>(updatedCustomer);
        }
    }
}
