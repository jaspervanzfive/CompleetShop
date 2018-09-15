using CompleetKassa.Database.Core.Entities;

namespace CompleetKassa.Database.Entities
{
    public class Resource : AuditableBaseEntity, IAuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Disabled { get; set; }
    }
}
