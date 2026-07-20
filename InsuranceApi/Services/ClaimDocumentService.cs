using AutoMapper;
using Azure.Core;
using InsuranceApi.Config;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Repositiries;
using InsuranceApi.Repositories;
using System.Security.Claims;

namespace InsuranceApi.Services
{
    public class ClaimDocumentService : IClaimDocumentService
    {
        private readonly IClaimDocumentRepository _documentRepo;
        private readonly IClaimRepository _claimRepo;
        private readonly ICustomerRepository _cusRepo;

        private readonly IMapper _mapper;

        public ClaimDocumentService(IClaimDocumentRepository documentRepo, IMapper mapper, IClaimRepository claimRepo, ICustomerRepository cusRepo)
        {
            _documentRepo = documentRepo;
            _mapper = mapper;
            _claimRepo = claimRepo;
            _cusRepo = cusRepo;
        }
        public async Task<PaginatedResponseDTO<ClaimDocumentResponseDTO>> ListDocuments(PaginationQueryDto query)
        {
            var documents = await _documentRepo.ListDocuments(query);

            return new PaginatedResponseDTO<ClaimDocumentResponseDTO>
            {
                Records = _mapper.Map<IEnumerable<ClaimDocumentResponseDTO>>(documents.Records),
                CurrentPage = documents.CurrentPage,
                PageSize = documents.PageSize,
                TotalRecords = documents.TotalRecords,
                TotalPages = documents.TotalPages,
                IsLastPage = documents.IsLastPage,
                SortBy = documents.SortBy,
                SortDirection = documents.SortDirection
            };
        }

        public async Task<ClaimDocumentResponseDTO?> GetDocumentById(int documentId)
        {
            var document = await _documentRepo.GetDocumentById(documentId);
            return _mapper.Map<ClaimDocumentResponseDTO>(document);
        }

        public async Task<IEnumerable<ClaimDocumentResponseDTO>> GetDocumentsByClaimId(int claimId)
        {
            var documents = await _documentRepo.GetDocumentsByClaimId(claimId);

            return _mapper.Map<IEnumerable<ClaimDocumentResponseDTO>>(documents);
        }

        public async Task<IEnumerable<ClaimDocumentResponseDTO>> GetDocumentsByCustomerId(int customerId)
        {
            var documents = await _documentRepo.GetDocumentsByCustomerId(customerId);
            return _mapper.Map<IEnumerable<ClaimDocumentResponseDTO>>(documents);
        }

        public async Task<IEnumerable<ClaimDocumentResponseDTO>> AddDocument(ClaimDocumentRequestDTO request, ClaimsPrincipal user)
        {
            var existingClaim = await _claimRepo.GetClaimById(request.ClaimId);
            if(existingClaim == null)
            {
                throw new Exception("Claim does not exists");
            }
            var customer = await _cusRepo.GetCustomerByUserId(user.GetUserId());
            if(customer == null)
            {
                throw new Exception("customer does not exists");

            }
            if (customer.User.ActiveStatus != ActiveStatus.Active)
            {
                throw new Exception("customer is not active");

            }
            if (existingClaim.Policy.CustomerId != customer.CustomerId)
            {
                throw new Exception("You can upload documents only for your own claims.");

            }
            if(existingClaim.ClaimStatus == ClaimStatus.Approved || existingClaim.ClaimStatus == ClaimStatus.Rejected)
            {
                throw new Exception("Documents cannot be added after claim is finalized.");
            }

            if(request.Files == null || request.Files.Count == 0)
            {
                throw new Exception("At least one file is required");
            }

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "ClaimDocuments");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var createdDocs = new List<ClaimDocument>();

            foreach(var file in request.Files)
            {
                if(file.Length == 0)
                {
                    continue;
                }
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var fullPath = Path.Combine(uploadPath, uniqueFileName);

                using(var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var doc = new ClaimDocument
                {
                    ClaimId = request.ClaimId,
                    DocumentName = request.DocumentName,
                    DocumentType = request.DocumentType,
                    DocumentReference = uniqueFileName,
                    UploadedDate = DateTime.Now
                };

                var created = await _documentRepo.AddClaimDocument(doc);
                createdDocs.Add(created);
            }

            return _mapper.Map<IEnumerable<ClaimDocumentResponseDTO>>(createdDocs);

        }

        public async Task DeleteDocument(int documentId, ClaimsPrincipal user)
        {
            var document = await _documentRepo.GetDocumentById(documentId);

            if (document == null)
            {
                throw new Exception("Document does not exist.");
            }
          
            
            var customer = await _cusRepo.GetCustomerByUserId(user.GetUserId());
            if (customer == null)
            {
                throw new Exception("customer does not exists");

            }
            if (customer.User.ActiveStatus != ActiveStatus.Active)
            {
                throw new Exception("customer is not active");

            }
            if (document.Claim.Policy.CustomerId != customer.CustomerId)
            {
                throw new Exception("You can delete documents only for your own claims.");

            }

            if (document.Claim.ClaimStatus == ClaimStatus.Approved || document.Claim.ClaimStatus == ClaimStatus.Rejected)
            {
                throw new Exception("Documents cannot be deleted after claim is finalized.");
            }

            await _documentRepo.DeleteDocument(document);

        }
    }
}
