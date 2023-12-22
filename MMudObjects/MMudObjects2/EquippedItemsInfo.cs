namespace MMudObjects
{
    public class EquippedItemsInfo : System.IComparable
    {
        public int CompareTo(object obj)
        {
            EquippedItemsInfo info = (EquippedItemsInfo)obj;

            if (Head.CompareTo(info.Head) == 0) return 0;
            if (Ears.CompareTo(info.Ears) == 0) return 0;
            if (Neck.CompareTo(info.Neck) == 0) return 0;
            if (Back.CompareTo(info.Back) == 0) return 0;
            if (Torso.CompareTo(info.Torso) == 0) return 0;
            if (Arms.CompareTo(info.Arms) == 0) return 0;
            if (Wrist.CompareTo(info.Wrist) == 0) return 0;
            if (Hands.CompareTo(info.Hands) == 0) return 0;
            if (Finger1.CompareTo(info.Finger1) == 0) return 0;
            if (Finger2.CompareTo(info.Finger2) == 0) return 0;
            if (Waist.CompareTo(info.Waist) == 0) return 0;
            if (Legs.CompareTo(info.Legs) == 0) return 0;
            if (Feet.CompareTo(info.Feet) == 0) return 0;
            if (Worn.CompareTo(info.Worn) == 0) return 0;
            if (OffHand.CompareTo(info.OffHand) == 0) return 0;
            if (Weapon.CompareTo(info.Weapon) == 0) return 0;

            return 1;
        }

        Armor Head;
        Armor Ears;
        Armor Neck;
        Armor Back;
        Armor Torso;
        Armor Arms;
        Armor Wrist;
        Armor Hands;
        Armor Finger1;
        Armor Finger2;
        Armor Waist;
        Armor Legs;
        Armor Feet;
        Armor Worn;
        Armor OffHand;
        Weapon Weapon;
    }
}