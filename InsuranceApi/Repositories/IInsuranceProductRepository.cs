using InsuranceApi.DTO;
using InsuranceApi.Models;

namespace InsuranceApi.Repositiries
{
    public interface IInsuranceProductRepository
    {
        Task<PaginatedResponseDTO<InsuranceProduct>> ListProducts(PaginationQueryDto query);
        Task<InsuranceProduct> CreateProduct(InsuranceProduct product);
        Task<InsuranceProduct?> DeactivateProduct(int productId);
        Task<InsuranceProduct?> ActivateProduct(int productId);
        Task<InsuranceProduct?> UpdateProduct(int productId, InsuranceProduct product);
        Task<InsuranceProduct?> GetProductById(int productId);

        Task<InsuranceProduct?> GetProductByProductName(string productName);

    }
}
