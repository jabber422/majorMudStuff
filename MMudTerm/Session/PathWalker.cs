using System;
using System.Collections.Generic;
using MMudTerm.Game;
using System.Diagnostics;
using MMudObjects;

namespace MMudTerm.Session
{
    public class PathWalker : IDisposable
    {
        SessionController m_controller;
        private List<MudPath> path;
        int path_index = 0;
        int step_index = 0;
        private MudPath endroom;
        private bool is_loop;

        public bool Active { get; set; }

        public PathWalker(List<MudPath> path, SessionController m_controller, bool is_loop = false)
        {
            this.path = path;
            this.m_controller = m_controller;
            this.m_controller._gameenv.NewGameEvent += _gameenv_NewGameEvent;
            this.endroom = this.path[this.path.Count - 1];
            this.is_loop = is_loop;
            this.Active = true;
        }

        private void _gameenv_NewGameEvent(EventType message)
        {
            if (!Active) return;
            switch (message)
            {
                case EventType.Room:
                    MoveToNextRoom();
                    break;
                case EventType.BadRoomMove:
                    if (step_index == 0)
                    {
                        path_index--;
                    }
                    else
                    {
                        step_index--;
                    }
                    break;
                case EventType.BadRoomMoveClosedDoor:
                    if (step_index == 0)
                    {
                        path_index--;
                    }
                    else
                    {
                        step_index--;
                    }
                    break;
                case EventType.BashDoorSuccess:
                case EventType.MessagesThatMakeUsPauseWhileWalking:
                    this.m_controller.SendLine();
                    break;
            }
        }

        public void MoveToNextRoom()
        {
            if (this.m_controller._gameenv.Monitor_Combat) {
                if (this.m_controller._gameenv._player.IsCombatEngaged)
                {
                    //in combat and comat is on, don't move
                    Debug.WriteLine("Won't move, in combat and combat is on");
                    return;
                }
                else if (this.m_controller._gameenv._current_room.AlsoHere.GetFirst("baddie") != null)
                {
                    //not in combat and combat is on, but there is something int he room to kill

                    Debug.WriteLine("Won't move, in combat and combat is on");
                    return;
                }
                else
                {

                }
            }
            var player_health = (float)this.m_controller._gameenv._player.Stats.CurHits / (float)this.m_controller._gameenv._player.Stats.MaxHits;
            if(player_health < 0.75) { return; }
            if (this.m_controller._gameenv._player.IsResting && this.m_controller._gameenv.Monitor_Rest && player_health < 0.75)
            {
                Debug.WriteLine("Won't move, need to rest and resting is on");
                return;
            }

            var current_room = this.m_controller._gameenv._current_room;
            if(current_room.MegaMudRoomHash == this.endroom.EndRoomHashCode)
            {
                if (this.is_loop)
                {
                    if(path.Count == path_index)
                    {
                        path_index--;
                    }
                }
                else
                {
                    //done
                    this.m_controller._gameenv.NewGameEvent -= _gameenv_NewGameEvent;
                    this.m_controller.PathWalkerFinished();
                    return;
                }
            }
            MudPathStep next_step = null;
            try
            {
                next_step = path[path_index].Steps[step_index];
                if (current_room.MegaMudRoomHash == next_step.RoomHashCode)
                {
                    foreach (RoomExit exit in current_room.RoomExits)
                    {
                        if (exit.ShortName.ToUpper() != next_step.Direction.ToUpper()) continue;
                        if (exit.IsDoor && !exit.IsOpen)
                        {
                            this.m_controller.SendLine("bash " + next_step.Direction);
                            return;
                        }

                    }
                    this.m_controller.SendLine(next_step.Direction);
                    step_index++;
                    if (step_index >= path[path_index].Steps.Count)
                    {
                        step_index = 0;
                        path_index++;
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
            }
            
            
        }

        public void Dispose()
        {
            this.m_controller._gameenv.NewGameEvent -= _gameenv_NewGameEvent;
        }

        public class DialogEventArgs : EventArgs
        {
            public string AdditionalInfo { get; set; }
            // Add other properties as needed
        }
    }
}
