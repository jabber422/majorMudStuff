namespace MMudObjects
{
    public class StoreItemInfo
    {
        CarryableItem Item {get;set;}
        int Max { get; set; }
        ItemRegenInfo RegenInfo { get; set; }
        int cost { get; set; }
        
    }
}