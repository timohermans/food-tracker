using Core.Data.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.TypeConfigurations;

internal class ProductScrapeJobConfiguration : IEntityTypeConfiguration<ProductScrapeJob>
{
    public void Configure(EntityTypeBuilder<ProductScrapeJob> builder)
    {
        builder.Property(p => p.Url).IsRequired();
        builder.HasIndex(p => p.Url).IsUnique();

        builder.Property(p => p.Content).HasColumnType("nvarchar(max)");
    }
}
