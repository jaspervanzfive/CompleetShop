using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompleetKassa.Database.Context.Configurations
{
	public class CategoryConfiguration : EntityMappingConfiguration<Category>
	{
		public override void Map(EntityTypeBuilder<Category> builder)
		{
			builder.HasKey(db => db.ID);
			builder.HasIndex(db => new { db.ID, db.Name }).IsUnique();

			builder
				.HasOne(db => db.ParentCategory)
				.WithMany()
				.HasForeignKey(db => db.ParentCategoryID)
				.OnDelete(DeleteBehavior.SetNull);
		}
	}
}
