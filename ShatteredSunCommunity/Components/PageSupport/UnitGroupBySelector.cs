using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    [DebuggerDisplay("[{Id}] {Selected}")]
    public class UnitGroupBySelector
    {
        private List<string> values;
        private string selected;
        private List<OptionSelector> options;
        public string Selected 
        {
            get => selected;
            set
            {
                selected = value;
                options.ForEach(option => option.IsSelected = option.Value == selected);
            }
        }
        public bool IsActive => selected != string.Empty;
        public List<OptionSelector> Options
        {
            get => options ??= values.Select(v => new OptionSelector(v)).ToList();
        }
        public int Id { get; }

        public string Header => Id == 0 ? "Group by:" : "then by";

        public UnitGroupBySelector(int id, List<string> values)
        {
            Id = id;
            this.values = values;
            selected = string.Empty;
        }

    }
}
