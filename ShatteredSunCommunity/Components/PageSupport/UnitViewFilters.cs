namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitViewFilters
    {
        public UnitGroupByFilters GroupBy { get; set; }
        public UnitViewFilters() 
        { 
            GroupBy = new UnitGroupByFilters();
        }

        public void OnChanged()
        {
            GroupBy.OnChanged();
        }
    }
}
