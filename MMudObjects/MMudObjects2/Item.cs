using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MMudObjects
{
    public class Item : IComparable
    {
        public Item(Item item)
        {
            this.Name = item.Name;
            this.Quantity = item.Quantity;
            if(this.Quantity == 0)
            {
                this.Quantity = 1;
            }
        }

        public Item(string name) { 
            this.Name = name;
            this.Quantity = 1;
        }

        public string Name { get; set; } 
        public int Id { get; set; }
        public int Limit { get; set; }

        public int Quantity { get; set; }

        public int CompareTo(object obj)
        {
            return this.Name == (obj as Item).Name ? 1 : 0;
        }

        public override string ToString()
        {
            return this.Name;
        }

        //public static List<Item> CreateListFromCsv(string csv)
        //{
        //    List<Item> result = new List<Item>();
        //    string[] tokens = csv.Split(new char[] { ',' });
        //    foreach(string s in tokens)
        //    {
        //        //s = s.Trim();
        //        CarryableItem i = new CarryableItem();
        //        Match m = Regex.Match(s.Trim(), @"^(\d+)? ?(\w+.*)$");
        //        if (m.Success)
        //        {
        //            if (m.Groups[1].Value != "")
        //            {
        //                i.Quantity = int.Parse(m.Groups[1].Value);
        //            }
        //            i.Name = m.Groups[2].Value;
        //            result.Add(i);
        //        }
        //        else
        //        {
        //            throw new Exception("what happened?");
        //        }
        //    }
        //    return result;
        //}
    }

    public class Price
    {
        int runic = 0;
        int platinum = 0;
        int gold = 0;
        int silver =0;
        int copper  = 0;
        private string v;

        int Total { get; }

        public Price()
        {

        }

        public Price(int runic, int platinum, int gold, int silver, int copper)
        {
            this.runic = runic;
            this.platinum = platinum;
            this.gold = gold;
            this.silver = silver;
            this.copper = copper;
        }

        //this string should be the last part of a for sale item list
        public Price(string v)
        {
            this.ParseString(v);
        }

        public void ParseString(string price_string)
        {
            string pattern = @"(runic coins?|platinum pieces?|gold crowns?|silver nobles?|copper farthings?)";
            string pattern2 = @"(\d+) " + pattern;
            //Item                         Quantity    Price
            //------------------------------------------------------

            //glass mirror                  35          10 gold crowns (You can't use)
            //torch                         250          Free
            //scroll of resist cold         25          30 gold crowns(Too powerful)
            string[] tokens = price_string.Split(',');
            foreach (string token in tokens)
            {
                Match m = Regex.Match(token, pattern2);
                if (m.Success)
                {
                    ParseMatch(m);
                }
            }
        }

        public void ParseMatch(Match match)
        {
            if (match.Success)
            {
                string coin_name = match.Groups[2].Value;
                switch (match.Groups[2].Value)
                {
                    case "runic coin":
                    case "runic coins":
                        this.runic = int.Parse(match.Groups[1].Value);
                        break;
                    case "platinum piece":
                    case "platinum pieces":
                        this.platinum = int.Parse(match.Groups[1].Value);
                        break;
                    case "gold crown":
                    case "gold crowns":
                        this.gold = int.Parse(match.Groups[1].Value);
                        break;
                    case "silver noble":
                    case "silver nobles":
                        this.silver = int.Parse(match.Groups[1].Value);
                        break;
                    case "copper farthing":
                    case "copper farthings":
                        this.copper = int.Parse(match.Groups[1].Value);
                        break;
                }
            }
        }

        public double CalcTotalAsCopper()
        {
            double total = 0;
            total += this.runic * 100;
            total += this.platinum * 100;
            total+= this.gold *10;
            total += this.silver * 10;
            total += this.copper;
            return total;
        }

        public List<CarryableItem> ToList()
        {
            List<CarryableItem> result = new List<CarryableItem>();
            if(this.runic > 0)
            {
                CarryableItem coins = new CarryableItem("runic coins");
                coins.Quantity = this.runic;
                result.Add(coins);
            }else if (this.platinum > 0)
            {
                CarryableItem coins = new CarryableItem("platinum pieces");
                coins.Quantity = this.platinum;
                result.Add(coins);
            }
            else if (this.gold > 0)
            {
                CarryableItem coins = new CarryableItem("gold crowns");
                coins.Quantity = this.gold;
                result.Add(coins);
            }
            else if (this.silver > 0)
            {
                CarryableItem coins = new CarryableItem("silver nobles");
                coins.Quantity = this.silver;
                result.Add(coins);
            }
            else if (this.copper > 0)
            {
                CarryableItem coins = new CarryableItem("copper farthings");
                coins.Quantity = this.copper;
                result.Add(coins);
            }
            return result;
        }
    }
    //mainly used for items read from a for sale list
    public class PurchasableItem : Item
    {
        public bool useable = false;
        public bool too_powerful;
        Price price = null;

        public PurchasableItem(string name, int quantity, Price price) : base(name)
        {
            this.Quantity = quantity;
            this.price = price;
        }
    }

    //used for anything that we can't classify, items start here
    public class UnknownItem : Item
    {
        public UnknownItem(string name) : base(name)
        {
        }
    }

    public class CarryableItem : Item
    {
        public CarryableItem(CarryableItem item) : base(item)
        { 
            this.Weight = item.Weight;
        }

        public CarryableItem(string name) : base(name)
        {
            this.Weight = 0;
        }

        public int Weight { get; set; }
        //public int Limit { get; }
        //public int Quantity { get; set; }

    }

     //this we can see, and pickup but not equip
    public class Sundry : CarryableItem
    {
        public Sundry(Sundry item) : base(item)
        {
            this.SundryType = item.SundryType;
        }

        public Sundry(string name) : base(name)
        {
        }

        EnumSundryType SundryType { get; set; }
    }

    public class EquipableItem : CarryableItem
    {
        public EquipableItem(EquipableItem item) : base(item)
        {
            this.Abilities = item.Abilities;
            this.Level = item.Level;
            this.AC = item.AC;
            this.DR = item.DR;
            this.Accuracy = item.Accuracy;
            this.Equiped = item.Equiped;
            this.Location = item.Location;
        }

        public EquipableItem(CarryableItem item) : base(item)
        {
        }

        public EquipableItem(string name) : base(name)
        { }

        List<ItemAbility> Abilities {get; set;}
        int Level { get; set; }
        int AC { get; set; }
        double DR { get; set; }
        int Accuracy { get; set; }

        public bool Equiped { get; set; }
        public string Location { get; set; }
    }

    public class Weapon : EquipableItem
    {
        public Weapon(string name) : base(name)
        {
        }

        EnumWeaponType WeaponType { get; set; }
        int MinDamage { get; set; }
        int MaxDamage { get; set; }
        int Speed { get; set; }
        int Strength { get; set; }
        int BackStab { get; set; }
    }

    public class Armor : EquipableItem
    {
        public Armor(string name) : base(name)
        {
        }

        EnumArmorType ArmorType { get; set; }
        EnumEquipmentSlot EquipmentSlot { get; set; }
       
    }

    public enum EnumArmorType
    {
        NATURAL, SILK, NINJA, LEATHER, CHAINMAIL, SCALEMAIL, PLATEMAIL
    }

    public enum EnumWeaponType
    {
        ONE_HAND_SHARP, TWo_HAND_SHARP, ONE_HAND_BLUNT, TWO_HAND_BLUNT
    }

    public enum EnumEquipmentSlot
    {
        ANYWHERE,
        HEAD, EYES, EARS, FACE,
        NECK,
        BACK, TORSO, ARMS, WORN,
        HANDS, WRIST, FINGER1, FINGER2, WAIST,
        OFF_HAND, MAIN_HAND, BOTH_HANDS,
        LEGS,
        FEET,
    }

    public enum EnumSundryType
    {
        SEIGN, SPECIAL, KEY, SCROLL, DRINK, FOOD, CONTAINER, LIGHT, PROJECTILE, 
    }
}