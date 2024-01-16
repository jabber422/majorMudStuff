using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MMudObjects
{
    public class AlsoHere : List<Entity>
    {
        public bool RoomContainsNPC()
        {
            return this.GetFirst("npc") != null ? true : false;
        }

        public Entity GetFirst(string search)
        {
            switch (search)
            {
                case "first":
                    return this[0];
                case "player":
                    foreach(Entity e in this)
                    {
                        if(e is Player)
                        {
                            return e;
                        }
                    }
                    break;
                case "npc":
                    foreach (Entity e in this)
                    {
                        if (e is Player)
                        {
                            continue;
                        }
                        return e;
                    }
                    break;
                case "baddie":
                    foreach (Entity e in this)
                    {
                        if (e is Player)
                        {
                            continue;
                        }
                        if (e.BaddieFlag) { return e; }
                    }
                    break;
            }
            return null;
        }
    }

    public class Room
    {
        int Map { get; set; }
        int RoomNumber { get; set; }
        String ShortDesc;
        String LongDesc;

        RoomLightEnum LightLevel;

        public List<RoomExit> RoomExits;
        public Dictionary<string, Item> VisibleItems;
        public Dictionary<string, Item> HiddenItems;
        public AlsoHere AlsoHere;

        public long MegaMudRoomHash
        {
            get
            {
                return Convert.ToInt64($"{this.MegaMudNameHash}{this.MegaMudExitsHash}", 16);
            }
        }
        private string MegaMudNameHash { get
            {
                long nValue = 0;
                for (var x = 0; x < this.Name.Length; x++)
                {
                    nValue += (x + 1) * this.Name[x];
                }
                string result = nValue.ToString("X");
                result = result.Length > 3 ? result.Substring(result.Length - 3) : result.PadLeft(3, '0');

                return result;
            }
        }

        
        string[] sExits = { "N", "S", "E", "W", "NE", "NW", "SE", "SW", "U", "D" };
        int[] nExitVal = { 1, 4, 1, 4, 1, 4, 1, 4, 1, 4 };
        int[] nExitPosition = { 5, 5, 4, 4, 3, 3, 2, 2, 1, 1 };
        
        private string MegaMudExitsHash { get {
                int[] nExitsCalculated = new int[6];
                for (int i = 0; i < 10; ++i) {
                    string sExit = sExits[i];
                    bool found = false;
                    bool bDoor = false;
                    foreach(RoomExit ex in this.RoomExits)
                    {
                        if(ex.ShortName == sExit) {
                            bDoor = ex.IsDoor;
                            found = true; break;
                        }
                    }

                    if (found)
                    {
                        nExitsCalculated[nExitPosition[i]] += nExitVal[i] * (bDoor ? 2 : 1);
                    }
                }

                string result = "";
                for (var x = 1; x <= 5; x++)
                {
                    result += nExitsCalculated[x].ToString("X");
                }

                return result;
            }
        }


        public string Name { get; set; }

        public string Description { get; set; }

        Boolean IsSafe { get; set; }
        public string Light { get; set; }
        public string Cause { get; set; }

        public Room()
        {
            this.RoomExits = new List<RoomExit>();
            this.VisibleItems = new Dictionary<string, Item>();
            this.HiddenItems = new Dictionary<string, Item>();
            this.AlsoHere = new AlsoHere();
        }

        public void Add(CarryableItem item)
        {
            if (this.VisibleItems.ContainsKey(item.Name))
            {
                var items_room_has = this.VisibleItems[item.Name];
                items_room_has.Quantity += item.Quantity;
            }
            else
            {
                this.VisibleItems.Add(item.Name, item);
            }
        }

        public void Remove(CarryableItem item)
        {
            if (this.VisibleItems.ContainsKey(item.Name))
            {
                var items_room_has = this.VisibleItems[item.Name];
                items_room_has.Quantity -= item.Quantity;
                if (items_room_has.Quantity <= 0)
                {
                    this.VisibleItems.Remove(items_room_has.Name);
                }
            }
            else
            {
                //should only happen if the inv object hasn't been loaded
            }
        }

        public void Add_Hidden(Item item)
        {
            if (this.HiddenItems.ContainsKey(item.Name))
            {
                var items_room_has = this.HiddenItems[item.Name];
                items_room_has.Quantity += item.Quantity;
            }
            else
            {
                this.HiddenItems.Add(item.Name, item);
            }
        }

        public void Add_Hidden(List<CarryableItem> items)
        {
            foreach(var item in items) { Add_Hidden(item); }
        }

        public void Add(List<CarryableItem> items)
        {
            foreach (var item in items) { Add(item); }
        }

        public void Remove(List<CarryableItem> items)
        {
            foreach (var item in items) { Remove(item); }
        }

        public void BashedDoor(string direction, bool worked)
        {
            if(direction == "Unkown") { return; }
            if(!worked) { return; }

            foreach (RoomExit re in this.RoomExits)
            {
                if(re.ExitEquals(direction))
                {
                    re.OpenDoor(worked);
                }
            }
        }

        public void DoorOpened(string direction, string action)
        {
            if (direction == "Unkown") { return; }
            foreach (RoomExit re in this.RoomExits)
            {
                if (re.ExitEquals(direction))
                {
                    re.OpenDoor(action == "open" ?true:false);
                }
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
