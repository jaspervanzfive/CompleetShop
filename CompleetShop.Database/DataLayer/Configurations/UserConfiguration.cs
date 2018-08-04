using CompleetShop.Database.Core.EF.Data.Configurations;
using CompleetShop.Database.EntityLayer;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompleetShop.Database.DataLayer.Configurations
{
    public class UserConfiguration : EntityMappingConfiguration<User>
	{
        public override void Map(EntityTypeBuilder<User> builder)
        {
            builder.Property(db => db.ID).ValueGeneratedOnAdd();
            builder.HasKey(db => db.ID);
            builder.HasIndex(db => db.ID);

            // Set concurrency token for entity
            builder.Property(db => db.Timestamp)
                .ValueGeneratedOnAddOrUpdate()
                .IsRowVersion();
        }
    }
}
