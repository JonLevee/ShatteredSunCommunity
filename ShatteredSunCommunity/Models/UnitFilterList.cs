using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ShatteredSunCommunity.Models
{
    public class UnitFilterList : List<UnitFilter>
    {

    }
    public class UnitFilter
    {
        public string FieldName { get; }
        public string DisplayName { get; }
        public IEnumerable<object> Values { get; }

        public UnitFilter(string fieldName, string displayName, IEnumerable<object> values)
        {
            FieldName = fieldName;
            DisplayName = displayName;
            Values = values;
        }

    }
}
