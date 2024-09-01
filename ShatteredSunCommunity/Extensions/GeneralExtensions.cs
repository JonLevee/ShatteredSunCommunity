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
                .SelectMany(u => u[key].AsStringArray)
                .Distinct()
                .Order();
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
        public static bool IsThumbnail(this UnitField field)
        {
            return thumbnails.Contains(field.Path);
        }
    }
}
