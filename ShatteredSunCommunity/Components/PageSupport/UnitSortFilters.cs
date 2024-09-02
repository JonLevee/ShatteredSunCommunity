using Microsoft.VisualBasic;
using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitSortFilters : List<UnitCommonFilter>, ISelectorOwner
    {
        private List<string> selectorValues;
        public string FirstItemHeader => "sort by";
        public string RemainingItemHeader => "then by";
        public List<UnitCommonSelector> Selectors { get; }
        public UnitSortFilters()
        {
            Selectors = new List<UnitCommonSelector>();
            selectorValues = new List<string>
            {
                string.Empty,
            };
        }

        public void Add(string field, IEnumerable<string> values)
        {
            var filter = new UnitCommonFilter(field, values);
            base.Add(filter);
            selectorValues.Add(filter.Display);
            Selectors.Add(new UnitCommonSelector(this, selectorValues, "add interstitial values"));
        }
    }
}
