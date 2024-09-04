using NLog.Filters;
using ShatteredSunCommunity.Models;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class FilterOptionSelectorItem
    {
        public Func<UnitFieldValue, UnitFieldValue, UnitFieldValue, bool> Filter { get; }
        public string Key { get; }
        public string JoiningWord { get; }
        public FilterOptionSelectorItem(string key, Func<UnitFieldValue, UnitFieldValue, UnitFieldValue, bool> filter) : this(key, null, filter)
        {
        }
        public FilterOptionSelectorItem(string key, string joiningWord, Func<UnitFieldValue, UnitFieldValue, UnitFieldValue, bool> filter)
        {
            Key = key;
            JoiningWord = joiningWord;
            this.Filter = filter;
        }
    }
}