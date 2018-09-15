using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using CompleetKassa.Database.Core.Entities;
using CompleetKassa.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Repositories
{
    public abstract class BaseRepository
    {
        protected IAppUser UserInfo;
        protected DbContext DbContext;

        public BaseRepository(IAppUser userInfo, DbContext dbContext)
        {
            UserInfo = userInfo;
            DbContext = dbContext;
        }

        public virtual int CommitChanges()
        {
            var dbSet = DbContext.Set<ChangeLog>();

            foreach (var change in GetChanges().ToList())
            {
                dbSet.Add(change);
            }

            return DbContext.SaveChanges();
        }

        public virtual Task<int> CommitChangesAsync()
        {
            var dbSet = DbContext.Set<ChangeLog>();

            foreach (var change in GetChanges().ToList())
            {
                dbSet.Add(change);
            }

            return DbContext.SaveChangesAsync();
        }

        protected virtual IEnumerable<ChangeLog> GetChanges()
        {
            var exclusions = DbContext.Set<ChangeLogExclusion>().ToList();

            foreach (var entry in DbContext.ChangeTracker.Entries())
            {
                if (entry.State != EntityState.Modified)
                    continue;

                var entityType = entry.Entity.GetType();

                if (exclusions.Where(item => item.EntityName == entityType.Name && item.PropertyName == "*").Count() == 1)
                    yield break;

                foreach (var property in entityType.GetTypeInfo().DeclaredProperties)
                {

                    // Validate if Navigation Property
                    if (property.GetGetMethod().IsVirtual == true)
                    {
                        continue;
                    }

                    // Validate if there is an exclusion for *.Property
                    if (exclusions.Where(item => item.EntityName == "*" && string.Compare(item.PropertyName, property.Name, true) == 0).Count() == 1)
                        continue;

                    // Validate if there is an exclusion for Entity.Property
                    if (exclusions.Where(item => item.EntityName == entityType.Name && string.Compare(item.PropertyName, property.Name, true) == 0).Count() == 1)
                        continue;

                    var originalValue = entry.Property(property.Name).OriginalValue;
                    var currentValue = entry.Property(property.Name).CurrentValue;

                    if (string.Concat(originalValue) == string.Concat(currentValue))
                        continue;

                    // Retrieve primary key value from entity instance
                    var keyName = DbContext.Model.FindEntityType(entry.Entity.GetType()).FindPrimaryKey().Properties.Select(x => x.Name).Single();
                    var keyValue = entry.Entity.GetType().GetProperty(keyName).GetValue(entry.Entity, null);

                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    var ipV4 = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString();
                    yield return new ChangeLog
                    {
                        ClassName = entityType.Name,
                        PropertyName = property.Name,
                        Key = keyValue?.ToString(),
                        OriginalValue = originalValue == null ? string.Empty : originalValue.ToString(),
                        CurrentValue = currentValue == null ? string.Empty : currentValue.ToString(),
                        UserName = UserInfo.Name,
                        ChangeDate = DateTime.Now,
                        HostName = System.Environment.MachineName,
                        IPv4 = ipV4
                    };
                }
            }
        }
    }
}
