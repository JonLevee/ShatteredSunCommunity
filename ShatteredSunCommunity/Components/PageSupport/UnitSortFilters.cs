using Microsoft.VisualBasic;
using ShatteredSunCommunity.Models;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitSortFilters : UnitCommonFilters
    {
        private List<string> selectorValues;
        public override string FirstItemHeader => "sort by";
        public override string RemainingItemHeader => "then by";
        public UnitSortFilters(UnitViewFilters parent) : base(parent)
        {
            selectorValues = new List<string>
            {
                string.Empty,
            };
        }

        private string GetSortField(UnitData unit, string field)
        {
            return unit.ContainsKey(field) ? unit[field].Text : "zzzzzz";
        }
        public IOrderedEnumerable<UnitData> OrderBy(IEnumerable<UnitData> units)
        {
            IOrderedEnumerable<UnitData> result = null;
            foreach (var selector in Selectors.Where(s => s.IsActive))
            {
                var filter = this.Single(f => f.Display == selector.Selected);
                if (result == null)
                {
                    result = units.OrderBy(u => GetSortField(u,filter.Field));
                }
                else
                {
                    result = result.ThenBy(u => GetSortField(u, filter.Field));
                }
            }
            return result ?? units.OrderBy(u => u["GeneralTpId"].Text);
        }

        public void Add(UnitCommonFilter filter)
        {
            base.Add(filter);
            selectorValues.Add(filter.Display);
            Selectors.Add(new UnitCommonSelector(this, selectorValues));
        }
    }
}
