using MMudObjects;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MMudObjects
{
    public class Entity
    {
        public Entity(string name) { 
            
            if(name == "" || name == null || name.Contains("dodge") || (name.Contains("you") ))// && name != "you"))
            {

            }
            name = name.Replace("\r\n", " ").Trim();
            string verb_pattern_entity = "nasty|big|angry|happy|nasty|fierce|small|large|fat|thin|short|tall";
            string noun = $"(?:({verb_pattern_entity}))?" + @"(.*)";
            Match match = Regex.Match(name, noun);
            if (match.Success)
            {
                this.Name = match.Groups[2].Value.Trim();
                string[] tokens= this.Name.Split(',');
                if(this is Player)
                {
                    this.Name = tokens[0];
                }

                this.Verb = match.Groups[1].Value.Trim();
            }
            else
            {
                Debug.WriteLine("Failed to match Entity: " + name);
                this.Name = name;
                this.Verb = "";                
            }
        }
        public virtual string Name { get; protected set; }

        public virtual string FullName { get { return $"{this.Verb} {this.Name}".Trim(); } }
        public virtual string Verb { get; private set; }

        public virtual bool IsCombatEngaged { get; set; }
        public bool BaddieFlag = true;
        public int damage_taken = 0;

        public override bool Equals(object obj)
        {
            if (obj is Entity other)
            {
                Console.WriteLine($"Comparing {this.Name} to {other.Name}");
                return this.FullName == other.FullName; // Example comparison
            }
            return false;
        }
    }

    public class NPC : Entity
    {
        public NPC(string name) : base(name)
        {
            this.Abilities = new List<ItemAbility>();
            this.Attacks = new List<MonsterAttackInfo>();
            this.RegenRooms = new List<Room>();
        }

        public NPC(Entity e) :base(e.FullName)
        {
            this.Abilities = new List<ItemAbility>();
            this.Attacks = new List<MonsterAttackInfo>();
            this.RegenRooms = new List<Room>();
        }

        public string Id { get; set; }
        public int Exp { get; set; }
        public int Regen { get; set; }
        public EnumNpcType Type { get; set; }
        public EnumNpcAlignment Alignment { get; set; }
        public int Health { get; set; }
        public int HealthRegen { get; set; }
        public int AC { get; set; }
        public int DR { get; set; }
        public int MR { get; set; }
        public int FollowPercentage { get; set; }
        public int CharmLevel { get; set; }
        public List<ItemAbility> Abilities { get; set; }
        
        public List<MonsterAttackInfo> Attacks { get; set; }
        
        public List<Room> RegenRooms { get; set; }
        
        public int Magic { get; set; }
    }

    public class Purse
    {
        public int runic = 0;
        public int platinum = 0;
        public int gold = 0;
        public int silver = 0;
        public int copper = 0;
        public int wealth = 0;

        public Purse(List<Item> coins)
        {
            foreach(Item coin in coins)
            {
                switch (coin.Name)
                {
                    case "runic coins":
                        this.runic = coin.Quantity; break;
                    case "platinum pieces":
                        this.platinum = coin.Quantity; break;
                    case "gold crowns":
                        this.gold = coin.Quantity; break;
                    case "silver nobles":
                        this.silver = coin.Quantity; break;
                    case "copper farthings":
                        this.copper = coin.Quantity; break;
                }
            }
        }
    }

    public class Inventory
    {
        private Dictionary<string, Item> items = null;
        public string weight
        {
            get { return this.current_weight + "/" + this.max_weight; }
            set { 
                this.current_weight = int.Parse(value.Split('/')[0]); 
                this.max_weight = int.Parse(value.Split('/')[1]); 
            }
        }
        public int current_weight = 0; 
        public int max_weight = 0;

        public Dictionary<string, Item> Items {  get { return this.items; } }

        public Inventory()
        {
            this.items = new Dictionary<string, Item> ();
            this.weight = "0/0";
        }

        //sets the current inventory, based on the 'inv' command in game
        public void SetInventory(Dictionary<string, Item> items, string weight)
        {
            this.items = items;
            this.weight = weight;
        }

        public Purse GetPurse()
        {
            string[] coin_names = new string[] { "runic coins", "platinum pieces", "gold crons", "silver nobles", "copper farthings" };
            List<Item> coins = new List<Item> ();
            foreach(string coin_name in coin_names)
            {
                Item new_coin = null;
                bool result = this.Items.TryGetValue(coin_name, out new_coin);
                if (result) { coins.Add(new_coin); }
            }

            return new Purse(coins);
        }

        public void Add(List<Item> items)
        {
            foreach(var item in items)
            {
                this.Add(item);
            }
        }

        public void Add(Item item)
        {
            if (this.items.ContainsKey(item.Name))
            {
                var items_player_has = this.items[item.Name];
                items_player_has.Quantity += item.Quantity;
            }
            else
            {
                this.items.Add(item.Name, item);
            }
        }

        public void Remove(List<Item> items)
        {
            foreach (var item in items)
            {
                this.Remove(item);
            }
        }

        public void Remove(Item item)
        {
            if (this.items.ContainsKey(item.Name))
            {
                var items_player_has = this.items[item.Name];
                items_player_has.Quantity -= item.Quantity;
                if(items_player_has.Quantity <= 0)
                {
                    this.items.Remove(items_player_has.Name);
                }
            }
            else
            {
                //should only happen if the inv object hasn't been loaded
            }
        }

        public Item GetItem(string coin_name)
        {
            List<string> item_names = this.items.Keys.ToList();
            if (item_names.Contains(coin_name))
            {
                return this.items[coin_name];
            }
            return null;
        }

        public void RemoveItem(string coin_name)
        {
            List<string> item_names = this.items.Keys.ToList();
            if (item_names.Contains(coin_name))
            {
                this.items.Remove(coin_name);
            }
        }

       
    }

    public class Player : Entity
    {
        public List<MessageResponse> ActiveMessage = new List<MessageResponse>();

        public Room Room { get; set; }

        public virtual double Exp
        {
            get { return this.Stats.Exp; }
            set { this.Stats.Exp = value; }
        }

        public bool IsResting { get; set; }
        public bool IsMeditating { get; set; }

        public Player(string name) : base(name)
        {
            if (this.Name.Trim().EndsWith("*"))
            {
                this.Name = this.Name.Remove(this.Name.Length - 1);
            }

            this.Stats = new PlayerStats();
            
            var tokens = this.Name.Split(' ');
            if(tokens.Length == 2)
            {
                this.Name = tokens[0];
                this.Stats.LastName = tokens[1];
            }

            
            this.Room = new Room();
            this.Inventory = new Inventory();
            //this.Equipped = new EquippedItemsInfo();
            this.Abilities = new List<ItemAbility>();
            this.QuestAbilities = new List<QuestAbility>();
            this.BaddieFlag = false;
            this.Buffs = new Dictionary<string, Spell>();
            

            
        }

        public Inventory Inventory { get; set; }
        //public virtual EquippedItemsInfo Equipped { get; set; }

        public virtual int Rank { get; set; }

        public List<ItemAbility> Abilities { get; set; }

        public List<QuestAbility> QuestAbilities { get; set; }

        public Dictionary<string,Spell> Buffs { get; set; }

        public int HealthRegenNormal { get; set; }
        public int HealthRegenResting { get; set; }
        public int ManaRegenNormal { get; set; }
        public int ManaRegenResting { get; set; }

        
        public virtual string GangName { get; set; }
        public virtual string Title { get; set; }
        public virtual string Alignment { get; set; }
        public virtual string LevelRange { get; set; }

        public virtual string LastName
        {
            get { return this.Stats.LastName; }
        }

        public PlayerStats Stats { get; set; }
        public bool Online { get; set; }
        public double GainedExp { get; set; }
        public string CombatEngagedCause { get; set; }
        public bool IsDiseased { get; set; }
        public bool IsPosioned { get; set; }
        public bool IsConfused { get; set; }
        public bool IsBlind { get; set; }

    }
}

public class PlayerStats
{
    public event EventHandler<PlayerStats> UpdatedPlayerStats;
    //this is a FirstName and possibly a LastName, space seperated, no special chars
    public string Name { get { return this._name; } set { this._name = value; } }


    public string Lives_CP = "";

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
        this.SpellCasting = newStats.SpellCasting;
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

    public int SpellCasting { get; set; }
    public int Perception { get; set; }
    public int Stealth { get; set; }
    public int Thievery { get; set; }
    public int Traps { get; set; }
    public int Picklocks { get; set; }
    public int Tracking { get; set; }
    public int Martial_Arts { get; set; }
    public int MagicRes { get; set; }


    //non stat block stuff
    public string FirstName { get { return this.Name.Split(' ')[0]; } }
    public string LastName
    {
        get;set;
    }

    public int CurHits { get; set; }
    public int MaxHits { get; set; }
    public int CurMana { get; set; }
    public int MaxMana { get; set; }
    public int AC { get; set; }
    public int DR { get; set; }

    public int Lives { get {
            string token = this.Lives_CP.Split(new char[] { '/' })[0];
            int lives = 0;
            int.TryParse(token, out lives);
            return lives;
        } }
    public int CP
    {
        get
        {
            string[] tokens = this.Lives_CP.Split(new char[] { '/' });
            int cp = 0;

            if (tokens.Length != 2) { return cp; }
            int.TryParse(tokens[1], out cp);
            return cp;
        }
    }

    public double NextLevelExp { get; set; }

    public PlayerStats()
    {
    }

    public PlayerStats(Dictionary<string, string> stats)
    {
        this.stats = stats;
        this.fromdict(this.stats);
    }

    private string _name = "";
    private string _firstName;
    private string _lastName;
    private Dictionary<string, string> stats;

    public void fromdict(Dictionary<string, string> stats)
    {
        foreach (KeyValuePair<String, string> kvp in stats)
        {
            string[] vals = null;
            switch (kvp.Key)
            {
                case "Lives/CP":
                    this.Lives_CP = kvp.Value;
                    break;
                case "Exp":
                    this.Exp = double.Parse(kvp.Value);
                    break;
                case "Perception":
                    this.Perception = int.Parse(kvp.Value);
                    break;
                case "Stealth":
                    this.Stealth = int.Parse(kvp.Value);
                    break;
                case "Hits":
                    vals = kvp.Value.Split('/');
                    this.CurHits = int.Parse(vals[0]);
                    this.MaxHits = int.Parse(vals[1]);
                    break;
                case "Current Hits":
                    this.CurHits = int.Parse(kvp.Value);
                    break;
                case "Armour":
                    vals = kvp.Value.Split('/');
                    this.AC = int.Parse(vals[0]);
                    this.DR = int.Parse(vals[1]);
                    break;
                case "Thievery":
                    this.Thievery = int.Parse(kvp.Value);
                    break;
                case "Mana":
                    vals = kvp.Value.Split('/');
                    this.CurMana = int.Parse(vals[0]);
                    this.MaxMana = int.Parse(vals[1]);
                    break;
                case "Current Mana":
                    this.CurMana = int.Parse(kvp.Value);
                    break;
                case "Spellcasting":
                    this.SpellCasting = int.Parse(kvp.Value);
                    break;
                case "Traps":
                    this.Traps = int.Parse(kvp.Value);
                    break;
                case "Picklocks":
                    this.Picklocks = int.Parse(kvp.Value);
                    break;
                case "Strength":
                    this.Strength = int.Parse(kvp.Value);
                    break;
                case "Agility":
                    this.Agility = int.Parse(kvp.Value);
                    break;
                case "Tracking":
                    this.Tracking = int.Parse(kvp.Value);
                    break;
                case "Intellect":
                    this.Intellect = int.Parse(kvp.Value);
                    break;
                case "Health":
                    this.Health = int.Parse(kvp.Value);
                    break;
                case "Arts":
                    this.Martial_Arts = int.Parse(kvp.Value);
                    break;
                case "Willpower":
                    this.Willpower = int.Parse(kvp.Value);
                    break;
                case "Charm":
                    this.Charm = int.Parse(kvp.Value);
                    break;
                case "MagicRes":
                    this.MagicRes = int.Parse(kvp.Value);
                    break;
                case "Level":
                    this.Level = int.Parse(kvp.Value);
                    break;
                //case "Resting":
                //    this.Resting = bool.Parse(kvp.Value);
                    //break;
                case "Name":
                    this.Name = kvp.Value;
                    break;
                case "Race":
                    this.Race = kvp.Value;
                    break;
                case "Class":
                    this.Class =kvp.Value;
                    break;
                default:
                    break;
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
