using NLog.Filters;
using ShatteredSunCommunity.Models;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitFilterItem
    {
        private string filterKey;
        private UnitCommonSelector parent;
        private FilterOptionHandler filterOptionHandler;
        public bool FilterKeySelected => !string.IsNullOrEmpty(FilterKey);

        public WritableUnitFieldValue FilterValueLo { get; set; }
        public WritableUnitFieldValue FilterValueHigh { get; set; }
        public FilterOptionSelectorItem FilterOptionSelectorItem { get; private set; }
        public string FilterKey
        {
            get => filterKey;
            set
            {
                filterKey = value;
                FilterOptionSelectorItem = filterOptionHandler.TryGetValue(filterKey, out var item) ? item : null;
                parent.Refresh();
            }
        }

        public UnitFilterItem(UnitCommonSelector parent)
        {
            this.parent = parent;
            FilterValueLo = new WritableUnitFieldValue();
            FilterValueHigh = new WritableUnitFieldValue();
            filterOptionHandler = DIContainer.Get<FilterOptionHandler>();
        }

        public bool CanFilter => FilterKeySelected &&
            FilterValueLo.HasValue &&
            (string.IsNullOrEmpty(FilterOptionSelectorItem.JoiningWord) || FilterValueHigh.HasValue);
        public bool Filter(UnitData unit)
        {
            // if this filter isn't complete in the UI, then don't filter
            if (!CanFilter)
            {
                return true;
            }

            var field = unit[parent.Selected];
            var success = FilterOptionSelectorItem.Filter(FilterValueLo, FilterValueHigh, field.Value);
            return success;
        }

        internal void Initialize()
        {
            FilterValueLo.UnitFieldType = FilterValueHigh.UnitFieldType = parent.IsActive ? parent.Filter.UnitFieldType : UnitFieldTypeEnum.String;
            filterKey = filterOptionHandler.First().Key;
            FilterOptionSelectorItem = filterOptionHandler.TryGetValue(filterKey, out var item) ? item : null;

        }
    }
    public class WritableUnitFieldValue : UnitFieldValue
    {
        public bool HasValue { get; private set; }
        public string Text
        {
            get => base.Text;
            set
            {
                HasValue = !string.IsNullOrEmpty(value);
                switch (UnitFieldType)
                {
                    case UnitFieldTypeEnum.String:
                        String = value;
                        break;
                    case UnitFieldTypeEnum.Image:
                        Image = value;
                        break;
                    case UnitFieldTypeEnum.Double:
                        Double = double.Parse(value);
                        break;
                    case UnitFieldTypeEnum.Long:
                        Long = long.Parse(value);
                        break;
                    case UnitFieldTypeEnum.StringArray:
                        StringArray = value.Split(UnitField.ARRAY_SEPARATOR);
                        break;
                    case UnitFieldTypeEnum.Bool:
                        Bool = bool.Parse(value);
                        break;
                    default:
                        throw new NotImplementedException($"Can't set {UnitFieldType}");
                }
            }
        }

        public WritableUnitFieldValue()
        {
            UnitFieldType = UnitFieldTypeEnum.String;
            String = string.Empty;
        }
    }
}