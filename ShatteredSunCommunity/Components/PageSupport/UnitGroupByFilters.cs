using Microsoft.VisualBasic;
using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitGroupByFilters : IEnumerable<UnitGroupByFilter>
    {
        private List<string> selectorValues;
        public List<UnitGroupByFilter> Filters { get; }
        public List<UnitGroupBySelector> Selectors { get; }
        public List<UnitViewHeaderRow> HeaderRows { get; }
        public UnitGroupByFilters()
        {
            Filters = new List<UnitGroupByFilter>();
            Selectors = new List<UnitGroupBySelector>();
            HeaderRows = new List<UnitViewHeaderRow>();
            selectorValues = new List<string>
            {
                string.Empty,
            };
        }

        public UnitViewHeaderRow GetDataHeaderRow(UnitGroupBySelector selector) => HeaderRows.SingleOrDefault(r => r.Selector == selector) ?? UnitViewHeaderRow.Empty;

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
                var columns = header.Columns.Select(c => units.Where(c.IncludeUnit).ToList()).ToList();
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

        public void OnChanged()
        {
            HeaderRows.Clear();
            foreach (var selector in Selectors.Where(s => s.IsActive))
            {
                var filter = Filters.Single(f => f.Display == selector.Selected);
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
            var filter = new UnitGroupByFilter(field, values);
            Filters.Add(filter);
            selectorValues.Add(filter.Display);
            Selectors.Add(new UnitGroupBySelector(this, selectorValues));
        }
        public IEnumerator<UnitGroupByFilter> GetEnumerator()
        {
            return Filters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Filters.GetEnumerator();
        }
    }
}
