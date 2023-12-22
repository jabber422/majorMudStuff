using System;
using System.Collections.Generic;

namespace MMudObjects
{
    public class PlayableClass
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Exp { get; set; }
        public List<EnumWeaponType> AllowedWeaponTypes { get; set; }
        public EnumArmorType AllowedArmorType { get; set; }
        public EnumMagicType MagicType {get;set;}
        public int Combat { get; set; }
        public int MinHealth { get; set; }
        public int MaxHealth { get; set; }
        public List<ItemAbility> Abilities { get; set; }

        public PlayableClass(string className)
        {
            //TODO: Authenticate this vs DB list. load stats on creation?
            this.Name = className;
        }

        internal static PlayableClass Create(string value)
        {
            return new PlayableClass(value);
        }
    }

    public static class PlayableClassFactory
    {
        public static PlayableClass CreteClassFromName(string className)
        {
            return new PlayableClass(className);
        }
    }
}