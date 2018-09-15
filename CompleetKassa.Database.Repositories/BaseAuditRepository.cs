using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using CompleetKassa.Database.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompleetKassa.Database.Repositories
{
    public abstract class BaseAuditRepository : BaseRepository
    {
 
		public BaseAuditRepository (IAppUser userInfo, DbContext dbContext) : base (userInfo, dbContext)
		{
		}

		protected virtual void Add<TEntity> (TEntity entity) where TEntity : class, IAuditableEntity
		{
			var cast = entity as IAuditableEntity;

			if (cast != null) {
				cast.CreationUser = UserInfo.Name;

				if (!cast.CreationDateTime.HasValue)
					cast.CreationDateTime = DateTime.Now;

				if(string.IsNullOrEmpty(cast.CreationIPv4) == true) {
					var host = Dns.GetHostEntry (Dns.GetHostName ());
					cast.CreationIPv4 = host.AddressList.FirstOrDefault (ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString ();
				}

				if (string.IsNullOrEmpty (cast.CreationHostName) == true) {
					cast.CreationHostName = System.Environment.MachineName;
				}

			}

			DbContext.Set<TEntity> ().Add (entity);
		}

		protected virtual void Update<TEntity> (TEntity entity) where TEntity : class, IAuditableEntity
		{
			var cast = entity as IAuditableEntity;

			if (cast != null) {
				cast.LastUpdateUser = UserInfo.Name;

				if (!cast.LastUpdateDateTime.HasValue)
					cast.LastUpdateDateTime = DateTime.Now;
			}
		}

		protected virtual void Remove<TEntity> (TEntity entity) where TEntity : class, IAuditableEntity
			=> DbContext.Set<TEntity> ().Remove (entity);
	}
}
