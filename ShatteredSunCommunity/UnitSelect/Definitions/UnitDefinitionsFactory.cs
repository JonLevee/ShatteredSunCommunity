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
    public class UnitDefinitionsFactory
    {
        private static IEnumerable<T> GetDistinct<T>(SanctuarySunData data, string key, Func<UnitField, T> getValue)
        {
            return data
                .Units
                .Where(u => u.ContainsKey(key))
                .Select(u => getValue(u[key]))
                .Distinct()
                .Order();
        }
        private static IEnumerable<string> GetDistinctFromArray(SanctuarySunData data, string key)
        {
            return data
                .Units
                .Where(u => u.ContainsKey(key))
                .SelectMany(u => u[key].AsStringArray)
                .Distinct()
                .Order();
        }

        public static UnitGroupByDefinitions GetGroupByDefinitions(IServiceProvider provider)
        {
            var data = provider.GetService<SanctuarySunData>();
            var definitions = new UnitGroupByDefinitions
            {
                new UnitGroupByDefinition("Faction", GetDistinct(data, "Faction", f=>f.Text)),
                new UnitGroupByDefinition("MovementType", GetDistinct(data, "MovementType", f=>f.Text)),
                new UnitGroupByDefinition("Orders", "GeneralOrders", GetDistinctFromArray(data, "GeneralOrders")),
                new UnitGroupByDefinition("Tags", GetDistinctFromArray(data, "Tags")),
                new UnitGroupByDefinition("Tech", GetDistinctFromArray(data, "Tags").Where(JsonHelper.TECHTIERS.Contains)),
            };
            return definitions;
        }

        public static UnitFilterDefinitions GetFilterDefinitions(IServiceProvider provider)
        {
            var definitions = new UnitFilterDefinitions();
            return definitions;
        }
    }
}
