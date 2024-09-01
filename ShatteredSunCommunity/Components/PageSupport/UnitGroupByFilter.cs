using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{

    [DebuggerDisplay("[{Field}] Selected:{Selected} Values:{ToString()}")]
    public class UnitGroupByFilter
    {
        public int Id { get; }
        public string Field { get; }
        public string Display { get; }
        public List<string> Values { get; }

        public UnitGroupByFilter(int id, string field, string display, IEnumerable<string> values)
        {
            Id = id;
            Field = field;
            Display = display;
            Values = values.ToList();
        }
        public UnitGroupByFilter(int id, string field, IEnumerable<string> values) : this(id, field, field, values)
        {

        }

        public override string ToString()
        {
            return string.Join(", ", Values);
        }
    }
}
