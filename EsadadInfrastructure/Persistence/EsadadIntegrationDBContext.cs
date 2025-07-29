using Esadad.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Esadad.Infrastructure.Persistence
{
    public class EsadadIntegrationDbContext : DbContext
{
    public DbSet<EsadadTransactionLog> EsadadTransactionsLogs { get; set; }
    public DbSet<EsadadTransactionJsonLog> EsadadTransactionsJsonLogs { get; set; }
    public DbSet<EsadadPaymentLog> EsadadPaymentsLogs { get; set; }


    public EsadadIntegrationDbContext(DbContextOptions<EsadadIntegrationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // No need to add additional property configurations
        base.OnModelCreating(modelBuilder);
    }
}
}
