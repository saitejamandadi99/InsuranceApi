using AutoMapper;
using InsuranceApi.Config;
using InsuranceApi.DTO;
using InsuranceApi.Middleware;
using InsuranceApi.Models;
using InsuranceApi.Repositiries;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace InsuranceApi.Services
{
    public class PolicyService : IPolicyService
    {

        private readonly IPolicyRepository _policyRepo;
        private readonly ICustomerRepository _cusRepo;
        private readonly IPolicyPlanRepository _planRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<PolicyService> _logger;


        public PolicyService(IPolicyRepository policyRepo, ICustomerRepository cusRepo, IPolicyPlanRepository planRepo, IMapper mapper, ILogger<PolicyService> logger)
        {
            _policyRepo = policyRepo;
            _cusRepo = cusRepo;
            _planRepo = planRepo;
            _mapper = mapper;
            _logger = logger;

        }
        public async Task<PolicyResponseDTO?> CancelPolicy(int policyId)
        {

            var existingPolicy = await _policyRepo.GetPolicyById(policyId);
            if(existingPolicy == null)
            {
                throw new Exception("Policy does not exists");
            }
            var policy = await _policyRepo.CancelPolicy(policyId);
            return _mapper.Map<PolicyResponseDTO>(policy);
        }

        public async Task<IEnumerable<PolicyResponseDTO>> GetPoliciesByCustomerId(int customerId)
        {
            var policies = await _policyRepo.GetPoliciesByCustomerId(customerId);
            return _mapper.Map<IEnumerable<PolicyResponseDTO>>(policies);
        }

        public async Task<IEnumerable<PolicyResponseDTO>> GetPoliciesByPlanId(int planId)
        {
            var policies = await _policyRepo.GetPoliciesByPlanId(planId);
            return _mapper.Map<IEnumerable<PolicyResponseDTO>>(policies);
        }

        public async Task<PolicyResponseDTO?> GetPolicyById(int policyId)
        {
            var existingPolicy = await _policyRepo.GetPolicyById(policyId);
            if (existingPolicy == null)
            {
                throw new Exception("Policy does not exists");
            }
            return _mapper.Map<PolicyResponseDTO>(existingPolicy);
        }

        public async Task<PolicyResponseDTO> IssuePolicy(AgentOrAdminPolicyIssueRequestDTO request)
        {
            var customer = await _cusRepo.GetCustomerById(request.CustomerId);
            if (customer == null)
            {
                throw new Exception("Customer not exists");
            }
            

            if (!IsCustomerProfileComplete(customer))
            {
                throw new Exception("Customer profile is incomplete.");
            }

            if (customer.User.ActiveStatus != ActiveStatus.Active)
            {
                throw new Exception("Customer is not active");
            }

            var plan = await _planRepo.GetPlanById(request.PlanId);
            if (plan == null)
            {
                throw new Exception("plan not exists");
            }
            if (plan.ActiveStatus != ActiveStatus.Active)
            {
                throw new Exception("plan is not active");
            }

            var addPolicy = new Policy
            {
                CustomerId = customer.CustomerId,
                PlanId = plan.PlanId,
                PolicyNumber =await GeneratePolicyNumber(),
                StartDate = request.StartDate,
                EndDate = request.StartDate.AddYears(plan.Duration),
                PolicyStatus = PolicyStatus.PendingPayment,
                TotalPremiumPaid = 0,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,

            };

            var createdPolicy = await _policyRepo.CreatePolicy(addPolicy);
            createdPolicy.Customer = customer;
            createdPolicy.PolicyPlan = plan;
            _logger.LogInformation("Policy '{PolicyNumber}' issued for customer '{CustomerId}'.",createdPolicy.PolicyNumber,customer.CustomerId);
            return _mapper.Map<PolicyResponseDTO>(createdPolicy);
        }

        public async Task<PaginatedResponseDTO<PolicyResponseDTO>> ListPolicies(PaginationQueryDto query)
        {
            var policies = await _policyRepo.ListPolicies(query);
            return new PaginatedResponseDTO<PolicyResponseDTO>
            {
                Records = _mapper.Map<IEnumerable<PolicyResponseDTO>>(policies.Records),
                CurrentPage = policies.CurrentPage,
                PageSize = policies.PageSize,
                TotalRecords = policies.TotalRecords,
                TotalPages = policies.TotalPages,
                IsLastPage = policies.IsLastPage,
                SortBy = policies.SortBy,
                SortDirection = policies.SortDirection
            };
        }

        public async Task<PolicyResponseDTO> PurchasePolicy(CustomerPolicyPurchaseRequestDTO request, ClaimsPrincipal user)
        {
            var userId = user.GetUserId();
            var customer = await _cusRepo.GetCustomerByUserId(userId);
            if(customer == null)
            {
                throw new Exception("Customer not exists");
            }
            if (customer.User.ActiveStatus != ActiveStatus.Active)
            {
                throw new Exception("Customer is not active");
            }

            if (!IsCustomerProfileComplete(customer))
            {
                throw new Exception("Customer profile is incomplete.");
            }

            var plan = await _planRepo.GetPlanById(request.PlanId);
            if (plan == null)
            {
                throw new Exception("plan not exists");
            }
            if (plan.ActiveStatus != ActiveStatus.Active)
            {
                throw new Exception("plan is not active");
            }

            var addPolicy = new Policy
            {
                CustomerId = customer.CustomerId,
                PlanId = plan.PlanId,
                PolicyNumber = await GeneratePolicyNumber(),
                StartDate = request.StartDate,
                EndDate = request.StartDate.AddYears(plan.Duration),
                PolicyStatus = PolicyStatus.PendingPayment,
                TotalPremiumPaid = 0,
                CreatedDate  = DateTime.Now,
                UpdatedDate  = DateTime.Now,

            };

            var createdPolicy = await _policyRepo.CreatePolicy(addPolicy);
            createdPolicy.Customer = customer;
            createdPolicy.PolicyPlan = plan;
            _logger.LogInformation("Customer '{CustomerId}' purchased policy '{PolicyNumber}'.",customer.CustomerId,createdPolicy.PolicyNumber);
            return _mapper.Map<PolicyResponseDTO>(createdPolicy);

        }
        public async Task<string> GeneratePolicyNumber()
        {
            string policyNumber;
            do
            {
                policyNumber = Random.Shared.NextInt64(1000000000, 9999999999).ToString();
            }
            while (await _policyRepo.GetPolicyByPolicyNumber(policyNumber) != null);
            return policyNumber; 

        }

        private bool IsCustomerProfileComplete(Customer customer)
        {
            return customer.DateOfBirth != default &&
                   !string.IsNullOrWhiteSpace(customer.Address) &&
                   !string.IsNullOrWhiteSpace(customer.City) &&
                   !string.IsNullOrWhiteSpace(customer.State) &&
                   !string.IsNullOrWhiteSpace(customer.Pincode) &&
                   !string.IsNullOrWhiteSpace(customer.NomineeName) &&
                   !string.IsNullOrWhiteSpace(customer.NomineeRelationship);
        }
    }
}
