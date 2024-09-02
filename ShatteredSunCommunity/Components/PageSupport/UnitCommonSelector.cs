using NLog.Filters;
using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    [DebuggerDisplay("[{Id}] {Selected}")]
    public class UnitCommonSelector
    {
        private readonly ISelectorOwner parent;
        private List<string> values;
        private string selected;
        private List<OptionSelector> options;
        public bool IsCheckBoxChecked { get; set; } = false;
        public string CheckBoxDescription { get; }

        public string Selected
        {
            get => selected;
            set
            {
                selected = value;
                options.ForEach(option =>
                {
                    option.IsSelected = option.Value == selected;
                    option.IsDisabled = parent.Selectors.Any(s => s != this && s.IsActive && s.Selected == option.Value);
                });
                // update disabled for all other selectors
                foreach (var selector in parent.Selectors)
                {
                    foreach (var option in selector.Options)
                    {
                        option.IsDisabled = parent.Selectors.Any(s => s != selector && s.IsActive && s.Selected == option.Value);
                    }
                }
            }
        }
        public bool IsActive => selected != string.Empty;
        public List<OptionSelector> Options
        {
            get => options ??= values.Select(v => new OptionSelector(v)).ToList();
        }

        public string Header => this == parent.Selectors.FirstOrDefault() ? parent.FirstItemHeader : parent.RemainingItemHeader;

        public UnitCommonSelector(ISelectorOwner parent, List<string> values, string checkBoxDescription = null)
        {
            this.parent = parent;
            this.values = values;
            selected = string.Empty;
            if (!string.IsNullOrEmpty(checkBoxDescription))
            {
                CheckBoxDescription = checkBoxDescription;
            }
        }

    }
}
