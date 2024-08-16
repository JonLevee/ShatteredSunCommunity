using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ShatteredSunCommunity.Models
{
    [DebuggerDisplay("[{Name}] {DisplayName}")]
    public class UnitData : Dictionary<string, UnitField>
    {
        [JsonIgnore]
        public string Name => this["GeneralName"].Text;

        [JsonIgnore]
        public string DisplayName => this["GeneralDisplayName"].Text;
    }

}
