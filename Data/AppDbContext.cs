using GLMS.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Client - Contract (1-many)
            modelBuilder.Entity<Client>()
                .HasMany(c => c.Contracts)
                .WithOne(c => c.Client)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Contract - ServiceRequest (1-many)
            modelBuilder.Entity<Contract>()
                .HasMany(c => c.ServiceRequests)
                .WithOne(sr => sr.Contract)
                .HasForeignKey(sr => sr.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // Decimal precision for Cost and ZARValue
            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.Cost)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.ZARValue)
                .HasColumnType("decimal(18,2)");
        }
    }
}
