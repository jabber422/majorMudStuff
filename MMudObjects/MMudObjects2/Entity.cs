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
        public NPC()
        {
            this.Abilities = new List<ItemAbility>();
            this.Attacks = new List<MonsterAttackInfo>();
            this.RegenRooms = new List<Room>();
        }
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

        public bool IsResting { get; set; }
        public bool IsMeditating { get; set; }

        public Player()
        {
            this.Stats = new PlayerStats();
            this.Room = new Room();
            this.Inventory = new List<CarryableItem>();
            this.Equipped = new EquippedItemsInfo();
            this.Abilities = new List<ItemAbility>();
            this.QuestAbilities = new List<QuestAbility>();
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
    public event EventHandler<PlayerStats> UpdatedPlayerStats;
    //this is a FirstName and possibly a LastName, space seperated, no special chars
    public string Name { get { return this._name; } set { UpdateName(value); } }
    
    
    public string Lives_CP { get; set; }
    
    public string Race { get; set; }
    public double Exp { get; set; }
    public string Class { get; set; }
    public int Level { get; set; }
    public int Strength { get; set; }
    public int Intellect { get; set; }
    public int Willpower { get; set; }
    public int Agility { get; set; }
    public int Health { get; set; }
    public int Charm { get; set; }
    
    public string Hits
    {
        get { return this.CurHits + "/" + this.MaxHits; }
        set
        {
            string[] tokens = value.Split(new char[] { '/' });
            this.CurHits = int.Parse(tokens[0]);
            this.MaxHits = int.Parse(tokens[1]);
        }
    }

    public void Update(PlayerStats newStats)
    {
        this.Name = newStats.Name;
        this.Lives_CP = newStats.Lives_CP;
        this.Race = newStats.Race;
        this.Class = newStats.Class;
        this.Exp = newStats.Exp;
        this.Level = newStats.Level;
        this.Strength = newStats.Strength;
        this.Intellect = newStats.Intellect;
        this.Willpower = newStats.Willpower;
        this.Agility = newStats.Agility;
        this.Health = newStats.Health;
        this.Charm = newStats.Charm;
        this.Hits = newStats.Hits;
        this.Armour_Class = newStats.Armour_Class;
        this.Perception = newStats.Perception;
        this.Stealth = newStats.Stealth;
        this.Thievery = newStats.Thievery;
        this.Traps = newStats.Traps;
        this.Picklocks = newStats.Picklocks;
        this.Tracking = newStats.Tracking;
        this.Martial_Arts = newStats.Martial_Arts;
        this.MagicRes = newStats.MagicRes;

        OnUpdated();
    }

    public void UpdateHealth(int hits)
    {
        this.CurHits = hits;
        OnUpdated();
    }

    public void UpdateHealth(int hits, int mana)
    {
        this.CurHits = hits;
        this.CurMana = mana;
        OnUpdated();
    }

    private void OnUpdated()
    {
        Log.Tag("PlayerStats", "OnUpdate Fired");
        this.UpdatedPlayerStats(null, this);
    }

    public string Armour_Class
    {
        get { return this.AC + "/" + this.DR; }
        set
        {
            string[] tokens = value.Split(new char[] { '/' });
            this.AC = int.Parse(tokens[0]);
            this.DR = int.Parse(tokens[1]);
        }
    }

    public int Perception { get; set; }
    public int Stealth { get; set; }
    public int Thievery { get; set; }
    public int Traps { get; set; }
    public int Picklocks { get; set; }
    public int Tracking { get; set; }
    public int Martial_Arts { get; set; }
    public int MagicRes { get; set; }


    //non stat block stuff
    public string FirstName { get { return this.Name.Split(new char[] { ' ' })[0]; } }
    public string LastName
    {
        get
        {
            string[] tokens = this.Name.Split(new char[] { ' ' });
            return (tokens.Length == 2) ? tokens[1] : "";
        }
    }

    public int CurHits { get; set; }
    public int MaxHits { get; set; }
    public int CurMana { get; set; }
    public int MaxMana { get; set; }
    public int AC { get; set; }
    public int DR { get; set; }

    public int Lives { get { return int.Parse(this.Lives_CP.Split(new char[] { '/' })[0]); } }
    public int CP { get { return int.Parse(this.Lives_CP.Split(new char[] { '/' })[1]); } }


    public PlayerStats()
    {
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
