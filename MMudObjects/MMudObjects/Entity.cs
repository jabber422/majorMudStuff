using MMudObjects;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MMudObjects
{
    public class Entity : ISerializable
    {
        string Name { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new System.NotImplementedException();
        }
    }

    public class NPC : Entity
    {
        string Id { get; set; }
        int Exp { get; set; }
        int Regen { get; set; }
        EnumNpcType Type { get; set; }
        EnumNpcAlignment Alignment { get; set; }
        int Health { get; set; }
        int HealthRegen { get; set; }
        int AC { get; set; }
        int DR { get; set; }
        int MR { get; set; }
        int FollowPercentage { get; set; }
        int CharmLevel { get; set; }
        List<ItemAbility> Abilities { get; set; }

        List<MonsterAttackInfo> Attacks { get; set; }

        List<Room> RegenRooms { get; set; }

        int Magic { get; set; }
    }

    public class Player : Entity
    {
        public Player(string firstName)
        {
            this.FirstName = firstName;
        }
        public List<CarryableItem> Inventory { get; set; }
        public virtual EquippedItemsInfo Equipped { get; set; }

        public PlayableClass Class { get; set; }
        public PlayableRace Race { get; set; }
        public virtual int Rank { get; set; }
        public virtual double Exp { get; set; }
        public virtual int Level { get; set; }

        public List<ItemAbility> Abilities { get; set; }

        public List<QuestAbility> QuestAbilities { get; set; }

        public List<Buff> Buffs { get; set; }

        public int HealthRegenNormal { get; set; }
        public int HealthRegenResting { get; set; }
        public int ManaRegenNormal { get; set; }
        public int ManaRegenResting { get; set; }

        public int Strength { get; set; }
        public int Intel { get; set; }
        public int Wisdom { get; set; }
        public int Agility { get; set; }
        public int Health { get; set; }
        public int Charm { get; set; }
        public virtual string LastName { get; set; }
        public virtual string GangName { get; set; }
        public virtual string Title { get; set; }
        public virtual string Alignment { get; set; }
        public virtual string LevelRange { get; set; }
        public string FirstName { get; }
    }
}


public enum EnumNpcType
{
    Leader, Solo, Follower, Stationary,
}

public enum EnumNpcAlignment
{ C_EVIL, L_EVIL, NEUTRAL, GOOD, L_GOOD}
