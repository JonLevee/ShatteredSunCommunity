using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text;
using NLua;
using ShatteredSunCommunity.MiscClasses;
using ShatteredSunCommunity.Models;
using System.Diagnostics;
using ShatteredSunCommunity.Extensions;
using ShatteredSunCommunity.Conversion;

namespace GenerateResourceZip
{
    internal class Program
    {
        private static bool VerifyUnitFieldType = true;
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
        private static UnitField[] GetPathsToAdd()
        {
            return
            [
                new UnitField("adjacency", UnitFieldTypeEnum.String),
                new UnitField("construction/buildPower", UnitFieldTypeEnum.Double),
                new UnitField("construction/canBuild", UnitFieldTypeEnum.String),
                new UnitField("construction/range", UnitFieldTypeEnum.Double),
                new UnitField("construction/upgradesTo", UnitFieldTypeEnum.String),
                new UnitField("defence/health/max", UnitFieldTypeEnum.Double),
                new UnitField("defence/health/regen", UnitFieldTypeEnum.Double),
                new UnitField("defence/shields/max", UnitFieldTypeEnum.Double),
                new UnitField("defence/shields/name", UnitFieldTypeEnum.String),
                new UnitField("defence/shields/regenDelay", UnitFieldTypeEnum.Double),
                new UnitField("defence/shields/rechargeTime", UnitFieldTypeEnum.Double),
                new UnitField("defence/shields/regen", UnitFieldTypeEnum.Double),
                new UnitField("economy/buildTime", UnitFieldTypeEnum.Double, isHeader: true),
                new UnitField("economy/cost/alloys", UnitFieldTypeEnum.Double, isHeader: true),
                new UnitField("economy/cost/energy", UnitFieldTypeEnum.Double, isHeader: true),
                new UnitField("economy/maintenanceConsumption/energy", UnitFieldTypeEnum.Double),
                new UnitField("economy/production/alloys", UnitFieldTypeEnum.Double),
                new UnitField("economy/production/energy", UnitFieldTypeEnum.Double),
                new UnitField("economy/storage/alloys", UnitFieldTypeEnum.Double),
                new UnitField("economy/storage/energy", UnitFieldTypeEnum.Double),
                new UnitField("general/class", UnitFieldTypeEnum.StringArray, onCreate: ConvertValueArrayToText),
                new UnitField("general/displayName", UnitFieldTypeEnum.String, isHeader: true),
                new UnitField("general/iconUI", UnitFieldTypeEnum.Image, isImage: true, isHeader: true),
                new UnitField("general/name", UnitFieldTypeEnum.String, isHeader: true),
                new UnitField("general/orders", UnitFieldTypeEnum.StringArray, displayName: "Orders", onCreate: (ld, ud, uf, node) => {
                    uf.Text = MakeTextFromArray(((JsonObject)node)
                        .Where(kv => ((bool?)kv.Value).GetValueOrDefault())
                        .Select(kv => kv.Key));
                }),
                new UnitField("general/tpId", UnitFieldTypeEnum.String, isHeader: true),
                new UnitField("intel/radarRadius", UnitFieldTypeEnum.Double),
                new UnitField("intel/visionRadius", UnitFieldTypeEnum.Double),
                new UnitField("movement/acceleration", UnitFieldTypeEnum.Double),
                new UnitField("movement/air", UnitFieldTypeEnum.Bool),
                new UnitField("movement/minSpeed", UnitFieldTypeEnum.Double),
                new UnitField("movement/rotationSpeed", UnitFieldTypeEnum.Double),
                new UnitField("movement/speed", UnitFieldTypeEnum.Double),
                new UnitField("movement/type", UnitFieldTypeEnum.String),
                new UnitField("tags", UnitFieldTypeEnum.StringArray, onCreate: ConvertValueArrayToText),
                new UnitField("faction", UnitFieldTypeEnum.String, isHeader: true, requireValidPath: false, onCreate: (ld, ud, uf, node) => {
                    var tpId = ud["GeneralTpId"].Text;
                    uf.Text = ld.Factions[tpId.Substring(0, 2)].Name;
                }),
                new UnitField("tier", UnitFieldTypeEnum.String, isHeader: true, requireValidPath: false, onCreate: (ld, ud, uf, node) => {
                    // we need access to tags above
                    uf.Text = ud["tags"].AsStringArray.Intersect(JsonHelper.TECHTIERS).Single();
                }),
                new UnitField("enabled", UnitFieldTypeEnum.Bool, isHeader: true, requireValidPath: false, onCreate: (ld, ud, uf, node) =>
                {
                    var tpId = ud["GeneralTpId"].Text;
                    ld.AvailableUnits.TryGetValue(tpId, out var value);
                    uf.Text = value.ToString();
                }
                ),
            ];
        }

        private static void ConvertValueArrayToText(JsonConversionLocalData ld, UnitData udt, UnitField uf, JsonNode node)
        {
            uf.Text = MakeTextFromArray(((JsonArray)node).Cast<JsonValue>().Select(v => v.ToString()));
        }

        private static string MakeTextFromArray(IEnumerable<string> values) => string.Join(UnitField.ARRAY_SEPARATOR, values);


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
            var localData = new JsonConversionLocalData();
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
                    localData.Factions.Add(faction.TpLetter, faction);
                }
            }

            using (GetLuaTable("common/units/availableUnits.lua", "AvailableUnits", out LuaTable table))
            {
                foreach (string key in table.Keys)
                {
                    localData.AvailableUnits.Add(key, (bool)table[key]);
                }
            }

            foreach (var file in Directory.GetFiles(Path.Combine(LuaRoot, "common/units/unitsTemplates"), "*.santp", SearchOption.AllDirectories))
            {
                var relativePath = file.Substring(LuaRoot.Length + 1);
                using (GetLuaTable(relativePath, "UnitTemplate", out LuaTable table))
                {
                    var unit = new UnitData();
                    // converting the table to json format makes it easier to process
                    var jsonNode = JsonHelper.ConvertLuaTableToJson(table);
                    TrimJsonNode(jsonNode);
                    File.WriteAllText("unit.json", JsonSerializer.Serialize(jsonNode, JsonHelper.JsonOptions));
                    var pathsToAdd = GetPathsToAdd();
                    foreach (var pathToAdd in pathsToAdd)
                    {
                        Add(localData, unit, jsonNode, pathToAdd);
                    }
                    data.Units.Add(unit);
                    //File.WriteAllText("unit1.json", JsonSerializer.Serialize(unit, JsonHelper.JsonOptions));
                    //File.WriteAllText("unitData.json", JsonSerializer.Serialize(data, JsonHelper.JsonOptions));
                }
            }

            var maxGroups = data.Units.SelectMany(u => u.Values.Select(v => v.PathParts.Length)).Max();
            if (maxGroups > JsonHelper.ExpectedMaxGroups)
                throw new InvalidOperationException($"max group count of {maxGroups} exceeds expected {JsonHelper.ExpectedMaxGroups}");
            var outputPath = "../../../../ShatteredSunCommunity/ShatteredSunUnitData.json";
            File.WriteAllText(outputPath, JsonSerializer.Serialize(data, JsonHelper.JsonOptions));

        }

        private void Add(
            JsonConversionLocalData ld,
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
                        Add(ld, unit, array[i].ToNullSafe(), field, pathParts, iPathPart);
                    }
                    return;
                }

            }
            if (node == null && field.RequireValidPath)
            {
                return;
            }
            var pascalCasedPathParts = pathParts.Select(p => p.Substring(0, 1).ToUpper() + p.Substring(1)).ToArray();
            field.Path = string.Join("/", pathParts);
            field.Name = string.Concat(pascalCasedPathParts);
            field.DisplayName = field.DisplayName ?? pascalCasedPathParts.Last();
            field.PathParts = pascalCasedPathParts;
            field.Text = node.ToStringNullSafe();
            field.OnCreate(ld, unit, node);
            if (VerifyUnitFieldType)
            {
                switch (field.UnitFieldType)
                {
                    case UnitFieldTypeEnum.String:
                        break;
                    case UnitFieldTypeEnum.Double:
                        var doubleValue = field.AsDouble;
                        break;
                    case UnitFieldTypeEnum.StringArray:
                        var stringArrayValue = field.AsStringArray;
                        Debug.Assert(stringArrayValue.Length > 1);
                        break;
                    case UnitFieldTypeEnum.Image:
                        break;
                    case UnitFieldTypeEnum.Bool:
                        var boolValue = field.AsBool;
                        break;
                }
            }

            unit.Add(field.Name, field);
        }

        private void Add(
            JsonConversionLocalData ld,
            UnitData unit,
            JsonNode node,
            UnitField field)
        {
            var pathParts = field.Path.Split('/').ToList();
            Add(ld, unit, node, field, pathParts, 0);
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
