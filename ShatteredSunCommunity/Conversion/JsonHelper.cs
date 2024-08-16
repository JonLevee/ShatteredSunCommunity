using ShatteredSunCommunity.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using NLua;
using System.Text.Json.Nodes;
using System.Diagnostics;

namespace ShatteredSunCommunity.Conversion
{
    public class JsonHelper
    {
        public static readonly int ExpectedMaxGroups = 3;
        public static readonly string[] TECHTIERS = Enumerable.Range(1, 9).Select(i => $"TECH{i}").ToArray();


        public static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            IgnoreReadOnlyFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault,
            Converters =
                        {
                            new LuaTableConverter(),
                            new JsonStringEnumConverter<UnitFieldTypeEnum>()
                        },
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
        };
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

        public static JsonNode ConvertLuaTableToJson(LuaTable luaTable)
        {
            var json = JsonSerializer.Serialize(luaTable, JsonHelper.JsonOptions);
            var jsonNode = JsonSerializer.Deserialize<JsonNode>(json, JsonHelper.JsonOptions);
            Debug.Assert(jsonNode != null);
            return jsonNode;
        }
    }
}
