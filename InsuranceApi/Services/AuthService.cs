using AutoMapper;
using InsuranceApi.Config;
using InsuranceApi.DTO;
using InsuranceApi.Middleware;
using InsuranceApi.Models;
using InsuranceApi.Repositories;
using Microsoft.Extensions.Options;

namespace InsuranceApi.Services
{
    public class AuthService:IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly JWTSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepo, ITokenService tokenService , IMapper mapper, IOptions<JWTSettings> options, ILogger<AuthService> logger)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
            _mapper = mapper;
            _jwtSettings = options.Value;
            _logger = logger;

        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO request)
        {
            var user = await _userRepo.GetUserByEmail(request.Email);
            if(user == null)
            {
                throw new Exception("User is not Found");
            }
            bool passwordMatched = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!passwordMatched)
            {
                throw new Exception("Invalid email or password");

            }
            if (user.ActiveStatus == ActiveStatus.Inactive)
            {
                throw new Exception("User is not Active");
            }

            _logger.LogInformation("User '{Fullname}' logged in successfully.",user.FullName);

            return new LoginResponseDTO
            {
                JwtToken = _tokenService.GenerateToken(user),
                UserEmail = user.Email,
                UserRole = user.Role.ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes)
            };
        }

        public async Task<SuccessResponseDTO<string>> RegisterCustomer(RegisterRequestDTO request)
        {
            var emailExists = await _userRepo.EmailExists(request.Email);
            if (emailExists)
            {
                throw new Exception("Email already exists.");
            }

            var user = _mapper.Map<User>(request);
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.Role = Role.Customer;
            user.ActiveStatus = ActiveStatus.Active;
            await _userRepo.AddUser(user);
            _logger.LogInformation("User '{Fullname}' Registered successfully.", user.FullName);

            return new SuccessResponseDTO<string>
            {
                Success = true,
                Message = "User registered successfully",
                Data = "Registration Successful"
            };
        }
    }
}
