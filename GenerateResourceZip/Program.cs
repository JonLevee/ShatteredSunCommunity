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
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Text.Json.Serialization.Metadata;
using System.Xml.Linq;

namespace GenerateResourceZip
{
    internal class Program
    {
        public string LuaRoot { get; private set; }
        private static readonly string[] pathsToRemove =
        {
            "collisionInfo",
            "construction/rollOffPoints",
            "construction/buildableOnResources",
            "defence/health/value",
            "footprint",
            "general/icon",
            "general/iconUIBuildSortPriority",
            "isFactory",
            "movement/airHover",
            "movement/animClips",
            "movement/mass",
            "movement/sortOrder",
            "skirtSize",
            "transport/storage",
            "turrets",
            "visuals",
        };

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

        private void TrimJsonNode(JsonNode root)
        {
            foreach (var pathToRemove in pathsToRemove)
            {
                var node = root;
                var pathParts = pathToRemove.Split('/');
                foreach (var pathPart in pathParts)
                {
                    if (node != null)
                        node = node[pathPart];
                }
                if (node?.Parent is JsonObject jObject)
                {

                    jObject.Remove(node.GetPropertyName());
                }
            }
        }
        public void GenerateData()
        {
            var data = new SanctuarySunData();
            using (GetLuaTable("common/systems/factions.lua", "FactionsData", out LuaTable table))
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
                foreach (string key in table.Keys)
                {
                    data.AvailableUnits.Add(key, (bool)table[key]);
                }
            }

            foreach (var file in Directory.GetFiles(Path.Combine(LuaRoot, "common/units/unitsTemplates"), "*.santp", SearchOption.AllDirectories))
            {
                var relativePath = file.Substring(LuaRoot.Length + 1);
                using (GetLuaTable(relativePath, "UnitTemplate", out LuaTable table))
                {
                    var unit = new UnitData();
                    // converting the table to json format makes it easier to process
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true,
                        Converters =
                        {
                            new LuaTableConverter(),
                        },
                        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
                    };

                    var json = JsonSerializer.Serialize(table, jsonOptions);
                    File.WriteAllText("unit.json", json);
                    var jsonNode = JsonSerializer.Deserialize<JsonNode>(json, jsonOptions);
                    TrimJsonNode(jsonNode);
                    json = JsonSerializer.Serialize(jsonNode, jsonOptions);
                    File.WriteAllText("unit1.json", json);

                    Add(unit, jsonNode, "adjacency");
                    Add(unit, jsonNode, "construction/buildPower");
                    Add(unit, jsonNode, "construction/canBuild");
                    Add(unit, jsonNode, "construction/range");
                    Add(unit, jsonNode, "construction/upgradesTo");
                    Add(unit, jsonNode, "defence/health/max");
                    Add(unit, jsonNode, "defence/health/regen");
                    Add(unit, jsonNode, "defence/shields/max");
                    Add(unit, jsonNode, "defence/shields/name");
                    Add(unit, jsonNode, "defence/shields/regenDelay");
                    Add(unit, jsonNode, "defence/shields/rechargeTime");
                    Add(unit, jsonNode, "defence/shields/regen");
                    Add(unit, jsonNode, "economy/buildTime");
                    Add(unit, jsonNode, "economy/cost/alloys");
                    Add(unit, jsonNode, "economy/cost/energy");
                    Add(unit, jsonNode, "economy/maintenanceConsumption/energy");
                    Add(unit, jsonNode, "economy/production/alloys");
                    Add(unit, jsonNode, "economy/production/energy");
                    Add(unit, jsonNode, "economy/storage/alloys");
                    Add(unit, jsonNode, "economy/storage/energy");
                    Add(unit, jsonNode, "general/class", getValue: GetClass);
                    Add(unit, jsonNode, "general/displayName");
                    Add(unit, jsonNode, "general/iconUI", field => field.IsImage = true);
                    Add(unit, jsonNode, "general/name");
                    Add(unit, jsonNode, "general/orders", getValue: GetOrders);
                    Add(unit, jsonNode, "general/tpId");
                    Add(unit, jsonNode, "intel/radarRadius");
                    Add(unit, jsonNode, "intel/visionRadius");
                    Add(unit, jsonNode, "movement/acceleration");
                    Add(unit, jsonNode, "movement/air");
                    Add(unit, jsonNode, "movement/minSpeed");
                    Add(unit, jsonNode, "movement/rotationSpeed");
                    Add(unit, jsonNode, "movement/speed");
                    Add(unit, jsonNode, "movement/type");
                    Add(unit, jsonNode, "tags", getValue: GetArray);


                    data.Units.Add(unit);
                    /*
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

                    var faction = new UnitItem
                    {
                        Group = "misc",
                        Name = "faction",
                        Value = data.Factions[unit.TpId.Substring(0, 2)].Name
                    };
                    unit.Add(faction.Name, faction);

                    var enabled = new UnitItem
                    {
                        Group = "misc",
                        Name = "enabled",
                        Value = data.AvailableUnits.ContainsKey(unit.TpId) ? data.AvailableUnits[unit.TpId] : false
                    };
                    unit.Add(enabled.Name, enabled);

                    data.Units.Add(unit.TpId, unit);
                    */
                }
            }

            var outputPath = "../../../../ShatteredSunCommunity/ShatteredSunUnitData.json";
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            File.WriteAllText(outputPath, JsonSerializer.Serialize(data, options));

        }

        private void Add(
            UnitData data,
            JsonNode node,
            List<string> pathParts,
            int iPathPart,
            Action<UnitField>? callback = null,
            Func<JsonNode, object>? getValue = null)
        {
            for (; node != null && iPathPart < pathParts.Count; iPathPart++)
            {
                var pathPart = pathParts[iPathPart];
                if (node is JsonObject jObject)
                {
                    node = jObject[pathPart];
                }
                else if (node is JsonArray array)
                {
                    var save = pathParts[iPathPart - 1];
                    for (var i = 0; i < array.Count; ++i)
                    {
                        pathParts[iPathPart - 1] = $"{save}[{i}]";
                        Add(data, array[i], pathParts, iPathPart, callback, getValue);
                    }
                    return;
                }

            }
            if (node == null)
            {
                return;
            }
            var groups = pathParts.Select(p => p.Substring(0, 1).ToUpper() + p.Substring(1)).ToArray();
            var field = new UnitField
            {
                Path = string.Join("/", pathParts),
                Name = string.Concat(groups),
                Groups = groups,
                IsHeader = false,
                IsImage = false,
                Value = (getValue ?? DefaultGetValue).Invoke(node)
            };
            callback?.Invoke(field);
            data.Add(field.Name, field);

        }
        private void Add(
            UnitData data,
            JsonNode node,
            string path,
            Action<UnitField>? callback = null,
            Func<JsonNode, object>? getValue = null)
        {
            var pathParts = path.Split('/').ToList();
            Add(data, node, pathParts, 0, callback, getValue);
        }

        private object DefaultGetValue(JsonNode node)
        {
            return node.AsValue();
        }

        private object GetOrders(JsonNode node)
        {
            return node
                .AsObject()
                .Where(kv => (bool)kv.Value)
                .Select(kv => kv.Key)
                .ToArray();
        }
        private object GetArray(JsonNode node)
        {
            return node
                .AsArray()
                .Cast<JsonValue>()
                .Select(v => v.GetValue<string>())
                .ToArray();
        }

        private object GetClass(JsonNode node)
        {
            return node
                .AsArray()
                .Cast<JsonValue>()
                .Select(v => v.GetValue<string>())
                .Where(s => !s.StartsWith("unitsFinalized"))
                .Single();
        }

        //private bool TryGetUnitItem<T>(Unit unit, LuaTable table, string path, string displayPath = null, bool isImage = false) 
        //    where T : UnitItem
        //{
        //    var pathParts = (displayPath ?? path).Split('/');
        //    var item = new UnitItem
        //    {
        //        Path = path,
        //        Group = pathParts[0],
        //        SubGroup = pathParts.Length > 2 ? pathParts[1] : string.Empty,
        //        Name = pathParts.Length > 1 ? pathParts[pathParts.Length - 1] : string.Empty,
        //    };

        //    object unitData = table;
        //    foreach (var key in path.Split('/'))
        //    {
        //        if (unitData == null)
        //        {
        //            return false;
        //        }
        //        unitData = ((LuaTable)unitData)[key];
        //    }
        //    if (unitData == null)
        //    {
        //        return false;
        //    }
        //    item.Value = unitData;
        //    unit.Add(displayPath ?? path, item);
        //    return true;
        //}

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

    public class LuaTableConverter : JsonConverter<LuaTable>
    {
        public override LuaTable? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, LuaTable value, JsonSerializerOptions options)
        {
            var text = string.Empty;
            var isList = Enumerable
                .Range(1, value.Keys.Count)
                .Zip(value.Keys.Cast<object>(), (i, k) => k is long && Equals((long)k, (long)i))
                .All(r => r);
            if (isList)
            {
                var list = value.Values.Cast<object>().ToArray();
                text = JsonSerializer.Serialize(list, options);
            }
            else
            {
                var dictionary = value.Keys.Cast<object>().ToDictionary(k => k, k => value[k]);
                text = JsonSerializer.Serialize(dictionary, options);
            }
            writer.WriteRawValue(text);
        }
    }

}