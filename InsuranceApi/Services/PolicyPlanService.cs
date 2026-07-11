using AutoMapper;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Repositiries;
using InsuranceApi.Repositories;

namespace InsuranceApi.Services
{
    public class PolicyPlanService :IPolicyPlanService
    {
        private readonly IPolicyPlanRepository _planRepo;
        private readonly IInsuranceProductRepository _insuRepo;
        private readonly IMapper _mapper;

        public PolicyPlanService(IPolicyPlanRepository planRepo, IMapper mapper, IInsuranceProductRepository insuRepo)
        {
            _planRepo = planRepo;
            _mapper = mapper;
            _insuRepo = insuRepo;
        }

        public async Task<PlanResponseDTO?> ActivatePlan(int planId)
        {
            var existingplan = await _planRepo.GetPlanById(planId);
            if(existingplan == null)
            {
                throw new Exception("Plan does not exists");
            }
            var plan = await _planRepo.ActivatePlan(planId);
            return _mapper.Map<PlanResponseDTO>(plan);
        }

        public async Task<PlanResponseDTO> CreatePlan(PlanRequestDTO request)
        {
            var product = await _insuRepo.GetProductById(request.ProductId);
            if(product == null)
            {
                throw new Exception("Product does not exists");
            }

            Console.WriteLine(product.ActiveStatus);
            Console.WriteLine((int)product.ActiveStatus);

            if (product.ActiveStatus != ActiveStatus.Active)
            {
                throw new Exception("Product is not active");

            }

            var planName = request.PlanName.Trim().ToLower();
            var existingPlan = await _planRepo.GetPlanByName(request.ProductId, planName);
            if(existingPlan != null)
            {
                throw new Exception("Plan already exists in the product");
            }

            if(request.PremiumAmount <= 0)
            {
                throw new Exception("Premium amount should be greater than 0");
            }

            if (request.CoverageAmount <= 0)
            {
                throw new Exception("Coverage amount should be greater than 0");
            }

            if (request.CoverageAmount < request.PremiumAmount)
            {
                throw new Exception("Coverage amount should be greater than Premium amount");
            }

            if (request.Duration <= 0)
            {
                throw new Exception("Duration should be greater than 0");
            }

            var plan = _mapper.Map<PolicyPlan>(request);
            plan.PlanName = planName;
            plan.ActiveStatus = ActiveStatus.Active;
            var addedPlan = await _planRepo.CreatePlan(plan);
            return _mapper.Map<PlanResponseDTO>(addedPlan);

        }

        public async Task<PlanResponseDTO?> DeactivatePlan(int planId)
        {
            var existingplan = await _planRepo.GetPlanById(planId);
            if (existingplan == null)
            {
                throw new Exception("Plan does not exists");
            }
            var plan = await _planRepo.DeactivatePlan(planId);
            return _mapper.Map<PlanResponseDTO>(plan);
        }

        public async Task<PlanResponseDTO?> GetPlanById(int planId)
        {
            var existingplan = await _planRepo.GetPlanById(planId);
            if (existingplan == null)
            {
                throw new Exception("Plan does not exists");
            }
            return _mapper.Map<PlanResponseDTO>(existingplan);

        }

        public async Task<IEnumerable<PlanResponseDTO>> GetPlansByProductId(int productId)
        {
            var plans = await _planRepo.GetPlansByProductId(productId);
            return _mapper.Map<IEnumerable<PlanResponseDTO>>(plans);
        }

        public async Task<PaginatedResponseDTO<PlanResponseDTO>> ListPlans(PaginationQueryDto query)
        {
            var plans = await _planRepo.ListPlans(query);

            return new PaginatedResponseDTO<PlanResponseDTO>
            {
                Records = _mapper.Map<IEnumerable<PlanResponseDTO>>(plans.Records),
                CurrentPage = plans.CurrentPage,
                PageSize = plans.PageSize,
                TotalRecords = plans.TotalRecords,
                TotalPages = plans.TotalPages,
                IsLastPage = plans.IsLastPage,
                SortBy = plans.SortBy,
                SortDirection = plans.SortDirection
            };
        }

        public async Task<PlanResponseDTO?> UpdatePlan(int planId, PlanRequestDTO request)
        {
            var product = await _insuRepo.GetProductById(request.ProductId);
            if (product == null)
            {
                throw new Exception("Product does not exists");
            }

            if (product.ActiveStatus != ActiveStatus.Active)
            {
                throw new Exception("Product is not active");

            }

            var existingplan = await _planRepo.GetPlanById(planId);
            if(existingplan == null)
            {
                throw new Exception("Plan does not exists for this product");
            }

            var planName = request.PlanName.Trim().ToLower();
            var duplicatePlan = await _planRepo.GetPlanByName(request.ProductId, planName);
            if (duplicatePlan != null && duplicatePlan.PlanId!= planId)
            {
                throw new Exception("Plan Name already exists in the product");
            }

            if (request.PremiumAmount <= 0)
            {
                throw new Exception("Premium amount should be greater than 0");
            }

            if (request.CoverageAmount <= 0)
            {
                throw new Exception("Coverage amount should be greater than 0");
            }

            if (request.CoverageAmount < request.PremiumAmount)
            {
                throw new Exception("Coverage amount should be greater than Premium amount");
            }

            if(request.Duration <= 0)
            {
                throw new Exception("Duration should be greater than 0");
            }

            var plan = _mapper.Map(request, existingplan);
            plan.PlanName = planName;
            var updatedPlan = await _planRepo.UpdatePlan(planId, plan);
            return _mapper.Map<PlanResponseDTO>(updatedPlan);
        }
    }
}
