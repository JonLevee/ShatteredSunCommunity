namespace ShatteredSunCommunity.Models
{
    public class SanctuarySunData
    {
        public UnitGroupByList GroupByList { get; set; }
        public UnitFilterList FilterList { get; set; }
        public Units Units { get; set; }

        public SanctuarySunData()
        {
            GroupByList = new UnitGroupByList();
            FilterList = new UnitFilterList();
            Units = new Units();
        }
    }
}
