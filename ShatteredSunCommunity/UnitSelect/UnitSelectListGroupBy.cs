using System.Collections;
using System.Collections.Frozen;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using ShatteredSunCommunity;
using ShatteredSunCommunity.Conversion;
using ShatteredSunCommunity.MiscClasses;
using ShatteredSunCommunity.Models;
using ShatteredSunCommunity.UnitSelect;
using ShatteredSunCommunity.UnitSelect.Definitions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShatteredSunCommunity.UnitSelect
{

    public class UnitSelectListGroupBy : INotifyPropertyChanging, IDisposable
    {
        private readonly List<UnitSelectListGroupByItem> _items = new List<UnitSelectListGroupByItem>();
        private readonly UnitGroupByDefinitions definitions;
        public event PropertyChangingEventHandler? PropertyChanging;

        public int Count => _items.Count;

        public UnitSelectListGroupBy(UnitGroupByDefinitions definitions)
        {
            this.definitions = definitions;
            foreach (var definition in definitions)
            {
                var item = new UnitSelectListGroupByItem(definitions);
                item.PropertyChanging += PropertyChanging;
                _items.Add(item);
            }
        }

        public UnitSelectListGroupByItem this[int index]
        {
            get { return index < _items.Count ? _items[index] : UnitSelectListGroupByItem.Empty; }
        }

        public void Dispose()
        {
            foreach (var item in _items)
            {
                item.PropertyChanging -= PropertyChanging;
            }
        }

    }
}
