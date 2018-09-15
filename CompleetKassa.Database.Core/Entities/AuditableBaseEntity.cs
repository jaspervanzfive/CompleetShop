using System;

namespace CompleetKassa.Database.Core.Entities
{
    public abstract class AuditableBaseEntity : IAuditableEntity
    {
        public int ID { get; set; }
        public string CreationUser { get; set; }
        public DateTime? CreationDateTime { get; set; }
        public string CreationIPv4 { get; set; }
        public string CreationHostName { get; set; }
        public string LastUpdateUser { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }
    }
}
