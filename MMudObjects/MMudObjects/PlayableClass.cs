using System.Collections.Generic;

namespace MMudObjects
{
    public class PlayableClass
    {
        string Name { get; set; }
        int Id { get; set; }
        int Exp { get; set; }
        List<EnumWeaponType> AllowedWeaponTypes { get; set; }
        EnumArmorType AllowedArmorType { get; set; }
        EnumMagicType MagicType {get;set;}
        int Combat { get; set; }
        int MinHealth { get; set; }
        int MaxHealth { get; set; }
        List<ItemAbility> Abilities { get; set; }



    }
}