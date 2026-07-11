using InsuranceApi.Data;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Repositiries;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DatabaseContext _context;
        public CustomerRepository(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<Customer> CreateProfile(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<PaginatedResponseDTO<Customer>> ListCustomers(PaginationQueryDto query)
        {
            var customers = _context.Customers
                .Include(c => c.User)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim().ToLower();

                customers = customers.Where(c =>
                    c.User.FullName.ToLower().Contains(search) ||
                    c.User.Email.ToLower().Contains(search) ||
                    c.City.ToLower().Contains(search) ||
                    c.State.ToLower().Contains(search) ||
                    c.Pincode.ToLower().Contains(search) ||
                    c.NomineeName.ToLower().Contains(search));
            }

            // Sorting
            customers = (query.SortBy?.ToLower(), query.SortDirection?.ToLower()) switch
            {
                ("fullname", "desc") => customers.OrderByDescending(c => c.User.FullName),
                ("fullname", _) => customers.OrderBy(c => c.User.FullName),

                ("city", "desc") => customers.OrderByDescending(c => c.City),
                ("city", _) => customers.OrderBy(c => c.City),

                ("state", "desc") => customers.OrderByDescending(c => c.State),
                ("state", _) => customers.OrderBy(c => c.State),

                ("dateofbirth", "desc") => customers.OrderByDescending(c => c.DateOfBirth),
                ("dateofbirth", _) => customers.OrderBy(c => c.DateOfBirth),

                ("createddate", "desc") => customers.OrderByDescending(c => c.CreatedDate),
                ("createddate", _) => customers.OrderBy(c => c.CreatedDate),

                _ => customers.OrderBy(c => c.CustomerId)
            };

            int totalRecords = await customers.CountAsync();

            int totalPages = (int)Math.Ceiling(totalRecords / (double)query.PageSize);

            var pagedCustomers = await customers
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PaginatedResponseDTO<Customer>
            {
                Records = pagedCustomers,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                IsLastPage = query.PageNumber >= totalPages,
                SortBy = query.SortBy,
                SortDirection = query.SortDirection
            };
        }

        public async Task<Customer?> GetCustomerById(int id)
        {
            return await _context.Customers.Include(c=>c.User).FirstOrDefaultAsync(c=>c.CustomerId==id);
        }

        public async Task<Customer?> GetCustomerByUserId(int userId)
        {
            return await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.UserId==userId);
        }
        public async Task<Customer?> GetCustomerByEmail(string email)
        {
            return await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.User != null && c.User.Email == email);
        }

        public async Task<Customer?> UpdateProfile(int id, Customer customer)
        {

            var existingCustomer = await GetCustomerById(id);
            if(existingCustomer != null)
            {
                existingCustomer.DateOfBirth = customer.DateOfBirth;
                existingCustomer.Address = customer.Address;
                existingCustomer.City = customer.City;
                existingCustomer.State = customer.State;
                existingCustomer.Pincode = customer.Pincode;
                existingCustomer.NomineeName = customer.NomineeName;
                existingCustomer.NomineeRelationship = customer.NomineeRelationship;
                existingCustomer.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return existingCustomer;
            }
            return null;
        }
    }
}
