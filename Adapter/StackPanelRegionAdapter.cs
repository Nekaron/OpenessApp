using Prism.Regions;
using System.Windows;
using System.Windows.Controls;

namespace OpenessApp.Adapter
{
    public class StackPanelRegionAdapter
      : RegionAdapterBase<StackPanel>
    {
        public StackPanelRegionAdapter(
            IRegionBehaviorFactory factory
        ) : base(factory) { }

        protected override void Adapt(IRegion region, StackPanel target)
        {
            region.ActiveViews.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (var v in e.NewItems)
                    {
                        var uiElement = v as UIElement;
                        if (uiElement != null)
                            target.Children.Add(uiElement);
                    }
                }
            };
        }

        protected override IRegion CreateRegion()
        {
            return new AllActiveRegion();
        }
    }
}
