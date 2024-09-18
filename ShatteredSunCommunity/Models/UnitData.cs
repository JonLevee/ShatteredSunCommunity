using ShatteredSunCommunity.Conversion;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
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

        [JsonIgnore]
        public string UnitIcon { get; set; }

        public IEnumerable<UnitDisplayHeader> GetDisplayFields()
        {
            var lastHeaders = Enumerable.Repeat(string.Empty, JsonHelper.ExpectedMaxGroups).ToArray();
            foreach (var field in Values.Where(f => !f.IsHeader))
            {
                var currentHeaders = field.GroupParts.ToArray();
                for (
                    int i = 0;
                    i < currentHeaders.Length && i < lastHeaders.Length &&
                        currentHeaders[i] == lastHeaders[i];
                        ++i)
                {
                    currentHeaders[i] = string.Empty;
                }
                var header = new UnitDisplayHeader(field.Text, field.ColSpan, currentHeaders);
                yield return header;
                lastHeaders = field.GroupParts;
            }
        }
    }

    [DebuggerDisplay("{GroupText}")]
    public class UnitDisplayHeader
    {
        public bool IsBreak { get; }
        public int ColSpan { get; }

        public string Text { get; }
        public string[] Groups { get; }
        public UnitDisplayHeader(string text, int colSpan, string[] groups)
        {
            IsBreak = !string.IsNullOrEmpty(groups[0]);
            Text = text;
            ColSpan = colSpan;
            Groups = groups;
        }

        public string GroupText => string.Join(",", Groups);
    }
}
