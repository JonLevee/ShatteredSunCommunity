namespace ShatteredSunCommunity.Models
{
    public class UnitFilterFieldOpType
    {
        public string Op { get; set; }
        public Func<UnitField, string, bool> IsMatch { get; set; }
    }

    public static class UnitFilterFieldOpTypes
    {
        public static UnitFilterFieldOpType EqualTo { get; } = new UnitFilterFieldOpType
        {
            Op = "==",
            IsMatch
        };
    }
    public class UnitFilterField
    {
        public string DisplayName { get; set; }
        public string FieldName { get; set; }
        public List<UnitFilterFieldOpType> SupportedOpTypes { get; set; }
    }
}
