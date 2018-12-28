using System.Collections.Generic;

namespace MMudObjects
{
    public abstract class Item
    {
        string Name { get; set; } 
        int Id { get; set; }
        int Limit { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }

    //used for anything that we can't classify, items start here
    public class UnknownItem : Item
    {

    }

    public abstract class CarryableItem : Item
    {
        int Weight { get; set; }
        int Limit { get; }
    }

     //this we can see, and pickup but not equip
    public class Sundry : CarryableItem
    {
        EnumSundryType SundryType { get; set; }
    }

    public abstract class EquipableItem : CarryableItem
    {
        List<ItemAbility> Abilities {get; set;}
        int Level { get; set; }
        int AC { get; set; }
        double DR { get; set; }
        int Accuracy { get; set; }
    }

    public abstract class Weapon : EquipableItem
    {
        
        EnumWeaponType WeaponType { get; set; }
        int MinDamage { get; set; }
        int MaxDamage { get; set; }
        int Speed { get; set; }
        int Strength { get; set; }
        int BackStab { get; set; }
    }

    public abstract class Armor : EquipableItem
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