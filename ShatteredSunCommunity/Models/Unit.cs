namespace ShatteredSunCommunity.Models
{
    public class Unit : Dictionary<string,UnitItem>
    {
        public string TpId => (string)this["general/tpId"].Value;
        public Unit() : base(StringComparer.OrdinalIgnoreCase) 
        { 
        }
    }

    public class UnitItem
    {
        public string Path { get; set; }
        public string Group { get; set; }
        public string SubGroup { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public bool IsImage { get; set; }
        public bool IsHeader { get; set; }

    }
}