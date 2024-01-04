using Infrastructure.Persistence.Configurations;
using Infrastructure.Persistence.Records.CategoryRecord;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options)
    {
        
    }
    
    public virtual DbSet<CategoryRecord> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // base.OnModelCreating(modelBuilder); //this line of code need to be there because of Identity primary keys

        ApplyConfiguration(modelBuilder);
    }

    private void ApplyConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryRecordConfiguration());
    }

}