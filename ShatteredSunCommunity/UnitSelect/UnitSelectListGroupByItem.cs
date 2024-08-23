using System.Collections.Frozen;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
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
    public class UnitSelectListGroupByItemChangedEventArgs : PropertyChangedEventArgs
    {
        public UnitSelectListGroupByItemChangedEventArgs(UnitSelectListGroupByItem instance, string propertyName, string previousValue) : base(propertyName)
        {
            Instance = instance;
            PreviousValue = previousValue;
        }

        public UnitSelectListGroupByItem Instance { get; }
        public string PreviousValue { get; }
    }
    public class UnitSelectListGroupByItem : INotifyPropertyChanged
    {
        public static readonly string Empty = "-- select group by --";
        private readonly UnitSelectListGroupBy parent;
        private string _selected;

        public event PropertyChangedEventHandler? PropertyChanged;

        public List<string> Options { get; }
        public string Header { get; set; }

        public UnitSelectListGroupByItem(UnitSelectListGroupBy parent, string selected)
        {
            this.parent = parent;
            _selected = selected;
            Options = new List<string>();
        }

        public void SetSelected(string selected)
        {
            _selected = selected;
        }

        public string Selected
        {
            get => _selected;
            set
            {
                var previousValue = _selected;
                _selected = value;
                PropertyChanged?.Invoke(this, new UnitSelectListGroupByItemChangedEventArgs(this, nameof(Selected), previousValue));
            }
        }
    }
}
