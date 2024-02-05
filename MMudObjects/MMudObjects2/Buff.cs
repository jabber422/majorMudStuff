using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

namespace MMudObjects
{
    public class Spell
    {
        private DataRow row;

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

        public Spell(DataRow row)
        {
            this.Abilities = new List<ItemAbility>();
            this.CastTime = DateTime.Now;

            this.Id = int.Parse(row["Number"].ToString());
            this.Name = row["Name"].ToString();
            this.ShortName = row["Short"].ToString();
            this.MagicType = (EnumMagicType)Enum.Parse(typeof(EnumMagicType), row["Magery"].ToString());
            this.Level = int.Parse(row["ReqLevel"].ToString());
            this.Mana = int.Parse(row["ManaCost"].ToString());
            this.Difficulty = int.Parse(row["Diff"].ToString());
            this.TargetType = (EnumTargetType)Enum.Parse(typeof(EnumTargetType), row["Targets"].ToString());
            this.AttackType = (EnumAttackType)Enum.Parse(typeof(EnumAttackType), row["AttType"].ToString());
            this.Duration = int.Parse(row["Dur"].ToString());
            this.MaxIncLVLs = int.Parse(row["MaxIncLVLs"].ToString());
            this.DurIncLVLs = int.Parse(row["DurIncLVLs"].ToString());
            this.DurInc = int.Parse(row["DurInc"].ToString());

            for (int i = 0; i < 10; i++)
            {
                var abil = new ItemAbility();
                abil.Abililty = int.Parse(row[$"Abil-{i}"].ToString());
                abil.Value = int.Parse(row[$"AbilVal-{i}"].ToString());
                this.Abilities.Add(abil);
            }
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
        Single=0,Self=1,SingleOrSelf=2,
        DivArea=3,
        Monster=4,DivAreaYou=5,
        Any=6,Item=7,
        MonsterOrSingle=8,
        DivAtt,DivParty,FullArea=11,FullAttackArea=12,FullPartyArea=13,
    }

    public enum EnumAttackType
    {
        NORMAL, LIGHTNING, HOT, COLD, WATER
    }
}