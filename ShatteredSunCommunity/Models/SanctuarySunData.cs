namespace ShatteredSunCommunity.Models
{
    public class SanctuarySunData
    {
        public AvailableUnits AvailableUnits { get; set; }
        public Factions Factions { get; set; }
        public Units Units { get; set; }

        public SanctuarySunData()
        {
            AvailableUnits = new AvailableUnits();
            Factions = new Factions();
            Units = new Units();
        }
    }
}
