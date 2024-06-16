using Core.Data.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.TypeConfigurations;

public class FoodEntryConfiguration : IEntityTypeConfiguration<FoodEntry>
{
    public void Configure(EntityTypeBuilder<FoodEntry> builder)
    {
        builder.Property(f => f.ProductId)
            .IsRequired();

        builder.HasOne(f => f.Product)
            .WithMany(p => p.FoodEntries);
    }
}