using System.Threading.Tasks;

namespace CompleetShop.Database.Core.Services
{
	public interface IRepository
	{
		int CommitChanges ();

		Task<int> CommitChangesAsync ();
	}
}
