using InsuranceApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceApi.DTO
{
    public class CustomerResponseDTO
    {
        public int CustomerId { get; set; }

        
        public string Email { get; set; }

        public string MobileNumber { get; set; }

        public DateOnly DateOfBirth { get; set; }
        public string Address { get; set; }

        public string City { get; set; }


        public string State { get; set; }

        public string Pincode { get; set; }

        public string NomineeName { get; set; }

        public string NomineeRelationship { get; set; }
    }
}
