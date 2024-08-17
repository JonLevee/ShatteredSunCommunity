using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ShatteredSunCommunity;
using ShatteredSunCommunity.Models;
using ShatteredSunCommunity.UnitSelect;

namespace ShatteredSunCommunity.UnitSelect
{
    public class UnitSelectListChangedEventArgs : EventArgs
    {
        public UnitSelectList SelectList { get; }
        public UnitSelectItem Item { get; }
        public UnitSelectListChangedEventArgs(UnitSelectList selectList, UnitSelectItem item) : base()
        {
            SelectList = selectList;
            Item = item;
        }
    }
}
