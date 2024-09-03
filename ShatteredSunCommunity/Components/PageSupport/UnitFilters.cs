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
        public void Add(UnitCommonFilter filter)
        {
            base.Add(filter);
            SelectorValues.Add(filter.Display);
            Selectors.Add(new UnitCommonSelector(this, SelectorValues));
        }

        public bool FilterUnits(UnitData unit)
        {
            if (!Selectors.Any(s => s.IsActive))
                return true;
            return true;
        }
    }
}
