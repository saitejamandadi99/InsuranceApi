using InsuranceApi.Models;
using System.ComponentModel.DataAnnotations;

namespace InsuranceApi.DTO
{
    public class ProductResponseDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public ProductType ProductType { get; set; }
        public string Description { get; set; } = string.Empty;

        public ActiveStatus ActiveStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
