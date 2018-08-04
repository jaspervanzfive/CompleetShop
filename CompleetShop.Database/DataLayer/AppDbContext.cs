using CompleetShop.Database.Core.EF.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection;

namespace CompleetShop.Database.DataLayer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Get configurations from current assembly, where this DbContext exists
            var currentAssembly = GetType().Assembly;
            modelBuilder.AddEntityConfigurationsFromAssembly(currentAssembly);

            //Get configurations from referenced assembly, Database.Core.EF-Data Configurations
            foreach (AssemblyName referencedAssembly in currentAssembly.GetReferencedAssemblies())
            {
                modelBuilder.AddEntityConfigurationsFromAssembly(Assembly.Load(referencedAssembly));
            }

            base.OnModelCreating(modelBuilder);
        }

        //TODO: Remove pluralizing
        private static void RemoveConventions(ModelBuilder modelBuilder)
        {
            // equivalent of modelBuilder.Conventions.AddFromAssembly(Assembly.GetExecutingAssembly());
            // look at this answer: https://stackoverflow.com/a/43075152/3419825

            // for the other conventions, we do a metadata model loop
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // equivalent of modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
                //TODO: Remove<PluralizingTableNameConvention> not working
                //entityType.Relational().TableName = entityType.DisplayName();

                // equivalent of modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
                // and modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
                entityType.GetForeignKeys()
                    .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
                    .ToList()
                    .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.Restrict);
            }

        }
    }
}

/*
public interface IEntityMappingConfiguration
    {
        void Map(ModelBuilder b);
    }

    public interface IEntityMappingConfiguration<T> : IEntityMappingConfiguration where T : class
    {
        void Map(EntityTypeBuilder<T> builder);
    }

    public abstract class EntityMappingConfiguration<T> : IEntityMappingConfiguration<T> where T : class
    {
        public abstract void Map(EntityTypeBuilder<T> b);

        public void Map(ModelBuilder b)
        {
            Map(b.Entity<T>());
        }
    }

    public static class ModelBuilderExtenions
    {
        private static IEnumerable<Type> GetMappingTypes(this Assembly assembly, Type mappingInterface)
        {
            return assembly.GetTypes().Where(x => !x.IsAbstract && x.GetInterfaces().Any(y => y.GetTypeInfo().IsGenericType && y.GetGenericTypeDefinition() == mappingInterface));
        }

        public static void AddEntityConfigurationsFromAssembly(this ModelBuilder modelBuilder, Assembly assembly)
        {
            var mappingTypes = assembly.GetMappingTypes(typeof (IEntityMappingConfiguration<>));
            foreach (var config in mappingTypes.Select(Activator.CreateInstance).Cast<IEntityMappingConfiguration>())
            {
                config.Map(modelBuilder);
            }
        }
    }

    Use:

    public class PersonConfiguration : EntityMappingConfiguration<Person>
    {
        public override void Map(EntityTypeBuilder<Person> b)
        {
            b.ToTable("Person", "HumanResources")
                .HasKey(p => p.PersonID);

            b.Property(p => p.FirstName).HasMaxLength(50).IsRequired();
            b.Property(p => p.MiddleName).HasMaxLength(50);
            b.Property(p => p.LastName).HasMaxLength(50).IsRequired();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddEntityConfigurationsFromAssembly(GetType().Assembly);
    }

    */
