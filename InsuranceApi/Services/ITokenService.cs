using InsuranceApi.Models;

namespace InsuranceApi.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
