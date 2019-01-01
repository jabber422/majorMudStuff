using MMudObjects;
using MMudTerm_Protocols.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HomeList
{
    public partial class PlayerForm : Form
    {
        Engine engine;
        delegate void DoUpdate();
        public PlayerForm(Engine e)
        {
            InitializeComponent();
            this.engine = e;
            this.engine.DataUpdateEvent += DataUpdateEventHandler;
        }

        public void DataUpdateEventHandler(List<string> targetProperties)
        {
            this.Invoke(new DoUpdate(ReadPlayerObject));
        }

        void ReadPlayerObject()
        {
            this.label_ac.Text = this.engine.Player.Stats.ArmourClass.ToString() + " / " + this.engine.Player.Stats.DamageRes.ToString();
            this.label_agil.Text = this.engine.Player.Stats.Agility.ToString();
            this.label_charm.Text = this.engine.Player.Stats.Charm.ToString();
            
            if(this.engine.Player.Stats.Class != null)
                this.label_class.Text = this.engine.Player.Stats.Class.Name;
            this.label_exp.Text = this.engine.Player.Stats.Exp.ToString();
            this.label_health.Text = this.engine.Player.Stats.Health.ToString();
            this.label_hits.Text = this.engine.Player.Stats.CurHits.ToString() + " / " + this.engine.Player.Stats.MaxHits.ToString();
            this.label_int.Text = this.engine.Player.Stats.Intellect.ToString(); ;
            this.label_liveCp.Text = this.engine.Player.Stats.Lives.ToString() + " / " + this.engine.Player.Stats.CP.ToString();
            this.label_lvl.Text = this.engine.Player.Stats.Level.ToString(); ;
            this.label_ma.Text = this.engine.Player.Stats.MartialArts.ToString();
            this.label_mr.Text = this.engine.Player.Stats.MagicRes.ToString();
            this.label_name.Text = this.engine.Player.Stats.Name;
            this.label_per.Text = this.engine.Player.Stats.Perception.ToString();
            this.label_pick.Text = this.engine.Player.Stats.Picklocks.ToString();
            if (this.engine.Player.Stats.Race != null)
                this.label_race.Text = this.engine.Player.Stats.Race.Name;
            this.label_stealth.Text = this.engine.Player.Stats.Stealth.ToString();
            this.label_str.Text = this.engine.Player.Stats.Strength.ToString();
            this.label_thief.Text = this.engine.Player.Stats.Thievery.ToString();
            this.label_track.Text = this.engine.Player.Stats.Tracking.ToString();
            this.label_trap.Text = this.engine.Player.Stats.Traps.ToString();
            this.label_wis.Text = this.engine.Player.Stats.Willpower.ToString();
        }
    }
}
