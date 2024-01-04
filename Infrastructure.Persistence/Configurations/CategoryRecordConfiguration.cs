using Infrastructure.Persistence.Records.CategoryRecord;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CategoryRecordConfiguration : IEntityTypeConfiguration<CategoryRecord>
    {
        public void Configure(EntityTypeBuilder<CategoryRecord> builder)
        {
            builder.ToTable("Category");

            builder.Property(e => e.Id)
                .IsRequired();

            builder.Property(e => e.Code)
                .HasMaxLength(20);
        }
    }
}