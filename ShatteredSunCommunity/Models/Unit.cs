namespace ShatteredSunCommunity.Models
{
    public class Unit : Dictionary<string,UnitItem>
    {
        public UnitItem TpId => this["general/tpId"];
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