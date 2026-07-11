using InsuranceApi.DTO;

namespace InsuranceApi.Services
{
    public interface IInsuranceProductService
    {
        Task<PaginatedResponseDTO<ProductResponseDTO>> ListProducts(PaginationQueryDto query);
        Task<ProductResponseDTO?> GetProductById(int productId);
        Task<ProductResponseDTO> CreateProduct(ProductRequestDTO request);
        Task<ProductResponseDTO?> UpdateProduct(int productId, ProductRequestDTO request);
        Task<ProductResponseDTO?> ActivateProduct(int productId);
        Task<ProductResponseDTO?> DeactivateProduct(int productId);
    }
}

