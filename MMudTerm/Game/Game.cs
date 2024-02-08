using global::MMudObjects;
using global::MMudTerm.Session;
using MmeDatabaseReader;
using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MMudTerm.Game
{

    //this represents a player and their current game sessions
    public class MajorMudBbsGame
    {
        public const int ROUND_TIME_IN_MS = 4000;
        public const float ROUND_TIME_IN_SEC = 4.0f;
        public Player _player = null;
        public Room _current_room = null;
        public Room _look_room = null;
        public List<Player> _players = new List<Player>();
        public CurrentCombat _current_combat = null;
        public List<string> _gossips = new List<string>();
        public List<string> _auctions = new List<string>();

        public RegexHell _matcher = null;

        public EventType result = EventType.None;
        public object result_data;
        SessionController _controller = null;

        bool monitor_on = false;
        bool monitor_combat = false;
        bool monitor_rest = false;
        bool monitor_buff = false;
        bool monitor_get = false;
        bool monitor_getcoins = false;

        object idle_timer_state = null;
        internal System.Timers.Timer idle_input_timer = null;
        internal System.Timers.Timer round_timer = null;
        private DateTime start_buff_timer;
        private Spell lastSpell;
        private Entity lastSpellCaster;
        private bool monitor_light;

        public delegate void NewGameEventHandler(EventType message);
        public event NewGameEventHandler NewGameEvent;


        public MajorMudBbsGame(SessionController controller)
        {
            this._matcher = new RegexHell();
            this._controller = controller;
            this._player = new Player("You");
            this._current_room = new Room();
            this._players = new List<Player>();
            this._current_combat = new CurrentCombat(this._controller);
            this.idle_input_timer = new System.Timers.Timer(10 * 1000);
            this.idle_input_timer.Elapsed += Idle_input_timer_Elapsed;

            this.round_timer = new System.Timers.Timer(ROUND_TIME_IN_MS);
            this.round_timer.Elapsed += Round_timer_Elapsed;
        }

        private void Round_timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #region monitors and handlers
        public bool Monitor_On
        {
            get { return this.monitor_on; }
            internal set
            {
                if (value)
                {
                    this.NewGameEvent += MajorMudBbsGame_All;
                }
                else
                {
                    this.NewGameEvent -= MajorMudBbsGame_All;
                }
                this.monitor_on = value;
            }
        }

        private void MajorMudBbsGame_All(EventType message)
        {
            switch (message)
            {
                case EventType.RoomSomethingMovedInto:
                //case EventType.RoomSomethingMovedOut:
                case EventType.ExperienceGain:
                    this._controller.SendLine();
                    break;
            }
        }

        public bool Monitor_Combat
        {
            get { return this.monitor_combat; }
            internal set
            {
                if (value)
                {
                    this._player.IsCombatEngaged = false;
                    this.NewGameEvent += NewGameEvent_Combat;
                    AttackFirstNpcInRoom();
                }
                else
                {
                    this.NewGameEvent -= NewGameEvent_Combat;
                }
                this.monitor_combat = value;
            }
        }
        public bool Monitor_Get {
            get { return this.monitor_get; }
            internal set
            {
                if (value)
                {
                    this.NewGameEvent += NewGameEvent_Get;
                }
                else
                {
                    this.NewGameEvent -= NewGameEvent_Get;
                }
                this.monitor_get = value;
            }
        }

        private void NewGameEvent_Get(EventType message)
        {
            throw new NotImplementedException();
        }

        public bool Monitor_GetCoins {
            get { return this.monitor_getcoins; }
            internal set
            {
                if (value)
                {
                    this.NewGameEvent += NewGameEvent_GetCoins;
                    NewGameEvent_GetCoins(EventType.Room);
                }
                else
                {
                    this.NewGameEvent -= NewGameEvent_GetCoins;
                }
                this.monitor_getcoins = value;
            }
        }

        private void GetCoins(Dictionary<string, Item> items)
        {
            foreach (Item t in items.Values)
            {
                if (t.Name.StartsWith("silver noble"))
                {
                    var count = t.Quantity;
                    this._controller.SendLine($"get {count} silver noble");
                }
                else if (t.Name.StartsWith("copper farthing"))
                {
                    var count = t.Quantity;
                    this._controller.SendLine($"get {count} copper farthing");
                }
                else if (t.Name.StartsWith("gold crown"))
                {
                    var count = t.Quantity;
                    this._controller.SendLine($"get {count} gold crown");
                }
                else if (t.Name.StartsWith("platinum piece"))
                {
                    var count = t.Quantity;
                    this._controller.SendLine($"get {count} platinum piece");
                }
                else if (t.Name.StartsWith("runic coin"))
                {
                    var count = t.Quantity;
                    this._controller.SendLine($"get {count} runic coin");
                }
            }
        }

        private void NewGameEvent_GetCoins(EventType message)
        {
            switch (message)
            {
                case EventType.Room:
                    this.GetCoins(this._current_room.VisibleItems);
                    break;
                case EventType.SeeHiddenItem:
                    this.GetCoins(this._current_room.HiddenItems);
                    break;
                case EventType.EntityDeath:
                    break;
            }
        }

        public bool Monitor_Buff {
            get { return this.monitor_buff; }
            internal set
            {
                if (value)
                {
                    this.NewGameEvent += NewGameEvent_Buff;
                }
                else
                {
                    this.NewGameEvent -= NewGameEvent_Buff;
                }
                this.monitor_buff = value;
            }
        }

        private void NewGameEvent_Buff(EventType message)
        {
            switch (message)
            {
                case EventType.BuffSpellAlreadyCastRound:
                    CastBuff();
                    break;
                case EventType.BuffExpired:
                    CastBuff();
                    break;
                case EventType.BuffSpellCastSuccess:
                case EventType.BuffSpellCastFail:
                    CastBuff();
                    if (this.start_buff_timer == null)
                    {
                        this.start_buff_timer = DateTime.Now;
                    }
                    else
                    {
                        TimeSpan ts = DateTime.Now - this.start_buff_timer;
                        Console.WriteLine(ts.TotalSeconds);
                        this.start_buff_timer = DateTime.Now;
                    }
                    break;
                default:
                    CastBuff();break;
                
            }
        }

        private void CastBuff()
        {
            var toMaintain = this._controller.UserConfig_Spells?.BuffsToMaintain();
            Dictionary<string, bool> toUpdate = new Dictionary<string, bool>();
            foreach (string s in toMaintain)
            {
                if (s == "") continue;
                toUpdate.Add(s, true);
            }

            foreach (Spell buff in this._player.Buffs.Values)
            {
                if (toUpdate.ContainsKey(buff.Name))
                {
                    toUpdate[buff.Name] = false;
                }
            }

            foreach (var kvp in toUpdate)
            {
                if (kvp.Value)
                {
                    Spell s = MMudData.GetSpell(new Spell(kvp.Key));
                    this._controller.SendLine(s.ShortName);
                    break;
                }
            }
        }

        public bool Monitor_Rest {
            get { return this.monitor_rest; }
            internal set
            {
                if (value)
                {
                    this.NewGameEvent += NewGameEvent_Health;
                }
                else
                {
                    this.NewGameEvent -= NewGameEvent_Health;
                }
                this.monitor_rest = value;
            }
        }

        public bool Monitor_Light
        {
            get { return this.monitor_light; }
            internal set
            {
                if (value)
                {
                    this.NewGameEvent += NewGameEvent_Light;
                }
                else
                {
                    this.NewGameEvent -= NewGameEvent_Light;
                }
                this.monitor_light = value;
            }
        }

        private void NewGameEvent_Light(EventType message)
        {
            switch (message)
            {
                case EventType.RoomLightToLow:
                    ProvideLight();
                    break;
            }
        }

        private void ProvideLight()
        {
            //do we have any spells?
            //do we have items
            List<Item> torches = MMudData.GetTorches();
            List<Item> torches_found_in_inv = this._player.Inventory.GetAny(torches);
            if(torches_found_in_inv.Count == 0)
            {
                Console.WriteLine("MonitorLight: No torches found in the Players Inv");
            }

            //sort this by cost?
            var sortedItems = torches_found_in_inv.OrderBy(item => item.Price * item.Currency).ToList();
            Item torch = sortedItems[0];
            this._controller.Send($"use {torch.Name}");

        }

        private void NewGameEvent_Health(EventType message)
        {
            switch (message)
            {
                //what do we care about?
                case EventType.Tick:
                    if (this._player.Stats.MaxHits <= 0) return;
                    var player_health = (float)this._player.Stats.CurHits / (float)this._player.Stats.MaxHits;
                    if (player_health < 0.75)
                    {
                        Console.WriteLine($"{this.GetType().Name}: Player should rest");
                        if (this._player.IsResting)
                        {
                            Console.WriteLine($"{this.GetType().Name}: Already resting!");
                            return;
                        }
                        if (this._player.IsCombatEngaged)
                        {
                            Console.WriteLine($"{this.GetType().Name}: Player is in combat and can not rest");
                            return;
                        }
                        if (this._current_room.AlsoHere.RoomContainsNPC())
                        {
                            Console.WriteLine($"{this.GetType().Name}: NPC in room, can't rest");
                            return;
                        }

                        if (this._current_combat.in_combat)
                        {
                            Console.WriteLine($"{this.GetType().Name}: Player is in comat and can not rest2");return;
                        }
                        this._controller.SendLine("rest");
                    }else if(player_health < 0.15)
                    {
                        this._controller.Disconnect();
                    }

                    break;
            }
        }


        private void AttackFirstNpcInRoom()
        {
            if (this._current_room.AlsoHere.RoomContainsNPC())
            {
                if (!this._player.IsCombatEngaged)
                {
                    Entity e = this._current_room.AlsoHere.GetFirst("baddie");
                    if(e is NPC)
                    {
                        if((e as NPC).Alignment == EnumNpcAlignment.L_GOOD)
                        {
                            Console.WriteLine($"{this.GetType().Name}: Won't Attack - {e.Name} is {(e as NPC).Alignment}");
                            return;
                        }
                    }
                    if (e != null)
                    {
                        this._controller.Send($"{this._current_combat.AttackString} {e.FullName}\r\n");
                    }
                }
            }
        }

        public void NewGameEvent_Combat(EventType message)
        {
            switch (message)
            {
                case EventType.Room:
                    AttackFirstNpcInRoom();
                    //Console.WriteLine("Combat - attacking " + message);
                    break;
                case EventType.CombatEngagedStart:
                    //this._player.IsCombatEngaged = true;
                    break;
                case EventType.CombatEngagedStop:  //combat off, we attack anything in the room again
                case EventType.CombatMiss: //something might have swung at us and missed?
                    //this._controller.SendLine("get sil");
                    //this._controller.SendLine("get cop");
                    if (!this._player.IsCombatEngaged)
                    {
                        //this.NewGameEvent_Combat(EventType.Room);
                    }
                    break;
                
                default:
                    //Console.WriteLine("Combat - ignored token " + message);
                    break;
            }
        }

        #endregion
        
        private void Idle_input_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this._controller.SendLine();
        }

        public void Process(string data)
        {
            //Console.WriteLine("Processing: " + data.Trim());
            Match match = null;
            List<(EventType, Object)> e = new List<(EventType, Object)> { (EventType.None, null) };
            this.result = EventType.None;
            this.result_data = null;

            if (data == "") { return; }

            while (data.Contains("\b"))
            {
                int backspaceIndex = data.IndexOf('\b');
                if (backspaceIndex > 0)
                {
                    data = data.Remove(backspaceIndex - 1, 2);
                }
                else
                {
                    // If backspace is the first character, just remove it
                    data = data.Remove(backspaceIndex, 1);
                }
            }

            Console.WriteLine("------------------------------------");
            Console.WriteLine("Data: " + data.Trim() );
            if (this._matcher.TryMatch(data, out match, out e))
            {
                //Console.WriteLine($"--> {e} <--");
                for (int i = 0; i < match.Groups.Count; ++i)
                {
                    //Console.WriteLine($"\tMatch: {match.Groups[i].Value}");
                }
                callback(e, match, data);
            }
            else
            {
                //need to check the messages database
                
                result = EventType.None;
                var b4 = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Not Matched");
                Console.ForegroundColor = b4;
                
            }
            Console.WriteLine("------------------------------------\r\n");
        }

        private void callback(List<(EventType, Object)> e, Match match, string data)
        {
            switch (e[0].Item1)
            {
                case EventType.Room:
                    ProcessRoom(match, data);
                    break;
                case EventType.Stats:
                    ProcessStat(match, data);
                    break;
                case EventType.Inventory:
                    ProcessInventory(match, data);
                    break;
                case EventType.CombatEngaged:
                case EventType.CombatEngagedStart:
                case EventType.CombatEngagedStop:
                    ProcessCombat(match, data);
                    break;
                case EventType.Rest:
                    ProcessResting(match, data);
                    break;
                case EventType.EquippedArmor:
                case EventType.EquippedWeapon:
                    ProcessEquippedSomething(match, data);
                    break;
                case EventType.Top:
                    ProcessTopList(match, data);
                    break;
                case EventType.Who:
                    ProcessWhoList(match, data);
                    break;
                case EventType.BoughtSomething:
                    ProcessBoughtSomething(match, data);
                    break;
                case EventType.SoldSomething:
                    ProcessSoldSomething(match, data);
                    break;
                case EventType.BadRoomMove:
                    ProcessBadRoomMove(match, data);
                    break;
                case EventType.SearchNotice:
                    ProcessSearch(match, data);
                    break;
                case EventType.SneakAttempt:
                    ProcessSneak(match, data);
                    break;
                case EventType.ForSaleList:
                    ProcessForSale(match, data);
                    break;
                
                case EventType.PickUpItem:
                    ProcessPickup(match, data);
                    break;
                case EventType.DropItem:
                    ProcessDropItem(match, data);
                    break;
                case EventType.HidItem:
                    ProcessHidSomething(match, data);
                    break;
                
                case EventType.ExperienceGain:
                    ProcessXpGain(match, data);
                    break;
                case EventType.ExperienceUpdate:
                    ProcessXpUpdate(match, data);
                    break;
                case EventType.CombatEngaged_3rdP:
                    ProcessCombat3rdP(match, data);
                    break;
                case EventType.CombatHit:
                    ProcessCombatDoHit(match, data);
                    break;
                case EventType.CombatMiss:
                    ProcessCombatDoMiss(match, data);
                    break;
                case EventType.SomeoneLeftTheGame:
                    ProcessHungUp(match, data);
                    break;
                case EventType.SomeoneEnteredTheGame:
                    ProcessEnteredRealm(match, data);
                    break;
                case EventType.RoomSomethingMovedInto:
                    ProcessMovedIntoRoom(match, data);
                    break;
                case EventType.RoomSomethingMovedOut:
                    ProcessMovedOutOfRoom(match, data);
                    break;
                case EventType.EntityDeath:
                    ProcessEntityDeath(match, data);
                    break;
                case EventType.BuffSpellCastSuccess_3rdP:
                    ProcessBuffSpellCastSuccess(match, data);
                    break;
                case EventType.BuffSpellCastFail_3rdP:
                    ProcessBuffSpellCastFailure(match, data);
                    break;
                case EventType.BuffExpired:
                    ProcessBuffSpellWearOff(match, data);
                    break;
                case EventType.HearMovement:
                    ProcessHearMovement(match, data);
                    break;
                case EventType.BashDoorFailure:
                case EventType.BashDoorSuccess:
                    ProcessBashDoor(match, data);
                    break;
                case EventType.DoorStateChange:
                case EventType.DoorLocked:
                    ProcessDoorCloseLocked(match, data);
                    break;
                case EventType.Gossip:
                    ProcessGossip(match, data);
                    break;
                case EventType.MessagesThatMakeUsPauseWhileWalking:
                case EventType.BuffSpellAlreadyCastRound:
                    result = e[0].Item1;
                    break;
                case EventType.SeeHiddenItem:
                    ProcessHiddenItems(match, data);
                    break;
                case EventType.CombatEngagedEvilWarning:
                    ProcessNotABaddie();
                    break;
                case EventType.MessageResponse:
                    ProcessMessageResponse(match, data, e);
                    break; 
                case EventType.MessageResponseBuffStart:
                    ProcessMessageResponseBuffStart(match, data, e);
                    break;
                case EventType.MessageResponseBuffEnd:
                    ProcessMessageResponseBuffEnd(match, data, e);
                    break;
                case EventType.RoomLightToLow:
                    ProcessRoomLightToLow(match, data, e);
                    break;
                default:
                    Console.WriteLine("fix: " + e[0].Item1);
                    break;
            }
        }

        private void ProcessRoomLightToLow(Match match, string data, List<(EventType, object)> e)
        {
            this.result = EventType.RoomLightToLow;
        }

        private void ProcessMessageResponse(Match match, string data, object item2)
        {
            //MessageResponse msg = (item2 as MessageResponse);
            //bool end = false;
            //if (match.ToString() == msg.EndsWith) {
            //    end = true;
            //}

            //if(match.Groups.Count == 3)
            //{
            //    var target = match.Groups[1].Value;
            //    //var dmg = match.Groups[2].Value;
            //    //if (target == "You")
            //    //{
            //    //    this._current_combat.PlayerHit(e, dmg, false);
            //    //}
            //    //else if (target == "you")
            //    //{
            //    //    this._current_combat.PlayerHitBy(e, dmg, false);
            //    //}
            //}

            //if(match.Groups.Count == 2)
            //{
            //    var dmg = match.Groups[1].Value;
            //}





            
            
            
        }

        private void ProcessNotABaddie()
        {
            Entity e = this._current_room.AlsoHere.GetFirst("baddie");
            if (e != null)
            {
                e.BaddieFlag = false;
            }
        }

        private Entity CreateConcreteEntity(Entity entity)
        {
            foreach (Player p in this._players)
            {
                if (p.Name == entity.Name)
                    return p;
            }
            //not a player, look in mme db
            entity = MMudData.GetNpc(entity);
            return entity;
        }

        private Entity CreateConcreteEntity(string entity)
        {
            return this.CreateConcreteEntity(new Entity(entity));
        }

        private Item CreateConcreteItem(string itemname)
        {
            Item i = null;
            string pattern = @"^(\d+) ([\S ]+)";
            Match m4 = Regex.Match(itemname, pattern);
            if (m4.Success)
            {
                Match coin_match = Regex.Match(m4.Groups[2].Value.Trim(), @"(runic coins?|platinum pieces?|gold crowns?|silver nobles?|copper farthings?)");
                if (coin_match.Success)
                {
                    i = new Coin(coin_match.Groups[1].Value);
                }
                else
                {
                    i = new Item(m4.Groups[2].Value.Trim());
                    i = MMudData.GetItem(i);
                }
                i.Quantity = int.Parse(m4.Groups[1].Value);

            }
            else
            {
                i = new Item(itemname);
                i = MMudData.GetItem(i);
            }

            return i;
        }

        private void ProcessDoorCloseLocked(Match match, string arg2)
        {
            if (match.Success)
            {
                string direction = match.Groups[1].Value;
                this._current_room.DoorOpened(direction, "closed");
                this.result = EventType.DoorClosed;
            }
        }

        private void ProcessOpenDoor(Match match, string arg2)
        {
            if (match.Success)
            {
                string[] tokens = arg2.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                string direction = "Unknown";
                string msg = "";

                if (tokens.Length == 2)
                {
                    direction = tokens[0].Split(' ')[1];
                }

                string action = match.Groups[1].Value;
                this._current_room.DoorOpened(direction, action);
                if (action == "open")
                {
                    this.result = EventType.DoorOpen;
                }
                else
                {
                    this.result = EventType.DoorClosed;
                }
            }
        }

        private void ProcessBashDoor(Match match, string arg2)
        {
            if (match.Success)
            {
                string[] tokens = arg2.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                string direction = "Unkown";
                string msg = "";

                if (tokens.Length == 2)
                {
                    direction = tokens[0].Split(' ')[1];
                    msg = tokens[1];
                }
                else
                {
                    msg = tokens[0];
                }

                bool worked = !msg.Contains(" fail!");
                this._current_room.BashedDoor(direction, worked);
                if (worked)
                {
                    this.result = EventType.BashDoorSuccess;
                }
                else
                {
                    this.result = EventType.BashDoorFailure;
                }               
            }
        }

        private void ProcessHearMovement(Match match, string arg2)
        {
            if (match.Success)
            {
                string direction = match.Groups[1].Value;
                this.result = EventType.HearMovement;
            }
        }

        #region Buff/Debuff handling
        private void ProcessBuffSpellCastFailure(Match match, string arg2)
        {
            if(match.Success)
            {
                string caster = match.Groups[1].Value;
                string spell = match.Groups[2].Value;
                if (caster == "You")
                {
                    this.result = EventType.BuffSpellCastFail;
                }
                else
                {
                    this.result = EventType.BuffSpellCastFail_3rdP;
                }
            }
        }


        private void ProcessBuffSpellWearOff(Match match, string arg2)
        {
            if (match.Success)
            {
                Spell spell = MMudData.GetSpell(new Spell(match.Groups[1].Value));
                this._controller._gameenv._player.Buffs.Remove(spell.Name);
                this.result = EventType.BuffExpired;
            }
        }

        private void ProcessBuffSpellCastSuccess(Match match, string arg2)
        {
            if (match.Success)
            {
                Entity caster = this.CreateConcreteEntity(match.Groups[1].Value);
                Spell spell = MMudData.GetSpell(new Spell(match.Groups[2].Value));

                Entity target = null;
                if (match.Groups[3].Value != "")
                {
                    target = this.CreateConcreteEntity(match.Groups[3].Value);
                }
                else if(spell.TargetType == EnumTargetType.Self || spell.TargetType == EnumTargetType.SingleOrSelf) {
                    target = caster;
                }

                if (caster.Name == this._player.Name &&(
                    target.Name == this._player.Stats.Name || target.Name.ToLower() == this._player.Name.ToLower()))
                {
                    if (this._player.Buffs.ContainsKey(spell.Name))
                    {
                        this._player.Buffs[spell.Name] = spell;
                    }
                    else
                    {
                        this._player.Buffs.Add(spell.Name, spell);
                    }

                    this.result = EventType.BuffSpellCastSuccess;
                }
                else {
                    this.result = EventType.BuffSpellCastSuccess_3rdP;
                }
                this.lastSpell = spell;
                this.lastSpellCaster = caster;
            }
        }

        private void ProcessMessageResponseBuffEnd(Match match, string data, object item2)
        {
            var lst = (item2 as List<(EventType, Object)>);

            MessageResponse msg = null;
            if (lst.Count == 1)
            {
                msg = (lst[0].Item2 as MessageResponse);
            }
            else
            {
                //can be n possible spells, x-ref vs buffs
                foreach(var possible_buff in lst)
                {
                    msg = (possible_buff.Item2 as MessageResponse);
                    Spell buff2 = MMudData.GetSpell(new Spell(msg.Name));
                    if (this._player.Buffs.ContainsKey(buff2.Name))
                    {
                        break; //found it
                    }
                }
            }

            Spell buff = MMudData.GetSpell(new Spell(msg.Name));
            try
            {
                this._player.Buffs.Remove(buff.Name);
            }
            catch (Exception e)
            {

            }
            this.result = EventType.BuffExpired;
        }

        private void ProcessMessageResponseBuffStart(Match match, string data, object item2)
        {
            //Hopefully we don't need this handlee, just the ends strings
            //MessageResponse msg = (item2 as MessageResponse);

            //Spell buff = MMudData.GetSpell(new Spell(msg.Name));
            //try
            //{
            //    if (!this._player.Buffs.ContainsKey(buff.Name))
            //    {
            //        this._player.Buffs.Add(buff.Name, buff);
            //    }
            //}
            //catch (Exception e)
            //{

            //}
            this.result = EventType.BuffSpellCastSuccess;
        }
        #endregion


        private void ProcessMovedOutOfRoom(Match match, string arg2)
        {
            if (match.Success)
            {
                Entity e = this.CreateConcreteEntity(match.Groups[1].Value);
                this._current_room.AlsoHere.Remove(e);
                this.result = EventType.RoomSomethingMovedOut;
            }
        }

        private void ProcessMovesToAttack(Match match, string arg2)
        {
            if (match.Success)
            {
                string attacker_name = match.Groups[1].Value;
                string target_name = match.Groups[2].Value;
                if(target_name == "You")
                {
                    this.result = EventType.PVP_ATTACKED;
                }
                else
                {
                    this.result = EventType.CombatEngagedStart_3rd;
                }
            }
        }

        //this won't work, since there is new verb.
        //there could be two thugs, and we remove the wrong one using entity.name
        private void ProcessEntityDeath(Match match, string arg2)
        {
            if (match.Success)
            {
                Entity e = this.CreateConcreteEntity(match.Groups[1].Value);
                var x = this._controller._gameenv._current_room.AlsoHere.Remove(e);
                if (!x)
                {

                }
                this.result = EventType.EntityDeath;
            }
            else
            {

            }
        }

        private void ProcessMovedIntoRoom(Match match, string arg2)
        {
            //string pattern_movement = "walks|crawls|scuttles|creeps";
            //string pattern_moves = "(?:" + pattern_movement + ")";
            //string moved_into_room = @"A(?:n)? ([A-Za-z ]+) " + pattern_moves + @" into the room from (?:the )?(\S+)\.";
            
            //Match m = Regex.Match(arg2, moved_into_room);

            if (match.Success)
            {
                Entity e = this.CreateConcreteEntity(match.Groups[1].Value);
                this._current_room.AlsoHere.Add(e);
                this.result = EventType.RoomSomethingMovedInto;
            }
            
        }

        private void ProcessEnteredRealm(Match match, string arg2)
        {
            string player_first_name = match.Groups[1].Value;

            foreach (Player p in this._players)
            {
                if (p.Name == player_first_name)
                {
                    p.Online = true;
                    this.result = EventType.SomeoneEnteredTheGame;
                    return;
                }
            }

            Player new_player = new Player(player_first_name);
            new_player.Online = true;
            this._players.Add(new_player);
            this.result = EventType.SomeoneEnteredTheGame;
        }

        private void ProcessHungUp(Match match, string arg2)
        {
            string player_first_name = match.Groups[1].Value;

            foreach (Player p in this._players)
            {
                if (p.Name == player_first_name)
                {
                    p.Online = false;
                    this.result = EventType.SomeoneLeftTheGame;
                    return;
                }
            }
            Player new_player = new Player(player_first_name);
            new_player.Online = false;
            this._players.Add(new_player);
            this.result = EventType.SomeoneLeftTheGame;
        }

        #region Combat handling
        private void ProcessCombatDoHit(Match match, string arg2)
        {
            //TODO: if 1 is set we had a crit

            //You clobber giant rat for 12 damage!
            var attacker = match.Groups[1].Value.Trim();
            var crit = match.Groups[2].Value.Trim();
            var target = match.Groups[3].Value.Trim();
            int dmg_done = int.Parse(match.Groups[4].Value);

            if (attacker == "You")
            {
                Entity e = AddAttackerToRoom(target);
                this._current_combat.PlayerHit(e, dmg_done, crit);
            }else if(target == "you")
            {
                Entity e =AddAttackerToRoom(attacker);
                this._current_combat.PlayerHitBy(e, dmg_done, crit);
            }
            else
            {
                //3rd party attacking something, do we care?
            }
            this.result = EventType.CombatHit;
        }

        private Entity AddAttackerToRoom(string target)
        {
            Entity target_entity = this.CreateConcreteEntity(target);
            foreach(Entity e in this._current_room.AlsoHere)
            {
                if (e.FullName == target_entity.FullName) return target_entity;
            }
            
            this._current_room.AlsoHere.Add(target_entity);
            return target_entity;
        }

        private void ProcessCombatDoMiss(Match match, string arg2)
        {
            var attacker = match.Groups[1].Value.Trim();
            var target = match.Groups[2].Value.Trim();
            //You swipe at kobold thief!

            if (attacker == "You")
            {
                Entity e = AddAttackerToRoom(target);
                this._current_combat.PlayerMissed(e);
            }
            else if(target == "you")
            {
                Entity e = AddAttackerToRoom(attacker);
                var is_dodge = arg2.Contains(", but you dodge");
                this._current_combat.PlayerMissedBy(e, is_dodge);
            }
            else
            {
                //3rd party attacking something, do we care?
            }
            this.result = EventType.CombatMiss;
        }
        #endregion


        private void ProcessResting(Match match, string arg2)
        {
            this._player.IsResting = true;
            this.result = EventType.Rest;
        }


        private void ProcessXpUpdate(Match match, string arg2)
        {
            if (match.Success)
            {
                double exp = double.Parse(match.Groups[1].Value);
                int level = int.Parse(match.Groups[2].Value);
                double left = double.Parse(match.Groups[3].Value);
                double needed = double.Parse(match.Groups[4].Value);
                double perc = double.Parse(match.Groups[5].Value);

                this._player.Stats.Exp = exp;
                this._player.Stats.Level = level;
                this._player.Stats.NextLevelExp = needed;

                this.result = EventType.StatsExperienceUpdate;
            }
        }

        private void ProcessXpGain(Match match, string arg2)
        {
            if (match.Success)
            {
                double gained_exp = double.Parse(match.Groups[1].Value);
                this._player.Stats.Exp += gained_exp;
                this._player.GainedExp += gained_exp;
                this.result = EventType.ExperienceGain;
            }
        }

        private void ProcessDropItem(Match match, string s)
        {
            if (match.Success)
            {
                //picked up item
                Item item = this.CreateConcreteItem(match.Groups[1].Value + " " + match.Groups[2].Value);

                this._player.Inventory.Remove(item);
                this._current_room.Add(item);
                this.result = EventType.DropItem;
            }
        }

        private void ProcessHidSomething(Match match, string arg2)
        {
            if (match.Success)
            {
                //picked up item
                Item item = this.CreateConcreteItem(match.Groups[1].Value + " " + match.Groups[2].Value);
                
                this._player.Inventory.Remove(item);
                this._current_room.Add_Hidden(item);
                
                this.result = EventType.DropItem;
                if (item is Coin)
                {
                    this.result = EventType.HidCoins;
                }
            }
        }

        private void ProcessPickup(Match match, string arg2)
        {
            string pattern1 = @"You picked up (\d+) ([\S ]+)\r\n";
            Match m1 = Regex.Match(arg2, pattern1, RegexOptions.Compiled);
            //this can only be coins, i dont think anything else can be picked up in multiples
            if (m1.Success)
            {
                string coin_name = m1.Groups[2].Value;
                Price price = new Price(coin_name);
                price.ParseMatch(m1);
                List<Item> coins = price.ToList();
                this._player.Inventory.Add(coins);
                this._current_room.Remove(coins);
                this.result = EventType.PickUpCoins;
                return;
            }

            string pattern2 = @"You (?:picked up|took) ([ \S]+)\.\r\n";
            Match m2 = Regex.Match(arg2, pattern2, RegexOptions.Compiled);
            if (m2.Success)
            {
                Item item = this.CreateConcreteItem(m2.Groups[1].Value);
                this._player.Inventory.Add(item);
                this._current_room.Remove(item);
                this.result = EventType.PickUpItem;
            }
        }
        
        private void ProcessEquippedSomething(Match match, string data)
        {
            if (match.Success)
            {
                Item i = new Item(match.Groups[1].Value);
                i = MMudData.GetItem(i);
                i.Equiped = true;
                this.result = EventType.EquippedArmor;
                if (data.Contains(" holding "))
                {
                    this.result = EventType.EquippedWeapon;
                }
            }
        }

        private void ProcessGossip(Match match, string data)
        {
            if (match.Success)
            {
                var player_name = match.Groups[1].Value;
                var type = match.Groups[2].Value;
                var msg = match.Groups[3].Value;
                if(type == "gossips")
                {
                    this._gossips.Add($"{player_name}: {msg}");
                }else if(type == "auctions")
                {
                    this._auctions.Add($"{player_name}: {msg}");
                }

                this.result = EventType.Gossip;
            }
        }

        public void ProcessTick(Match match, string s)
        {
            Dictionary<string, string> stats = new Dictionary<string, string>();
            this._player.Stats.CurHits = int.Parse(match.Groups[1].Value);
            bool found_rest = false;
            if (match.Groups[2].Success)
            {
                if (match.Groups[2].Value == "Resting")
                {
                    found_rest = true;
                }
                else
                {
                    this._player.Stats.CurMana = int.Parse(match.Groups[2].Value);
                }
            }

            if (match.Groups[3].Success)
            {
                found_rest = true;
            }

            this._player.IsResting = found_rest;
            if (found_rest)
            {
                this._player.IsCombatEngaged = false;
            }

            this.result = EventType.Tick;
        }

        private void ProcessForSale(Match match, string arg2)
        {
            string[] tokens = arg2.Split(new string[] { "for sale here:" }, StringSplitOptions.RemoveEmptyEntries);
            tokens = tokens[1].Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string line = tokens[0];
            int quantity_idx = 0, price_idx = 0;
            quantity_idx = line.IndexOf("Quantity");
            price_idx = line.IndexOf("Price");
            for (int i = 2; i < tokens.Length; ++i)
            {
                string name = "";
                int quantity = 0;
                Price cost = null;
                line = tokens[i];
                name = line.Substring(0, quantity_idx).Trim();
                quantity = int.Parse(tokens[i].Substring(quantity_idx, 5).Trim());
                cost = new Price(tokens[i].Substring(price_idx - 4).Trim());
                //PurchasableItem shop_item = new PurchasableItem(name, quantity, cost);
                //shop_item.useable = line.Contains("(You can't use)") ? false : true;
                //shop_item.too_powerful = line.Contains("(Too powerful)") ? true : false;
            }

            this.result = EventType.ShopList;
        }

        private void ProcessSneak(Match match, string arg2)
        {
            throw new NotImplementedException();
        }

        private void ProcessSearch(Match match, string arg2)

        {
            //throw new NotImplementedException();
            //Your search revealed nothing. 
            //You notice <something> here.
            string[] hidden_items = match.Groups[1].Value.Trim().Split(',');
            foreach (string s in hidden_items)
            {
                Item i = new Item(s.Trim());
                this._current_room.Add_Hidden(i);
            }
            this.result = EventType.SearchFound;
        }

        

        private void ProcessBadRoomMove(Match match, string arg2)
        {
            //throw new NotImplementedException();
        }

        private void ProcessSoldSomething(Match match, string arg2)
        {
            Item item_bought = null;
            List<Item> cost_as_items = null;
            if (match.Success)
            {
                item_bought = new Item(match.Groups[1].Value);
                cost_as_items = new Price(match.Groups[2].Value.Trim()).ToList();
            }
            this._player.Inventory.Remove(item_bought);
            this._player.Inventory.Add(cost_as_items);
            this.result = EventType.SoldSomething;
        }

        private void ProcessBoughtSomething(Match match, string arg2)
        {
            Item item_bought = null;
            List<Item> cost_as_items = null;

            if (match.Success)
            {
                item_bought = new Item(match.Groups[1].Value);
                cost_as_items = new Price(match.Groups[2].Value.Trim()).ToList();
            }
            this._player.Inventory.Add(item_bought);
            this._player.Inventory.Remove(cost_as_items);
            this.result = EventType.BoughtSomething;
        }

        private void ProcessStat(Match match, string s)
        {
            string[] char_creation_work_around = s.Split(new string[] { "SAVESAVE" }, StringSplitOptions.None);
            if (char_creation_work_around.Length == 2) {
                s = char_creation_work_around[1];
            }
            //This regex matches everything... except level... wtf
            string pattern_stat = @"(\S+)(?: Class)?:(?:[ \t]+|\*?)(\S+)(?: \S+)?";
            Regex r = new Regex(pattern_stat);
            MatchCollection mc = r.Matches(s);

            Dictionary<string, string> stats = new Dictionary<string, string>();
            foreach (Match m in mc)
            {
                if (m.Success)
                {
                    try
                    {
                        stats.Add(m.Groups[1].Value, m.Groups[2].Value);
                    } catch (Exception ex)
                    {

                    }
                }
            }

            //the : in Level:
            if (!stats.ContainsKey("Level")) { 
            var start_idx = s.IndexOf("Level:") + "Level:".Length;
            var level_string = s.Substring(s.IndexOf("Level") + "Level".Length + 1, s.IndexOf("Stealth") - start_idx).Trim();
            stats.Add("Level", level_string);
            }

            this._player.Stats = new PlayerStats(stats);
            this.result = EventType.Stats;
        }

        private void ProcessHiddenItems(Match match, string s)
        {
            if (match.Success)
            {
                foreach (var item in ProcessItemsSeen(match.Groups[1].Value).Values){
                    this._current_room.Add_Hidden(item);
                }
                this.result = EventType.SeeHiddenItem;
            }
        }

        //handles the block of data that makes up a typical room frame
        //TODO: under load i've seen the room block split across Process calls,
        //  this decoder needs to be split up to handle the sections standalone
        private void ProcessRoom(Match m, string s)
        {
            Dictionary<string, string> room_info = new Dictionary<string, string>();
            string[] tokens = s.Split(new string[] {"Obvious exits:" }, StringSplitOptions.RemoveEmptyEntries);
            room_info["exits"] = tokens[1].Trim();
            int idx2 = room_info["exits"].IndexOf("The room is dimly lit");
            if (idx2 > 0)
            {
                room_info["light"] = "dim";
                room_info["exits"] = room_info["exits"].Remove(idx2).Trim();
            }
            else
            {
                room_info["light"] = "normal";
            }
            
            int idx = room_info["exits"].IndexOf('\b');
            while (idx > 0)
            {
                string before = room_info["exits"].Substring(0, idx - 1);
                string after = room_info["exits"].Substring(idx + 1);
                room_info["exits"] = before + after;
                idx = room_info["exits"].IndexOf('\b');
            }

            tokens = tokens[0].Split(new string[] { "Also here:" }, StringSplitOptions.RemoveEmptyEntries);
            if(tokens.Length > 1)
            {
                room_info["here"] = tokens[1].Trim().Trim('.');
            }

            
            tokens = tokens[0].Split(new string[] { "You notice " }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length > 1)
            {
                room_info["items"] = tokens[1].Trim();
            }

            tokens = tokens[0].Split(new string[] { "    " }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length > 1)
            {
                room_info["desc"] = tokens[1].Trim();
            }

            var tokenss = tokens[0].Split(new string[] { "\r\n" }, StringSplitOptions.None);
            room_info["name"] = tokenss[tokenss.Length -2].Trim();

            if (tokenss.Length > 2)
            {
                room_info["cause"] = FigureOutCause_Room(tokenss[tokenss.Length - 3].Trim());
            }
            else
            {
                room_info["cause"] = "crlf";
            }

            
            Room room = new Room();
            room.Name = room_info["name"];
            room.Light = room_info["light"];
            room.Cause = room_info["cause"];

            
            if (room_info.ContainsKey("desc"))
            {
                room.Description = room_info["desc"];
            }

            if (room_info.ContainsKey("here") && room_info["here"] != "")
            {
                AlsoHere entities = new AlsoHere();
                var fullNameCounts = new Dictionary<string, int>();
                if (room.Cause == "crlf")
                {
                    //if we refreshed the current room, don't add all new 'alsohere', only add the delta.
                    //this way the hp's of the 'also here' targets don't get reset to default
                    entities = this._current_room.AlsoHere;
                    //count what is in the current room, us FullName as the key, this is how we handle
                    //two 'angry orc' Entities in the same room
                    foreach (var e in entities)
                    {
                        if (fullNameCounts.ContainsKey(e.FullName))
                        {
                            fullNameCounts[e.FullName]++;
                        }
                        else
                        {
                            fullNameCounts.Add(e.FullName, 1);
                        }
                    }
                }

                List<Entity> newEntities = new List<Entity>();
                //add the new Also here entites to the new AlsoHere collection as needed
                foreach (string also_here in room_info["here"].Split(','))
                {
                    Entity e = this.CreateConcreteEntity(also_here.Trim());
                    newEntities.Add(e);
                    if (fullNameCounts.TryGetValue(e.FullName, out int count))
                    {
                        int currentCountInAlsoHere = entities.Count(n => n.FullName == e.FullName);
                        if (currentCountInAlsoHere < count)
                        {
                            entities.Add(e);
                            fullNameCounts[e.FullName]++;
                        }
                    }
                    else
                    {
                        //Name wasn't found add the new Entity to the Alsohere
                        entities.Add(e);
                        fullNameCounts.Add(e.FullName, 1);
                    }
                }
                room.AlsoHere = entities;
                // Remove entities that have left the room
                foreach (var existingEntity in room.AlsoHere.ToList())
                {
                    if (!newEntities.Any(e => e.FullName == existingEntity.FullName))
                    {
                        room.AlsoHere.Remove(existingEntity);
                    }
                }

                
            }

            if (room_info.ContainsKey("items") && room_info["items"] != "")
            {
                room.VisibleItems = ProcessItemsSeen(room_info["items"]);
            }

            List<RoomExit> room_exits = new List<RoomExit>();
            foreach (string exit_here in room_info["exits"].Split(','))
            {
                RoomExit room_exit = new RoomExit(exit_here.Trim());
                room_exits.Add(room_exit);
            }
            room.RoomExits = room_exits;

            if (room.Cause.StartsWith("look,"))
            {
                this._look_room = room;
                this.result = EventType.RoomLook;
            }
            else
            {
                this._current_combat.UpdateRoom();
                this._current_room = room;
                this.result = EventType.Room;
            }

            
        }
        Dictionary<string, Item> ProcessItemsSeen(string csv_list)
        {
            Dictionary<string, Item> items = new Dictionary<string, Item>();
            foreach (string item_here in csv_list.Trim().Split(','))
            {
                string item_here2 = item_here.Trim();
                if (item_here2.EndsWith("here."))
                {
                    var x = item_here2.IndexOf("here.");
                    item_here2 = item_here2.Remove(x - 1).Trim();
                }

                item_here2 = item_here2.Replace("\r\n", " ");
                Item i = CreateConcreteItem(item_here2);

                items.Add(i.Name, i);
            }
            return items;
        }

        private string FigureOutCause_Room(string v)
        {
            //this can be
            //CRLF
            //l|lo|loo|look dir
            if(v == "\r\n" || v == "")
            {
                return "crlf";
            }
            Match m = Regex.Match(v, @"(?:l|lo|loo|look) (\S+)");
            if (m.Success)
            {
                return "look," + m.Groups[1].Value.Trim();
            }

            m = Regex.Match(v, @"(n|s|w|e|ne|nw|se|sw|u|d)");
            if (m.Success)
            {
                return "move," + m.Groups[1].Value.Trim();
            }

            Console.WriteLine("Unknown Room cause: |" + v + "|");
            return "";
        }

        

        private void ProcessCombat3rdP(Match match, string s)
        {
            if (match.Success)
            {
                Player p = new Player(match.Groups[1].Value);
                this.CreateConcreteEntity(match.Groups[2].Value);
                //this._current_combat.CombatEngaged()
                this.result = EventType.CombatEngagedStart_3rd;
            }
        }

        private void ProcessCombat(Match m, string s)
        {
            string cause = "Unknown";
            string combat_engaged = "";
            string[] tokens = s.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 2)
            {
                cause = FigureOutCause_CombatEngage(tokens[0]);
                combat_engaged = tokens[1];
            }
            else if(tokens.Length > 2)
            {
                cause = FigureOutCause_CombatEngage(tokens[tokens.Length-2]);
                combat_engaged = tokens[tokens.Length-1];
            }
            else
            {
                combat_engaged = tokens[0];
            }

            if (combat_engaged.Contains("*Combat Engaged*"))
            {
                this._player.IsCombatEngaged = true;
                this._player.CombatEngagedCause = cause;
                this.result = EventType.CombatEngagedStart;
            }
            else if (combat_engaged.Contains("*Combat Off*"))
            {
                this.result = EventType.CombatEngagedStop;
                this._player.CombatEngagedCause = cause;
                this._player.IsCombatEngaged = false;
            }
            else
            {
                throw new Exception("this shouldn't ever happen");
            }
        }

        private string FigureOutCause_CombatEngage(string v)
        {
            string attack_cmd = "";
            string target = "";
            string[] tokens = v.Split(' ');
            switch (tokens[0])
            {
                case "a":
                case "attack":
                    attack_cmd = "attack";
                    break;
                case "aa":
                case "bash":
                    attack_cmd = "bash";
                    break;
                case "sma":
                case "smash":
                    attack_cmd = "smash";
                    break;
                case "c":
                    break;
                default:
                    attack_cmd = "Unknown";
                    return attack_cmd;

            }

            //1 token means the player sent: a
            //2+ tokens is: a monster
            if (tokens.Length >= 2)
            {
                target = tokens[1];
                return attack_cmd + "," + target;
            }
            return attack_cmd;
        }

        private void ProcessInventory(Match m, string s)
        {
            //            You are carrying 2 silver nobles, 20 copper farthings, padded vest(Torso),   
            //padded boots(Feet), padded helm(Head), padded pants(Legs), padded gloves
            //(Hands), club(Weapon Hand)
            //You have no keys.                                                             
            //Wealth: 40 copper farthings
            //Encumbrance: 556 / 4032 - None[13 %]
            int carry_index = 0, cash_index = 0, enc_index = 0;
            string[] lines = s.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; ++i)
            {
                string line = lines[i];
                if (line.StartsWith("You are carrying "))
                {
                    carry_index = i;
                }
                else if (line.StartsWith("Wealth: "))
                {
                    cash_index = i;
                }
                else if (line.StartsWith("Encumbrance: "))
                {
                    enc_index = i;
                }
            }

            Dictionary<string, string> inv = new Dictionary<string, string>();
            string carry = "";
            for (int i = carry_index; i < cash_index; ++i)
            {
                carry += lines[i] + "\r\n";
            }
            carry = carry.Replace("\r\n", " ");

            int keys_idx = carry.IndexOf("You have no keys.");
            if(keys_idx == -1)
            {
                keys_idx = carry.IndexOf("You have the following keys:");
            }

            if (keys_idx > -1)
            {
                carry = carry.Remove(keys_idx);
            }
            inv.Add("carry", carry);
            inv.Add("cash", lines[cash_index]);
            inv.Add("enc", lines[enc_index]);

            string pattern = @"(\d+\s+)?([a-zA-Z\s]+?)(?:\s*\(([^)]+)\))?(?=\s*,|\s*$)";
            carry = carry.Substring("You are carrying".Length);
            MatchCollection mc = Regex.Matches(carry, pattern);

            Dictionary<string, Item> items = new Dictionary<string, Item>();
            foreach (Match m3 in mc)
            {
                if (m3.Success)
                {
                    //index 1 is quantity
                    //index 2 is the item name, this cane be plural if above is > 1
                    //index 3 is where the item is equppied
                    int count = 0;
                    int.TryParse(m3.Groups[1].Value, out count);

                    string item_name = m3.Groups[2].Value.Trim();
                    if (item_name == "()") { continue; }
                    if (item_name == "") { continue; }

                    Item item = new Item(item_name);
                    Match coin_match = Regex.Match(item_name, @"(runic coins?|platinum pieces?|gold crowns?|silver nobles?|copper farthings?)");
                    if (coin_match.Success)
                    {
                        item = new Coin(coin_match.Groups[1].Value);
                    }
                    if (count == 0) { count = 1; }
                    item.Quantity = count;


                    if (m3.Groups[3].Value != string.Empty)
                    {
                        item.Equiped = true;
                    }

                    if (items.ContainsKey(item.Name))
                    {
                        items[item.Name].Quantity += item.Quantity;
                    }
                    else
                    {
                        items.Add(item.Name, item);
                    }
                }
            }

            string weight = lines[enc_index].Split(':')[1].Split('-')[0].Trim();
            this._player.Inventory.SetInventory(items, weight);
            this.result = EventType.Inventory;
        }

        private void ProcessWhoList(Match m, string s)
        {
            string[] lines = s.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            List<Player> who_list = new List<Player>();
            for (int i = 3; i < lines.Length; ++i)
            {
                string[] tokens = lines[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string alignment = "";
                string first_name = "";
                string last_name = "";
                string title = "";
                string gang_name = "";

                int idx = 0;

                string token = tokens[idx];
                if(token == "Saint" || token == "Lawful" || token == "Good" || token == "Seedy" || token == "Outlaw" || token == "Criminal" || token == "Villain" || token == "FIEND")
                {
                    alignment = token;
                    idx++;
                    first_name = tokens[idx];
                    idx++;
                }
                else
                {
                    first_name = tokens[idx];
                    idx++;
                }

                token = tokens[idx];
                if(token == "x" || token == "-")
                {
                    idx++;
                }
                else
                {
                    last_name = tokens[idx];
                    idx++;idx++;
                }

                title = tokens[idx];
                idx++;

                bool capture_gang = false;
                for(int j=idx; j< tokens.Length; ++j)
                {
                    if (tokens[j] == "of")
                    {
                        j++;
                        capture_gang = true;
                    }
                    else
                    {
                        if (capture_gang)
                        {
                            gang_name += tokens[j];
                        }
                        else
                        {
                            title += tokens[j];
                        }
                    }
                }
                if(last_name == "")
                {

                }
                Player new_player = new Player(first_name.Trim());
                new_player.Stats.LastName = last_name.Trim();
                new_player.Title = title;
                new_player.Online = true;
                new_player.GangName = gang_name;
                new_player.Alignment = alignment;
                
                who_list.Add(new_player);
            }
            result = EventType.Who;
            this.UpdatePlayerList(who_list);
        }

        private void ProcessTopList(Match m, string s)
        {
            string[] top_list_parts = s.Split(new string[] { "\r\nRank Name" }, StringSplitOptions.None);
            string[] top_parts = top_list_parts[1].Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string last_line = top_parts[0];
            last_line = "Rank Name" + last_line;

            int rank_idx = last_line.IndexOf("Rank");
            int name_idx = last_line.IndexOf("Name");
            int class_idx = last_line.IndexOf("Class");
            int gang_idx = last_line.IndexOf("Gang");
            int exp_idx = last_line.IndexOf("Experience");

            List<Player> who_list = new List<Player>();
            for (int i = 2; i < top_parts.Length; i++)
            {
                string line = top_parts[i];
                if (line == "") { continue; }

                Player player = new Player(line.Substring(name_idx, class_idx - name_idx).Trim());
                player.Rank = int.Parse(line.Substring(rank_idx, 3));
                player.Stats.Class = line.Substring(class_idx, gang_idx - class_idx).Trim();
                if (exp_idx != -1)
                {
                    player.GangName = line.Substring(gang_idx, exp_idx-gang_idx).Trim();
                }
                else
                {//exp is hidden in top
                    player.GangName = line.Substring(gang_idx).Trim();
                }
                player.Exp = double.Parse(line.Substring(exp_idx).Trim());

                who_list.Add(player);
            }

            result = EventType.Top;
            this.UpdatePlayerList(who_list);
        }

        private void UpdatePlayerList(List<Player> who_list)
        {
            List<Player> new_players_to_add = new List<Player>();

            foreach (Player player in who_list)
            {
                bool player_found = false;
                foreach (Player current_player in this._players)
                {
                    if (player.Name == current_player.Name)
                    {
                        player_found = true;
                        if (result == EventType.Top)
                        {
                            current_player.Rank = player.Rank;
                            current_player.Exp = player.Exp;

                        }
                        else if (result == EventType.Top)
                        {
                            current_player.Alignment = player.Alignment;
                            current_player.GangName = player.GangName;
                            current_player.Title = player.Title;
                            current_player.Online = true;
                        }
                    }
                }

                if (!player_found)
                {
                    if(player.FullName.IndexOf(" ") > 0)
                    {
                        string[] tokens = player.FullName.Split(' ');
   

                    }
                    this._players.Add(player);

                }
            }
        }

        internal void HandleNewGameEvent(EventType result, object data)
        {
            this.NewGameEvent.Invoke(result);
        }
    }

    //This will match a string against many different regex patterns
    public class RegexMatcher
    {
        Dictionary<Regex, List<(EventType, Object)>> regexPatterns;

        public RegexMatcher(Dictionary<string, List<(EventType, Object)>> common_patterns)
        {
            this.regexPatterns = new Dictionary<Regex, List<(EventType, Object)>>();
            foreach (KeyValuePair<string, List<(EventType, Object)>> kvp in common_patterns)
            {
                Regex r = new Regex(kvp.Key, RegexOptions.Compiled);
                this.regexPatterns.Add(r, kvp.Value);
            }
        }

        public bool TryMatch(string input, out Match match, out List<(EventType, Object)> callback)
        {
            foreach (var r in regexPatterns)
            {
                var regex = r.Key;
                if (regex.IsMatch(input))
                {
                    match = regex.Match(input);
                    callback = r.Value;
                    return true;
                }
            }

            match = null;
            callback = new List<(EventType, Object)> { (EventType.None, null) };
            return false;
        }
    }
}
