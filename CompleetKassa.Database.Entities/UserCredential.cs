using System;
using CompleetKassa.Database.Core.Entities;

namespace CompleetKassa.Database.Entities
{
    public class UserCredential : AuditableBaseEntity, IAuditableEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public virtual int? UserID { get; set; }
        public virtual User User { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
