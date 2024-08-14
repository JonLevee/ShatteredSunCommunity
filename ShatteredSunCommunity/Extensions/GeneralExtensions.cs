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
    }
}
