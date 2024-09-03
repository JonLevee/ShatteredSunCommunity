using ShatteredSunCommunity.Models;
using System.Text.RegularExpressions;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitViewFilters : IRefresh
    {
        private bool showUnitThumbnails;
        public event EventHandler Changed;
        public UnitGroupByFilters GroupBy { get; }
        public UnitSortFilters SortFilters { get; }
        public UnitFilters Filters { get; }
        public bool ShowUnitThumbnails
        {
            get => showUnitThumbnails;
            set
            {
                showUnitThumbnails = value;
                Refresh();
            }
        }
        public UnitViewFilters()
        {
            GroupBy = new UnitGroupByFilters(this);
            SortFilters = new UnitSortFilters(this);
            Filters = new UnitFilters(this);
        }

        public void Refresh()
        {
            GroupBy.UpdateHeaderRows();
            GroupBy.UpdateSelectors();
            SortFilters.UpdateSelectors(GroupBy.Selectors);
            Filters.UpdateSelectors(GroupBy.Selectors.Concat(SortFilters.Selectors));
            Changed?.Invoke(this, EventArgs.Empty);
        }
        public UnitViewHeaderRow GetDataHeaderRow(UnitCommonSelector selector) => GroupBy.HeaderRows.SingleOrDefault(r => r.Selector == selector) ?? UnitViewHeaderRow.Empty;

        public IEnumerable<UnitViewDataRow> GetRows(Units units)
        {
            if (!GroupBy.HeaderRows.Any())
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
                yield break;
            }
            var header = GroupBy.HeaderRows.Last();
            var columns = header
                .Columns
                .Select(c => SortFilters.OrderBy(
                    units
                        .Where(Filters.FilterUnits)
                        .Where(c.IncludeUnit)
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
}
