using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ShatteredSunCommunity;
using ShatteredSunCommunity.Models;
using ShatteredSunCommunity.UnitSelect;

namespace ShatteredSunCommunity.UnitSelect
{
    [DebuggerDisplay("[{Text}] [{Value.Text}]")]
    public class UnitSelectList
    {
        private Dictionary<string, UnitSelectItem> items;
        public event EventHandler<UnitSelectListChangedEventArgs> SelectedChanged;
        public UnitSelectItem Value { get; private set; }

        public string Text
        {
            get => Value.DisplayName; 
            set
            {
                Value = items[value];
                OnItemValueChanged(Value);
            }
        }

        public bool ValueDisabled => !IsSelected;
        public bool IsSelected => Text != UnitSelectItem.NOTSELECTED;
        public bool IsValueSelected => Value.IsSelected;

        public UnitSelectItem[] SortedItems => items.Values.OrderBy(x => x.DisplayName).ToArray();
        public UnitSelectList()
        {
             items = new Dictionary<string, UnitSelectItem>();
            Add(new UnitSelectItem(UnitSelectItem.NOTSELECTED, UnitSelectItem.NOTSELECTED, new object[] { }));
            Text = UnitSelectItem.NOTSELECTED;
        }

        public void OnItemValueChanged(UnitSelectItem item)
        {
            var args = new UnitSelectListChangedEventArgs(this, item);
            SelectedChanged?.Invoke(this, args);
        }

        public void Add(UnitSelectItem item)
        {
            var copy = item.Copy(this);
            items.Add(copy.DisplayName, copy);
        }

        public List<string> GetHeaders()
        {
            if (IsSelected && IsValueSelected)
            {
                return Value.GetHeaders();
            }
            return new List<string> { string.Empty };
        }
    }
}
