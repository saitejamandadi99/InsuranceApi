using InsuranceApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace InsuranceApi.Data
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext()
        {
            
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options ):base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<InsuranceProduct> InsuranceProducts { get; set; }
        public DbSet<PolicyPlan> PolicyPlans { get; set; }

        public DbSet<Policy> Policies { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<PremiumPayment> PremiumPayments { get; set; }
        public DbSet<ClaimDocument> ClaimDocuments { get; set; }
        public DbSet<ClaimStatusHistory> ClaimStatusHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Decimal Precision

            modelBuilder.Entity<PolicyPlan>().Property(p => p.CoverageAmount).HasPrecision(18, 2);

            modelBuilder.Entity<PolicyPlan>().Property(p => p.PremiumAmount).HasPrecision(18, 2);

            modelBuilder.Entity<Policy>().Property(p => p.TotalPremiumPaid).HasPrecision(18, 2);

            modelBuilder.Entity<PremiumPayment>().Property(p => p.Amount).HasPrecision(18, 2);

            modelBuilder.Entity<Claim>().Property(c => c.ClaimAmount).HasPrecision(18, 2);

            // Unique Constraints

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<InsuranceProduct>().HasIndex(p => p.ProductName).IsUnique();

            modelBuilder.Entity<Policy>().HasIndex(p => p.PolicyNumber).IsUnique();

            modelBuilder.Entity<Claim>().HasIndex(c => c.ClaimNumber).IsUnique();

            modelBuilder.Entity<PremiumPayment>().HasIndex(p => p.TransactionReference).IsUnique();

            // One-To-One User -> Customer

            modelBuilder.Entity<User>().HasOne(u => u.Customer).WithOne(c => c.User).HasForeignKey<Customer>(c => c.UserId);

            // InsuranceProduct -> PolicyPlan


            modelBuilder.Entity<InsuranceProduct>().HasMany(i => i.PolicyPlans).WithOne(p => p.InsuranceProduct).HasForeignKey(p => p.ProductId).OnDelete(DeleteBehavior.Restrict);

            // Customer -> Policy

            modelBuilder.Entity<Customer>().HasMany(c => c.Policies).WithOne(p => p.Customer).HasForeignKey(p => p.CustomerId).OnDelete(DeleteBehavior.Restrict);

            // PolicyPlan -> Policy

            modelBuilder.Entity<PolicyPlan>().HasMany(p => p.Policies).WithOne(p => p.PolicyPlan).HasForeignKey(p => p.PlanId).OnDelete(DeleteBehavior.Restrict);

            // Policy -> PremiumPayment

            modelBuilder.Entity<Policy>().HasMany(p => p.PremiumPayments).WithOne(pp => pp.Policy).HasForeignKey(pp => pp.PolicyId).OnDelete(DeleteBehavior.Cascade);

            // Policy -> Claim

            modelBuilder.Entity<Policy>().HasMany(p => p.Claims).WithOne(c => c.Policy).HasForeignKey(c => c.PolicyId).OnDelete(DeleteBehavior.Cascade);

            // Claim -> ClaimDocument

            modelBuilder.Entity<Claim>().HasMany(c => c.ClaimDocuments).WithOne(cd => cd.Claim).HasForeignKey(cd => cd.ClaimId).OnDelete(DeleteBehavior.Cascade);

            // Claim -> ClaimStatusHistory

            modelBuilder.Entity<Claim>().HasMany(c => c.ClaimStatusHistories).WithOne(ch => ch.Claim).HasForeignKey(ch => ch.ClaimId).OnDelete(DeleteBehavior.Cascade);

            // User -> ClaimStatusHistory

            modelBuilder.Entity<User>().HasMany(u => u.ClaimStatusHistories).WithOne(ch => ch.User).HasForeignKey(ch => ch.UpdatedBy).OnDelete(DeleteBehavior.Restrict);
        }

    }
}
