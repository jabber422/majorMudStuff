using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMudObjects
{
    public abstract class Room
    {
        int Map { get; set; }
        int RoomNumber { get; set; }
        String ShortDesc;
        String LongDesc;

        RoomLightEnum LightLevel;

        List<RoomExit> RoomExits;
        List<Item> VisibleItems;
        List<Item> HiddenItems;
        List<Entity> AlsoHere;

        
        Boolean IsSafe { get; set; }
    }

    public enum RoomLightEnum
    {
        DARK,
        DIM,
        NORMAL,
        BRIGHT,
        BLINDING
    }

    public class Lair : Room
    {

    }

    public class Store: Room
    {
        List<StoreItemInfo> PossibleItems { get; set; }
        List<StoreItemInfo> CurrentItems { get; set; }
    }


}
