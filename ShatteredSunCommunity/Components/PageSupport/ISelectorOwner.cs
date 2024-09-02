using Microsoft.VisualBasic;
using ShatteredSunCommunity.Models;
using System.Collections;
using System.Diagnostics;

namespace ShatteredSunCommunity.Components.PageSupport
{
    public interface ISelectorOwner
    {
        string FirstItemHeader { get; }
        string RemainingItemHeader { get; }
        List<UnitCommonSelector> Selectors { get; }
    }
}
