using InsuranceApi.Models;

namespace InsuranceApi.DTO
{
    public class UserStatusUpdateRequestDTO
    {
        public ActiveStatus ActiveStatus { get; set; }

        public string Remarks { get; set; } = string.Empty;
    }
}
