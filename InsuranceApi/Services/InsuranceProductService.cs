using AutoMapper;
using InsuranceApi.DTO;
using InsuranceApi.Middleware;
using InsuranceApi.Models;
using InsuranceApi.Repositiries;

namespace InsuranceApi.Services
{
    public class InsuranceProductService : IInsuranceProductService
    {
        private readonly IInsuranceProductRepository _prodRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<InsuranceProductService> _logger;

        public InsuranceProductService(IInsuranceProductRepository prodRepo, IMapper mapper, ILogger<InsuranceProductService> logger)
        {
            _prodRepo = prodRepo;
            _mapper = mapper;
            _logger = logger;

        }

        public async Task<ProductResponseDTO?> ActivateProduct(int productId)
        {
            var existingProduct = await _prodRepo.GetProductById(productId);
            if (existingProduct == null)
            {
                throw new Exception("product does not exists");
            }
            var product = await _prodRepo.ActivateProduct(productId);
            return _mapper.Map<ProductResponseDTO>(product);
        }

        public async Task<ProductResponseDTO> CreateProduct(ProductRequestDTO request)
        {
            var productName = request.ProductName.Trim().ToLower();
            var existingProduct = await _prodRepo.GetProductByProductName(productName);
            if(existingProduct != null)
            {
                throw new Exception("Product with this name already exists");
            }

            var addProduct = _mapper.Map<InsuranceProduct>(request);
            addProduct.ProductName = productName;
            addProduct.ActiveStatus = ActiveStatus.Active;
            var product = await _prodRepo.CreateProduct(addProduct);

            _logger.LogInformation("Insurance product '{ProductName}' created.",product.ProductName);
            return _mapper.Map<ProductResponseDTO>(product);
        }

        public async Task<ProductResponseDTO?> DeactivateProduct(int productId)
        {
            var existingProduct = await _prodRepo.GetProductById(productId);
            if(existingProduct == null)
            {
                throw new Exception("product does not exists");
            }
            var product = await _prodRepo.DeactivateProduct(productId);
            return _mapper.Map<ProductResponseDTO>(product);
        }

        public async Task<ProductResponseDTO?> GetProductById(int productId)
        {
            var product = await _prodRepo.GetProductById(productId);
            if (product == null)
            {
                throw new Exception("Product does not exist");
            }
            return _mapper.Map<ProductResponseDTO>(product);
        }

        public async Task<PaginatedResponseDTO<ProductResponseDTO>> ListProducts(PaginationQueryDto query)
        {
            var products = await _prodRepo.ListProducts(query);

            return new PaginatedResponseDTO<ProductResponseDTO>
            {
                Records = _mapper.Map<IEnumerable<ProductResponseDTO>>(products.Records),
                CurrentPage = products.CurrentPage,
                PageSize = products.PageSize,
                TotalRecords = products.TotalRecords,
                TotalPages = products.TotalPages,
                IsLastPage = products.IsLastPage,
                SortBy = products.SortBy,
                SortDirection = products.SortDirection
            };
        }

        public async Task<ProductResponseDTO?> UpdateProduct(int productId, ProductRequestDTO request)
        {
            var existingProduct = await _prodRepo.GetProductById(productId);
            if (existingProduct == null)
            {
                throw new Exception("product does not exists");
            }
            var productName = request.ProductName.Trim().ToLower();
            var duplicateProduct = await _prodRepo.GetProductByProductName(productName);
            if (duplicateProduct != null && duplicateProduct.ProductId != productId)
            {
                throw new Exception("Product name already exists.");
            }
            _mapper.Map(request, existingProduct);
            existingProduct.ProductName = productName;
            var updatedProduct = await _prodRepo.UpdateProduct(productId, existingProduct);
            return _mapper.Map<ProductResponseDTO>(updatedProduct);
        }
    }
}
