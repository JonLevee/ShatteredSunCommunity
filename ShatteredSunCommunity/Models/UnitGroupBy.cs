using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ShatteredSunCommunity.Models
{
    public class UnitGroupByList : List<UnitGroupBy>
    {
    }
    public class UnitGroupBy
    {
        public string FieldName { get; }
        public string DisplayName { get; }
        public List<object> Values { get; }

        public UnitGroupBy(string fieldName, string displayName, IEnumerable<object> values)
        {
            FieldName = fieldName;
            DisplayName = displayName;
            Values = values.ToList();
        }
    }
}
