using ShatteredSunCommunity.Conversion;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ShatteredSunCommunity.Models
{
    public class UnitFieldValue
    {
        public UnitFieldTypeEnum UnitFieldType { get; set; }

        public long? Long { get; set; }
        public bool? Bool { get; set; }
        public double? Double { get; set; }
        public string[]? StringArray { get; set; }
        public string? String { get; set; }
        public string? Image { get; set; }

        [JsonIgnore]
        public string Text
        {
            get
            {
                switch (UnitFieldType)
                {
                    case UnitFieldTypeEnum.String:
                        Debug.Assert(String != null);
                        return String;
                    case UnitFieldTypeEnum.Image:
                        Debug.Assert(Image != null);
                        return Image;
                    case UnitFieldTypeEnum.Double:
                        Debug.Assert(Double != null);
                        return Double.ToString();
                    case UnitFieldTypeEnum.Long:
                        Debug.Assert(Long != null);
                        return Long.ToString();
                    case UnitFieldTypeEnum.StringArray:
                        Debug.Assert(StringArray != null);
                        return string.Join(UnitField.ARRAY_SEPARATOR, StringArray);
                    case UnitFieldTypeEnum.Bool:
                        Debug.Assert(Bool != null);
                        return Bool.ToString();
                    default:
                        throw new NotImplementedException($"Can't convert {UnitFieldType}");
                }
            }
        }

    }
}
