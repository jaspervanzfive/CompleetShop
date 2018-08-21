using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompleetKassa.Database.Context.Configurations
{
	public class ProductConfiguration : EntityMappingConfiguration<Product>
	{
		public override void Map(EntityTypeBuilder<Product> builder)
		{
			builder.HasKey(db => db.ID);
			builder.HasIndex(db => new { db.ID, db.Code }).IsUnique();

			// Exclude property
			builder.Ignore(db => db.CategoryName);
			builder.Ignore(db => db.SubCategoryName);

			// Foreign Key
			builder
				.HasOne(db => db.Category)
				.WithMany(db => db.Products)
				.HasForeignKey(db => db.CategoryID)
				.OnDelete(DeleteBehavior.SetNull);

			// Set concurrency token for entity
			builder.Property(t => t.Timestamp)
				.ValueGeneratedOnAddOrUpdate()
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.IsRowVersion();
		}
	}
}
