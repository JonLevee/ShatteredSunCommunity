using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ShatteredSunCommunity;
using ShatteredSunCommunity.Models;
using ShatteredSunCommunity.UnitSelect;

namespace ShatteredSunCommunity.UnitSelect
{
    [DebuggerDisplay("[{DisplayName}] {Text} Values:{ToString()}")]
    public class UnitSelectItem
    {
        public const string NOTSELECTED = "-- select --";

        public UnitSelectList Parent { get; set; }

        public string DisplayName { get; }
        public string FieldName { get; }
        public List<object> Values { get; }

        public object Value { get; private set; }

        public string Text
        {
            get => Value?.ToString();
            set
            {
                Value = value;
                Parent.OnItemValueChanged(this);
            }
        }
        public bool IsSelected => Value != NOTSELECTED;

        public UnitSelectItem(string displayName, string fieldName, IEnumerable<object> values)
        {
            DisplayName = displayName;
            FieldName = fieldName;
            Values = values.ToList();
            Values.Insert(0, NOTSELECTED);
            Value = NOTSELECTED;
        }

        public UnitSelectItem Copy(UnitSelectList unitSelectList)
        {
            var copy = (UnitSelectItem)MemberwiseClone();
            copy.Parent = unitSelectList;
            return copy;
        }

        public List<string> GetHeaders()
        {
            var headers = new List<string>();
            if (IsSelected)
            {
                headers.Add(string.Empty);
                return headers;
            }
            headers.AddRange(Values.Skip(1).Select(x => x.ToString()));
            return headers;
        }
        public override string ToString()
        {
            return string.Join(",", Values.Select(x => x.ToString()));
        }
    }
}
