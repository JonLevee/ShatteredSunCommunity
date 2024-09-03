using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{

    [DebuggerDisplay("[{Field}] Selected:{Selected} Values:{ToString()}")]
    public class UnitCommonFilter
    {
        public string Field { get; }
        public string Display { get; }
        public IReadOnlyList<string> Values { get; }
        public bool SupportsIntersticial { get; }

        public UnitCommonFilter(string field, string display, IEnumerable<string> values, bool supportsIntersticial)
        {
            Field = field;
            Display = display;
            Values = values.ToList();
            SupportsIntersticial = supportsIntersticial;
        }

        public override string ToString()
        {
            return string.Join(", ", Values);
        }
    }
}
