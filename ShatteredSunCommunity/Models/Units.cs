namespace ShatteredSunCommunity.Models
{
    public class Units : List<UnitData>
    {

    }

    public class UnitData : Dictionary<string, UnitField>
    {

    }

    public class UnitField
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string[] Groups { get; set; }
        public bool IsImage { get; set; }
        public bool IsHeader { get; set; }
        public object Value { get; set; }
    }
}