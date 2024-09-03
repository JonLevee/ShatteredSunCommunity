using Microsoft.VisualBasic;
using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitGroupByFilters : List<UnitCommonFilter>, ISelectorOwner
    {
        private List<string> selectorValues;
        private readonly UnitViewFilters parent;

        public string FirstItemHeader => "group by";
        public string RemainingItemHeader => "then by";
        public List<UnitCommonSelector> Selectors { get; }
        public List<UnitViewHeaderRow> HeaderRows { get; }
        public UnitGroupByFilters(UnitViewFilters parent)
        {
            Selectors = new List<UnitCommonSelector>();
            HeaderRows = new List<UnitViewHeaderRow>();
            selectorValues = new List<string>
            {
                string.Empty,
            };
            this.parent = parent;
            parent.Changed += (o, e) => OnChanged();
        }

        public void Refresh()
        {
            parent.Refresh();
        }

        public UnitViewHeaderRow GetDataHeaderRow(UnitCommonSelector selector) => HeaderRows.SingleOrDefault(r => r.Selector == selector) ?? UnitViewHeaderRow.Empty;

        public IEnumerable<UnitViewDataRow> GetRows(Units units)
        {
            if (!HeaderRows.Any())
            {
                foreach (var unit in units)
                {
                    yield return new UnitViewDataRow
                    {
                        Units =
                        {
                            unit
                        }
                    };
                }
            }
            else
            {
                var header = HeaderRows.Last();
                var columns = header
                    .Columns
                    .Select(c => parent.SortFilters.OrderBy(
                        units.Where(c.IncludeUnit)
                        ).ToList())
                    .ToList();
                var maxRows = columns.Max(c => c.Count);
                for (var iRow = 0; iRow < maxRows; ++iRow)
                {
                    var row = new UnitViewDataRow();
                    foreach (var col in columns)
                    {
                        row.Units.Add(col.Count > iRow ? col[iRow] : null);
                    }
                    yield return row;
                }
            }
        }

        private void OnChanged()
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
        public void Add(string field, IEnumerable<string> values)
        {
            var filter = new UnitCommonFilter(field, values);
            base.Add(filter);
            selectorValues.Add(filter.Display);
            Selectors.Add(new UnitCommonSelector(this, selectorValues));
        }
    }
}
