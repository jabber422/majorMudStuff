using MMudObjects;
using MMudTerm.Session;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.LinkLabel;

namespace MMudTerm.Game
{
    public class RegexHell
    {
        private RegexMatcher _matcher;
        Dictionary<string, List<(EventType, Object)>> _common_patterns = null;
        private string _verbs;
        private string _moves;
        private List<string> _death_msgs;

        private string mob_name_pattern = @"([A-Za-z ]+)";
        private string player_name_pattern = @"(\S+|[Yy]ou|[Hh]e|[Ss]he)";
        private string cast_pattern = @"(?:casts?|invokes? the)";


        public RegexHell()
        {
            this._common_patterns = RegexListwMacros();                   
        }

        public List<string> EntityDeathStrings(){
            if (this._death_msgs != null) { return this._death_msgs; }
            List<string> death_msgs = new System.Collections.Generic.List<string>();
            death_msgs.Add(@" collapes with a dull thump");
            death_msgs.Add(@" collapses, its legs curling tightly around it");
            death_msgs.Add(@" collapses with a groan");
            death_msgs.Add(@" collapses with a grunt.");
            death_msgs.Add(@" collapses in a filthy heap\.");
            death_msgs.Add(@" collapses into a broken heap\.");
            death_msgs.Add(@" falls to the ground with a yelp, and is still");
            death_msgs.Add(@" falls to the ground with a shrill cry\.");
            death_msgs.Add(@" falls to the ground with a grunt!");
            death_msgs.Add(@" falls to the ground with a tortured squeak");
            death_msgs.Add(@" dissolves into a puddle of bluish goo");
            death_msgs.Add(@" falls dead at your feet\.");
            death_msgs.Add(@" squeaks loudly, and falls to the ground!");
            death_msgs.Add(@" crumbles into a pile of dust\.");
            death_msgs.Add(@"'s wrapping unravels, revealing nothing but dust\.");
            death_msgs.Add(@" vanishes with an eerie wail\.");

            return death_msgs;
        }

        //VERBS
        public string Verbs()
        {
            if (this._verbs != null) { return this._verbs; }
            string verb_pattern_club_3rd = "smashes|clobbers|slams|whaps|smacks|beats";
            string verb_pattern_pierce_3rd = "lunges|stabs|impales|skewers";
            string verb_pattern_cut_3rd = "slices|slashes|cuts|hacks";
            string verb_pattern_natural_3rd = "chomps|bites|claws|rips|whips|chills|punches|kicks|jumpkicks";
            string verb_pattern_miss_3rd = "swipes|flails|snaps|lashes|swings|reaches out";    
            string verb_pattern_bow_3rd = "shoots a bolt";
            string present_tense_3rd_person_verb = $"{verb_pattern_bow_3rd}|{verb_pattern_club_3rd}|{verb_pattern_pierce_3rd}|{verb_pattern_cut_3rd}|{verb_pattern_natural_3rd}|{verb_pattern_miss_3rd}";

            string verb_pattern_club = "smash|clobber|slam|whap|smack";
            string verb_pattern_pierce = "lunge|stab|impale|skewer";
            string verb_pattern_cut = "slice|slash|cut|hack";
            string verb_pattern_natural = "chomp|bite|claw|rip|whip|punch|kick|jumpkick";
            string verb_pattern_miss = "swipe|flail|snap|lash|swing|shoot a bolt";
            string verb_pattern_bow = "shoot a bolt at|hurl your shuriken and strike";

            string present_tense_1st_person_verb = $"{verb_pattern_bow}|{verb_pattern_club}|{verb_pattern_pierce}|{verb_pattern_miss}|{verb_pattern_cut}|{verb_pattern_natural}";

            string verb = $"(?:{present_tense_1st_person_verb}|{present_tense_3rd_person_verb})";
            return verb;
        }
        
        //MOVES
        public string MoveStrings()
        {
            if(this._moves != null) { return this._moves; }
            string pattern_movement = "walks|crawls|scuttles|creeps|sneaks|oozes|flaps|lopes";
            return pattern_movement;
        }

        public Dictionary<string, List<(EventType, Object)>> RegexList()
        {
            Dictionary<string, List<(EventType, Object)>> expanded = ProcessDict(this.RegexListwMacros());
            
            Dictionary<string, List<(EventType, Object)>> expanded2 = ProcessDict(MessagesCache.RegexListwMacros());

            foreach(var kvp in expanded2)
            {
                if (expanded.ContainsKey(kvp.Key))
                {
                }
                else
                {
                    expanded.Add(kvp.Key, kvp.Value);
                }
            }

            return expanded;
        }

        private Dictionary<string, List<(EventType, Object)>> ProcessDict(Dictionary<string, List<(EventType, Object)>> dictionary)
        {
            Dictionary<string, List<(EventType, Object)>> expanded = new Dictionary<string, List<(EventType, Object)>>();
            foreach (KeyValuePair<string, List<(EventType, Object)>> kvp in dictionary)
            {
                string key = kvp.Key;
                if (key.Contains("VERBS"))
                {
                    key = key.Replace("VERBS", Verbs());
                }
                if (key.Contains("MOVES"))
                {
                    key = key.Replace("MOVES", MoveStrings());
                }
                if (key.Contains("DEATHMSGS"))
                {
                    key = key.Replace("DEATHMSGS", string.Join("|", EntityDeathStrings()));
                }
                if (key.Contains("PLAYER"))
                {
                    key = key.Replace("PLAYER", player_name_pattern);
                }
                if (key.Contains("MOB"))
                {
                    key = key.Replace("MOB", mob_name_pattern);
                }
                if (key.Contains("CAST"))
                {
                    key = key.Replace("CAST", cast_pattern);
                }

                expanded[key] = kvp.Value;
            }

            return expanded;
        }

        public Dictionary<string, List<(EventType, Object)>> RegexListwMacros()
        {
            if (this._common_patterns != null) { return this._common_patterns; }

            string pattern_stat = @"Name: ";
            string pattern_room = @"Obvious exits: ";
            string pattern_inv = @"You are carrying ";

            Dictionary<string, List<(EventType, Object)>> common_patterns = new Dictionary<string, List<(EventType, Object)>>();

            
            common_patterns.Add(pattern_room, new List<(EventType, Object)> { (EventType.Room, null) });
            common_patterns.Add(pattern_stat, new List<(EventType, Object)> { (EventType.Stats, null) });
            common_patterns.Add(pattern_inv, new List<(EventType, Object)> { (EventType.Inventory, null) });
            common_patterns.Add(@"\*Combat ", new List<(EventType, Object)> { (EventType.CombatEngaged, null) });
            common_patterns.Add(@"You are now resting.", new List<(EventType, Object)> { (EventType.Rest, null) });
            //You now have no weapon readied.
            //You have removed padded helm.
            common_patterns.Add(@"You are now wearing ([A-Za-z ]+)\.", new List<(EventType, Object)> { (EventType.EquippedArmor, null) });
            common_patterns.Add(@"You are now holding ([A-Za-z ]+)\.", new List<(EventType, Object)> { (EventType.EquippedWeapon, null) });
            common_patterns.Add(@"Top Heroes of the Realm", new List<(EventType, Object)> { (EventType.Top, null) });
            common_patterns.Add(@"   Current Adventurers", new List<(EventType, Object)> { (EventType.Who, null) });
            common_patterns.Add(@"You just bought ([\S ]+) for ([\S ]+)\.", new List<(EventType, Object)> { (EventType.BoughtSomething, null) });
            common_patterns.Add(@"You sold ([\S ]+) for ([\S ]+)\.", new List<(EventType, Object)> { (EventType.SoldSomething, null) });

            common_patterns.Add(@"The room is very dark - you can't see anything", new List<(EventType, Object)> { (EventType.RoomLightToLow, null) });
            common_patterns.Add(@"Your search revealed nothing\.", new List<(EventType, Object)> { (EventType.SearchNothing, null) });

            common_patterns.Add(@"You pull the lever\. Off in the distance you hear a small click\.", new List<(EventType, Object)> { (EventType.MessagesThatMakeUsPauseWhileWalking, null) });


            common_patterns.Add(@"There is no exit in that direction!", new List<(EventType, Object)> { (EventType.BadRoomMove, null) });
            common_patterns.Add(@"You are not permitted in that room!", new List<(EventType, Object)> { (EventType.BadRoomMove, null) });
            //You may not drag anyone through this exit.
            common_patterns.Add(@"There is a closed door in that direction!", new List<(EventType, Object)> { (EventType.BadRoomMoveClosedDoor, null) });
            common_patterns.Add(@"The door is closed!", new List<(EventType, Object)> { (EventType.BadRoomMoveClosedDoor, null) });

            common_patterns.Add(@"You notice (.*) here\.", new List<(EventType, Object)> { (EventType.SeeHiddenItem, null) });
            common_patterns.Add(@"Attempting to sneak...", new List<(EventType, Object)> { (EventType.SneakAttempt, null) });
            common_patterns.Add(@"The following items are for sale here:", new List<(EventType, Object)> { (EventType.ForSaleList, null) });
            common_patterns.Add(@"You picked up ", new List<(EventType, Object)> { (EventType.PickUpItem, null) });
            common_patterns.Add(@"You took ", new List<(EventType, Object)> { (EventType.PickUpItem, null) });
            common_patterns.Add(@"You dropped (\d+) ([\S ]+)\.", new List<(EventType, Object)> { (EventType.DropItem, null) });
            common_patterns.Add(@"You dropped ([ \S]+).", new List<(EventType, Object)> { (EventType.DropItem, null) });
            common_patterns.Add(@"You hid (\d+) ([\S ]+)\.", new List<(EventType, Object)> { (EventType.HidItem, null) });
            common_patterns.Add(@"You hid ([ \S]+).", new List<(EventType, Object)> { (EventType.HidItem, null) });

            common_patterns.Add(@"You gain (\d+) experience\.", new List<(EventType, Object)> { (EventType.ExperienceGain, null) });
            common_patterns.Add(@"Exp: (\d+) Level: (\d+) Exp needed for next level: (\d+) \((\d+)\) \[(\d+)%\]", new List<(EventType, Object)> { (EventType.ExperienceUpdate, null) });

            //this message is always in the same block as entity death, need to handle how to split this up.
            //may when we parse the cash drop, we somehome stick the entity death message back on the todo like

            //common_patterns.Add(@"(\d+) (runic|platinum|gold|silver|copper) drop to the ground\.", ProcessCashDropFromEntity);
            common_patterns.Add(@"You have the following spells:", new List<(EventType, Object)> { (EventType.SpellBook, null) });
            //          You have the following spells:
            //          Level Mana Short Spell Name
            //1   4    blur blur
            //2   4    illu illuminate
            //3   4    smit smite
            //4   3    fjet frost jet
            //4   6    rcol resist cold
            //5   6    shld ethereal shield
            //6   8    rfir resist fire


            common_patterns.Add(@"You are blind\.", new List<(EventType, Object)> { (EventType.PlayerBlind, null) });


            //
            // string pattern_movement = "walks|crawls|scuttles|creeps|sneaks|oozes|flaps|lopes";

            //

            //[HP= 85 / KAI = 2]:***

            //Sorry to interrupt here, but the server will be shutting
            //down in 5 minutes for the nightly "auto-cleanup"
            //process.Please finish up and log off... thank you!


            common_patterns.Add("To do this action, you must turn off your evil warnings.", new List<(EventType, Object)> { (EventType.CombatEngagedEvilWarning, null) });



            string moves_to_attack = @"PLAYER moves to attack MOB\.";
            common_patterns.Add(moves_to_attack, new List<(EventType, Object)> { (EventType.CombatEngaged_3rdP, null) });

            string pattern_do_hit2 = @"The MOB (critically |surprise )?VERBS PLAYER for (\d+) damage!";
            //The small cave worm chomps you for 2 damage!
            common_patterns.Add(pattern_do_hit2, new List<(EventType, Object)> { (EventType.CombatHit, null) });

            //You whap angry kobold thief for 9 damage!
            string pattern_do_hit = @"PLAYER (critically |surprise )?VERBS MOB for (\d+) damage!";
            common_patterns.Add(pattern_do_hit, new List<(EventType, Object)> { (EventType.CombatHit, null) });

            //The nasty kobold thief lunges at you with their shortsword, but you dodge!
            //The orc rogue slashes you for 10 damage!
            

            //Darmius whaps big lashworm for 12 damage!
            //The cave bear claws at you, but you dodge out of the way!
            List<string> patterns = new List<string>();
            //Patterns when the monster misses us or another Player
            patterns.Add(@"The MOB VERBS at PLAYER with ");
            patterns.Add(@"The MOB VERBS (?:at|for) PLAYER!");
            patterns.Add(@"The MOB VERBS at PLAYER, but PLAYER dodges? out of the way!");
            //when we miss, or another player
            patterns.Add(@"PLAYER VERBS at MOB with ");
            patterns.Add(@"PLAYER VERBS at MOB!");
            patterns.Add(@"PLAYER VERBS MOB!");  //3p only

            foreach (string s in patterns) common_patterns.Add(s, new List<(EventType, Object)> { (EventType.CombatMiss, null) });
            

            //------------------------------------



            string hung_up = @"PLAYER just (?:disconnected|hung up)!!!";
            common_patterns.Add(hung_up, new List<(EventType, Object)> { (EventType.SomeoneLeftTheGame, null) });

            string left_the_realm = @"PLAYER just left the Realm\.";
            common_patterns.Add(left_the_realm, new List<(EventType, Object)> { (EventType.SomeoneLeftTheGame, null) });

            string entered_the_realm = @"PLAYER just entered the Realm\.";
            common_patterns.Add(entered_the_realm, new List<(EventType, Object)> { (EventType.SomeoneEnteredTheGame, null) });


            

            //string moved_into_room =  @"([A-Za-z ]+) walks into the room from the (\S+)\.";
            //string moved_into_room2 = @"A ([A-Za-z ]+) crawls into the room from the (\S+)\.";
            //string moved_into_room = @"A(?:n)? ([A-Za-z ]+) " + pattern_moves + @" in|into the room from (?:the )?(\S+)\.";
            string moved_into_room = @"(?:A |An )?MOB (?:MOVES) (?:in|into) the room from (?:the )?(\S+)\.";

            common_patterns.Add(moved_into_room, new List<(EventType, Object)> { (EventType.RoomSomethingMovedInto, null) });

            string moved_into_room2 = @"A MOB materializes in the room\.";
            common_patterns.Add(moved_into_room2, new List<(EventType, Object)> { (EventType.RoomSomethingMovedInto, null) });

            string moved_out_of_room = @"MOB just left to the (\S+)\.";
            common_patterns.Add(moved_out_of_room, new List<(EventType, Object)> { (EventType.RoomSomethingMovedOut, null) });


        //The kobold thief falls to the ground with a shrill cry.

//        Data: with 200000
//You withdrew 200000 copper farthings.
//Not Matched
//------------------------------------

//------------------------------------
//Data: give 20 pl to cas
//You gave Caslus 20 platinum
//Not Matched

            //How the f... do i know when the user makes a player move?



            string entity_death = @"The MOB(?:DEATHMSGS)";
            common_patterns.Add(entity_death, new List<(EventType, Object)> { (EventType.EntityDeath, null) });
            
            common_patterns.Add("You have already cast a spell this round!", new List<(EventType, Object)> { (EventType.BuffSpellAlreadyCastRound, null) });
            
            //Cthulhu attempted to cast ethereal shield, but failed.
            //You attempt to cast ethereal shield, but fail.
            common_patterns.Add(@"PLAYER attempted to cast ([a-z ]+), but failed\.", new List<(EventType, Object)> { (EventType.BuffSpellCastFail_3rdP, null) });
            common_patterns.Add(@"PLAYER attempt to cast ([a-z ]+), but fail\.", new List<(EventType, Object)> { (EventType.BuffSpellCastFail_3rdP, null) });
            //You cast smite on Darmius!
            //Cthulhu casts ethereal shield on Cthulhu!
            //TheGorn invokes the way of the tiger on TheGorn!
            //You cast ethereal shield, and a shimmering field forms about you.
            common_patterns.Add(@"PLAYER (?:casts?|invokes? the) ([a-z ]+) on PLAYER!", new List<(EventType, Object)> { (EventType.BuffSpellCastSuccess_3rdP, null) });
            common_patterns.Add(@"PLAYER (?:casts?|invokes? the) ([a-z ]+) at PLAYER, ", new List<(EventType, Object)> { (EventType.BuffSpellCastSuccess_3rdP, null) });
            common_patterns.Add(@"PLAYER (?:casts?|invokes? the) ([a-z ]+) on PLAYER\.", new List<(EventType, Object)> { (EventType.BuffSpellCastSuccess_3rdP, null) });
            common_patterns.Add(@"PLAYER (?:casts?|invokes? the) ([a-z ]+) on PLAYER, ", new List<(EventType, Object)> { (EventType.BuffSpellCastSuccess_3rdP, null) });
            common_patterns.Add(@"PLAYER (?:casts?|invokes? the) ([a-z ]+), ", new List<(EventType, Object)> { (EventType.BuffSpellCastSuccess_3rdP, null) });
            
            //The effects of the mummy's curse wears off!
            //common_patterns.Add(@"The effects of the ([A-Za-z ']+) wears? off!", new List<(EventType, Object)> { (EventType.BuffExpired, null) });
            //common_patterns.Add(@"The effects of ([A-Za-z ']+) wears? off!", new List<(EventType, Object)> { (EventType.BuffExpired, null) });
            //common_patterns.Add(@"PLAYER slow down\.", new List<(EventType, Object)> { (EventType.BuffExpired, null) });

            string you_hear_movement = @"You hear movement to the (\S+)";
            
            common_patterns.Add(you_hear_movement, new List<(EventType, Object)> { (EventType.HearMovement, null) });
            string you_hear_movement2 = @"You hear movement (\S+) you!";
            common_patterns.Add(you_hear_movement2, new List<(EventType, Object)> { (EventType.HearMovement, null) });

            string failed_door_open = @"Your attempts to bash through fail!";
            common_patterns.Add(failed_door_open, new List<(EventType, Object)> { (EventType.BashDoorFailure, null) });

            string bashed_door_open = @"You bashed the door open\.";
            common_patterns.Add(bashed_door_open, new List<(EventType, Object)> { (EventType.BashDoorSuccess, null) });

            string open_door = @"The door is now (open|closed)\.";
            common_patterns.Add(open_door, new List<(EventType, Object)> { (EventType.DoorStateChange, null) });

            string door_close_lock = @"The door to the (\S+) just locked!";
            common_patterns.Add(door_close_lock, new List<(EventType, Object)> { (EventType.DoorLocked, null) });

            string door_locked = @"The door is locked.";
            common_patterns.Add(door_locked, new List<(EventType, Object)> { (EventType.DoorLocked, null) });

            string telepath = @"PLAYER telepaths: (.*)";
            common_patterns.Add(telepath, new List<(EventType, Object)> { (EventType.TelepathRcvd, null) });

            string gos = @"PLAYER (gossips|auctions): (.*)";
            common_patterns.Add(gos, new List<(EventType, Object)> { (EventType.Gossip, null) });

            //The room is very dark - you can't see anything

            //Health:    53/75    [70%]  Kai:   4/4   [100%]
            return common_patterns;
        }

        public Action<EventType, Match, string> Matched { get; internal set; }

        internal bool TryMatch(string data, out Match match, out List<(EventType, Object)> e)
        {
            if(this._matcher == null)
            {
                this._matcher = new RegexMatcher(this.RegexList());
            }

            return this._matcher.TryMatch(data, out match, out e);
        }

        internal void Reload()
        {
            this._matcher = new RegexMatcher(this.RegexList());
        }
    }
}