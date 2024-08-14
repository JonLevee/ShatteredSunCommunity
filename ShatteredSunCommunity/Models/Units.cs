using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ShatteredSunCommunity.Models
{
    public class Units : List<UnitData>
    {

    }

    public class UnitData : Dictionary<string, UnitField>
    {

    }

    [DebuggerDisplay("[{Path}] {Text}")]
    public class UnitField
    {
        public const string ARRAY_SEPARATOR = ", ";
        [JsonIgnore]
        private Action<SanctuarySunData, UnitData, UnitField, JsonNode?>? onCreate;

        [JsonIgnore]
        public bool RequireValidPath { get; set; }

        public string Path { get; set; }
        public string Name { get; set; }
        public string[] Groups { get; set; }
        public bool IsImage { get; set; }
        public bool IsHeader { get; set; }

        public string Text { get; set; }

        [JsonIgnore]
        public long AsLong => long.Parse(Text);

        [JsonIgnore]
        public double AsDouble => double.Parse(Text);
        
        [JsonIgnore]
        public string[] AsArray => Text.Split(ARRAY_SEPARATOR);

        public UnitField()
        {
            RequireValidPath = true;
        }

        public UnitField(
            string path, 
            bool isImage = false, 
            bool isHeader = false,
            bool requireValidPath = true,
            Action<SanctuarySunData,UnitData,UnitField, JsonNode>? onCreate = null)
        {
            Path = path;
            IsImage = isImage;
            IsHeader = isHeader;
            RequireValidPath = requireValidPath;
            this.onCreate = onCreate;
        }

        public void OnCreate(SanctuarySunData ssd, UnitData ud, JsonNode? node)
        {
            onCreate?.Invoke(ssd, ud, this, node);
        }

        public override string ToString() => Text;
    }
}
