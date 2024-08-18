using System.Collections.Frozen;
using System.ComponentModel;
using System.Diagnostics;
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
    public class UnitSelectListGroupByItem : INotifyPropertyChanging
    {
        public static readonly UnitSelectListGroupByItem Empty = new UnitSelectListGroupByItem(null);
        private string _text;
        private UnitGroupByDefinitions definitions;

        public UnitSelectListGroupByItem(UnitGroupByDefinitions definitions)
        {
            this.definitions = definitions;
            Values.AddRange(definitions.Select(d=>d.Name));
        }

        public event PropertyChangingEventHandler? PropertyChanging;

        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }

        public List<string> Values { get; } = new List<string>();

        private void Set(
            ref string backingValue,
            string value,
            [CallerMemberName]
            string name = null)
        {
            backingValue = value;
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));
        }
    }
}
