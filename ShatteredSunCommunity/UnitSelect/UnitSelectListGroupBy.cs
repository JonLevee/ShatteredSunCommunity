using System.Collections;
using System.Collections.Frozen;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using ShatteredSunCommunity;
using ShatteredSunCommunity.Conversion;
using ShatteredSunCommunity.MiscClasses;
using ShatteredSunCommunity.Models;
using ShatteredSunCommunity.UnitSelect;
using ShatteredSunCommunity.UnitSelect.Definitions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShatteredSunCommunity.UnitSelect
{
    public class UnitSelectListGroupBy : IDisposable
    {
        private readonly List<string> _allSelectValues;
        private readonly List<UnitSelectListGroupByItem> _visibleSelectValues;
        private readonly UnitGroupByDefinitions definitions;
        private readonly SelectionState selectionState;

        public int Count => _allSelectValues.Count - 1;

        public IEnumerable<UnitSelectListGroupByItem> Items => _visibleSelectValues;
        private UnitSelectListGroupByItem NewEmptyItem() => new UnitSelectListGroupByItem(this, UnitSelectListGroupByItem.Empty);

        public UnitSelectListGroupBy(UnitGroupByDefinitions definitions, SelectionState selectionState)
        {
            this.definitions = definitions;
            this.selectionState = selectionState;
            _allSelectValues = definitions
                .Select(d => d.Name)
                .ToList();
            _allSelectValues.Insert(0, UnitSelectListGroupByItem.Empty);
            _visibleSelectValues = new List<UnitSelectListGroupByItem>();
            RecalculateOptions();
        }

        private void OnSelectedChanged(object sender, PropertyChangedEventArgs args)
        {
            var changedArgs = (UnitSelectListGroupByItemChangedEventArgs)args;
            var item = changedArgs.Instance;
            if (item.Selected == UnitSelectListGroupByItem.Empty)
            {
                item.PropertyChanged -= OnSelectedChanged;
                _visibleSelectValues.Remove(item);
            }
            RecalculateOptions();
            selectionState.OnSelectionStateChanged();
        }

        private void RecalculateOptions()
        {
            var header = "Group by";
            if (_visibleSelectValues.LastOrDefault()?.Selected != UnitSelectListGroupByItem.Empty)
            {
                var item = NewEmptyItem();
                item.PropertyChanged += OnSelectedChanged;
                _visibleSelectValues.Add(item);
            }
            var options = _allSelectValues.ToList();
            for (int i = 0; i < _visibleSelectValues.Count; i++)
            {
                var selected = _visibleSelectValues[i];
                if (options.Contains(selected.Selected))
                {
                    selected.Options.Clear();
                    selected.Options.AddRange(options);
                    options.Remove(selected.Selected);
                    selected.Header = header;
                    header = "Then by";
                }
                else
                {
                    selected.PropertyChanged -= OnSelectedChanged;
                    _visibleSelectValues.Remove(selected);
                    RecalculateOptions();
                    return;
                }
            }
        }

        public void Dispose()
        {
            _visibleSelectValues.ForEach(item => item.PropertyChanged -= OnSelectedChanged);
        }
    }
}
