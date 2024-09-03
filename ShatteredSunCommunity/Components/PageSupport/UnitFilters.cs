using Microsoft.VisualBasic;
using ShatteredSunCommunity.Models;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public class UnitFilters : UnitCommonFilters
    {
        public override string FirstItemHeader => "filter";
        public override string RemainingItemHeader => "filter";

        public UnitFilters(UnitViewFilters parent) : base(parent)
        {

        }

        public override void UpdateSelectors(IEnumerable<UnitCommonSelector> otherSelectorsToCheck = null)
        {
            var selectorsToCheck = null == otherSelectorsToCheck ? Selectors : otherSelectorsToCheck.Concat(Selectors);
            // these filters don't depend on any other filters being set, just remove inactive then
            // add to bottom
            Selectors.RemoveAll(s=>!s.IsActive);
            AppendInactiveSelector(selectorsToCheck, false);
        }

        public bool FilterUnits(UnitData unit)
        {
            if (!Selectors.Any(s => s.IsActive))
                return true;
            return true;
        }
    }
}
