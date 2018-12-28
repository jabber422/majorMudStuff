using System.Collections.Generic;

namespace MMudObjects
{
    public class PlayableRace
    {
        int Id { get; set; }
        int Name { get; set; }
        double Exp { get; set; }
        int Health { get; set; }
        List<ItemAbility> Abilities { get; set; }
        int StrengthMin { get; set; }
        int StrengthMax { get; set; }
        int IntelMin { get; set; }
        int IntelMax { get; set; }
        int WisdomMin { get; set; }
        int WisdomMax { get; set; }
        int AgilityMin { get; set; }
        int AgilityMax { get; set; }
        int HealthMin { get; set; }
        int HealthMax { get; set; }
        int CharmMin { get; set; }
        int CharmMax { get; set; }
    }
}