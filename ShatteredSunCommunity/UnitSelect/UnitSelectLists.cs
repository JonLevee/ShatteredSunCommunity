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
    public class UnitSelectLists : IDisposable
    {
        public UnitSelectListGroupBy GroupBy { get; }
        public UnitSelectListFilter Filters { get; }

        public int Count => GroupBy.Count;
        public UnitSelectLists(
            UnitSelectListGroupBy unitGroupBy,
            UnitSelectListFilter filters)
        {
            GroupBy = unitGroupBy;
            Filters = filters;
        }
        public void Dispose()
        {
        }
    }
}
