using ShatteredSunCommunity.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using NLua;

namespace ShatteredSunCommunity.Conversion
{
    public class JsonConversionLocalData
    {
        public AvailableUnits AvailableUnits { get; set; }
        public Factions Factions { get; set; }

        public JsonConversionLocalData()
        {
            AvailableUnits = new AvailableUnits();
            Factions = new Factions();
        }
    }
}
