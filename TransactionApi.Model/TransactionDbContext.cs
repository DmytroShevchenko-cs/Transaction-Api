using Microsoft.EntityFrameworkCore;
using TransactionApi.Model.Entity;

namespace TransactionApi.Model;

public class TransactionDbContext(DbContextOptions<TransactionDbContext> options) : DbContext(options)
{

    public DbSet<TransactionEntity> Transactions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransactionDbContext).Assembly);
    }
}