using MMudObjects;
using MMudTerm.Game;
using System;

namespace MMudTerm.Session
{
    internal class EquipAllScript
    {
        private SessionController controller;
        private Action callback;
        private int abort_counter = 0;

        public EquipAllScript(SessionController controller, Action callback)
        {
            this.controller = controller;
            this.callback = callback;
        }

        internal void Execute()
        {
            this.controller._gameenv.NewGameEvent += _gameenv_NewGameEvent;
            EquipItem();
        }

        private void _gameenv_NewGameEvent(Game.EventType message)
        {
            switch (message)
            {
                case EventType.EquippedArmor:
                case EventType.EquippedWeapon:
                    EquipItem();
                    break;
                default:
                    if (abort_counter >= 15)
                    {
                        this.controller._gameenv.NewGameEvent -= _gameenv_NewGameEvent;
                        callback();
                    }
                    abort_counter++;
                    break;
            }
        }

        private void EquipItem()
        {
            foreach (var kvp in this.controller._gameenv._player.Inventory.Items)
            {
                if (kvp.Value.Equiped != true)
                {
                    this.controller.SendLine($"equip {kvp.Key}");
                    break;
                }
            }
            this.controller._gameenv.NewGameEvent -= _gameenv_NewGameEvent;
            callback();
        }
    }
}