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
        public string LuaRoot { get; private set; }
        public string SteamRoot { get; private set; }
        public string WWWRoot { get; private set; }

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
        private static CreateUnitField[] GetPathsToAdd()
        {
            return
            [
                new CreateUnitField("adjacency", UnitFieldTypeEnum.String),
                new CreateUnitField("construction/buildPower", UnitFieldTypeEnum.Long),
                new CreateUnitField("construction/canBuild", UnitFieldTypeEnum.String),
                new CreateUnitField("construction/range", UnitFieldTypeEnum.Double),
                new CreateUnitField("construction/upgradesTo", UnitFieldTypeEnum.String),
                new CreateUnitField("defence/health/max", UnitFieldTypeEnum.Long),
                new CreateUnitField("defence/health/regen", UnitFieldTypeEnum.Long),
                new CreateUnitField("defence/shields/max", UnitFieldTypeEnum.Long),
                new CreateUnitField("defence/shields/name", UnitFieldTypeEnum.String),
                new CreateUnitField("defence/shields/regenDelay", UnitFieldTypeEnum.Long),
                new CreateUnitField("defence/shields/rechargeTime", UnitFieldTypeEnum.Long),
                new CreateUnitField("defence/shields/regen", UnitFieldTypeEnum.Long),
                new CreateUnitField("economy/buildTime", UnitFieldTypeEnum.Long, isHeader: true),
                new CreateUnitField("economy/cost/alloys", UnitFieldTypeEnum.Long, isHeader: true),
                new CreateUnitField("economy/cost/energy", UnitFieldTypeEnum.Long, isHeader: true),
                new CreateUnitField("economy/maintenanceConsumption/energy", UnitFieldTypeEnum.Long),
                new CreateUnitField("economy/production/alloys", UnitFieldTypeEnum.Long),
                new CreateUnitField("economy/production/energy", UnitFieldTypeEnum.Long),
                new CreateUnitField("economy/storage/alloys", UnitFieldTypeEnum.Long),
                new CreateUnitField("economy/storage/energy", UnitFieldTypeEnum.Long),
                new CreateUnitField("general/class", UnitFieldTypeEnum.StringArray, onCreate: ConvertValueArrayToStringArray),
                new CreateUnitField("general/displayName", UnitFieldTypeEnum.String, isHeader: true),
                new CreateUnitField("general/iconUI", UnitFieldTypeEnum.Image, isImage: true, isHeader: true),
                new CreateUnitField("general/name", UnitFieldTypeEnum.String, isHeader: true),
                new CreateUnitField("general/orders", UnitFieldTypeEnum.StringArray, displayName: "Orders", onCreate: (ld, ud, uf, node) => {
                    uf.Value.StringArray = (node as JsonObject)?
                        .Where(kv => ((bool?)kv.Value).GetValueOrDefault())
                        .Select(kv => kv.Key)
                        .ToArray() ?? [];
                }),
                new CreateUnitField("general/tpId", UnitFieldTypeEnum.String, isHeader: true),
                new CreateUnitField("intel/radarRadius", UnitFieldTypeEnum.Long),
                new CreateUnitField("intel/visionRadius", UnitFieldTypeEnum.Long),
                new CreateUnitField("movement/acceleration", UnitFieldTypeEnum.Double),
                new CreateUnitField("movement/air", UnitFieldTypeEnum.Bool),
                new CreateUnitField("movement/minSpeed", UnitFieldTypeEnum.Double),
                new CreateUnitField("movement/rotationSpeed", UnitFieldTypeEnum.Double),
                new CreateUnitField("movement/speed", UnitFieldTypeEnum.Double),
                new CreateUnitField("movement/type", UnitFieldTypeEnum.String),
                new CreateUnitField("tags", UnitFieldTypeEnum.StringArray, onCreate: ConvertValueArrayToStringArray),
                new CreateUnitField("faction", UnitFieldTypeEnum.String, isHeader: true, requireValidPath: false, onCreate: (ld, ud, uf, node) => {
                    var tpId = ud["GeneralTpId"].Text;
                    uf.Value.String = ld.Factions[tpId.Substring(0, 2)].Name;
                }),
                new CreateUnitField("tier", UnitFieldTypeEnum.String, isHeader: true, requireValidPath: false, onCreate: (ld, ud, uf, node) => {
                    // we need access to tags above
                    var tags = ud["tags"].Value.StringArray ?? [];
                    uf.Value.String = tags.Intersect(JsonHelper.TECHTIERS).Single();
                }),
                new CreateUnitField("enabled", UnitFieldTypeEnum.Bool, isHeader: true, requireValidPath: false, onCreate: (ld, ud, uf, node) =>
                {
                    var tpId = ud["GeneralTpId"].Text;
                    ld.AvailableUnits.TryGetValue(tpId, out var value);
                    uf.Value.Bool = value;
                }
                ),
            ];
        }

        private static void ConvertValueArrayToStringArray(JsonConversionLocalData ld, UnitData udt, UnitField uf, JsonNode? node)
        {
            Debug.Assert(node != null);
            uf.Value.StringArray = ((JsonArray)node).Cast<JsonValue>().Select(v => v.ToString()).ToArray();
        }

        public Program()
        {
            LuaRoot = string.Empty;
        }
        static void Main(string[] args)
        {
            var steamInfo = new SteamInfo();
            var steamRoot = steamInfo.GetRoot();
            var luaRoot = Path.Combine(steamRoot, @"engine\LJ\lua");
            var relativeRootTarget = @"ShatteredSunCommunity\wwwroot";
            var wwwRoot = Directory.GetCurrentDirectory();
            var wwwRootTarget = Path.Combine(wwwRoot, relativeRootTarget);
            while (!Directory.Exists(wwwRootTarget))
            {
                wwwRoot = Directory.GetParent(wwwRoot)?.FullName;
                if (wwwRoot == null)
                    break;
                wwwRootTarget = Path.Combine(wwwRoot, relativeRootTarget);
            }
            if (!Directory.Exists(wwwRootTarget))
            {
                throw new DirectoryNotFoundException(wwwRootTarget);
            }

            var program = new Program
            {
                WWWRoot = wwwRootTarget,
                SteamRoot = steamRoot,
                LuaRoot = luaRoot,
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
            CopyAllFilesToWWWRoot();
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

        private void CopyAllFilesToWWWRoot()
        {

            CopyFilesToWWWRoot(@"engine\Sanctuary_Data\Resources\UI\Gameplay\IconsUnits", "IconUnits", "*.png");
            CopyFilesToWWWRoot(@"engine\Sanctuary_Data\Resources\UI\UI\MapEditor\Icons", "IconUnits", "block_icon.png");
            CopyFilesToWWWRoot(@"engine\Sanctuary_Data\Resources\UI\UI\MapEditor\Images", "Images", "background_logo.png");
            var iconUnitDir = Path.Combine(WWWRoot, "IconUnits");
            File.Move(Path.Combine(iconUnitDir,"block_icon.png"), Path.Combine(iconUnitDir, "IconNotFound.png"));
        }

        private void CopyFilesToWWWRoot(string relativeSource, string relativeTarget, string fileMask)
        {
            var sourceDir = Path.Combine(SteamRoot, relativeSource);
            var targetDir = Path.Combine(WWWRoot, relativeTarget);
            foreach(var file in Directory.GetFiles(targetDir,fileMask))
            {
                File.Delete(file);
            }
            var filesToCopy = Directory.GetFiles(sourceDir, fileMask);
            if (!filesToCopy.Any())
                throw new FileNotFoundException($"{sourceDir} => '{fileMask}'");
            foreach (var file in Directory.GetFiles(sourceDir, fileMask))
            {
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));
            }
        }

        private void Add(
            JsonConversionLocalData ld,
            UnitData unit,
            JsonNode node,
            CreateUnitField createField,
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
                        Add(ld, unit, array[i].ToNullSafe(), createField, pathParts, iPathPart);
                    }
                    return;
                }

            }
            if (node == null && createField.RequireValidPath)
            {
                return;
            }
            var pascalCasedPathParts = pathParts.Select(p => p.Substring(0, 1).ToUpper() + p.Substring(1)).ToArray();
            var field = new UnitField
            {
                Path = string.Join("/", pathParts),
                PathParts = pascalCasedPathParts,
                Name = string.Concat(pascalCasedPathParts),
                IsImage = createField.IsImage,
                IsHeader = createField.IsHeader,
                DisplayName = createField.DisplayName ?? pascalCasedPathParts.Last(),
                Value =
                {
                    UnitFieldType = createField.UnitFieldType,
                }
            };
            var onCreate = createField.OnCreate ?? OnCreateDefault;
            onCreate(ld, unit, field, node);
            unit.Add(field.Name, field);
        }

        private void Add(
            JsonConversionLocalData ld,
            UnitData unit,
            JsonNode node,
            CreateUnitField createField)
        {
            var pathParts = createField.Path.Split('/').ToList();
            Add(ld, unit, node, createField, pathParts, 0);
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
        private void OnCreateDefault(JsonConversionLocalData ld, UnitData ud, UnitField uf, JsonNode? node)
        {
            Debug.Assert(node != null);
            // JsonConversionLocalData, UnitData, UnitField, JsonNode?
            switch (uf.Value.UnitFieldType)
            {
                case UnitFieldTypeEnum.String:
                    uf.Value.String = node.GetValue<string>();
                    break;
                case UnitFieldTypeEnum.Double:
                    uf.Value.Double = node.GetValue<double>();
                    break;
                case UnitFieldTypeEnum.Long:
                    uf.Value.Long = node.GetValue<long>();
                    break;
                case UnitFieldTypeEnum.StringArray:
                    uf.Value.StringArray = node.GetValue<string[]>();
                    break;
                case UnitFieldTypeEnum.Image:
                    uf.Value.Image = node.GetValue<string>();
                    break;
                case UnitFieldTypeEnum.Bool:
                    uf.Value.Bool = node.GetValue<bool>();
                    break;
            }
        }

        public class CreateUnitField(
            string path,
            UnitFieldTypeEnum unitFieldType,
            bool isImage = false,
            bool isHeader = false,
            bool requireValidPath = true,
            string? displayName = null,
            Action<JsonConversionLocalData, UnitData, UnitField, JsonNode?>? onCreate = null)
        {
            public Action<JsonConversionLocalData, UnitData, UnitField, JsonNode?>? OnCreate { get; } = onCreate;
            public bool RequireValidPath { get; set; } = requireValidPath;

            public string Path { get; } = path;
            public string? DisplayName { get; } = displayName;

            public int ColSpan { get; set; }
            public bool IsImage { get; set; } = isImage;
            public bool IsHeader { get; set; } = isHeader;

            public UnitFieldTypeEnum UnitFieldType { get; set; } = unitFieldType;
        }
    }

}
