using Core.Data.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.TypeConfigurations;

internal class IngredientScrapeJobConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.Property(p => p.Name).IsRequired();
        builder.HasIndex(p => p.Name).IsUnique();
    }
}
