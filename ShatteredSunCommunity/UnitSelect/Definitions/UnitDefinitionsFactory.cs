using System.Collections.Frozen;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using ShatteredSunCommunity;
using ShatteredSunCommunity.Conversion;
using ShatteredSunCommunity.Extensions;
using ShatteredSunCommunity.MiscClasses;
using ShatteredSunCommunity.Models;
using ShatteredSunCommunity.UnitSelect;
using ShatteredSunCommunity.UnitSelect.Definitions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShatteredSunCommunity.UnitSelect.Definitions
{
    public class UnitDefinitionsFactory
    {
        public static UnitGroupByDefinitions GetGroupByDefinitions(IServiceProvider provider)
        {
            var data = provider.GetService<SanctuarySunData>();
            var definitions = new UnitGroupByDefinitions
            {
                new UnitGroupByDefinition("Faction", data.GetDistinct("Faction", f=>f.Text)),
                new UnitGroupByDefinition("MovementType", data.GetDistinct("MovementType", f=>f.Text)),
                new UnitGroupByDefinition("Orders", "GeneralOrders", data.GetDistinctFromArray("GeneralOrders")),
                new UnitGroupByDefinition("Tags", data.GetDistinctFromArray("Tags")),
                new UnitGroupByDefinition("Tech", data.GetDistinctFromArray("Tags").Where(JsonHelper.TECHTIERS.Contains)),
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
