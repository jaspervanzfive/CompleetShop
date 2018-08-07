using System.Threading.Tasks;

namespace CompleetKassa.Database.Core.Data
{
	public interface IRepository
	{
		int CommitChanges ();

		Task<int> CommitChangesAsync ();
	}
}
