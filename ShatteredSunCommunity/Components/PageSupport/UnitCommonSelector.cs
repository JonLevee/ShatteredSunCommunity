using NLog.Filters;
using ShatteredSunCommunity.Models;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    [DebuggerDisplay("[{Id}] {Selected}")]
    public class UnitCommonSelector
    {
        private readonly UnitCommonFilters parent;
        private List<string> values;
        private string selected;
        private List<OptionSelector> options;
        private bool isCheckBoxChecked;
        public UnitFilterItem FilterItem { get; }

        public bool IsCheckBoxChecked
        {
            get => isCheckBoxChecked;
            set
            {
                isCheckBoxChecked = value;
                parent.Refresh();
            }
        }

        public bool IsCheckBoxDisabled { get; set; }

        /// <summary>
        /// only used for avoiding refreshes
        /// </summary>
        /// <param name="value"></param>
        public void SetSelected(string value, bool isCheckBoxChecked)
        {
            selected = value;
            this.IsCheckBoxChecked = isCheckBoxChecked;
            Filter = parent.GetFilter(this);
        }

        public string Selected
        {
            get => selected;
            set
            {
                selected = value;
                Filter = parent.GetFilter(this);
                var selectedOption = Options.Find(o => o.Value == selected);
                Options.ForEach(o => o.IsSelected = false);
                selectedOption.IsSelected = true;
                FilterItem?.SetUnitFieldType();
                parent.Refresh();
            }
        }


        public bool IsActive => selected != OptionSelector.DefaultValue;
        public UnitCommonFilter Filter { get; private set; }
        public List<OptionSelector> Options
        {
            get => options ??= values.Select(v => new OptionSelector(v)).ToList();
        }

        public string Header => this == parent.Selectors.FirstOrDefault() ? parent.FirstItemHeader : parent.RemainingItemHeader;

        public UnitCommonSelector(UnitCommonFilters parent, List<string> values, bool useUnitFilterItems)
        {
            this.parent = parent;
            this.values = values;
            selected = OptionSelector.DefaultValue;
            if (useUnitFilterItems)
            {
                FilterItem = new UnitFilterItem(this);
            }
        }

        public void Refresh()
        {
            parent.Refresh();
        }
    }
}