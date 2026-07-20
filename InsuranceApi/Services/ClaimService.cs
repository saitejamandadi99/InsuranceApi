using AutoMapper;
using InsuranceApi.Config;
using InsuranceApi.DTO;
using InsuranceApi.Middleware;
using InsuranceApi.Models;
using InsuranceApi.Repositiries;
using InsuranceApi.Repositories;
using System.Security.Claims;

namespace InsuranceApi.Services
{
    public class ClaimService :IClaimService
    {
        private readonly IClaimRepository _claimRepo;
        private readonly IPolicyRepository _policyRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IClaimDocumentRepository _documentRepo;
        private readonly IClaimStatusHistoryRepository _historyRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<ClaimService> _logger;


        public ClaimService(IClaimRepository claimRepo,IPolicyRepository policyRepo,ICustomerRepository customerRepo,IClaimDocumentRepository documentRepo,IClaimStatusHistoryRepository historyRepo,IMapper mapper, ILogger<ClaimService> logger)
        {
            _claimRepo = claimRepo;
            _policyRepo = policyRepo;
            _customerRepo = customerRepo;
            _documentRepo = documentRepo;
            _historyRepo = historyRepo;
            _mapper = mapper;
            _logger = logger;

        }

        public async Task<ClaimResponseDTO> RaiseClaim(ClaimRequestDTO request,ClaimsPrincipal user)
        {
            var userId = user.GetUserId();

            var customer = await _customerRepo.GetCustomerByUserId(userId);

            if (customer == null)
            {
                throw new Exception("Customer does not exist.");
            }

            if (customer.User.ActiveStatus != ActiveStatus.Active)
            {
                throw new Exception("Customer is not active.");
            }

            var policy = await _policyRepo.GetPolicyById(request.PolicyId);
            if (policy == null)
            {

                throw new Exception("Policy does not exist.");
            }
            // Customer can raise claims only for his own policy
            if (policy.CustomerId != customer.CustomerId)
            {

                throw new Exception("You can raise claims only for your own policies.");
            }

            if (policy.PolicyStatus != PolicyStatus.Active)
            {
                throw new Exception("Claim can be raised only for active policies.");

            }
            if (request.ClaimAmount <= 0)
            {

                throw new Exception("Claim amount should be greater than 0.");
            }
            // Claim amount should not exceed coverage amount
            if (request.ClaimAmount > policy.PolicyPlan.CoverageAmount)
            {

                throw new Exception("Claim amount exceeds policy coverage amount.");
            }
            var reservedAmount = await _claimRepo.GetReserveAmountByPolicyId(policy.PolicyId);
            var availableAmount = policy.PolicyPlan.CoverageAmount - reservedAmount;

            if (request.ClaimAmount > availableAmount)
            {
                throw new Exception($"only {availableAmount:C} coverage is availabe for this policy");
            }
            if (request.IncidentDate > DateTime.Now)
            {

                throw new Exception("Incident date cannot be a future date.");
            }
            if(request.IncidentDate > DateTime.Now)
            {
                throw new Exception("Incident date cannot be a future date");
            }
            if(request.Files == null || request.Files.Count == 0 || request.Files.Any(f=>f.Length == 0))
            {
                throw new Exception("At least one valid document is required");
            }
            
            var claim = new InsuranceApi.Models.Claim
            {
                ClaimNumber = await GenerateClaimNumber(),
                PolicyId = request.PolicyId,
                ClaimAmount = request.ClaimAmount,
                ClaimReason = request.ClaimReason.Trim(),
                IncidentDate = request.IncidentDate,
                ClaimStatus = ClaimStatus.Pending,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            var createdClaim = await _claimRepo.CreateClaim(claim);


            // Save all supporting documents
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "ClaimDocuments");
            if (!Directory.Exists(uploadPath))
            {

                Directory.CreateDirectory(uploadPath);
            }
            foreach (var file in request.Files)
            {
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var fullPath = Path.Combine(uploadPath, uniqueFileName);
                using(var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var document = new ClaimDocument
                {
                    ClaimId = createdClaim.ClaimId,
                    DocumentName = "Supporting Document",
                    DocumentType = "Reference",
                    DocumentReference = uniqueFileName
                };
                await _documentRepo.AddClaimDocument(document);
            }
            // Initial claim history
            var history = new ClaimStatusHistory
            {
                ClaimId = createdClaim.ClaimId,
                PreviousStatus = ClaimStatus.Pending,
                NewStatus = ClaimStatus.Pending,
                Remarks = "Claim Raised",
                UpdatedBy = userId,
                UpdatedDate = DateTime.Now
            };
            await _historyRepo.CreateClaimStatusHistory(history);
            createdClaim.Policy = policy;

            _logger.LogInformation("Claim '{ClaimNumber}' raised for policy '{PolicyNumber}'.",createdClaim.ClaimNumber,policy.PolicyNumber);
            return _mapper.Map<ClaimResponseDTO>(createdClaim);
        }

        public async Task<string> GenerateClaimNumber()
        {
            string claimNumber;
            do
            {
                claimNumber = Random.Shared.NextInt64(1000000000, 9999999999).ToString();
            } 
            while (await _claimRepo.GetClaimByClaimNumber(claimNumber) != null);
            return claimNumber;
        }

        

       

        public async Task<ClaimResponseDTO> OfficerReview(int claimId, OfficerRemarkDTO request, ClaimsPrincipal user)
        {
            var userId = user.GetUserId();
            var newStatus =request.ClaimStatus;
            var claim = await _claimRepo.GetClaimById(claimId);
            if (claim == null)
            {

                throw new Exception("Claim does not exist.");
            }

            if (claim.ClaimStatus != ClaimStatus.Pending)
            {
                throw new Exception("Only pending claims can be reviewed by an officer.");
            }

            if (newStatus != ClaimStatus.RecommendedForApproval && newStatus != ClaimStatus.RecommendedForRejection)
            {
                throw new Exception("Officer can only recommend to approve or reject");

            }

            return await UpdateClaimStatus(claimId, newStatus, request.OfficerRemarks, true, userId);
        }


        public async Task<ClaimResponseDTO> AdminReview(int claimId,AdminRemarkDTO request,ClaimsPrincipal user)
        {
            var userId = user.GetUserId();

            var claim = await _claimRepo.GetClaimById(claimId);

            if (claim == null)
            {

                throw new Exception("Claim does not exist.");
            }

            var newStatus = request.ClaimStatus;

            if (newStatus != ClaimStatus.Approved && newStatus != ClaimStatus.Rejected)
            {
                throw new Exception("Admin can only approve or reject claims.");
            }

            if (newStatus == ClaimStatus.Approved && claim.ClaimStatus != ClaimStatus.RecommendedForApproval)
            {
                throw new Exception("Only claims recommended for approval can be approved.");
            }

            if (newStatus == ClaimStatus.Rejected &&
                claim.ClaimStatus != ClaimStatus.RecommendedForRejection)
            {
                throw new Exception("Only claims recommended for rejection can be rejected.");
            }

            return await UpdateClaimStatus(
                claimId,
                newStatus,
                request.AdminRemarks,
                false,
                userId);
        }


        public async Task<ClaimResponseDTO?> GetClaimById(int claimId)
        {
            var claim = await _claimRepo.GetClaimById(claimId);

            if (claim == null)
            {
                throw new Exception("Claim does not exist.");
            }
            return _mapper.Map<ClaimResponseDTO>(claim);
        }

        public async Task<IEnumerable<ClaimResponseDTO>> GetClaimsByCustomerId(int customerId)
        {
            var claims = await _claimRepo.GetClaimByCustomerId(customerId);
            return _mapper.Map<IEnumerable<ClaimResponseDTO>>(claims);
        }

        public async Task<PaginatedResponseDTO<ClaimResponseDTO>> ListClaims(PaginationQueryDto query)
        {
            var claims = await _claimRepo.ListClaims(query);
            return new PaginatedResponseDTO<ClaimResponseDTO>
            {
                Records = _mapper.Map<IEnumerable<ClaimResponseDTO>>(claims.Records),
                CurrentPage = claims.CurrentPage,
                PageSize = claims.PageSize,
                TotalRecords = claims.TotalRecords,
                TotalPages = claims.TotalPages,
                IsLastPage = claims.IsLastPage,
                SortBy = claims.SortBy,
                SortDirection = claims.SortDirection
            };
        }
        public async Task<IEnumerable<ClaimResponseDTO>> GetClaimsByPolicyId(int policyId)
        {
            var claims = await _claimRepo.GetClaimsByPolicyId(policyId);
            return _mapper.Map<IEnumerable<ClaimResponseDTO>>(claims);
        }




        private async Task<ClaimResponseDTO> UpdateClaimStatus(int claimId,ClaimStatus newStatus,string remarks,bool isOfficer, int updatedBy)
        {
            var claim = await _claimRepo.GetClaimById(claimId);
            if(claim == null)
            {
                throw new Exception("Claim does not exists");
            }

            if (claim.ClaimStatus == ClaimStatus.Approved || claim.ClaimStatus == ClaimStatus.Rejected)
            {
                throw new Exception("Approved or rejected claims cannot be modified.");
            }

            var previousStatus = claim.ClaimStatus;

            claim.ClaimStatus = newStatus;

            if (isOfficer)
            {

                claim.OfficerRemarks = remarks;
            }
            else
            {
                claim.AdminRemarks = remarks;

            }

            await _claimRepo.UpdateClaim(claim);

            var history = new ClaimStatusHistory
            {
                ClaimId = claim.ClaimId,
                PreviousStatus = previousStatus,
                NewStatus = newStatus,
                Remarks = remarks,
                UpdatedBy = updatedBy,
                UpdatedDate = DateTime.Now
            };
            await _historyRepo.CreateClaimStatusHistory(history);
            return _mapper.Map<ClaimResponseDTO>(claim);
        }

    }
}
