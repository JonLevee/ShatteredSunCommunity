using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ShatteredSunCommunity.Models
{
    [DebuggerDisplay("[{Name}] {DisplayName}")]
    public class UnitData : Dictionary<string, UnitField>
    {
        public UnitData() : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }

}
