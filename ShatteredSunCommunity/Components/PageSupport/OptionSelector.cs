using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    [DebuggerDisplay("{Value} {IsSelected ? \"selected\" : \"\"} {IsDisabled ? \"disabled\" : \"\"}")]
    public class OptionSelector
    {
        public string Value { get; }
        public bool IsSelected { get; set; }
        public bool IsDisabled { get; set; }
        public OptionSelector(string value)
        {
            Value = value;
        }
    }
}
