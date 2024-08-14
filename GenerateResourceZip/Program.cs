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
using static System.Runtime.InteropServices.JavaScript.JSType;
using ShatteredSunCommunity.Extensions;

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
        private static readonly UnitField[] pathsToAdd =
        {
            new UnitField("adjacency"),
            new UnitField("construction/buildPower"),
            new UnitField("construction/canBuild"),
            new UnitField("construction/range"),
            new UnitField("construction/upgradesTo"),
            new UnitField("defence/health/max"),
            new UnitField("defence/health/regen"),
            new UnitField("defence/shields/max"),
            new UnitField("defence/shields/name"),
            new UnitField("defence/shields/regenDelay"),
            new UnitField("defence/shields/rechargeTime"),
            new UnitField("defence/shields/regen"),
            new UnitField("economy/buildTime", isHeader: true),
            new UnitField("economy/cost/alloys", isHeader: true),
            new UnitField("economy/cost/energy", isHeader: true),
            new UnitField("economy/maintenanceConsumption/energy"),
            new UnitField("economy/production/alloys"),
            new UnitField("economy/production/energy"),
            new UnitField("economy/storage/alloys"),
            new UnitField("economy/storage/energy"),
            new UnitField("general/class", onCreate: ConvertValueArrayToText),
            new UnitField("general/displayName", isHeader: true),
            new UnitField("general/iconUI", isImage: true, isHeader: true),
            new UnitField("general/name", isHeader: true),
            new UnitField("general/orders", onCreate: (ssd,ud,uf,node)=> {
                uf.Text = MakeTextFromArray(((JsonObject)node)
                    .Where(kv=>((bool?)kv.Value).GetValueOrDefault())
                    .Select(kv=>kv.Key));
            }),
            new UnitField("general/tpId", isHeader: true),
            new UnitField("intel/radarRadius"),
            new UnitField("intel/visionRadius"),
            new UnitField("movement/acceleration"),
            new UnitField("movement/air"),
            new UnitField("movement/minSpeed"),
            new UnitField("movement/rotationSpeed"),
            new UnitField("movement/speed"),
            new UnitField("movement/type"),
            new UnitField("tags", onCreate: ConvertValueArrayToText),
            new UnitField("faction", isHeader: true, requireValidPath:false, onCreate: (ssd,ud,uf,node)=> {
                var tpId = ud["GeneralTpId"].Text;
                uf.Text = ssd.Factions[tpId.Substring(0, 2)].Name;
            }),
            new UnitField("enabled", isHeader: true, requireValidPath:false, onCreate: (ssd, ud, uf,node) =>
            {
                var tpId = ud["GeneralTpId"].Text;
                ssd.AvailableUnits.TryGetValue(tpId, out var value);
                uf.Text = value.ToString();
            }
            ),
        };

        private static void ConvertValueArrayToText(SanctuarySunData ssd, UnitData udt, UnitField uf, JsonNode node)
        {
            uf.Text = MakeTextFromArray(((JsonArray)node).Cast<JsonValue>().Select(v => v.ToString()));
        }

        private static string MakeTextFromArray(IEnumerable<string> values) => string.Join(UnitField.ARRAY_SEPARATOR, values);

        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            IgnoreReadOnlyFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull|JsonIgnoreCondition.WhenWritingDefault,
            Converters =
                        {
                            new LuaTableConverter(),
                        },
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };

        public Program()
        {
            LuaRoot = string.Empty;
        }
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
                    Debug.Assert(item != null);
                    var faction = new Faction
                    {
                        Name = item["name"].ToStringNullSafe(),
                        TpLetter = item["tpLetter"].ToStringNullSafe(),
                        Tag = item["tag"].ToStringNullSafe(),
                        InitialUnit = item["initialUnit"].ToStringNullSafe(),
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

                    var json = JsonSerializer.Serialize(table, jsonOptions);
                    var jsonNode = JsonSerializer.Deserialize<JsonNode>(json, jsonOptions);
                    Debug.Assert(jsonNode != null);
                    TrimJsonNode(jsonNode);
                    json = JsonSerializer.Serialize(jsonNode, jsonOptions);
                    foreach(var pathToAdd in pathsToAdd)
                    {
                        Add(data, unit, jsonNode, pathToAdd);
                    }
                    //File.WriteAllText("unit.json", json);

                    data.Units.Add(unit);
                }
            }

            var outputPath = "../../../../ShatteredSunCommunity/ShatteredSunUnitData.json";
            File.WriteAllText(outputPath, JsonSerializer.Serialize(data, jsonOptions));

        }

        private void Add(
            SanctuarySunData data,
            UnitData unit,
            JsonNode node,
            UnitField field,
            List<string> pathParts,
            int iPathPart)
        {
            for (; node != null && iPathPart < pathParts.Count; iPathPart++)
            {
                var pathPart = pathParts[iPathPart];
                if (node is JsonObject jObject)
                {
                    node = jObject[pathPart].ToNullSafe();
                }
                else if (node is JsonArray array)
                {
                    var save = pathParts[iPathPart - 1];
                    for (var i = 0; i < array.Count; ++i)
                    {
                        pathParts[iPathPart - 1] = $"{save}[{i}]";
                        Add(data, unit, array[i].ToNullSafe(), field, pathParts, iPathPart);
                    }
                    return;
                }

            }
            if (node == null && field.RequireValidPath)
            {
                return;
            }
            var groups = pathParts.Select(p => p.Substring(0, 1).ToUpper() + p.Substring(1)).ToArray();
            field.Path = string.Join("/", pathParts);
            field.Name = string.Concat(groups);
            field.Groups = groups;
            field.Text = node.ToStringNullSafe();
            field.OnCreate(data, unit, node);
            unit.Add(field.Name, field);
        }

        private void Add(
            SanctuarySunData data,
            UnitData unit,
            JsonNode node,
            UnitField field)
        {
            var pathParts = field.Path.Split('/').ToList();
            Add(data, unit, node, field, pathParts, 0);
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
