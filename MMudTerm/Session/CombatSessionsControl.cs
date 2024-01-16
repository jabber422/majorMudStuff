using MMudObjects;
using MMudTerm.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MMudTerm.Session
{
    public partial class CombatSessionsControl : UserControl
    {
        SessionController _controller;
        public CombatSessionsControl(SessionController controller)
        {
            this._controller = controller;
            InitializeComponent();
            
        }

        internal void Update(EventType token)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<EventType>(Update), token);
            }
            else
            {
                switch (token)
                {
                    case EventType.Room:
                    case EventType.SearchFound:
                        this.UpdateRoom();
                        break;
                    case EventType.Tick:
                        //this.UpdateTick();
                        break;
                    case EventType.Combat:
                        this.UpdateCombat();
                        break;
                }
            }
        }

        private void UpdateRoom()
        {
            this.label_roomname_value.Text = this._controller._gameenv._current_room.Name;
            string also_here = "";
            foreach (Entity e in this._controller._gameenv._current_room.AlsoHere)
            {
                if (e == null) { continue; }
                also_here += e.Name + " ";
            }
            this.label_also_here_value.Text = also_here;

            CurrentCombat _curcombat = this._controller._gameenv._current_combat;
            List<CombatSession> to_remove =    new List<CombatSession>();
            foreach (CombatSession session in _curcombat._combats.Values)
            {
                Entity found = null;
                foreach (Entity e in this._controller._gameenv._current_room.AlsoHere)
                {
                    if (e == null) { continue; }
                    if (e.Name == session.target.Name)
                    {
                        found = e; break;
                    }
                }

                if (found == null)
                {
                    to_remove.Add(session);
                }
            }

            this._controller._gameenv._current_combat.Remove(to_remove);
        }


        private void UpdateCombat()
        {
            this.splitContainer1.Panel2.Controls.Clear();
            CurrentCombat _curcombat = this._controller._gameenv._current_combat;
            foreach(CombatSession session in _curcombat._combats.Values)
            {
                CombatSessionControl csc = new CombatSessionControl(session);
                csc.Location = new Point(10, 10);
                this.splitContainer1.Panel2.Controls.Add(csc);
            }

            this.textBox_crit.Text = this._controller._gameenv._current_combat.player_crits.ToString();
            this.textBox_hit.Text = this._controller._gameenv._current_combat.player_hits.ToString();
            this.textBox_dodge.Text = this._controller._gameenv._current_combat.player_dodge.ToString();
            this.textBox_hit_avg.Text = this._controller._gameenv._current_combat.player_misses.ToString();
            
            this.textBox_hit_rng_min.Text = this._controller._gameenv._current_combat.target_miss_player.ToString();
            this.textBox_hit_rng_max.Text = this._controller._gameenv._current_combat.target_hit_player.ToString();
            this.textBox_crit_rng_min.Text = this._controller._gameenv._current_combat.target_crit_player.ToString();
            this.textBox_crit_rng_max.Text = this._controller._gameenv._current_combat.target_dodge_player.ToString();

        }
    }
}
