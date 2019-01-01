using MMudObjects;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace MMudObjects
{
    public class Entity : ISerializable
    {
        public virtual string Name { get; set; }

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
        public Room Room { get; set; }

        public virtual double Exp
        {
            get { return this.Stats.Exp; }
            set { this.Stats.Exp = value; }
        }
        public Player(string name)
        {
            string[] tokens = name.Split(new char[] { ' ' });
            this.Stats = new PlayerStats(name);
            this.Room = new Room();
        }

        public List<CarryableItem> Inventory { get; set; }
        public virtual EquippedItemsInfo Equipped { get; set; }

        public virtual int Rank { get; set; }

        public List<ItemAbility> Abilities { get; set; }

        public List<QuestAbility> QuestAbilities { get; set; }

        public List<Buff> Buffs { get; set; }

        public int HealthRegenNormal { get; set; }
        public int HealthRegenResting { get; set; }
        public int ManaRegenNormal { get; set; }
        public int ManaRegenResting { get; set; }

        
        public virtual string GangName { get; set; }
        public virtual string Title { get; set; }
        public virtual string Alignment { get; set; }
        public virtual string LevelRange { get; set; }

        public string FirstName {
            get { return this.Stats.FirstName; }
        }

        public virtual string LastName
        {
            get { return this.Stats.LastName; }
        }

        public virtual string Name
        {
            get { return this.Stats.Name; }
            set { this.Stats.UpdateName(value); }
        }

        public PlayerStats Stats { get; set; }
    }
}

public class PlayerStats
{
    //this is a FirstName and possibly a LastName, space seperated, no special chars
    public string Name { get { return this._name; } }
    public string FirstName { get { return this._firstName; } }
    public string LastName { get { return this._lastName; } }

    public int Lives { get; set; }
    public int CP { get; set; }
    public int Strength { get; set; }
    public int Intellect { get; set; }
    public int Willpower { get; set; }
    public int Agility { get; set; }
    public int Health { get; set; }
    public int Charm { get; set; }

    public PlayableClass Class { get; set; }
    public PlayableRace Race { get; set; }

    public double Exp { get; set; }
    public int Level { get; set; }

    public int MaxHits { get; set; }
    public int CurHits { get; set; }
    public int ArmourClass { get; set; }
    public int DamageRes { get; set; }

    public int Perception { get; set; }
    public int Stealth { get; set; }
    public int Thievery { get; set; }
    public int Traps { get; set; }
    public int Picklocks { get; set; }
    public int Tracking { get; set; }
    public int MartialArts { get; set; }
    public int MagicRes { get; set; }

    //Dictionary<String, int> IntStats;

    public PlayerStats(string name)
    {
        UpdateName(name);
    }

    public void Update(DataChangeItem dci)
    {
        switch (dci.targetProperty)
        {
            case "Player.Stats.Name":
                this.UpdateName(dci.groups[1].Value);
                break;
            case "Player.Stats.LivesCP":
                this.Lives = int.Parse(dci.groups[1].Value);
                this.CP = int.Parse(dci.groups[2].Value);
                break;
            case "Player.Stats.Race":
                this.Race = PlayableRace.Create(dci.groups[1].Value);
                break;
            case "Player.Stats.Class":
                this.Class = PlayableClass.Create(dci.groups[1].Value);
                break;
            case "Player.Stats.Strength":
                this.Strength = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Intellect":
                this.Intellect = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Willpower":
                this.Willpower = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Agility":
                this.Agility = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Health":
                this.Health = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Charm":
                this.Charm = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Exp":
                this.Exp = double.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Level":
                this.Level = int.Parse(dci.groups[1].Value);
                break;

            case "Player.Stats.MaxHits":
                this.MaxHits = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.CurHits":
                this.CurHits = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Hits":
                this.CurHits = int.Parse(dci.groups[1].Value);
                this.MaxHits = int.Parse(dci.groups[2].Value);
                break;
            case "Player.Stats.ArmourClass":
                this.ArmourClass = int.Parse(dci.groups[1].Value);
                this.DamageRes = int.Parse(dci.groups[2].Value);
                break;
            case "Player.Stats.DamageRes":
                this.DamageRes = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Perception":
                this.Perception = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Stealth":
                this.Stealth = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Thievery":
                this.Thievery = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Traps":
                this.Traps = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Picklocks":
                this.Picklocks = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.Tracking":
                this.Tracking = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.MartialArts":
                this.MartialArts = int.Parse(dci.groups[1].Value);
                break;
            case "Player.Stats.MagicRes":
                this.MagicRes = int.Parse(dci.groups[1].Value);
                break;
             default:
                break;

        }
    }

    private string _name;
    private string _firstName;
    private string _lastName;

    internal void UpdateName(string value)
    {
        if(this._name == null)
        {
            this._name = value;
            string[] tokens = this._name.Split(new char[] { ' ' });
            this._firstName = tokens[0];

            if(tokens.Length == 2)
            {
                this._lastName = tokens[1];
            }
        }
        else
        {
            string[] tokens = value.Split(new char[] { ' ' });
            if(this._firstName != tokens[0])
            {
                Log.Warn("Player first name has changed!  From: {0}, To: {0}", this._firstName, tokens[0]);
                this._firstName = tokens[0];
            }

            if (tokens.Length == 2)
            {
                if (this._lastName != tokens[1])
                {
                    Log.Warn("Player last name has changed!  From: {0}, To: {0}", this._firstName, tokens[0]);
                    this._lastName = tokens[1];
                }
            }
        }

    }
}

public enum EnumNpcType
{
    Leader, Solo, Follower, Stationary,
}

public enum EnumNpcAlignment
{ C_EVIL, L_EVIL, NEUTRAL, GOOD, L_GOOD}

public static class ReflectionHelper
{
    public static Object GetPropValue(this Object obj, String propName)
    {
        string[] nameParts = propName.Split('.');
        if (nameParts.Length == 1)
        {
            return obj.GetType().GetProperty(propName).GetValue(obj, null);
        }

        foreach (String part in nameParts)
        {
            if (obj == null) { return null; }

            Type type = obj.GetType();
            PropertyInfo info = type.GetProperty(part);
            if (info == null) { return null; }

            obj = info.GetValue(obj, null);
        }
        return obj;
    }
}
