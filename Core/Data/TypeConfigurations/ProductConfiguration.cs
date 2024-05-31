using Core.Data.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Data.TypeConfigurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(x => x.Nutriscore)
            .HasConversion(
                n => n.ToString(),
                n => (Nutriscore)Enum.Parse(typeof(Nutriscore), n!));

        builder.HasMany(x => x.Ingredients)
            .WithMany(x => x.Products)
            .UsingEntity("ProductIngredients");

        builder.HasOne(x => x.NutritionInfo)
            .WithOne(x => x.Product);
    }
}
