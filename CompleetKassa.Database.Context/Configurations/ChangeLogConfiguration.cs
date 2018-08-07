using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompleetKassa.Database.Context.Configurations
{
    public class ChangeLogConfiguration : EntityMappingConfiguration<ChangeLog>
	{
        public override void Map(EntityTypeBuilder<ChangeLog> builder)
        {
            builder.Property(db => db.ID).ValueGeneratedOnAdd();
            builder.HasKey(db => db.ID);
            builder.HasIndex(db => db.ID);
        }
    }
}
