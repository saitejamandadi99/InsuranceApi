using InsuranceApi.Data;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Repositories
{
    public class ClaimDocumentRepository : IClaimDocumentRepository
    {
        private readonly DatabaseContext _context;
        public ClaimDocumentRepository(DatabaseContext context)
        {
            _context = context; 
        }

        public async Task<ClaimDocument> AddClaimDocument(ClaimDocument document)
        {
            _context.ClaimDocuments.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task DeleteDocument(ClaimDocument document)
        {
            _context.ClaimDocuments.Remove(document);
            await _context.SaveChangesAsync();
        }

        public async Task<ClaimDocument?> GetDocumentById(int documentId)
        {
            return await _context.ClaimDocuments.Include(c => c.Claim).ThenInclude(c => c.Policy).ThenInclude(c => c.Customer).FirstOrDefaultAsync(c => c.DocumentId == documentId);
        }

        public async Task<IEnumerable<ClaimDocument>> GetDocumentsByClaimId(int claimId)
        {
            return await _context.ClaimDocuments.Include(c => c.Claim).ThenInclude(c => c.Policy).ThenInclude(c => c.Customer).Where(c => c.ClaimId == claimId).ToListAsync();

        }

        public async Task<PaginatedResponseDTO<ClaimDocument>> ListDocuments(PaginationQueryDto query)
        {
            var documents = _context.ClaimDocuments
                .Include(c => c.Claim)
                    .ThenInclude(c => c.Policy)
                        .ThenInclude(c => c.Customer)
                            .ThenInclude(c => c.User)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim().ToLower();

                documents = documents.Where(d =>
                    d.DocumentName.ToLower().Contains(search) ||
                    d.DocumentType.ToLower().Contains(search) ||
                    d.Claim.ClaimNumber.ToLower().Contains(search) ||
                    d.Claim.Policy.Customer.User.FullName.ToLower().Contains(search));
            }

            // Sorting
            documents = (query.SortBy?.ToLower(), query.SortDirection?.ToLower()) switch
            {
                ("documentname", "desc") => documents.OrderByDescending(d => d.DocumentName),
                ("documentname", _) => documents.OrderBy(d => d.DocumentName),

                ("documenttype", "desc") => documents.OrderByDescending(d => d.DocumentType),
                ("documenttype", _) => documents.OrderBy(d => d.DocumentType),

                ("uploadeddate", "desc") => documents.OrderByDescending(d => d.UploadedDate),
                ("uploadeddate", _) => documents.OrderBy(d => d.UploadedDate),

                _ => documents.OrderBy(d => d.DocumentId)
            };

            int totalRecords = await documents.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)query.PageSize);

            var pagedDocuments = await documents
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return new PaginatedResponseDTO<ClaimDocument>
            {
                Records = pagedDocuments,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                IsLastPage = query.PageNumber >= totalPages,
                SortBy = query.SortBy,
                SortDirection = query.SortDirection
            };
        }

        public async Task<IEnumerable<ClaimDocument>> GetDocumentsByCustomerId(int customerId)
        {
            return await _context.ClaimDocuments.Include(c => c.Claim).ThenInclude(c => c.Policy).ThenInclude(c => c.Customer).Where(c => c.Claim.Policy.CustomerId == customerId).ToListAsync();

        }
    }
}
