using CompleetShop.Database.Core.EF.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompleetShop.Database.Core.EF.Data.Configurations
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
