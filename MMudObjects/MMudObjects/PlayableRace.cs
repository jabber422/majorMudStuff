using System.Collections.Generic;

namespace MMudObjects
{
    public class PlayableRace
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Exp { get; set; }
        public int Health { get; set; }
        public List<ItemAbility> Abilities { get; set; }
        public int StrengthMin { get; set; }
        public int StrengthMax { get; set; }
        public int IntelMin { get; set; }
        public int IntelMax { get; set; }
        public int WisdomMin { get; set; }
        public int WisdomMax { get; set; }
        public int AgilityMin { get; set; }
        public int AgilityMax { get; set; }
        public int HealthMin { get; set; }
        public int HealthMax { get; set; }
        public int CharmMin { get; set; }
        public int CharmMax { get; set; }
    }
}