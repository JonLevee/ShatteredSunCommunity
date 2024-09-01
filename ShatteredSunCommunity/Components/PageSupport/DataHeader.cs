using NLog.Filters;
using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    [DebuggerDisplay("Columns:{Columns.Count} {ColumnsDebugString}")]
    public class UnitViewHeaderRow
    {
        public static readonly UnitViewHeaderRow Empty = new UnitViewHeaderRow(null, null);
        public List<UnitViewHeaderCol> Columns { get; }
        public UnitGroupByFilter Filter { get; }
        public UnitGroupBySelector Selector { get; }

        public UnitViewHeaderRow(UnitGroupByFilter filter, UnitGroupBySelector selector)
        {
            Columns = new List<UnitViewHeaderCol>();
            Filter = filter;
            Selector = selector;
        }


        private string ColumnsDebugString => string.Join(",", Columns.Select(c => c.Header));
    }

    [DebuggerDisplay("{Header}")]
    public class UnitViewHeaderCol
    {
        private readonly UnitViewHeaderRow row;
        private string header;
        private readonly Func<UnitData, bool> parentIncludeUnit;

        public string Header => $"{header}[{Colspan}]";
        public int Colspan { get; set; }

        public UnitViewHeaderCol(UnitViewHeaderRow row, string header, UnitViewHeaderCol parentCol = null)
        {
            this.row = row;
            this.header = header;
            parentIncludeUnit = null == parentCol ? (unit) => true : parentCol.IncludeUnit;
            Colspan = 1;
        }

        public bool IncludeUnit(UnitData unit)
        {
            return unit[row.Filter.Field].Text == header && parentIncludeUnit(unit);
        }
    }

    public class UnitViewDataRow
    {
        public List<UnitData> Units { get; } = new List<UnitData>();
    }

}
