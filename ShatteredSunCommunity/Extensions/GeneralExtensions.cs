using ShatteredSunCommunity.Components.PageSupport;
using ShatteredSunCommunity.Models;
using System.Diagnostics.Contracts;

namespace ShatteredSunCommunity.Extensions
{
    public static class GeneralExtensions
    {
        public static string ToStringNullSafe(this object? value)
        {
            Contract.Ensures(Contract.Result<string>() != null);
            return (value ?? string.Empty).ToString();
        }

        public static T ToNullSafe<T>(this T? value)
        {
            Contract.Ensures(Contract.Result<string>() != null);
            return value;
        }

        public static UnitCommonFilter GetFilter(this SanctuarySunData data, string key, bool supportsIntersticial = false, bool useFreeFormFilter = false)
        {
            var fields = data
                .Units
                .Where(u => u.ContainsKey(key))
                .Select(u => u[key])
                .ToList();
            var first = fields.First();
            var values = first.Value.UnitFieldType == UnitFieldTypeEnum.StringArray
                ? fields.SelectMany(f => f.Value.StringArray)
                : fields.Select(f => f.Text)
                .ToList();
            return new UnitCommonFilter(first, values.Distinct().Order(), supportsIntersticial, useFreeFormFilter);
        }

        private static readonly HashSet<string> thumbnails =
            [
                "defence/health/max",
                "economy/cost/alloys",
                "economy/cost/energy",
                "general/displayName",
                "general/name",
                "general/tpId",
                "movement/type",
                "faction",
                "tier",
            ];
        // need to move this somewhere else
        public static bool GetThumbnail(this UnitField field)
        {
            return thumbnails.Contains(field.Path);
        }
    }
}
