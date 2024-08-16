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
        [JsonIgnore]
        private Action<JsonConversionLocalData, UnitData, UnitField, JsonNode?>? onCreate;

        [JsonIgnore]
        public bool RequireValidPath { get; set; }

        public string Path { get; set; }
        public string Name { get; set; }
        public string[] Groups { get; set; }
        public bool IsImage { get; set; }
        public bool IsHeader { get; set; }

        public string Text { get; set; }

        public UnitFieldTypeEnum UnitFieldType { get; set; }

        [JsonIgnore]
        public long AsLong => long.Parse(Text);

        [JsonIgnore]
        public bool AsBool => bool.Parse(Text);

        [JsonIgnore]
        public double AsDouble => double.Parse(Text);

        [JsonIgnore]
        public string[] AsStringArray => Text.Split(ARRAY_SEPARATOR);

        public UnitField()
        {
            RequireValidPath = true;
        }

        public UnitField(
            string path,
            UnitFieldTypeEnum unitFieldType,
            bool isImage = false,
            bool isHeader = false,
            bool requireValidPath = true,
            Action<JsonConversionLocalData, UnitData, UnitField, JsonNode>? onCreate = null)
        {
            Path = path;
            UnitFieldType = unitFieldType;
            IsImage = isImage;
            IsHeader = isHeader;
            RequireValidPath = requireValidPath;
            this.onCreate = onCreate;
        }

        public void OnCreate(JsonConversionLocalData ld, UnitData ud, JsonNode? node)
        {
            onCreate?.Invoke(ld, ud, this, node);
        }

        public override string ToString() => Text;
    }

}
