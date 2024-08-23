using System.Collections;
using System.Collections.Frozen;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using ShatteredSunCommunity;
using ShatteredSunCommunity.Conversion;
using ShatteredSunCommunity.MiscClasses;
using ShatteredSunCommunity.Models;
using ShatteredSunCommunity.UnitSelect;
using ShatteredSunCommunity.UnitSelect.Definitions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ShatteredSunCommunity.UnitSelect
{
    public static class UnitSelectListGroupCalculator
    {
        public static UnitSelectListGroupedItemResult GetGroupings(UnitGroupByDefinitions definitions, UnitSelectListGroupBy groupings, Units units)
        {
            var result = new UnitSelectListGroupedItemResult();
            foreach (var groupBy in groupings.Items)
            {
                if (groupBy.Selected != UnitSelectListGroupByItem.Empty)
                {
                    result.AddRow(definitions.Single(d => d.Name == groupBy.Selected));
                }
            }
            File.WriteAllText("headings.json", JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }));
            return result;
        }
    }
    [DebuggerDisplay("[{Rows.Count}]")]
    public class UnitSelectListGroupedItemResult
    {
        public List<UnitSelectListGroupedRow> Rows { get; }
        public UnitSelectListGroupedItemResult()
        {
            Rows = new List<UnitSelectListGroupedRow>();
        }

        public void AddRow(UnitGroupByDefinition definition)
        {
            Rows.Add(new UnitSelectListGroupedRow(definition, Rows.LastOrDefault()));

        }
    }

    [DebuggerDisplay("{Group} Columns:{Columns.Count}")]
    public class UnitSelectListGroupedRow
    {
        private readonly UnitGroupByDefinition definition;

        public string Group => definition.Name;
        public List<UnitSelectListGroupedColumn> Columns { get; set; }

        public UnitSelectListGroupedRow(UnitGroupByDefinition definition, UnitSelectListGroupedRow previousRow)
        {
            this.definition = definition;
            Columns = new List<UnitSelectListGroupedColumn>();

            if (previousRow == null)
            {
                foreach (var item in definition.Items)
                {
                    Columns.Add(new UnitSelectListGroupedColumn(item, definition.Filter));
                }
            }
            else
            {
                foreach (var column in previousRow?.Columns)
                {
                    column.ColSpan *= definition.Items.Count;
                }
                foreach (var column in previousRow.Columns)
                {
                    foreach (var item in definition.Items)
                    {
                        Columns.Add(new UnitSelectListGroupedColumn(item, ud => column.Filter(definition.Filter(ud)), column));
                    }
                }
            }
        }
    }

    [DebuggerDisplay("{Name} ColSpan:{ColSpan}")]
    public class UnitSelectListGroupedColumn
    {
        public int ColSpan { get; set; }
        public string Name { get; }
        public string AggregateName { get; }

        [JsonIgnore]
        public Func<IEnumerable<UnitData>, IEnumerable<UnitData>> Filter { get; }
        public UnitSelectListGroupedColumn(string name, Func<IEnumerable<UnitData>, IEnumerable<UnitData>> filter, UnitSelectListGroupedColumn group = null)
        {
            Name = name;
            AggregateName = null == group ? name : group.AggregateName + ":" + name;
            Filter = filter;
            ColSpan = 1;
        }
    }
}
