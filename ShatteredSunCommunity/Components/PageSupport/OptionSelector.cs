using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    [DebuggerDisplay("{Value} {IsSelected ? \"selected\" : \"\"} {IsDisabled ? \"disabled\" : \"\"}")]
    public class OptionSelector
    {
        public static readonly string DefaultValue = string.Empty;
        public string Value { get; }
        public bool IsSelected { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDefault => Value == OptionSelector.DefaultValue;

        public OptionSelector(string value)
        {
            Value = value;
        }
    }
}
