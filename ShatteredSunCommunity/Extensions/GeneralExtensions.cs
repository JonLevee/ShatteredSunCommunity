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

        public static IEnumerable<T> GetDistinct<T>(this SanctuarySunData data, string key, Func<UnitField, T> getValue)
        {
            return data
                .Units
                .Where(u => u.ContainsKey(key))
                .Select(u => getValue(u[key]))
                .Distinct()
                .Order();
        }

        public static IEnumerable<string> GetDistinctFromArray(this SanctuarySunData data, string key)
        {
            return data
                .Units
                .Where(u => u.ContainsKey(key))
                .SelectMany(u => u[key].Value.StringArray)
                .Distinct()
                .Order();
        }

        public static UnitCommonFilter GetFilter(this SanctuarySunData data, string key, bool supportsIntersticial = false)
        {
            var fields = data
                .Units
                .Where(u => u.ContainsKey(key))
                .Select(u => u[key]);
            var first = fields.First();
            var values = first.UnitFieldType == UnitFieldTypeEnum.StringArray
                ? fields.SelectMany(f=>f.Value.StringArray)
                : fields.Select(f=>f.Text);
            return new UnitCommonFilter(first.Name, first.DisplayName, values.Distinct().Order(), supportsIntersticial);
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
