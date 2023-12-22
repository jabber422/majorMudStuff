namespace MMudObjects
{
    public abstract class Spell
    {
        int Id { get; set; }
        string Name { get; set; }
        string ShortName { get; set; }
        EnumMagicType MagicType {get;set;}
        int Level { get; set; }
        int Mana { get; set; }
        int Difficulty { get; set; }

        EnumTargetType TargetType { get; set; }
        EnumAttackType AttackType { get; set; }
        
    }
    public class Buff : Spell
    {
    }

    public enum EnumTargetType
    {
        FULL_AREA_ATTACK, SELF, MONSTER_OR_USER, 
    }

    public enum EnumAttackType
    {
        NORMAL, LIGHTNING, HOT, COLD, WATER
    }
}