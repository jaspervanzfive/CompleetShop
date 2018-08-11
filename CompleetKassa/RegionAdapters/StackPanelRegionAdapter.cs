using System.Windows;
using System.Windows.Controls;
using Prism.Regions;

namespace CompleetKassa.RegionAdapters
{
	public class StackPanelRegionAdapter : RegionAdapterBase<StackPanel>
	{
		public StackPanelRegionAdapter(IRegionBehaviorFactory behaviorFactory) :
			base(behaviorFactory)
		{
		}
		protected override void Adapt(IRegion region, StackPanel regionTarget)
		{
			region.Views.CollectionChanged += (s, e) =>
			{
				if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
					foreach (FrameworkElement element in e.NewItems)
						regionTarget.Children.Add(element);
			};
		}

		protected override IRegion CreateRegion()
		{
			return new AllActiveRegion();
		}
	}
}
