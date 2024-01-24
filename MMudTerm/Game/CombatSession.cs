using global::MMudObjects;
using System.Collections.Generic;

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
}
