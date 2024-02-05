using global::MMudObjects;
using global::MMudTerm.Session;
using System.Collections.Generic;

namespace MMudTerm.Game
{
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
        public int player_max_damage = 0;
        public int player_min_damage = 99999;
        public int player_max_crit_damage = 0;
        public int player_min_crit_damage = 99999;


        public int target_miss_player = 0;
        public int target_hit_player = 0;
        public int target_crit_player = 0;
        public int target_dodge_player = 0;
        public float player_crit_average = 0.0f;
        public float player_hit_average = 0.0f;
        public float player_total_crit_dmg = 0.0f;
        public float player_total_hit_dmg = 0.0f;

        public float player_dodge_rate { get
            {
                float dodge_rate = (float)player_dodge / (float)(target_miss_player +player_dodge);
                dodge_rate *= 100;
                return dodge_rate;
            } }

        public float player_miss_rate
        {
            get
            {
                float hit_rate = (float)player_misses / (float)(player_hits + player_misses);
                hit_rate *= 100;
                return hit_rate;
            }
        }

        public float player_hit_rate { get
            {
                float hit_rate = (float)player_hits / (float)(player_hits + player_misses);
                hit_rate *= 100;
                return hit_rate;
            } 
        }

        public float player_crit_rate
        {
            get
            {
                float hit_rate = (float)player_crits / (float)(player_hits + player_misses);
                hit_rate *= 100;
                return hit_rate;
            }
        }

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
                if (dmg_done > this.player_max_crit_damage) this.player_max_crit_damage = dmg_done;
                if (dmg_done < this.player_min_crit_damage) this.player_min_crit_damage = dmg_done;
                this.player_total_crit_dmg += dmg_done;
                this.player_crit_average = (float)(this.player_total_crit_dmg) / (float)this.player_crits;

            }
            else //surprise too
            {
                this.player_hits++;
                if (dmg_done > this.player_max_damage) this.player_max_damage = dmg_done;
                if (dmg_done < this.player_min_damage) this.player_min_damage = dmg_done;
                this.player_total_hit_dmg += dmg_done;
                this.player_hit_average = (float)this.player_total_hit_dmg / (float)this.player_hits;
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
}
