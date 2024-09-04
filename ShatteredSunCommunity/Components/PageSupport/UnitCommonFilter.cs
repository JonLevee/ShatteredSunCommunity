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
        public UnitFieldTypeEnum UnitFieldType { get; }
        public IReadOnlyList<string> Values { get; }
        public bool SupportsIntersticial { get; }
        public bool UseFreeFormFilter { get; }

        public UnitCommonFilter(UnitField field, IEnumerable<string> values, bool supportsIntersticial, bool useFreeFormFilter)
        {
            Field = field.Name;
            Display = field.DisplayName;
            UnitFieldType = field.Value.UnitFieldType;
            Values = values.ToList();
            SupportsIntersticial = supportsIntersticial;
            UseFreeFormFilter = useFreeFormFilter;
        }

        public override string ToString()
        {
            return string.Join(", ", Values);
        }
    }
}
