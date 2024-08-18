﻿using System.Collections.Frozen;
using System.Diagnostics;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using ShatteredSunCommunity;
using ShatteredSunCommunity.Conversion;
using ShatteredSunCommunity.MiscClasses;
using ShatteredSunCommunity.Models;
using ShatteredSunCommunity.UnitSelect;
using ShatteredSunCommunity.UnitSelect.Definitions;

namespace ShatteredSunCommunity.UnitSelect.Definitions
{
    public class UnitGroupByDefinitions : List<UnitGroupByDefinition>
    {
        public UnitGroupByDefinitions()
        {
        }
    }
}
