using InsuranceApi.Models;
using System.ComponentModel.DataAnnotations;

namespace InsuranceApi.DTO
{
    public class ProductRequestDTO
    {
        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "product name should be between 3 to 100 characters")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product Type is required")]
        public ProductType ProductType { get; set; }

        [Required(ErrorMessage = "Description is requried")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "description should be between 3 to 200 characters")]
        public string Description { get; set; }

        //[Required(ErrorMessage = "Active Status is requried")]
        //public ActiveStatus ActiveStatus { get; set; }
    }
}
