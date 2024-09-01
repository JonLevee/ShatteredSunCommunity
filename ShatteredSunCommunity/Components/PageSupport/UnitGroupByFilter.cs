using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{

    [DebuggerDisplay("[{Field}] Selected:{Selected} Values:{ToString()}")]
    public class UnitGroupByFilter
    {
        public string Field { get; }
        public string Display { get; }
        public List<string> Values { get; }

        public UnitGroupByFilter(string field, string display, IEnumerable<string> values)
        {
            Field = field;
            Display = display;
            Values = values.ToList();
        }
        public UnitGroupByFilter(string field, IEnumerable<string> values) : this(field, field, values)
        {

        }

        public override string ToString()
        {
            return string.Join(", ", Values);
        }
    }
}
