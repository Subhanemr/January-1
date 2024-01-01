using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiShop.Models;

namespace MultiShop.Configurations
{
    public class SizeConfiguration : IEntityTypeConfiguration<Size>
    {
        public void Configure(EntityTypeBuilder<Size> builder)
        {
            builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(c => c.Name).IsUnique();
        }
    }
}
