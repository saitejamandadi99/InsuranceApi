using InsuranceApi.Data;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly DatabaseContext _context;
        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResponseDTO<User>> GetAllUsers(PaginationQueryDto query)
        {
            var users = _context.Users.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim().ToLower();

                users = users.Where(u =>
                    u.FullName.ToLower().Contains(search) ||
                    u.Email.ToLower().Contains(search));
            }

            // Sorting
            users = (query.SortBy?.ToLower(), query.SortDirection?.ToLower()) switch
            {
                ("fullname", "desc") => users.OrderByDescending(u => u.FullName),
                ("fullname", _) => users.OrderBy(u => u.FullName),

                ("email", "desc") => users.OrderByDescending(u => u.Email),
                ("email", _) => users.OrderBy(u => u.Email),

                ("role", "desc") => users.OrderByDescending(u => u.Role),
                ("role", _) => users.OrderBy(u => u.Role),

                ("activestatus", "desc") => users.OrderByDescending(u => u.ActiveStatus),
                ("activestatus", _) => users.OrderBy(u => u.ActiveStatus),

                ("createddate", "desc") => users.OrderByDescending(u => u.CreatedDate),
                ("createddate", _) => users.OrderBy(u => u.CreatedDate),

                _ => users.OrderBy(u => u.UserId)
            };

            int totalRecords = await users.CountAsync();

            int totalPages = (int)Math.Ceiling(totalRecords / (double)query.PageSize);

            var pagedUsers = await users
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PaginatedResponseDTO<User>
            {
                Records = pagedUsers,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                IsLastPage = query.PageNumber >= totalPages,
                SortBy = query.SortBy,
                SortDirection = query.SortDirection
            };
        }

        public async Task<bool> EmailExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email); // anyAsync = checks the existince 
            

        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;

        }

        public async Task<User?> ActivateUser(int id)
        {
            var existingUser = await GetUserById(id);
            if(existingUser == null)
            {
                return null;
            }
            existingUser.ActiveStatus = ActiveStatus.Active;
            existingUser.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return existingUser;
            
        }

        public async Task<User?> DeactivateUser(int id)
        {
            var existingUser = await GetUserById(id);
            if (existingUser == null)
            {
                return null;
            }
            existingUser.ActiveStatus = ActiveStatus.Inactive;
            existingUser.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return existingUser;
        }
    }
}
