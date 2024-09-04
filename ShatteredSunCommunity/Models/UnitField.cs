using ShatteredSunCommunity.Conversion;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ShatteredSunCommunity.Models
{
    [DebuggerDisplay("[{Path}] {Text}")]
    public class UnitField
    {
        public const string ARRAY_SEPARATOR = ", ";

        public string Path { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// for grouping and filtering
        /// </summary>
        public string DisplayName { get; set; }
        public string[] PathParts { get; set; }
        [JsonIgnore]
        public string[] GroupParts { get; set; }
        [JsonIgnore]
        public int ColSpan { get; set; }
        public bool IsImage { get; set; }
        public bool IsHeader { get; set; }
        public bool IsThumbnail { get; set; }

        public UnitFieldValue Value { get; set; } = new UnitFieldValue();

        [JsonIgnore]
        public string Text => Value.Text;

        public override string ToString()
        {
            return Text;
        }
    }
}