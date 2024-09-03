using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using ShatteredSunCommunity.Models;
using System;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitGroupByFilters : UnitCommonFilters
    {

        public override string FirstItemHeader => "group by";
        public override string RemainingItemHeader => "then by";
        public List<UnitViewHeaderRow> HeaderRows { get; }
        public UnitGroupByFilters(UnitViewFilters parent) : base(parent)
        {
            HeaderRows = new List<UnitViewHeaderRow>();
        }


        public void UpdateHeaderRows()
        {
            HeaderRows.Clear();
            foreach (var selector in Selectors.Where(s => s.IsActive))
            {
                var filter = this.Single(f => f.Display == selector.Selected);
                var row = new UnitViewHeaderRow(filter, selector);
                // repeat for the number of columns in the previous row
                var lastRowColumns = HeaderRows.LastOrDefault()?.Columns;
                var repeatCount = lastRowColumns?.Count ?? 1;
                // update all previous row columns colspans to account for our new row
                foreach (var r in HeaderRows)
                {
                    r.IsLastHeader = false;
                    foreach (var c in r.Columns)
                    {
                        c.Colspan *= filter.Values.Count;
                    }
                }
                if (lastRowColumns == null)
                {
                    row.Columns.AddRange(filter.Values.Select(v => new UnitViewHeaderCol(row, v)));
                }
                else
                {
                    foreach (var column in lastRowColumns)
                    {
                        row.Columns.AddRange(filter.Values.Select(v => new UnitViewHeaderCol(row, v, column)));
                    }
                }
                HeaderRows.Add(row);
            }
        }

        public void Add(UnitCommonFilter filter)
        {
            base.Add(filter);
            SelectorValues.Add(filter.Display);
            Selectors.Add(new UnitCommonSelector(this, SelectorValues));
        }
    }
}
