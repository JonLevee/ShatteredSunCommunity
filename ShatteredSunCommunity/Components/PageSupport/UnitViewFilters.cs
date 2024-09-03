using System.Text.RegularExpressions;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitViewFilters : IRefresh
    {
        private bool showUnitThumbnails;
        public event EventHandler Changed;
        public UnitGroupByFilters GroupBy { get; }
        public UnitSortFilters SortFilters { get; }
        public bool ShowUnitThumbnails
        {
            get => showUnitThumbnails;
            set
            {
                showUnitThumbnails = value;
                Refresh();
            }
        }
        public UnitViewFilters()
        {
            GroupBy = new UnitGroupByFilters(this);
            SortFilters = new UnitSortFilters(this);
        }

        public void Refresh()
        {
            GroupBy.UpdateHeaderRows();
            GroupBy.UpdateSelectors();
            SortFilters.UpdateSelectors(GroupBy.Selectors);
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
