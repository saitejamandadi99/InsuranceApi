using InsuranceApi.Data;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using InsuranceApi.Repositiries;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InsuranceApi.Repositories
{
    public class PremiumPaymentRepository:IPremiumPaymentRepository
    {
        private readonly DatabaseContext _context;
        public PremiumPaymentRepository(DatabaseContext context)
        {
            _context = context; 
        }

        public async Task<PremiumPayment?> GetPaymentById(int paymentId)
        {
            return await _context.PremiumPayments.Include(p => p.Policy).FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<IEnumerable<PremiumPayment>> GetPaymentsByCustomerId(int customerId)
        {
            return await _context.PremiumPayments.Include(p => p.Policy).ThenInclude(p=>p.Customer).Where(p=>p.Policy.CustomerId == customerId).ToListAsync();

        }

        public async Task<IEnumerable<PremiumPayment>> GetPaymentsByPolicyId(int policyId)
        {
            return await _context.PremiumPayments.Include(p => p.Policy).Where(p => p.PolicyId == policyId).ToListAsync();
        }

        public async Task<PaginatedResponseDTO<PremiumPayment>> ListPayments(PaginationQueryDto query)
        {
            var payments =  _context.PremiumPayments.Include(p => p.Policy).ThenInclude(p => p.Customer).ThenInclude(c=>c.User).Include(p=>p.Policy).ThenInclude(p=>p.PolicyPlan).AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim().ToLower();
                payments = payments.Where(p =>
                p.TransactionReference.ToLower().Contains(search) ||
                p.Policy.PolicyNumber.ToLower().Contains(search) ||
                p.Policy.Customer.User.FullName.ToLower().Contains(search));
                
            }

            //sorting 

            payments = (query.SortBy?.ToLower(), query.SortDirection?.ToLower()) switch
            {
                ("transactionreference", "desc")=>payments.OrderByDescending(p=>p.TransactionReference),
                ("transactionreference", _)=>payments.OrderBy(p=>p.TransactionReference),

                ("amount", "desc") => payments.OrderByDescending(p => p.Amount),
                ("amount", _) => payments.OrderBy(p => p.Amount),


                ("paymentmode", "desc") => payments.OrderByDescending(p => p.PaymentMode),
                ("paymentmode", _) => payments.OrderBy(p => p.PaymentMode),

                ("paymentdate", "desc") => payments.OrderByDescending(p => p.PaymentDate),
                ("paymentdate", _) => payments.OrderBy(p => p.PaymentDate),

                ("paymentstatus", "desc") => payments.OrderByDescending(p => p.PaymentStatus),
                ("paymentstatus", _) => payments.OrderBy(p => p.PaymentStatus),
                _ => payments.OrderBy(p=>p.PaymentId)
            };

            int totalRecords = await payments.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)query.PageSize);

            var pagedPayments = await payments.Skip((query.PageNumber - 1) * query.PageSize)
                                .Take(query.PageSize)
                                .ToListAsync();

            return new PaginatedResponseDTO<PremiumPayment>
            {
                Records = pagedPayments,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                IsLastPage = query.PageNumber >= totalPages,
                SortBy = query.SortBy,
                SortDirection = query.SortDirection
            };
        }

        public async Task<PremiumPayment> RecordPayment(PremiumPayment payment)
        {
            _context.PremiumPayments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;

        }

        public async Task<PremiumPayment?> GetPaymentByTransactionReference(string transactionReference)
        {
            return await _context.PremiumPayments.FirstOrDefaultAsync(p => p.TransactionReference == transactionReference);
        }

    }
}
