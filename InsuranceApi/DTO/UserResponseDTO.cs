using InsuranceApi.Models;
using System.ComponentModel.DataAnnotations;

namespace InsuranceApi.DTO
{
    public class UserResponseDTO
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string MobileNumber { get; set; }
        public Role Role { get; set; }

        public ActiveStatus ActiveStatus { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
