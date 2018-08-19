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
			builder.HasIndex(db => new { db.ID, db.Name }).IsUnique();

			builder.Property(db => db.CategoryName).HasColumnName("Category_Name");

			// Set concurrency token for entity
			builder.Property(db => db.Timestamp)
				.ValueGeneratedOnAddOrUpdate()
				.IsRowVersion();
		}
	}
}
