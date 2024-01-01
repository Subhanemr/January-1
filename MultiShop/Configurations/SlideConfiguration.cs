using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiShop.Models;

namespace MultiShop.Configurations
{
    public class SlideConfiguration : IEntityTypeConfiguration<Slide>
    {
        public void Configure(EntityTypeBuilder<Slide> builder)
        {
            builder.Property(c => c.Title).IsRequired().HasMaxLength(50);
            builder.Property(c => c.SubTitle).IsRequired().HasMaxLength(150);
            builder.Property(c => c.ButtonText).IsRequired().HasMaxLength(50);
        }
    }
}
