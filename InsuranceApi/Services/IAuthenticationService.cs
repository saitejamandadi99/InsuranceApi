using InsuranceApi.DTO;

namespace InsuranceApi.Services
{
    public interface IAuthService
    {
        Task<SuccessResponseDTO<string>> RegisterCustomer(RegisterRequestDTO request);
        Task<LoginResponseDTO> Login(LoginRequestDTO request);

    }
}
