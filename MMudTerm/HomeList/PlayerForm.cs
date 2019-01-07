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
            this.engine.Player.Stats.UpdatedPlayerStats += Stats_UpdatedPlayerStats;
        }

        private void Stats_UpdatedPlayerStats(object sender, PlayerStats e)
        {
            this.Invoke((MethodInvoker)delegate { ReadPlayerObject(); });
        }

        public void DataUpdateEventHandler(List<string> targetProperties)
        {
            if(targetProperties.Any(tar => tar.StartsWith("Player.Stats")))
            {
                this.Invoke(new DoUpdate(ReadPlayerObject));
            }

            if(targetProperties.Any( tar => tar.StartsWith("Player.Room")))
            {
                this.Invoke(new DoUpdate(ReadRoomObject));
            }
        }

        private void ReadRoomObject()
        {
            this.label_roomName.Text = this.engine.Player.Room.Name;
            this.textBox_roomDesc.Text = this.engine.Player.Room.Description;
            this.textBox_exits.Text = ToCsv(this.engine.Player.Room.RoomExits.ToList<object>());
            this.textBox_Here.Text = ToCsv(this.engine.Player.Room.AlsoHere.ToList<object>());
            this.textBox_Items.Text = ToCsv(this.engine.Player.Room.VisibleItems.ToList<object>());
            this.textBox_hiddenItems.Text = ToCsv(this.engine.Player.Room.HiddenItems.ToList<object>());
        }

        string ToCsv(List<object> obj)
        {
            string result = "";
            foreach(object o in obj)
            {
                result += o.ToString() + ", ";
            }

            return result.TrimEnd(new char[] { ',', ' ' }); ;
        }

        void ReadPlayerObject()
        {
            this.label_ac.Text = this.engine.Player.Stats.Armour_Class;
            this.label_agil.Text = this.engine.Player.Stats.Agility.ToString();
            this.label_charm.Text = this.engine.Player.Stats.Charm.ToString();
            this.label_class.Text = this.engine.Player.Stats.Class;
            this.label_exp.Text = this.engine.Player.Stats.Exp.ToString();
            this.label_health.Text = this.engine.Player.Stats.Health.ToString();
            this.label_hits.Text = this.engine.Player.Stats.Hits;
            this.label_int.Text = this.engine.Player.Stats.Intellect.ToString(); ;
            this.label_liveCp.Text = this.engine.Player.Stats.Lives_CP;
            this.label_lvl.Text = this.engine.Player.Stats.Level.ToString(); ;
            this.label_ma.Text = this.engine.Player.Stats.Martial_Arts.ToString();
            this.label_mr.Text = this.engine.Player.Stats.MagicRes.ToString();
            this.label_name.Text = this.engine.Player.Stats.Name;
            this.label_per.Text = this.engine.Player.Stats.Perception.ToString();
            this.label_pick.Text = this.engine.Player.Stats.Picklocks.ToString();
            this.label_race.Text = this.engine.Player.Stats.Race;
            this.label_stealth.Text = this.engine.Player.Stats.Stealth.ToString();
            this.label_str.Text = this.engine.Player.Stats.Strength.ToString();
            this.label_thief.Text = this.engine.Player.Stats.Thievery.ToString();
            this.label_track.Text = this.engine.Player.Stats.Tracking.ToString();
            this.label_trap.Text = this.engine.Player.Stats.Traps.ToString();
            this.label_wis.Text = this.engine.Player.Stats.Willpower.ToString();
        }
    }
}
