using NLog.Filters;
using ShatteredSunCommunity.Models;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class FilterOptionHandler : ReadOnlyDictionary<string, FilterOptionSelectorItem>
    {

        public FilterOptionHandler(IDictionary<string, FilterOptionSelectorItem> keyValuePairs) : base(keyValuePairs)
        {
        }
    }
}