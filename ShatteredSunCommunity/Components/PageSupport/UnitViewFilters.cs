namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitViewFilters
    {
        public UnitGroupByFilters GroupBy { get; }
        public UnitSortFilters SortFilters { get; }
        public UnitViewFilters()
        {
            GroupBy = new UnitGroupByFilters(this);
            SortFilters = new UnitSortFilters();
        }

        public void OnChanged()
        {
            GroupBy.OnChanged();
        }
    }
}
