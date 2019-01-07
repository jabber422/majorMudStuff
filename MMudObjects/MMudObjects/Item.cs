using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MMudObjects
{
    public abstract class Item : IComparable
    {
        string Name { get; set; } 
        int Id { get; set; }
        int Limit { get; set; }

        public int CompareTo(object obj)
        {
            return this.Name == (obj as Item).Name ? 1 : 0;
        }

        public override string ToString()
        {
            return this.Name;
        }

        public static List<Item> CreateListFromCsv(string csv)
        {
            List<Item> result = new List<Item>();
            string[] tokens = csv.Split(new char[] { ',' });
            foreach(string s in tokens)
            {
                //s = s.Trim();
                CarryableItem i = new CarryableItem();
                Match m = Regex.Match(s.Trim(), @"^(\d+)? ?(\w+.*)$");
                if (m.Success)
                {
                    if (m.Groups[1].Value != "")
                    {
                        i.Quantity = int.Parse(m.Groups[1].Value);
                    }
                    i.Name = m.Groups[2].Value;
                    result.Add(i);
                }
                else
                {
                    throw new Exception("what happened?");
                }
            }
            return result;
        }
    }

    //used for anything that we can't classify, items start here
    public class UnknownItem : Item
    {

    }

    public class CarryableItem : Item
    {
        public int Weight { get; set; }
        public int Limit { get; }
        public int Quantity { get; set; }
    }

     //this we can see, and pickup but not equip
    public class Sundry : CarryableItem
    {
        EnumSundryType SundryType { get; set; }
    }

    public class EquipableItem : CarryableItem
    {
        List<ItemAbility> Abilities {get; set;}
        int Level { get; set; }
        int AC { get; set; }
        double DR { get; set; }
        int Accuracy { get; set; }
    }

    public class Weapon : EquipableItem
    {
        
        EnumWeaponType WeaponType { get; set; }
        int MinDamage { get; set; }
        int MaxDamage { get; set; }
        int Speed { get; set; }
        int Strength { get; set; }
        int BackStab { get; set; }
    }

    public class Armor : EquipableItem
    {
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