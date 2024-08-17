using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ShatteredSunCommunity;
using ShatteredSunCommunity.Models;
using ShatteredSunCommunity.UnitSelect;

namespace ShatteredSunCommunity.UnitSelect
{
    public class UnitSelectLists
    {
        public UnitSelectList GroupByList { get; }
        public UnitSelectList FilterList { get; }

        public UnitSelectLists()
        {
            GroupByList = new UnitSelectList();
            FilterList = new UnitSelectList();
        }
    }
}
