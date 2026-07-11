using AutoMapper;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Repositories;

namespace InsuranceApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<UserResponseDTO?> ActivateUser(int id)
        {
            var user = await _userRepo.ActivateUser(id);
            if(user == null)
            {
                throw new Exception("User Not Found");
            }
            return _mapper.Map<UserResponseDTO>(user);

        }

        public async Task<UserResponseDTO> AddUser(RegisterRequestDTO dto)
        {
            if (await _userRepo.EmailExists(dto.Email))
            {
                throw new Exception("Email already Exists");
            }

            var user = _mapper.Map<User>(dto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Role = Role.Officer;
            var addedUser = await _userRepo.AddUser(user);
            return _mapper.Map<UserResponseDTO>(addedUser);

        }

        public async Task<UserResponseDTO?> DeactivateUser(int id)
        {
            var user = await _userRepo.DeactivateUser(id);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<PaginatedResponseDTO<UserResponseDTO>> GetAllUsers(PaginationQueryDto query)
        {
            var users = await _userRepo.GetAllUsers(query);

            return new PaginatedResponseDTO<UserResponseDTO>
            {
                Records = _mapper.Map<IEnumerable<UserResponseDTO>>(users.Records),
                CurrentPage = users.CurrentPage,
                PageSize = users.PageSize,
                TotalRecords = users.TotalRecords,
                TotalPages = users.TotalPages,
                IsLastPage = users.IsLastPage,
                SortBy = users.SortBy,
                SortDirection = users.SortDirection
            };
        }

        public async Task<UserResponseDTO?> GetUserById(int id)
        {
            var user = await _userRepo.GetUserById(id);
            if(user == null)
            {
                throw new Exception("User not Found");
            }
            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<User> ValidateCustomerUser(int userId)
        {
            var user = await _userRepo.GetUserById(userId);

            if (user == null)
            {

                throw new Exception("User not found.");
            }



            if (user.ActiveStatus != ActiveStatus.Active)
            {

                throw new Exception("User is inactive.");
            }

            if (user.Role != Role.Customer)
            {

                throw new Exception("Only customers are allowed.");
            }

            return user;
        }
    }
}
