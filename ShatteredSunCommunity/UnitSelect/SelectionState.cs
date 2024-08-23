using System.Collections;
using System.Collections.Frozen;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class SelectionState
    {
        public EventHandler SelectionStateChanged;
        public SelectionState()
        {
        }

        public void OnSelectionStateChanged()
        {
            SelectionStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
