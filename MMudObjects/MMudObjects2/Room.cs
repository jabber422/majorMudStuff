using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public List<RoomExit> RoomExits;
        public Dictionary<string, Item> VisibleItems;
        public Dictionary<string, Item> HiddenItems;
        public List<Entity> AlsoHere;

        public string Name { get; set; }

        public string Description { get; set; }

        Boolean IsSafe { get; set; }

        public Room()
        {
            this.RoomExits = new List<RoomExit>();
            this.VisibleItems = new Dictionary<string, Item>();
            this.HiddenItems = new Dictionary<string, Item>();
            this.AlsoHere = new List<Entity>();
        }

        public void Update(DataChangeItem dci)
        {
           
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
