using Microsoft.EntityFrameworkCore;
using TransactionApi.DAL.Entity;

namespace TransactionApi.DAL;

public class TransactionDbContext(DbContextOptions<TransactionDbContext> options) : DbContext(options)
{

    public DbSet<TransactionEntity> Transactions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TransactionDbContext).Assembly);
    }
}