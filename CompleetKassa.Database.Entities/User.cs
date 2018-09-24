using System;
using System.Collections.Generic;
using CompleetKassa.Database.Core.Entities;

namespace CompleetKassa.Database.Entities
{
    public class User : AuditableBaseEntity, IAuditableEntity
    {
        public User()
        {
        }

        public User(int userID)
        {
            ID = userID;
        }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime? Timestamp { get; set; }

        //Foreign Key
        public virtual UserCredential UserCredential { get; set; }

        public virtual ICollection<JUserRole> UserRoles { get; set; }
    }
}
