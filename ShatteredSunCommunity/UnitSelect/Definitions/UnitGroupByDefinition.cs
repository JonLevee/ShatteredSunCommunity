using System.Collections.Frozen;
using System.Diagnostics;
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

namespace ShatteredSunCommunity.UnitSelect.Definitions
{
    public class UnitGroupByDefinition
    {
        public string Name { get; }
        public string Key { get; }
        public IReadOnlyList<string> Items { get; }
        private IReadOnlySet<string> _lookup;

        public UnitGroupByDefinition(string name, string key, IEnumerable<string> items)
        {
            Name = name;
            Key = key;
            Items = items.ToList();
            _lookup = Items.ToHashSet();
        }

        public UnitGroupByDefinition(string name, IEnumerable<string> items) : this(name, name, items)
        {
        }

        public IEnumerable<UnitData> Filter(IEnumerable<UnitData> units)
        {
            foreach (var unit in units.Where(u => u.ContainsKey(Key)))
            {
                var field = unit[Key];
                switch (field.UnitFieldType)
                {
                    case UnitFieldTypeEnum.Double:
                    case UnitFieldTypeEnum.Image:
                    case UnitFieldTypeEnum.Bool:
                    case UnitFieldTypeEnum.String:
                        if (_lookup.Contains(field.Text))
                            yield return unit;
                        break;
                    case UnitFieldTypeEnum.StringArray:
                        foreach (var value in field.AsStringArray)
                        {
                            if (_lookup.Contains(value))
                            {
                                yield return unit;
                                break;
                            }
                        }
                        break;
                }
            }
        }
    }
}
