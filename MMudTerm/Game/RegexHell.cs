using MMudObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.LinkLabel;

namespace MMudTerm.Game
{
    public class RegexHell
    {
        private RegexMatcher _matcher;
        Dictionary<string, EventType> _common_patterns = null;
        private string _verbs;
        private string _moves;
        private List<string> _death_msgs;

        private string mob_name_pattern = @"([A-Za-z ]+)";
        private string player_name_pattern = @"(\S+|[Yy]ou)";


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
            string verb_pattern_bow = "shoot a bolt at";

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

        public Dictionary<string, EventType> RegexList()
        {
            Dictionary<string, EventType> expanded = new Dictionary<string, EventType>();
            foreach(KeyValuePair<string, EventType> kvp in this.RegexListwMacros())
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

                expanded[key] = kvp.Value;
            }
            return expanded;
        }

        public Dictionary<string, EventType> RegexListwMacros()
        {
            if (this._common_patterns != null) { return this._common_patterns; }

            string pattern_stat = @"Name: ";
            string pattern_room = @"Obvious exits: ";
            string pattern_inv = @"You are carrying ";

            Dictionary<string, EventType> common_patterns = new Dictionary<string, EventType>();

            common_patterns.Add(pattern_room, EventType.Room);
            common_patterns.Add(pattern_stat, EventType.Stats);
            common_patterns.Add(pattern_inv, EventType.Inventory);
            common_patterns.Add(@"\*Combat ", EventType.Combat);
            common_patterns.Add(@"You are now resting.", EventType.Rest);
            //You now have no weapon readied.
            //You have removed padded helm.
            common_patterns.Add(@"You are now wearing ([A-Za-z ]+)\.", EventType.EquippedArmor);
            common_patterns.Add(@"You are now holding ([A-Za-z ]+)\.", EventType.EquippedWeapon);
            common_patterns.Add(@"Top Heroes of the Realm", EventType.Top);
            common_patterns.Add(@"   Current Adventurers", EventType.Who);
            common_patterns.Add(@"You just bought ([\S ]+) for ([\S ]+)\.", EventType.BoughtSomething);
            common_patterns.Add(@"You sold ([\S ]+) for ([\S ]+)\.", EventType.SoldSomething);

            

            common_patterns.Add(@"You pull the lever\. Off in the distance you hear a small click\.", EventType.MessagesThatMakeUsPauseWhileWalking);


            common_patterns.Add(@"There is no exit in that direction!", EventType.BadRoomMove);
            common_patterns.Add(@"You are not permitted in that room!", EventType.BadRoomMove);
            //You may not drag anyone through this exit.
            common_patterns.Add(@"There is a closed door in that direction!", EventType.BadRoomMoveClosedDoor);
            common_patterns.Add(@"The door is closed!", EventType.BadRoomMoveClosedDoor);

            common_patterns.Add(@"You hid ", EventType.HidItem);
            common_patterns.Add(@"You notice (.*) here.", EventType.SearchNotice);
            common_patterns.Add(@"Attempting to sneak...", EventType.SneakAttempt);
            common_patterns.Add(@"The following items are for sale here:", EventType.ForSaleList);
            common_patterns.Add(@"You picked up ", EventType.PickUpItem);
            common_patterns.Add(@"You took ", EventType.PickUpItem);
            common_patterns.Add(@"You dropped (\d+) ([\S ]+)\.", EventType.DropItem);
            common_patterns.Add(@"You dropped ([ \S]+).\r\n", EventType.DropItem);

            common_patterns.Add(@"You gain (\d+) experience\.", EventType.ExperienceGain);
            common_patterns.Add(@"Exp: (\d+) Level: (\d+) Exp needed for next level: (\d+) \((\d+)\) \[(\d+)%\]", EventType.ExperienceUpdate);

            //this message is always in the same block as entity death, need to handle how to split this up.
            //may when we parse the cash drop, we somehome stick the entity death message back on the todo like

            //common_patterns.Add(@"(\d+) (runic|platinum|gold|silver|copper) drop to the ground\.", ProcessCashDropFromEntity);
            common_patterns.Add(@"You have the following spells:", EventType.SpellBook);
            //          You have the following spells:
            //          Level Mana Short Spell Name
            //1   4    blur blur
            //2   4    illu illuminate
            //3   4    smit smite
            //4   3    fjet frost jet
            //4   6    rcol resist cold
            //5   6    shld ethereal shield
            //6   8    rfir resist fire


            common_patterns.Add(@"You are blind\.", EventType.PlayerBlind);


            //
            // string pattern_movement = "walks|crawls|scuttles|creeps|sneaks|oozes|flaps|lopes";

            //

            //[HP= 85 / KAI = 2]:***

            //Sorry to interrupt here, but the server will be shutting
            //down in 5 minutes for the nightly "auto-cleanup"
            //process.Please finish up and log off... thank you!






            string moves_to_attack = @"PLAYER moves to attack MOB\.";
            common_patterns.Add(moves_to_attack, EventType.CombatEngaged_3rdP);


            
            //You whap angry kobold thief for 9 damage!
            string pattern_do_hit = @"PLAYER (critically |surprise )?VERBS MOB for (\d+) damage!";
            common_patterns.Add(pattern_do_hit, EventType.CombatHit);

            //The nasty kobold thief lunges at you with their shortsword, but you dodge!
            //The orc rogue slashes you for 10 damage!
            string pattern_do_hit2 = @"The MOB (critically |surprise )?VERBS PLAYER for (\d+) damage!";
            //The small cave worm chomps you for 2 damage!
            common_patterns.Add(pattern_do_hit2, EventType.CombatHit);

            //Darmius whaps big lashworm for 12 damage!
            //The cave bear claws at you, but you dodge out of the way!

            string pattern_do_miss = @"The MOB VERBS at PLAYER with ";
            common_patterns.Add(pattern_do_miss, EventType.CombatMiss);
            string pattern_do_miss3 = @"The MOB VERBS at PLAYER!";
            common_patterns.Add(pattern_do_miss3, EventType.CombatMiss);
            string pattern_do_miss5 = @"The MOB VERBS at PLAYER, but PLAYER dodge out of the way!";
            common_patterns.Add(pattern_do_miss5, EventType.CombatMiss);
            string pattern_do_miss2 = @"PLAYER VERBS at MOB with ";

            //string pattern_do_miss = @"(\S+|You) " + verb + @" at ([\S ]+)";
            common_patterns.Add(pattern_do_miss2, EventType.CombatMiss);
            string pattern_do_miss4 = @"PLAYER VERBS at MOB!";
            common_patterns.Add(pattern_do_miss4, EventType.CombatMiss);


//------------------------------------



            string hung_up = @"PLAYER just (?:disconnected|hung up)!!!";
            common_patterns.Add(hung_up, EventType.SomeoneLeftTheGame);

            string left_the_realm = @"PLAYER just left the Realm\.";
            common_patterns.Add(left_the_realm, EventType.SomeoneLeftTheGame);

            string entered_the_realm = @"PLAYER just entered the Realm\.";
            common_patterns.Add(entered_the_realm, EventType.SomeoneEnteredTheGame);


            

            //string moved_into_room =  @"([A-Za-z ]+) walks into the room from the (\S+)\.";
            //string moved_into_room2 = @"A ([A-Za-z ]+) crawls into the room from the (\S+)\.";
            //string moved_into_room = @"A(?:n)? ([A-Za-z ]+) " + pattern_moves + @" in|into the room from (?:the )?(\S+)\.";
            string moved_into_room = @"(?:A |An )?([A-Za-z ]+) (?:MOVES) (?:in|into) the room from (?:the )?(\S+)\.";

            common_patterns.Add(moved_into_room, EventType.RoomSomethingMovedInto);

            string moved_into_room2 = @"A ([A-Za-z ]+) materializes in the room\.";
            common_patterns.Add(moved_into_room2, EventType.RoomSomethingMovedInto);

            string moved_out_of_room = @"(\S+) just left to the (\S+)\.";
            common_patterns.Add(moved_out_of_room, EventType.RoomSomethingMovedOut);


            //The kobold thief falls to the ground with a shrill cry.

            

            string entity_death = @"The ([A-Za-z ]+)(?:DEATHMSGS)";
            common_patterns.Add(entity_death, EventType.EntityDeath);
            //Your swing at filthbug hits, but glances off its armour.


            //Cthulhu casts ethereal shield on Cthulhu!
            string buff_spell_cast = @"(\S+) casts ([a-z ]+) on (\S+)!";
            common_patterns.Add(buff_spell_cast, EventType.BuffSpellCastSuccess_3rdP);

            //Cthulhu attempted to cast ethereal shield, but failed.
            string buff_spell_cast_fail = @"(\S+) attempted to cast ([a-z ]+), but failed.";
            common_patterns.Add(buff_spell_cast_fail, EventType.BuffSpellCastFail_3rdP);

            //The effects of the mummy's curse wears off!
            string buff_spell_wore_off = @"The effects of the ([A-Za-z ']+) wears off!";
            common_patterns.Add(buff_spell_wore_off, EventType.BuffExpired);

            string you_hear_movement = @"You hear movement to the (\S+)";
            
            common_patterns.Add(you_hear_movement, EventType.HearMovement);
            string you_hear_movement2 = @"You hear movement (\S+) you!";
            common_patterns.Add(you_hear_movement2, EventType.HearMovement);

            string failed_door_open = @"Your attempts to bash through fail!";
            common_patterns.Add(failed_door_open, EventType.BashDoorFailure);

            string bashed_door_open = @"You bashed the door open\.";
            common_patterns.Add(bashed_door_open, EventType.BashDoorSuccess);

            string open_door = @"The door is now (open|closed)\.";
            common_patterns.Add(open_door, EventType.DoorStateChange);

            string door_close_lock = @"The door to the (S+) just locked!";
            common_patterns.Add(door_close_lock, EventType.DoorLocked);

            string door_locked = @"The door is locked.";
            common_patterns.Add(door_locked, EventType.DoorLocked);

            string telepath = @"PLAYER telepaths: (.*)";
            common_patterns.Add(telepath, EventType.TelepathRcvd);

            string gos = @"PLAYER (gossips|auctions): (.*)";
            common_patterns.Add(gos, EventType.Gossip);

            //The room is very dark - you can't see anything

            //Health:    53/75    [70%]  Kai:   4/4   [100%]
            return common_patterns;
        }

        public Action<EventType, Match, string> Matched { get; internal set; }

        internal bool TryMatch(string data, out Match match, out EventType e)
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