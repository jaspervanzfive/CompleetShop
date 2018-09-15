using System;

namespace CompleetKassa.Database.Core.Entities
{
    public interface IAuditableEntity : IEntity
    {
        string CreationUser { get; set; }
        DateTime? CreationDateTime { get; set; }
        string CreationIPv4 { get; set; }
        string CreationHostName { get; set; }
        string LastUpdateUser { get; set; }
        DateTime? LastUpdateDateTime { get; set; }
    }
}