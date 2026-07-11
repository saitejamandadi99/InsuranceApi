using InsuranceApi.Data;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Repositiries;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Repositories
{
    public class InsuranceProductRepository : IInsuranceProductRepository
    {
        private readonly DatabaseContext _context;
        public InsuranceProductRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<InsuranceProduct?> ActivateProduct(int productId)
        {
            var existingInsuranceProduct = await GetProductById(productId);
            if(existingInsuranceProduct != null)
            {
                existingInsuranceProduct.ActiveStatus = ActiveStatus.Active;
                existingInsuranceProduct.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return existingInsuranceProduct;
            }
            return null;
        }

        public async Task<InsuranceProduct> CreateProduct(InsuranceProduct product)
        {
            _context.InsuranceProducts.Add(product);
            product.ActiveStatus = ActiveStatus.Active;
            product.CreatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<InsuranceProduct?> DeactivateProduct(int productId)
        {
            var existingProduct = await GetProductById(productId);
            if(existingProduct != null)
            {
                existingProduct.ActiveStatus = ActiveStatus.Inactive;
                existingProduct.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return existingProduct;
            }
            return null;
        }


        public async Task<InsuranceProduct?> GetProductById(int productId)
        {
            return await _context.InsuranceProducts.FindAsync(productId);
        }

        public async Task<InsuranceProduct?> GetProductByProductName(string productName)
        {
            return await _context.InsuranceProducts.FirstOrDefaultAsync(p => p.ProductName == productName);
        }

        public async Task<PaginatedResponseDTO<InsuranceProduct>> ListProducts(PaginationQueryDto query)
        {
            var products=  _context.InsuranceProducts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim().ToLower();
                products = products.Where(p => p.ProductName.ToLower().Contains(search) || p.Description.ToLower().Contains(search));
            }

            products = (query.SortBy?.ToLower(), query.SortDirection?.ToLower()) switch
            {
                ("productname", "desc") => products.OrderByDescending(p => p.ProductName),
                ("productname", _) => products.OrderBy(p => p.ProductName),

                ("producttype", "desc") => products.OrderByDescending(p => p.ProductType),
                ("producttype", _) => products.OrderBy(p => p.ProductType),

                ("activestatus", "desc") => products.OrderByDescending(p => p.ActiveStatus),
                ("activestatus", _) => products.OrderBy(p => p.ActiveStatus),

                ("createddate", "desc") => products.OrderByDescending(p => p.CreatedDate),
                ("createddate", _) => products.OrderBy(p => p.CreatedDate),

                _ => products.OrderBy(p => p.ProductId)
            };

            int totalRecords = await products.CountAsync();

            int totalPages = (int)Math.Ceiling(totalRecords / (double)query.PageSize);

            var pagedProducts = await products
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PaginatedResponseDTO<InsuranceProduct>
            {
                Records = pagedProducts,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                IsLastPage = query.PageNumber >= totalPages,
                SortBy = query.SortBy,
                SortDirection = query.SortDirection
            };


        }

        public async Task<InsuranceProduct?> UpdateProduct(int productId, InsuranceProduct product)
        {

            var existingProduct = await GetProductById(productId);
            if (existingProduct != null)
            {
                existingProduct.ProductName = product.ProductName;
                existingProduct.ProductType = product.ProductType;
                existingProduct.Description = product.Description;
                existingProduct.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return existingProduct;
            }
            return null;
        }
    }
}
