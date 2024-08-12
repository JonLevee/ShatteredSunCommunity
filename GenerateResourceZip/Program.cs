using System.Text.Json.Nodes;
using System.Text.Json;
using ShatteredSunCommunity;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using NLua;
using ShatteredSunCommunity.MiscClasses;
using ShatteredSunCommunity.Models;
using System.IO;
using System.Reflection;

namespace GenerateResourceZip
{
    internal class Program
    {
        public string LuaRoot { get; private set; }

        static void Main(string[] args)
        {
            var steamInfo = new SteamInfo();
            var root = Path.Combine(steamInfo.GetRoot(), @"engine\LJ\lua");
            var program = new Program
            {
                LuaRoot = root,
            };

            program.GenerateData();

        }

        public void GenerateData()
        {
            var data = new SanctuarySunData();
            using(GetLuaTable("common/systems/factions.lua", "FactionsData", out LuaTable table))
            {
                foreach (var item in table.Values.Cast<LuaTable>())
                {
                    var faction = new Faction
                    {
                        Name = item["name"].ToString(),
                        TpLetter = item["tpLetter"].ToString(),
                        Tag = item["tag"].ToString(),
                        InitialUnit = item["initialUnit"].ToString(),
                    };
                    data.Factions.Add(faction.TpLetter, faction);
                }
            }

            using (GetLuaTable("common/units/availableUnits.lua", "AvailableUnits", out LuaTable table))
            {
                foreach(string key in table.Keys)
                {
                    data.AvailableUnits.Add(key, (bool)table[key]);
                }
            }

            foreach(var file in Directory.GetFiles(Path.Combine(LuaRoot, "common/units/unitsTemplates"), "*.santp", SearchOption.AllDirectories))
            {
                var relativePath = file.Substring(LuaRoot.Length + 1);
                using (GetLuaTable(relativePath, "UnitTemplate", out LuaTable table))
                {
                    var unit = new Unit();

                    TryGetUnitItem(unit, table, "adjacency");
                    TryGetUnitItem(unit, table, "construction/buildPower");
                    TryGetUnitItem(unit, table, "construction/canBuild");
                    TryGetUnitItem(unit, table, "construction/range");
                    TryGetUnitItem(unit, table, "construction/upgradesTo");
                    TryGetUnitItem(unit, table, "defence/health/max");
                    TryGetUnitItem(unit, table, "defence/health/regen");
                    TryGetUnitItem(unit, table, "defence/shields");
                    TryGetUnitItem(unit, table, "economy/buildTime");
                    TryGetUnitItem(unit, table, "economy/cost/alloys");
                    TryGetUnitItem(unit, table, "economy/cost/energy");
                    TryGetUnitItem(unit, table, "economy/maintenanceConsumption/energy");
                    TryGetUnitItem(unit, table, "economy/production/alloys");
                    TryGetUnitItem(unit, table, "economy/production/energy");
                    TryGetUnitItem(unit, table, "economy/storage/alloys");
                    TryGetUnitItem(unit, table, "economy/storage/energy");
                    if (TryGetUnitItem(unit, table, "general/class"))
                    {
                        unit["general/class"].Value = ((LuaTable)unit["general/class"].Value)
                            .Values
                            .Cast<string>()
                            .Where(item => !item.StartsWith("unitsFinalized"))
                            .Single();
                    }
                    TryGetUnitItem(unit, table, "general/displayName");
                    TryGetUnitItem(unit, table, "general/iconUI", isImage: true);
                    TryGetUnitItem(unit, table, "general/name");
                    if (TryGetUnitItem(unit, table, "general/orders"))
                    {
                        var orders = (LuaTable)unit["general/orders"].Value;
                        var updated = orders
                            .Keys
                            .Cast<string>()
                            .Where(key => (bool)orders[key])
                            .Order()
                            .ToList();
                        unit["general/orders"].Value = updated;
                    }
                    TryGetUnitItem(unit, table, "general/tpId");
                    TryGetUnitItem(unit, table, "intel/radarRadius");
                    TryGetUnitItem(unit, table, "intel/visionRadius");
                    TryGetUnitItem(unit, table, "movement/acceleration");
                    TryGetUnitItem(unit, table, "movement/air");
                    TryGetUnitItem(unit, table, "movement/minSpeed");
                    TryGetUnitItem(unit, table, "movement/rotationSpeed");
                    TryGetUnitItem(unit, table, "movement/speed");
                    TryGetUnitItem(unit, table, "movement/type");
                    TryGetUnitItem(unit, table, "tags");


                    data.Units.Add((string)unit.TpId.Value, unit);
                }
            }


        }

        private bool TryGetUnitItem(Unit unit, LuaTable table, string path, string displayPath = null, bool isImage = false)
        {
            var pathParts = (displayPath ?? path).Split('/');
            var item = new UnitItem
            {
                Path = path,
                Group = pathParts[0],
                SubGroup = pathParts.Length > 2 ? pathParts[1] : string.Empty,
                Name = pathParts.Length > 1 ? pathParts[pathParts.Length - 1] : string.Empty,
            };

            object unitData = table;
            foreach (var key in path.Split('/'))
            {
                if (unitData == null)
                {
                    return false;
                }
                unitData = ((LuaTable)unitData)[key];
            }
            if (unitData == null)
            {
                return false;
            }
            item.Value = unitData;
            unit.Add(displayPath ?? path, item);
            return true;
        }

        private IDisposable GetLuaTable(string relativePath, string tableName, out LuaTable table)
        {
            var lua = new Lua();
            var fullPath = Path.Combine(LuaRoot, relativePath);
            lua.State.Encoding = Encoding.UTF8;
            lua.DoFile(fullPath);
            table = (LuaTable)lua[tableName];
            return new Disposable(lua.Dispose);
        }
    }

}