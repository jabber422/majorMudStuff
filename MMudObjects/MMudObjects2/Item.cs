using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MMudObjects
{
    public class Item : IComparable
    {
        public EnumItemType Type { get; set; }
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
        public int Encum { get; set; }
        public int Price { get; set; }
        public int Currency { get; set; }
        public bool Gettable { get; set; }

        public List<ItemAbility> Abilities { get; set; }

        public int Level { get; set; }
        public int AC { get; set; }
        public double DR { get; set; }
        public int Accuracy { get; set; }

        public bool Equiped { get; set; }

        public EnumWeaponType WeaponType { get; set; }
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public int Speed { get; set; }
        public int Strength { get; set; }
        public int BackStab { get; set; }
        public EnumArmorType ArmorType { get; set; }
        public EnumEquipmentSlot EquipmentSlot { get; set; }

        public int CompareTo(object obj)
        {
            return this.Name == (obj as Item).Name ? 1 : 0;
        }

        public override string ToString()
        {
            return this.Name;
        }       
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

        public List<Item> ToList()
        {
            List<Item> result = new List<Item>();
            if(this.runic > 0)
            {
                Item coins = new Item("runic coins");
                coins.Quantity = this.runic;
                result.Add(coins);
            }else if (this.platinum > 0)
            {
                Item coins = new Item("platinum pieces");
                coins.Quantity = this.platinum;
                result.Add(coins);
            }
            else if (this.gold > 0)
            {
                Item coins = new Item("gold crowns");
                coins.Quantity = this.gold;
                result.Add(coins);
            }
            else if (this.silver > 0)
            {
                Item coins = new Item("silver nobles");
                coins.Quantity = this.silver;
                result.Add(coins);
            }
            else if (this.copper > 0)
            {
                Item coins = new Item("copper farthings");
                coins.Quantity = this.copper;
                result.Add(coins);
            }
            return result;
        }
    }
    //mainly used for items read from a for sale list
    //public class PurchasableItem : Item
    //{
    //    public bool useable = false;
    //    public bool too_powerful;
    //    Price price = null;

    //    public PurchasableItem(string name, int quantity, Price price) : base(name)
    //    {
    //        this.Quantity = quantity;
    //        this.price = price;
    //    }
    //}

    ////used for anything that we can't classify, items start here
    //public class UnknownItem : Item
    //{
    //    public UnknownItem(string name) : base(name)
    //    {
    //    }
    //}
  

    // //this we can see, and pickup but not equip
    //public class Sundry : Item
    //{
    //    public Sundry(Sundry item) : base(item)
    //    {
    //        this.SundryType = item.SundryType;
    //    }

    //    public Sundry(string name) : base(name)
    //    {
    //    }

    //    public EnumSundryType SundryType { get; set; }
    //}

   
    //public class Weapon : Item
    //{
    //    public Weapon(Item item) :base(item)
    //    {
            
    //    }


    //}

    //public class Armor : Item
    //{
   
    //    public Armor(Item item) : base(item.Name)
    //    {
            
    //    }

       
       
    //}

    public enum EnumArmorType
    {
        NATURAL, SILK, NINJA, LEATHER, CHAINMAIL, SCALEMAIL, PLATEMAIL
    }

    public enum EnumWeaponType
    {
        ONE_HAND_SHARP, TWO_HAND_SHARP, ONE_HAND_BLUNT, TWO_HAND_BLUNT
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
        SIGN, SPECIAL, KEY, SCROLL, DRINK, FOOD, CONTAINER, LIGHT, PROJECTILE, 
    }

    public enum EnumItemType
    {
        Armor=0,
        Weapon = 1,
        Thrown =2,
        Sign=3,
        Food=4,
        Potion=5,
        Light=6,
        Key=7,
        Chest=8,
        Scroll=9,
        Gem=10,
    }
}