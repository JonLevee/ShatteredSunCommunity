namespace ShatteredSunCommunity.Models
{
    public class SanctuarySunData
    {
        public Units Units { get; set; }
        public List<UnitGroupingField> GroupByFields { get; set; }
        public List<UnitGroupingField> SortFields { get; set; }
        public List<UnitFilterField> UnitFilters { get; set; }

        public SanctuarySunData()
        {
            Units = new Units();
            GroupByFields = new List<UnitGroupingField>();
            SortFields = new List<UnitGroupingField>();
            UnitFilters = new List<UnitFilterField>();
        }
    }
}
