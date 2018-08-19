using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompleetKassa.Database.Context.Configurations
{
	public class UserCredentialConfiguration : EntityMappingConfiguration<UserCredential>
	{
		public override void Map(EntityTypeBuilder<UserCredential> builder)
		{
			builder.Property(t => t.ID).ValueGeneratedOnAdd();
			builder.HasKey(t => t.ID);
			builder.HasIndex(t => t.ID);

			// Set concurrency token for entity
			builder.Property(t => t.Timestamp)
				.ValueGeneratedOnAddOrUpdate()
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.IsRowVersion();
		}
	}
}
