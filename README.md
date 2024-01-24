# majorMudStuff
Different Majormud related projects in C# .net 4.0

## Next Steps... aka The Path to Alpha
- Setup/Teardown flows
-- BBS Clean up causes a crash, this flow needs to be ironed out.  Setup/Teardown flows need to be checked and hardened.
-- Telnet handshake is very basic - BBS's that send Device Status Report (DSR) will no render Ansi until we do a =a.
-- Reverse Colors needs to be implemented, test it via 'train stats'
-- Menu navigation is not working with arrow keys.  (Ie push Up while in train stats view)  This doesn't work today.  I think part of this is due to above
-- Figure out why Bear's BBS main menu reders incorreclty
- Pathing
-- The transition from Walk to somewhere, then loop that place needs to be implements
-- Pathing GUI button states need to be fixed.  Hitting stop while stopped should continue the loop etc...
-- Stop moving/looping on player input needs to be implemented
-- Need to detect when we bash things that are not a door, causes a pause until the next room draw when moving and opening doors.
- Monitoring
-- Magic/Buffs tracking need to be implemented
-- Health options need to be implement.  User can set % to heal, attack etc... copy MegaMud for this.
-- Same thing for magic, reseting
-- Combat tracking stats need to be finished.  Need to figure out how to detect a 'round'
- Implement a Backlog
- Need to break out Macro's into a form.
- +More

## Goals to be a succesful POC - DONE
- Be able to process the game stream into organized data - Done
-- Stats, Who, Room, TopLists, Combat, Resting, HP/MP ticks - Done
- Render ANSI in a terminal window - Done
- Script the Mummy @ lvl 10 and ignore the blindness successfully
- Two Sessions running at the same time, same server, 2 chars.  Be able to demonstrate that the sessions can 'see' each other.
-- A Party window with Tick #s but none of the MegaMud handshake
- MME Integration
-- Somehow show the stats for an item?  ToolTip?
-- Calculate Swings based on the current Player data
- Logon/Disconnect flows work 100%


## MMudTerm is the main Project
This is the starting point for the project.  There is a form capable of creating 'session' forms.  
Each session takes a config with bbs information.  It uses this to connect to the remote server
Each session has a decoder MMudTerm_Protocols, this converts byte[] buffer into Queue\<TermCmd\>
Each session has a controller.  The controller handles the data stream from the bbs and converts it to Queue\<TermCmd\>
Each session has a Terminal view.  The controller sends the Q to the Terminal to be rendered
Each session has state.  Offline, Connected, Logon, InGame. The controller sends the Q to the state object to be handled
Each SessionState implements: public SessionsState Handle(Queue\<TermCmds\>).  The method processes the current Q and returns the next state.
- Offline - Disconnected State
- Connected - We called connect on the socket, we only process IAC commands, everything else is ignored
- Logon - Now we are just looking for strings, trying to logon to the bbs and get into the game
- InGame - The state we want to be in.

SessionStateInGame, where as previous states process the TermCmds 1 at a time, the In Game state buffers TermCmds.
- Handle(Queue\<TermCmds\>) will buffer all strings, cr and lf TermCmds. black and white mud, no ANSI color.
- Handle(Queue\<TermCmds\>) will chunk up the strings based on the '[HP=#]:' or '[HP=#/MA=#]:' symbols (And the Resting Flag)
-- Everything between these symbols is buffered up and Sent to the Game Processor.  This means the Who list, no matter how long, is one block of data.
-- Most blocks look like 
```look\r\n
Room Name\r\n
Description string\r\n
You notice a club.\r\n
Also here: guardsman.\r\n
Obvious exits: east, west\r\n
```
SessionStateInGame has a MajorMudBbsGame object, this object represents the current state of MajorMud. see MMudObjects.
SessionsStateInGame implements public SessionState Handle(string game_message_string). This will Identify the string and Process it as required
MajorMudBbsGame has many objects to represet the game work
game.player - The current player, their stats, hps, inventory, location... everything about the player
game.current_room - same as game.player.current_room
game.who - list of Player objects representing the players in the Game
If a game_message_string is identified, it will be decoded into the appropriate Object and send to game.Update(data, "what")
- game.Update(stats_object, 'stats')
- game.Update(room_object, 'current_room')
- game.Update(room_object, 'look_room')
- game.Update(stats_object, 'hptick')


