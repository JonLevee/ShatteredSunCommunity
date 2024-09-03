using Microsoft.VisualBasic;
using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitSortFilters : List<UnitCommonFilter>, ISelectorOwner
    {
        private UnitViewFilters parent;
        private List<string> selectorValues;
        public string FirstItemHeader => "sort by";
        public string RemainingItemHeader => "then by";
        public List<UnitCommonSelector> Selectors { get; }
        public void Refresh()
        {
            parent.Refresh();
        }
        public UnitSortFilters(UnitViewFilters parent)
        {
            this.parent = parent;
            Selectors = new List<UnitCommonSelector>();
            selectorValues = new List<string>
            {
                string.Empty,
            };
        }

        public IOrderedEnumerable<UnitData> OrderBy(IEnumerable<UnitData> units)
        {
            IOrderedEnumerable<UnitData> result = null;
            foreach (var selector in Selectors.Where(s => s.IsActive))
            {
                var filter = this.Single(f => f.Display == selector.Selected);
                if (result == null)
                {
                    result = units.OrderBy(u => u[filter.Field].Text);
                }
                else
                {
                    result = result.ThenBy(u => u[filter.Field].Text);
                }
            }
            return result ?? units.OrderBy(u => u["tpId"].Text);
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
