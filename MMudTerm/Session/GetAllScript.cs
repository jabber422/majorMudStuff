using MMudTerm.Game;
using System;

namespace MMudTerm.Session
{
    internal class GetAllScript
    {
        private SessionController controller;
        private Action toolStripButton_get_all_Click_callback;
        private int abort_counter = 0;

        public GetAllScript(SessionController controller, Action toolStripButton_get_all_Click_callback)
        {
            this.controller = controller;
            this.toolStripButton_get_all_Click_callback = toolStripButton_get_all_Click_callback;
        }

        internal void Execute()
        {
            this.controller._gameenv.NewGameEvent += _gameenv_NewGameEvent;
            GetItem();
        }

        private void _gameenv_NewGameEvent(Game.EventType message)
        {
            switch (message)
            {
                case EventType.PickUpItem:
                case EventType.PickUpCoins:
                    GetItem();
                    break;
                case EventType.Room:
                    if (abort_counter >= 10)
                    {
                        this.controller._gameenv.NewGameEvent -= _gameenv_NewGameEvent;
                        toolStripButton_get_all_Click_callback();
                    }
                    abort_counter++;
                    break;
            }
        }

        private void GetItem()
        {
            foreach(var kvp in this.controller._gameenv._current_room.VisibleItems)
            {
                this.controller.SendLine($"get {kvp.Key}");
                break;
            }

            abort_counter++;
            if (this.controller._gameenv._current_room.VisibleItems.Count == 0)
            {
                this.controller._gameenv.NewGameEvent -= _gameenv_NewGameEvent;
                toolStripButton_get_all_Click_callback();
            }

            if(abort_counter >= 10)
            {
                this.controller._gameenv.NewGameEvent -= _gameenv_NewGameEvent;
                toolStripButton_get_all_Click_callback();
            }
        }
    }
}