using global::MMudObjects;
using global::MMudTerm.Session;
using MmeDatabaseReader;
using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MMudTerm.Game
{
    //this is a Player's combat against a single target
    public class CombatSession
    {
        public Entity target = null;
        public int damage_done = 0;
        public int damage_taken = 0;
        public bool IsBeingAttackByThisEntity = false;
        List<int[]> rounds = new List<int[]>();

        public CombatSession(Entity target)
        {
            this.target = target;
        }

        internal void BeingAttackByThisEntity()
        {
            this.IsBeingAttackByThisEntity = true;
        }

        internal void PlayerHit(int dmg_done)
        {
            this.damage_taken += dmg_done;
            this.target.damage_taken += dmg_done;
        }

        internal void PlayerHitBy(int dmg_done)
        {
            this.damage_done += dmg_done;
            
            BeingAttackByThisEntity();
        }

        internal void PlayerMissed()
        {
            //throw new NotImplementedException();
        }

        internal void PlayerMissedBy()
        {
            BeingAttackByThisEntity();
        }
    }

    //manages what is happening while we are in combat
    public class CurrentCombat
    {
        public bool in_combat = false;
        public Dictionary<string, CombatSession> _combats = new Dictionary<string, CombatSession>();
        public Dictionary<string, CombatSession> _victories = new Dictionary<string, CombatSession>();
        public List<Entity> _entities = new List<Entity>();
        SessionController _controller = null;
        Entity _current_target = null;

        public int player_misses = 0;
        public int player_hits = 0;
        public int player_crits = 0;
        public int player_dodge = 0;
        
        public int target_miss_player = 0;
        public int target_hit_player = 0;
        public int target_crit_player = 0;
        public int target_dodge_player = 0;

        public object AttackString { get; internal set; }

        public CurrentCombat(SessionController controller)
        {
            this._controller = controller;
            this.AttackString = "a";
        }

        public void UpdateRoom()
        {
            this._entities = this._controller._gameenv._current_room.AlsoHere;
        }

        private CombatSession GetSession(Entity e)
        {
            //if (!this._controller._gameenv._player.InCombat)
            //{
            //    this._controller._gameenv._player.InCombat = true;
            //}

            if (!_combats.ContainsKey(e.Name))
            {
                _combats.Add(e.Name, new CombatSession(e));
            }
            return _combats[e.Name];
        }

        //player hit something for some damage
        internal void PlayerHit(Entity e, int dmg_done, string crit)
        {
            CombatSession session = GetSession(e);
            if (crit == "critically")
            {
                this.player_crits++;
            }
            else //surprise too
            {
                this.player_hits++;
            }
            session.PlayerHit(dmg_done);
            this._current_target = session.target;
        }

        internal void PlayerHitBy(Entity e, int dmg_done, string crit)
        {
            GetSession(e).PlayerHitBy(dmg_done);
            if (crit == "critically")
            {
                this.target_crit_player++;
                
            }
            else
            {
                this.target_hit_player++;
            }
        }

        internal void PlayerMissedBy(Entity e, bool is_dodge=false)
        {
            GetSession(e).PlayerMissedBy();
            if (is_dodge) this.player_dodge++;else
            this.target_miss_player++;
        }

        internal void PlayerMissed(Entity e)
        {
            CombatSession session = GetSession(e);
            session.PlayerMissed();
            this.player_misses++;
        }

        internal void Remove(List<CombatSession> to_remove)
        {
            foreach(CombatSession session in to_remove)
            {
                this._combats.Remove(session.target.Name);
                //this._victories.Add(session.target.Name, session);
            }
        }
    }

    //this represents a player and their current game sessions
    public class MajorMudBbsGame
    {
        public Player _player = null;
        public Room _current_room = null;
        public Room _look_room = null;
        public List<Player> _players = new List<Player>();
        public CurrentCombat _current_combat = null;
        public List<string> _gossips = new List<string>();
        public List<string> _auctions = new List<string>();
        
        public RegexHell _matcher = null;      
        
        public EventType result = EventType.None;

        SessionController _controller = null;

        #region monitors
        bool monitor_on = false;
        bool monitor_combat = false;
        bool monitor_rest = false;
        bool monitor_buff = false;
        bool monitor_get = false;
        bool monitor_getcoins = false;

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
                case EventType.RoomSomethingMovedOut:
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
                    AttackFirstNpcInRoom();
                }
                else
                {
                    this.NewGameEvent -= NewGameEvent_GetCoins;
                }
                this.monitor_getcoins = value;
            }
        }

        private void NewGameEvent_GetCoins(EventType message)
        {
            switch (message)
            {
                case EventType.Room:
                    foreach(Item t in this._current_room.VisibleItems.Values)
                    {
                        if(t.Name.StartsWith("silver noble"))
                        {
                            var count = t.Quantity;
                            this._controller.SendLine($"get {count} silver noble");
                        }
                        else if (t.Name.StartsWith("copper farthing"))
                        {
                            var count = t.Quantity;
                            this._controller.SendLine($"get {count} copper farthing");
                        }
                    }
                    break;
                case EventType.EntityDeath:
                    //this._controller.SendLine("get sil");
                    //this._controller.SendLine("get cop");
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
            throw new NotImplementedException();
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
                this.monitor_combat = value;
            }
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
                        Debug.WriteLine("Player should rest");
                        if (this._player.IsResting)
                        {
                            Debug.WriteLine("Already resting!");
                            return;
                        }
                        if (this._player.IsCombatEngaged)
                        {
                            Debug.WriteLine("Player is in combat and can not rest");
                            return;
                        }
                        if (this._current_room.AlsoHere.RoomContainsNPC())
                        {
                            Debug.WriteLine("NPC in room, can't rest");
                            return;
                        }

                        if (this._current_combat.in_combat)
                        {
                            Debug.WriteLine("Player is in comat and can not rest2");return;
                        }
                        this._controller.SendLine("rest");
                    }else if(player_health < 0.15)
                    {
                        this._controller.Disconnect();
                    }

                    break;
            }
        }

        public delegate void NewGameEventHandler(EventType message);
        public event NewGameEventHandler NewGameEvent;


        private void AttackFirstNpcInRoom()
        {
            if (this._current_room.AlsoHere.RoomContainsNPC())
            {
                if (!this._player.IsCombatEngaged)
                {
                    Entity e = this._current_room.AlsoHere.GetFirst("npc");
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
                case EventType.CombatEngagedStop:
                    break;
                case EventType.Combat:

                case EventType.CombatMiss:
                    //this._controller.SendLine("get sil");
                    //this._controller.SendLine("get cop");
                    if (!this._player.IsCombatEngaged)
                    {
                        this.NewGameEvent_Combat(EventType.Room);
                    }
                    break;
                
                default:
                    //Console.WriteLine("Combat - ignored token " + message);
                    break;
            }
        }

        #endregion

        object idle_timer_state = null;
        internal System.Timers.Timer idle_input_timer = null;

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
        }

        private void Idle_input_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this._controller.SendLine();
        }

        public void Process(string data)
        {
            //Console.WriteLine("Processing: " + data.Trim());
            Match match = null;
            EventType e = EventType.None;
            this.result = EventType.None;

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

            Debug.WriteLine("------------------------------------");
            Debug.WriteLine("Data: " + data.Trim() );
            if (this._matcher.TryMatch(data, out match, out e))
            {
                Debug.WriteLine($"--> {e} <--");
                for (int i = 0; i < match.Groups.Count; ++i)
                {
                    Console.WriteLine($"\tMatch: {match.Groups[i].Value}");
                }
                callback(e, match, data);
            }
            else
            {
                result = EventType.None;
                Debug.WriteLine("Not Matched");               
            }
            Debug.WriteLine("------------------------------------\r\n");
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
                string spell = match.Groups[1].Value;
                this.result = EventType.BuffExpired;
            }
        }

        private void ProcessBuffSpellCastSuccess(Match match, string arg2)
        {
            if (match.Success)
            {
                string caster = match.Groups[1].Value;
                string spell = match.Groups[2].Value;
                string target = match.Groups[3].Value;
                if (caster == "You")
                {
                    this.result = EventType.BuffSpellCastSuccess;
                }
                else {
                    this.result = EventType.BuffSpellCastSuccess_3rdP;
                }
            }
        }

        private void ProcessMovedOutOfRoom(Match match, string arg2)
        {
            if (match.Success)
            {
                Entity e = new Entity(match.Groups[1].Value);
                e = MmeDatabaseReader.MMudData.GetNpc(e);
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
                Entity e = new Entity(match.Groups[1].Value);
                e = MmeDatabaseReader.MMudData.GetNpc(e);
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
                Entity e = new Entity(match.Groups[1].Value);
                e = MmeDatabaseReader.MMudData.GetNpc(e);
                this._current_room.AlsoHere.Add(e);
                this.result = EventType.RoomSomethingMovedInto;
            }
            
        }

        private void ProcessEnteredRealm(Match match, string arg2)
        {
            string player_first_name = match.Groups[1].Value;

            foreach (Player p in this._players)
            {
                if (p.FirstName == player_first_name)
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
                if (p.FirstName == player_first_name)
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

        private void ProcessCombatDoHit(Match match, string arg2)
        {
            //TODO: if 1 is set we had a crit
            
            //You clobber giant rat for 12 damage!
            Entity attacker = new Entity(match.Groups[1].Value.Trim());
            var crit = match.Groups[2].Value;
            Entity target = new Entity(match.Groups[3].Value);
            int dmg_done = int.Parse(match.Groups[4].Value);

            if (attacker.Name == "You")
            {
                target = MmeDatabaseReader.MMudData.GetNpc(target);
                AddAttackerToRoom(target);
                this._current_combat.PlayerHit(target, dmg_done, crit);
            }else if(target.Name == "you")
            {
                attacker = MmeDatabaseReader.MMudData.GetNpc(attacker);
                AddAttackerToRoom(attacker);
                this._current_combat.PlayerHitBy(attacker, dmg_done, crit);
            }
            else
            {
                //3rd party attacking something, do we care?
            }
            this.result = EventType.Combat;
        }

        private void AddAttackerToRoom(Entity target)
        {
            if(target.Name == "beast")
            {

            }
            foreach(Entity e in this._current_room.AlsoHere)
            {
                if (e.FullName == target.FullName) return;
            }

            
            this._current_room.AlsoHere.Add(target);
        }

        private void ProcessCombatDoMiss(Match match, string arg2)
        {
            Entity attacker = new Entity(match.Groups[1].Value.Trim());
            Entity target = new Entity(match.Groups[2].Value.Trim());
            //You swipe at kobold thief!

            if (attacker.Name == "You")
            {
                target = MmeDatabaseReader.MMudData.GetNpc(target);
                AddAttackerToRoom(target);
                this._current_combat.PlayerMissed(target);
            }
            else if(target.Name == "you")
            {
                attacker = MmeDatabaseReader.MMudData.GetNpc(attacker);
                AddAttackerToRoom(attacker);
                var is_dodge = arg2.Contains(", but you dodge");
                this._current_combat.PlayerMissedBy(attacker, is_dodge);
            }
            else
            {
                //3rd party attacking something, do we care?
            }
            this.result = EventType.Combat;
        }

        private void ProcessResting(Match match, string arg2)
        {
            this._player.IsResting = true;
            this.result = EventType.Rest;
        }

        private void ProcessCashDropFromEntity(Match match, string arg2)
        {
            //cash drop on death
            //multiple hits, need to regex collection against the string and process
            //5 silver drop to the ground.
            //14 copper drop to the ground.
            //do we care?  it should be in the room desc on the next room process
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

        private void ProcessDropCash(Match match, string arg2)
        {
            if (match.Success)
            {
                string coin_name = match.Groups[2].Value;
                Price price = new Price(coin_name);
                price.ParseMatch(match);
                List<Item> coins = price.ToList();
                this._player.Inventory.Remove(coins);
                this._current_room.Add(coins);
                this.result = EventType.DropCoins;
            }
        }

        private void ProcessDropItem(Match match, string s)
        {
            if (match.Success)
            {
                //picked up item
                Item item = new Item(match.Groups[1].Value);
                this._player.Inventory.Remove(item);
                this._current_room.Add(item);
                this.result = EventType.DropItem;
            }
        }

        private void ProcessPickup(Match match, string arg2)
        {
            string pattern1 = @"You picked up (\d+) ([\S ]+)\r\n";
            Match m1 = Regex.Match(arg2, pattern1, RegexOptions.Compiled);
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
                //picked up item
                Item item = new Item(m2.Groups[1].Value);
                item = MMudData.GetItem(item);
                this._player.Inventory.Add(item);
                this._current_room.Remove(item);
                this.result = EventType.PickUpItem;
            }
        }

        //main entry point, this takes known whole blocks of text and matches the text block to a parser
        
        private void callback(EventType e, Match match, string data)
        {
            switch (e)
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
                case EventType.Combat:
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
                case EventType.HidItem:
                    ProcessHidSomething(match, data);
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
                case EventType.CombatHitPlayer:
                case EventType.CombatMissPlayer:
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
                default:
                    Console.WriteLine("fix" + e)
                        ; break;
                    
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


        //processes the hp/mp block
        public void ProcessTick(Match match, string s)
        {
            Dictionary<string, string> stats = new Dictionary<string, string>();
            this._player.Stats.CurHits = int.Parse(match.Groups[1].Value);
            if (match.Groups[2].Success)
            {
                this._player.Stats.CurMana = int.Parse(match.Groups[2].Value);
            }

            if (match.Groups[3].Success)
            {
                this._player.IsResting = match.Groups[3].Value == "Resting" ? true : false;
            }
            else
            {
                this._player.IsResting = false;
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

        private void ProcessHidSomething(Match match, string arg2)
        {
            string pattern1 = @"You hid (\d+) ([\S ]+)\.";
            Match m1 = Regex.Match(arg2, pattern1, RegexOptions.Compiled);
            if (m1.Success)
            {
                string coin_name = m1.Groups[2].Value;
                Price price = new Price();
                price.ParseMatch(m1);

                List<Item> item = price.ToList();
                this._player.Inventory.Remove(item);
                this._current_room.Add_Hidden(item);
                this.result = EventType.HidCoins;
                return;
            }

            string pattern2 = @"You hid ([ \S]+)\.\r\n";
            Match m2 = Regex.Match(arg2, pattern2, RegexOptions.Compiled);
            if (m2.Success)
            {
                //picked up item
                Item item = new Item(m2.Groups[1].Value);
                this._player.Inventory.Remove(item);
                this._current_room.Add_Hidden(item);
                this.result = EventType.HidItem;
            }
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

        //processes the player stat block as a whole frame
        private void ProcessStat(Match match, string s)
        {
            string[] char_creation_work_around = s.Split(new string[] { "SAVESAVE" }, StringSplitOptions.None);
            if(char_creation_work_around.Length == 2) {
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
                    }catch(Exception ex)
                    {

                    }
                }
            }

            //the : in Level:
            var start_idx = s.IndexOf("Level:") + "Level:".Length;
            var level_string = s.Substring(s.IndexOf("Level")+"Level".Length+1, s.IndexOf("Stealth")- start_idx).Trim();
            stats.Add("Level", level_string);

            this._player.Stats = new PlayerStats(stats);
            this.result = EventType.Stats;
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

            tokens = tokens[0].Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            room_info["name"] = tokens[tokens.Length -1].Trim();

            if (tokens.Length == 2)
            {
                room_info["cause"] = FigureOutCause_Room(tokens[0].Trim());
            }
            else
            {
                room_info["cause"] = "crlf";
            }

            
            Room room = new Room();
            room.Name = room_info["name"];
            if (room_info.ContainsKey("desc"))
            {
                room.Description = room_info["desc"];
            }

            AlsoHere entities = new AlsoHere();
            if (room_info.ContainsKey("here") && room_info["here"] != "")
            {
                foreach (string also_here in room_info["here"].Split(','))
                {
                    Entity entity = new Entity(also_here.Trim());
                    Player p = this.IsPlayer(entity);
                    if(p!= null)
                    {
                        entity = p;
                    }
                    else
                    {
                        entity = MmeDatabaseReader.MMudData.GetNpc(entity);
                    }
                    entities.Add(entity);
                }
                room.AlsoHere = entities;
            }

            if (room_info.ContainsKey("items") && room_info["items"] != "")
            {
                Dictionary<string, Item> items = new Dictionary<string, Item>();
                foreach (string item_here in room_info["items"].Split(','))
                {
                    string item_here2 = item_here.Trim();
                    if (item_here2.EndsWith("here."))
                    {
                        var x = item_here2.IndexOf("here.");
                        item_here2 = item_here2.Remove(x-1).Trim();
                    }

                    Item i = null;
                    string pattern = @"^(\d+) ([\S ]+)";
                    Match m4 = Regex.Match(item_here2, pattern);
                    if (m4.Success)
                    {
                        i = new Item(m4.Groups[2].Value.Trim());
                        i = MMudData.GetItem(i);
                        i.Quantity = int.Parse(m4.Groups[1].Value);
                    }
                    else
                    {
                        i = new Item(item_here2);
                        i = MMudData.GetItem(i);
                    }

                    items.Add(i.Name, i);
                }
                room.VisibleItems = items;
            }

            List<RoomExit> room_exits = new List<RoomExit>();
            foreach (string exit_here in room_info["exits"].Split(','))
            {
                RoomExit room_exit = new RoomExit(exit_here.Trim());
                room_exits.Add(room_exit);
            }
            room.RoomExits = room_exits;
            room.Light = room_info["light"];
            room.Cause = room_info["cause"];

            if (room_info["cause"].StartsWith("look,"))
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

        private string FigureOutCause_Room(string v)
        {
            //this can be
            //CRLF
            //l|lo|loo|look dir
            if(v == "\r\n")
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

        private Player IsPlayer(Entity entity)
        {
            foreach (Player p in this._players)
            {
                if (p.FirstName == entity.Name)
                    return p;
            }
            return null;
        }

        private void ProcessCombat3rdP(Match match, string s)
        {
            if (match.Success)
            {
                Player p = new Player(match.Groups[1].Value);
                Entity e = new Entity(match.Groups[2].Value);
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
                case "smash":
                    attack_cmd = "smash";
                    break;
                default:
                    attack_cmd = "Unknown";
                    return attack_cmd;

            }

            target = tokens[1];
            return attack_cmd + "," + target;
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
            if (keys_idx > -1)
            {
                carry = carry.Remove(keys_idx);
            }
            inv.Add("carry", carry);
            inv.Add("cash", lines[cash_index]);
            inv.Add("enc", lines[enc_index]);

            string pattern = @"(\d+\s+)?([a-zA-Z\s]+?)(?:\s*\(([^)]+)\))?(?=\s*,|\s*$)";
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
                Player new_player = new Player($"{first_name} {last_name}".Trim());
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
                    if (player.FirstName == current_player.FirstName)
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

        internal void HandleNewGameEvent(EventType result)
        {
            this.NewGameEvent.Invoke(result);
        }
    }

    //This will match a string against many different regex patterns
    public class RegexMatcher
    {
        Dictionary<Regex, EventType> regexPatterns;

        public RegexMatcher(Dictionary<string, EventType> common_patterns)
        {
            this.regexPatterns = new Dictionary<Regex, EventType>();
            foreach (KeyValuePair<string, EventType> kvp in common_patterns)
            {
                Regex r = new Regex(kvp.Key, RegexOptions.Compiled);
                this.regexPatterns.Add(r, kvp.Value);
            }
        }

        public bool TryMatch(string input, out Match match, out EventType callback)
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
            callback = EventType.None;
            return false;
        }
    }
}
