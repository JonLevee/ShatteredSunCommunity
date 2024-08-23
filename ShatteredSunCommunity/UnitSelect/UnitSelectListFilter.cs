﻿using System.Collections.Frozen;
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
    public class UnitSelectListFilter
    {
        private readonly List<UnitSelectListFilterItem> _items = new List<UnitSelectListFilterItem>();
        private readonly UnitGroupByDefinitions definitions;
        public event EventHandler Changed;

        public int Count => _items.Count;

        public UnitSelectListFilterItem this[int index]
        {
            get { return index < _items.Count ? _items[index] : UnitSelectListFilterItem.Empty; }
        }
    }
}
