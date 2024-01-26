using System;
using System.Collections.Generic;

namespace MMudObjects
{
    public class Spell
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public EnumMagicType MagicType {get;set;}
        public int Level { get; set; }
        public int Mana { get; set; }
        public int Difficulty { get; set; }
        
        public EnumTargetType TargetType { get; set; }
        public EnumAttackType AttackType { get; set; }
        public int Duration { get; set; }

        public List<ItemAbility> Abilities { get; set; }

        public Spell(string name)
        {
            this.Name = name;
            this.Abilities = new List<ItemAbility>();
            this.CastTime = DateTime.Now;
        }

        public DateTime CastTime { get; set; }
        public int DurIncLVLs { get; set; }
        public int DurInc { get; set; }
        public int MaxIncLVLs { get; set; }
    }
    //public class Buff : Spell
    //{
    //}

    public enum EnumTargetType
    {
        FULL_AREA_ATTACK, SELF, MONSTER_OR_USER, 
    }

    public enum EnumAttackType
    {
        NORMAL, LIGHTNING, HOT, COLD, WATER
    }
}