using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompleetKassa.Database.Context.Configurations
{
	public class JRoleResourceConfiguration : EntityMappingConfiguration<JRoleResource>
	{
		public override void Map (EntityTypeBuilder<JRoleResource> builder)
		{
			builder.HasKey (ur => new { ur.RoleID, ur.ResourceID });

			builder.HasOne (ur => ur.Role)
				.WithMany (rr => rr.RoleResources)
				.HasForeignKey (ur => ur.RoleID);
		}
	}
}
