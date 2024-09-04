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

        public UnitFieldTypeEnum UnitFieldType { get; set; }

        public UnitFieldValue Value { get; set; } = new UnitFieldValue();

        [JsonIgnore]
        public string Text 
        { 
            get
            {
                switch(UnitFieldType)
                {
                    case UnitFieldTypeEnum.String:
                        Debug.Assert(Value.Text != null);
                        return Value.Text;
                    case UnitFieldTypeEnum.Image:
                        Debug.Assert(Value.Image != null);
                        return Value.Image;
                    case UnitFieldTypeEnum.Double:
                        Debug.Assert(Value.Double != null);
                        return Value.Double.ToString();
                    case UnitFieldTypeEnum.StringArray:
                        Debug.Assert(Value.StringArray != null);
                        return string.Join(UnitField.ARRAY_SEPARATOR, Value.StringArray);
                    case UnitFieldTypeEnum.Bool:
                        Debug.Assert(Value.Bool != null);
                        return Value.Bool.ToString();
                    default:
                        throw new NotImplementedException($"Can't convert {UnitFieldType}");
                }
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class UnitFieldValue
    {
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public long? Long { get; set; }
        public bool? Bool { get; set; }
        public double? Double { get; set; }
        public string[]? StringArray { get; set; }
        public string? Text { get; set; }
        public string? Image { get; set; }
    }
}
