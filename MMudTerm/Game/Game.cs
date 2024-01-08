using global::MMudObjects;
using global::MMudTerm.Session;

using System;
using System.Collections.Generic;

using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;



namespace MMudTerm.Game
{
    //this is a Player's combat against a single target
    public class CombatSession
    {
        public Entity target = null;
        public int damage_done = 0;
        public int damage_taken = 0;
        public bool IsBeingAttackByThisEntity = false;

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

        public CurrentCombat(SessionController controller)
        {
            this._controller = controller;
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
        internal void PlayerHit(Entity e, int dmg_done)
        {
            CombatSession session = GetSession(e);
            session.PlayerHit(dmg_done);
            this._current_target = session.target;
        }

        internal void PlayerHitBy(Entity e, int dmg_done)
        {
            GetSession(e).PlayerHitBy(dmg_done);
        }

        internal void PlayerMissedBy(Entity e)
        {
            GetSession(e).PlayerMissedBy();
        }

        internal void PlayerMissed(Entity e)
        {
            CombatSession session = GetSession(e);
            session.PlayerMissed();
            this._current_target = session.target;
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
        public List<Player> _players = new List<Player>();
        public CurrentCombat _current_combat = null;

        private string pattern_stat = @"Name: ";
        private string pattern_room = @"Obvious exits: ";
        private string pattern_inv = @"You are carrying ";

        private RegexMatcher _matcher = null;
        public Room _look_room;
        public string result = null;
        string[] coins = new string[] { "platinum piece", "gold crown", "silver noble", "copper farthing" };

        SessionController _controller = null;

        public bool EnterTheGame { get; internal set; }

        public MajorMudBbsGame(SessionController controller)
        {
            Dictionary<string, Action<Match, string>> common_patterns = new Dictionary<string, Action<Match, string>>();
            common_patterns.Add(pattern_room, ProcessRoom);
            common_patterns.Add(pattern_stat, ProcessStat);
            common_patterns.Add(pattern_inv, ProcessInventory);
            common_patterns.Add(@"\*Combat ", ProcessCombat);
            common_patterns.Add(@"You are now resting.", ProcessResting);
            common_patterns.Add(@"Top Heroes of the Realm", ProcessTopList);
            common_patterns.Add(@"   Current Adventurers", ProcessWhoList);
            common_patterns.Add(@"You just bought ([\S ]+) for ([\S ]+)\.", ProcessBoughtSomething);
            common_patterns.Add(@"You sold ([\S ]+) for ([\S ]+)\.", ProcessSoldSomething);


            common_patterns.Add(@"There is no exit in that direction!", ProcessBadRoomMove);
            common_patterns.Add(@"You are not permitted in that room!", ProcessBadRoomMove);
            common_patterns.Add(@"There is a closed door in that direction!", ProcessBadRoomMove);
            common_patterns.Add(@"The door is closed!", ProcessBadRoomMove);

            common_patterns.Add(@"You hid ", ProcessHidSomething);
            common_patterns.Add(@"You notice (.*) here.", ProcessSearch);
            common_patterns.Add(@"Attempting to sneak...", ProcessSneak);
            common_patterns.Add(@"The following items are for sale here:", ProcessForSale);
            common_patterns.Add(@"You picked up ", ProcessPickup);
            common_patterns.Add(@"You took ", ProcessPickup);
            common_patterns.Add(@"You dropped (\d+) ([\S ]+)\.", ProcessDropCash);
            common_patterns.Add(@"You dropped ([ \S]+).\r\n", ProcessDropItem);

            common_patterns.Add(@"You gain (\d+) experience\.", ProcessXpGain);
            common_patterns.Add(@"Exp: (\d+) Level: (\d+) Exp needed for next level: (\d+) \((\d+)\) \[(\d+)%\]", ProcessXpUpdate);

            common_patterns.Add(@"(\d+) (runic|platinum|gold|silver|copper) drop to the ground\.", ProcessCashDropFromMob);


            string verb_pattern_club_3rd = "smashes|clobbers|slams|whaps|smacks|beats";
            string verb_pattern_pierce_3rd = "lunges|stabs|impales|skewers";
            string verb_pattern_cut_3rd = "slices|slashes|cuts";
            string verb_pattern_natural_3rd = "chomps|bites|claws|rips|whips|chills";
            string verb_pattern_miss_3rd = "swipes|flails|snaps|lashes|swings|reaches out";
            string present_tense_3rd_person_verb = $"{verb_pattern_club_3rd}|{verb_pattern_pierce_3rd}|{verb_pattern_cut_3rd}|{verb_pattern_natural_3rd}|{verb_pattern_miss_3rd}";

            string verb_pattern_club = "smash|clobber|slam|whap|smack";
            string verb_pattern_pierce = "lunge|stab|impale|skewer";
            string verb_pattern_cut = "slice|slash|cut";
            string verb_pattern_natural = "chomp|bite|claw|rip|whip";
            string verb_pattern_miss = "swipe|flail|snap|lash|swing|shoot a bolt";
            string verb_pattern_bow = "shoot a bolt at";

            string present_tense_1st_person_verb = $"{verb_pattern_bow}|{verb_pattern_club}|{verb_pattern_pierce}|{verb_pattern_miss}|{verb_pattern_cut}|{verb_pattern_natural}";

            string verb = $"(?:{present_tense_1st_person_verb}|{present_tense_3rd_person_verb})";

            string moves_to_attack = @"(\S+) moves to attack ([A-Za-z ]+)\.";
            common_patterns.Add(moves_to_attack, ProcessMovesToAttack);

            string pattern_do_hit = @"(\S+|You) (critically |surprise )?" + verb + @" ([\S ]+) for (\d+) damage!";
            common_patterns.Add(pattern_do_hit, ProcessCombatDoHit);

            string pattern_do_miss = @"(\S+|You) " + verb + @" at ([\S ]+)!";
            common_patterns.Add(pattern_do_miss, ProcessCombatDoMiss);

            //The shade chills you with its touch for 1 damage!
            string pattern_rcv_hit = @"The ([\S ]+) " + verb + @" you (?:[\S ]*)for (\d+) damage!";
            common_patterns.Add(pattern_rcv_hit, ProcessCombatRecieveHit);

            //The big filthbug claws at you, but your armour deflects the blow!
            string pattern_rcv_miss = @"The ([\S ]+) " + verb + @" at|for you";
            common_patterns.Add(pattern_rcv_miss, ProcessCombatRecieveMiss);

            string hung_up = @"(\S+) just (?:disconnected|hung up)!!!";
            common_patterns.Add(hung_up, ProcessHungUp);
            
            string left_the_realm = @"(\S+) just left the Realm\.";
            common_patterns.Add(left_the_realm, ProcessHungUp);

            string entered_the_realm = @"(\S+) just entered the Realm\.";
            common_patterns.Add(entered_the_realm, ProcessEnteredRealm);

            string pattern_movement = "walks|crawls|scuttles|creeps|sneaks|oozes|flaps|lopes";
            string pattern_moves = "(?:" + pattern_movement + ")";

            //string moved_into_room =  @"([A-Za-z ]+) walks into the room from the (\S+)\.";
            //string moved_into_room2 = @"A ([A-Za-z ]+) crawls into the room from the (\S+)\.";
            string moved_into_room = @"A(?:n)? ([A-Za-z ]+) " + pattern_moves + @" in|into the room from (?:the )?(\S+)\.";
            common_patterns.Add(moved_into_room, ProcessMovedIntoRoom);

            string moved_into_room2= @"A ([A-Za-z ]+) materializes in the room\.";
            common_patterns.Add(moved_into_room2, ProcessMovedIntoRoom);

            string moved_out_of_room = @"(\S+) just left to the (\S+)\.";
            common_patterns.Add(moved_out_of_room, ProcessMovedOutOfRoom);

            List<string> death_msgs = new System.Collections.Generic.List<string>();
            death_msgs.Add(@" collapes with a dull thump");
            death_msgs.Add(@" collapses, its legs curling tightly around it");
            death_msgs.Add(@" collapses with a groan");
            death_msgs.Add(@" collapses in a filthy heap\.");
            death_msgs.Add(@" collapses into a broken heap\.");
            death_msgs.Add(@" falls to the ground with a yelp, and is still");
            death_msgs.Add(@" dissolves into a puddle of bluish goo");
            death_msgs.Add(@" falls to the ground with a tortured squeak");
            death_msgs.Add(@" squeaks loudly, and falls to the ground!");
            death_msgs.Add(@" crumbles into a pile of dust\.");
            death_msgs.Add(@"'s wrapping unravels, revealing nothing but dust\.");
            death_msgs.Add(@" vanishes with an eerie wail\.");

            string dms = String.Join("|", death_msgs);

            string entity_death = @"The ([A-Za-z ]+)(?:" + dms + ")";
            common_patterns.Add(entity_death, ProcessEntityDeath);


            //Cthulhu casts ethereal shield on Cthulhu!
            string buff_spell_cast = @"(\S+) casts ([a-z ]+) on (\S+)!";
            common_patterns.Add(buff_spell_cast, ProcessBuffSpellCastSuccess);

            //Cthulhu attempted to cast ethereal shield, but failed.
            string buff_spell_cast_fail = @"(\S+) attempted to cast ([a-z ]+), but failed.";
            common_patterns.Add(buff_spell_cast_fail, ProcessBuffSpellCastFailure);

            //The effects of the mummy's curse wears off!
            string buff_spell_wore_off = @"The effects of the ([A-Za-z ']+) wears off!";
            common_patterns.Add(buff_spell_wore_off, ProcessBuffSpellWearOff);

            string you_hear_movement = @"You hear movement to the (\S+)";
            common_patterns.Add(you_hear_movement, ProcessHearMovement);

            string failed_door_open = @"Your attempts to bash through fail!";
            common_patterns.Add(failed_door_open, ProcessBashDoor);

            string bashed_door_open = @"You bashed the door open\.";
            common_patterns.Add(bashed_door_open, ProcessBashDoor);

            string open_door = @"The door is now (open|closed)\.";
            common_patterns.Add(open_door, ProcessOpenDoor);

            string door_close_lock = @"The door to the (S+) just locked!";
            common_patterns.Add(door_close_lock, ProcessDoorCloseLocked);

            string door_locked = @"The door is locked.";
            common_patterns.Add(door_locked, ProcessDoorCloseLocked);



            this._matcher = new RegexMatcher(common_patterns);

            this._controller = controller;
            this._player = new Player("You");
            this._current_room = new Room();
            this._players = new List<Player>();
            this._current_combat = new CurrentCombat(this._controller);
        }

        private void ProcessDoorCloseLocked(Match match, string arg2)
        {
            if (match.Success)
            {
                string direction = match.Groups[1].Value;
                this._current_room.DoorOpened(direction, "closed");
                this.result = "door_closed";
            }
        }

        private void ProcessOpenDoor(Match match, string arg2)
        {
            if (match.Success)
            {
                string[] tokens = arg2.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                string direction = "Unkown";
                string msg = "";

                if (tokens.Length == 2)
                {
                    direction = tokens[0].Split(' ')[1];
                }

                string action = match.Groups[1].Value;
                this._current_room.DoorOpened(direction, action);
                if (action == "open")
                {
                    this.result = "door_open";
                }
                else
                {
                    this.result = "door_closed";
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
                    this.result = "bash_door_success";
                }
                else
                {
                    this.result = "bash_door_fail";
                }               
            }
        }

        private void ProcessHearMovement(Match match, string arg2)
        {
            if (match.Success)
            {
                string direction = match.Groups[1].Value;
                this.result = "hear_movement";
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
                    this.result = "buff_spell_cast_failure";
                }
                else
                {
                    this.result = "3rdP_buff_spell_cast_failure";
                }
            }
        }


        private void ProcessBuffSpellWearOff(Match match, string arg2)
        {
            if (match.Success)
            {
                string spell = match.Groups[1].Value;
                this.result = "buff_wore_off";
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
                    this.result = "buff_spell_cast_success";
                }
                else {
                    this.result = "3rdP_buff_spell_cast_success";
                }
            }
        }

        private void ProcessMovedOutOfRoom(Match match, string arg2)
        {
            if (match.Success)
            {
                Entity e = new Entity(match.Groups[1].Value);
                this._current_room.AlsoHere.Remove(e);
                this.result = "also_here";
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
                    this.result = "PVP_ATTACKED";
                }
                else
                {
                    this.result = "3rd_Party_Combat_Start";
                }
            }
        }

        private void ProcessEntityDeath(Match match, string arg2)
        {
            if (match.Success)
            {
                Entity e = new Entity(match.Groups[1].Value);
                this._controller._gameenv._current_room.AlsoHere.Remove(e);
                this.result = "entity_death";
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
                this._current_room.AlsoHere.Add(new Entity(match.Groups[1].Value));
                this.result = "moved_into_room";
            }
            
        }

        private void ProcessEnteredRealm(Match match, string arg2)
        {
            string player_first_name = match.Groups[1].Value;

            foreach (Player p in this._players)
            {
                if (p.FirstName == player_first_name)
                {
                    p.Online = false;
                    this.result = "player_entered";
                    return;
                }
            }

            Player new_player = new Player(player_first_name);
            new_player.Online = true;
            this._players.Add(new_player);
        }

        private void ProcessHungUp(Match match, string arg2)
        {
            string player_first_name = match.Groups[1].Value;

            foreach (Player p in this._players)
            {
                if (p.FirstName == player_first_name)
                {
                    p.Online = false;
                    return;
                }
            }
        }

        private void ProcessCombatDoHit(Match match, string arg2)
        {
            //TODO: if 1 is set we had a crit
            
            //You clobber giant rat for 12 damage!
            Entity attacker = new Entity(match.Groups[1].Value.Trim());
            Entity e = new Entity(match.Groups[3].Value);

            int dmg_done = int.Parse(match.Groups[4].Value);

            if (attacker.Name == "You")
            {
                this._current_combat.PlayerHit(e, dmg_done);
            }
            else
            {
                //3rd party attacking something, do we care?
            }
            this.result = "combat";
        }

        private void ProcessCombatRecieveHit(Match match, string arg2)
        {
            Entity attacker = new Entity(match.Groups[1].Value.Trim());
            Entity e = new Entity(match.Groups[1].Value.Trim());

            int dmg_done = int.Parse(match.Groups[2].Value);
            if (attacker.Name == "You")
            {
                this._current_combat.PlayerHitBy(e, dmg_done);
            }
            else
            {
                //3rd party attacking something, do we care?
            }
            
            this.result = "combat";
        }

        private void ProcessCombatDoMiss(Match match, string arg2)
        {
            Entity attacker = new Entity(match.Groups[1].Value.Trim());
            Entity e = new Entity(match.Groups[1].Value.Trim());
            //You swipe at kobold thief!
            
            if (attacker.Name == "You")
            {
                this._current_combat.PlayerMissed(e);
            }
            else
            {
                //3rd party attacking something, do we care?
            }
            this.result = "combat";
        }

        private void ProcessCombatRecieveMiss(Match match, string arg2)
        {
            Entity attacker = new Entity(match.Groups[1].Value.Trim());
            //The thin kobold thief lunges at you with their shortsword!
            Entity e = new Entity(match.Groups[1].Value);
            
            if (attacker.Name == "You")
            {
                this._current_combat.PlayerMissedBy(e);
            }
            else
            {
                //3rd party attacking something, do we care?
            }
            this.result = "combat";
        }

        private void ProcessResting(Match match, string arg2)
        {
            this._player.IsResting = true;
            this.result = "resting";
        }

        private void ProcessCashDropFromMob(Match match, string arg2)
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

                this.result = "exp_update";
            }
        }

        private void ProcessXpGain(Match match, string arg2)
        {
            if (match.Success)
            {
                double gained_exp = double.Parse(match.Groups[1].Value);
                this._player.Stats.Exp += gained_exp;
                this._player.GainedExp += gained_exp;
                this.result = "exp";
            }
        }

        private void ProcessDropCash(Match match, string arg2)
        {
            if (match.Success)
            {
                string coin_name = match.Groups[2].Value;
                Price price = new Price(coin_name);
                price.ParseMatch(match);
                List<CarryableItem> coins = price.ToList();
                this._player.Inventory.Remove(coins);
                this._current_room.Add(coins);
                this.result = "drop_cash";
            }
        }

        private void ProcessDropItem(Match match, string s)
        {
            if (match.Success)
            {
                //picked up item
                CarryableItem item = new CarryableItem(match.Groups[1].Value);
                this._player.Inventory.Remove(item);
                this._current_room.Add(item);
                this.result = "drop_item";
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
                List<CarryableItem> coins = price.ToList();
                this._player.Inventory.Add(coins);
                this._current_room.Remove(coins);
                this.result = "pickup_cash";
                return;
            }

            string pattern2 = @"You (?:picked up|took) ([ \S]+)\.\r\n";
            Match m2 = Regex.Match(arg2, pattern2, RegexOptions.Compiled);
            if (m2.Success)
            {
                //picked up item
                CarryableItem item = new CarryableItem(m2.Groups[1].Value);

                this._player.Inventory.Add(item);
                this._current_room.Remove(item);
                this.result = "pickup_item";
            }
        }

        //main entry point, this takes known whole blocks of text and matches the text block to a parser
        public void Process(string data)
        {
            //Console.WriteLine("Processing: " + data.Trim());
            Match match = null;
            Action<Match, string> callback = null;
            this.result = null;

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

            if (this._matcher.TryMatch(data, out match, out callback))
            {
                callback(match, data);
            }
            else
            {
                result = null;
                Debug.WriteLine("------------Unknown------");
                Debug.WriteLine(data);
                Debug.WriteLine("------------------------------------");
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

            this.result = "tick";
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
                PurchasableItem shop_item = new PurchasableItem(name, quantity, cost);
                shop_item.useable = line.Contains("(You can't use)") ? false : true;
                shop_item.too_powerful = line.Contains("(Too powerful)") ? true : false;
            }

            this.result = "for_sale_list";
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
            this.result = "search_found";
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

                List<CarryableItem> item = price.ToList();
                this._player.Inventory.Remove(item);
                this._current_room.Add_Hidden(item);
                this.result = "hid_cash";
                return;
            }

            string pattern2 = @"You hid ([ \S]+)\.\r\n";
            Match m2 = Regex.Match(arg2, pattern2, RegexOptions.Compiled);
            if (m2.Success)
            {
                //picked up item
                CarryableItem item = new CarryableItem(m2.Groups[1].Value);
                this._player.Inventory.Remove(item);
                this._current_room.Add_Hidden(item);
                this.result = "hid_item";
            }
        }

        private void ProcessBadRoomMove(Match match, string arg2)
        {
            //throw new NotImplementedException();
        }

        private void ProcessSoldSomething(Match match, string arg2)
        {
            CarryableItem item_bought = null;
            List<CarryableItem> cost_as_items = null;
            if (match.Success)
            {
                item_bought = new CarryableItem(match.Groups[1].Value);
                cost_as_items = new Price(match.Groups[2].Value.Trim()).ToList();
            }
            this._player.Inventory.Remove(item_bought);
            this._player.Inventory.Add(cost_as_items);
            this.result = "bought_something";
        }

        private void ProcessBoughtSomething(Match match, string arg2)
        {
            CarryableItem item_bought = null;
            List<CarryableItem> cost_as_items = null;

            if (match.Success)
            {
                item_bought = new CarryableItem(match.Groups[1].Value);
                cost_as_items = new Price(match.Groups[2].Value.Trim()).ToList();
            }
            this._player.Inventory.Add(item_bought);
            this._player.Inventory.Remove(cost_as_items);
            this.result = "bought_something";
        }

        //processes the player stat block as a whole frame
        private void ProcessStat(Match match, string s)
        {
            string[] char_creation_work_around = s.Split(new string[] { "SAVESAVE" }, StringSplitOptions.None);
            if(char_creation_work_around.Length == 2) {
                s = char_creation_work_around[1];
                    }
            string pattern_stat = @"(\S+)(?: Class)?:(?:[ \t]+|\*?)(\S+)(?: \S+)?";
            Regex r = new Regex(pattern_stat);
            MatchCollection mc = r.Matches(s);

            Dictionary<string, string> stats = new Dictionary<string, string>();
            foreach (Match m in mc)
            {
                if (m.Success)
                {
                    stats.Add(m.Groups[1].Value, m.Groups[2].Value);
                }
            }

            this._player.Stats = new PlayerStats(stats);
            this.result = "stats";
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

            //TODO: are we looking? did we move? are we in the same room?
            Room room = new Room();
            room.Name = room_info["name"];
            if (room_info.ContainsKey("desc"))
            {
                room.Description = room_info["desc"];
            }

            List<Entity> entities = new List<Entity>();
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
                    if (item_here2.EndsWith(" here."))
                    {
                        item_here2 = item_here2.Remove(item_here.IndexOf(" here."));
                    }

                    Item i = null;
                    string pattern = @"^(\d+) ([\S ]+)";
                    Match m4 = Regex.Match(item_here2, pattern);
                    if (m4.Success)
                    {
                        i = new Item(m4.Groups[2].Value.Trim());
                        i.Quantity = int.Parse(m4.Groups[1].Value);
                    }
                    else
                    {
                        i = new Item(item_here2);
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
                this.result = "look_room";
            }
            else
            {
                this._current_combat.UpdateRoom();
                this._current_room = room;
                this.result = "room";
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
                if (p.Name == entity.Name)
                    return p;
            }
            return null;
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
            else
            {
                combat_engaged = tokens[0];
            }

            if (combat_engaged.Contains("*Combat Engaged*"))
            {
                this._player.IsCombatEngaged = true;
                this._player.CombatEngagedCause = cause;
                this.result = "combat_engaged_start";
            }
            else if (combat_engaged.Contains("*Combat Off*"))
            {
                this.result = "combat_engaged_stop";
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
                    attack_cmd = "spell?";
                    break;

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
            carry = carry.Remove(keys_idx);
            inv.Add("carry", carry);
            inv.Add("cash", lines[cash_index]);
            inv.Add("enc", lines[enc_index]);

            string pattern = @"(\d+\s+)?([a-zA-Z\s]+?)(?:\s*\(([^)]+)\))?(?=\s*,|\s*$)";
            MatchCollection mc = Regex.Matches(carry, pattern);

            Dictionary<string, CarryableItem> items = new Dictionary<string, CarryableItem>();
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

                    CarryableItem item = new CarryableItem(item_name);
                    if (count == 0) { count = 1; }
                    item.Quantity = count;


                    if (m3.Groups[3].Value != string.Empty)
                    {
                        item = new EquipableItem(item);
                        (item as EquipableItem).Equiped = true;
                        (item as EquipableItem).Location = m3.Groups[3].Value;
                    }

                    if (items.ContainsKey(item.Name))
                    {
                        //this happens when you have an item named 'foo' in your inv
                        //  and an item named 'foo (Worn)' in your inv.
                        //  You have 2 foo's
                        items[item.Name].Quantity += item.Quantity;
                        if (item is EquipableItem && !(items[item.Name] is EquipableItem))
                        {
                            //if we have a foo in our inv and we found an equipped foo
                            EquipableItem new_item = item as EquipableItem;
                            new_item.Quantity += items[item.Name].Quantity;
                            items[item.Name] = new_item;
                        }
                    }
                    else
                    {
                        items.Add(item.Name, item);
                    }
                }
            }

            string weight = lines[enc_index].Split(':')[1].Split('-')[0].Trim();
            this._player.Inventory.SetInventory(items, weight);
            this.result = "inv";
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

                //there could be more title and/or the 'of <gang>' ending

                //idx++;
                //if(tokens.Length >= idx) {  continue; }







                //string line = lines[i];
                //int idx2 = line.IndexOf("-");
                //if (idx2== -1)
                //{
                //    idx2 = line.IndexOf(" x ");
                //}
                //string left_section = line.Substring(0, idx);

                //Player new_player = new Player(left_section.Substring(9).Trim());
                //new_player.Alignment = left_section.Substring(0, 8).Trim();
                
                //string right_section = line.Substring(idx + 2);
                //string[] right_section_tokens = right_section.Split(new string[] { " of " }, StringSplitOptions.RemoveEmptyEntries);
                //new_player.Title = right_section_tokens[0].Trim();
                //if (right_section_tokens.Length > 1)
                //{
                //    new_player.GangName = right_section_tokens[1].Trim();
                //}
                //new_player.Online = true;
                who_list.Add(new_player);
            }
            result = "who";
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

            result = "top";
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
                        if (result == "top")
                        {
                            current_player.Rank = player.Rank;
                            current_player.Exp = player.Exp;

                        }
                        else if (result == "who")
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
    }

    //This will match a string against many different regex patterns
    public class RegexMatcher
    {
        Dictionary<Regex, Action<Match, string>> regexPatterns;

        public RegexMatcher(Dictionary<string, Action<Match, string>> common_patterns)
        {
            this.regexPatterns = new Dictionary<Regex, Action<Match, string>>();
            foreach (KeyValuePair<string, Action<Match, string>> kvp in common_patterns)
            {
                Regex r = new Regex(kvp.Key, RegexOptions.Compiled);
                this.regexPatterns.Add(r, kvp.Value);
            }
        }

        public bool TryMatch(string input, out Match match, out Action<Match, string> callback)
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
            callback = null;
            return false;
        }
    }
}
