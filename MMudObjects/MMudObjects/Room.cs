using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMudObjects
{
    public class Room
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

        public Room()
        {
            this.RoomExits = new List<RoomExit>();
            this.VisibleItems = new List<Item>();
            this.HiddenItems = new List<Item>();
            this.AlsoHere = new List<Entity>();
        }

        public void Update(DataChangeItem dci)
        {
            switch (dci.targetProperty)
            {
                case "Player.Room.ObviousExits":
                    string exits = dci.groups[1].Value;
                    string[] tokens = exits.Split(new char[] { ',' });
                    this.RoomExits.Clear();
                    foreach (string t in tokens){
                        this.RoomExits.Add(new RoomExit(t));
                    }
                    break;
                case "Player.Room.ObviousItems":
                    break;
            }
        }

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
