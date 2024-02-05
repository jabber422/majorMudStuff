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
    public partial class HealthControl : UserControl
    {
        private SessionController controller;

        public HealthControl(SessionController controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        public HealthControlValues Save()
        {
            var data =  new HealthControlValues();
            data.RestMax = numericUpDown1.Value;
            data.RestIfBelow = numericUpDown2.Value;
            data.HealRest = numericUpDown4.Value;
            data.HealCombat = numericUpDown3.Value;
            data.RunBelow = numericUpDown6.Value;
            data.HangBelow = numericUpDown5.Value;
            data.HealPeriod = int.Parse(textBox1.Text);
            
            data.RestMaxMP = numericUpDown12.Value;
            data.RestIfBelowMP = numericUpDown11.Value;
            data.HealRestMP = numericUpDown10.Value;
            data.HealCombatMP = numericUpDown9.Value;
            data.RunBelowMP = numericUpDown8.Value;
            data.HangBelowMP = numericUpDown7.Value;

            data.UseMeditate = checkBox_usemeditate.Checked;
            data.MeditateB4Rest = checkBox1.Checked;
            
            data.PreRest = textBox2.Text;
            data.PostRest = textBox3.Text;
            data.PartyWait = textBox5.Text;
            data.PartyResume = textBox4.Text;

            return data;
        }

        public void Load(HealthControlValues data)
        {
            numericUpDown1.Value = data.RestMax;
            numericUpDown2.Value = data.RestIfBelow;
            numericUpDown4.Value = data.HealRest;
            numericUpDown3.Value = data.HealCombat;
            numericUpDown6.Value = data.RunBelow;
            numericUpDown5.Value = data.HangBelow;
            textBox1.Text = data.HealPeriod.ToString();
            numericUpDown12.Value = data.RestMaxMP;
            numericUpDown11.Value = data.RestIfBelowMP;
            numericUpDown10.Value = data.HealRestMP;
            numericUpDown9.Value = data.HealCombatMP;
            numericUpDown8.Value = data.RunBelowMP;
            numericUpDown7.Value = data.HangBelowMP;
            checkBox_usemeditate.Checked = data.UseMeditate;
            checkBox1.Checked = data.MeditateB4Rest;
            textBox2.Text = data.PreRest;
            textBox3.Text = data.PostRest;
            textBox5.Text = data.PartyWait;
            textBox4.Text = data.PartyResume;
        }
        

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            var maxhits = (decimal)this.controller._gameenv?._player?.Stats?.MaxHits;
            if (maxhits == 0)
            {
                return;
            }

            decimal cur_hits = maxhits * (numericUpDown1.Value / 100m);
            this.label3.Text = $"{cur_hits:0}/{maxhits}";
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            //label4
            var maxhits = (decimal)this.controller._gameenv?._player?.Stats?.MaxHits;
            if (maxhits == 0)
            {
                return;
            }

            decimal cur_hits = maxhits * (numericUpDown2.Value / 100m);
            this.label4.Text = $"{cur_hits:0}/{maxhits}";
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            //label7
            var maxhits = (decimal)this.controller._gameenv?._player?.Stats?.MaxHits;
            if (maxhits == 0)
            {
                return;
            }

            decimal cur_hits = maxhits * (numericUpDown3.Value / 100m);
            this.label7.Text = $"{cur_hits:0}/{maxhits}";
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            //10
            var maxhits = (decimal)this.controller._gameenv?._player?.Stats?.MaxHits;
            if (maxhits == 0)
            {
                return;
            }

            decimal cur_hits = maxhits * (numericUpDown4.Value / 100m);
            this.label10.Text = $"{cur_hits:0}/{maxhits}";
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            //13
            var maxhits = (decimal)this.controller._gameenv?._player?.Stats?.MaxHits;
            if (maxhits == 0)
            {
                return;
            }

            decimal cur_hits = maxhits * (numericUpDown5.Value / 100m);
            this.label13.Text = $"{cur_hits:0}/{maxhits}";
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            //16
            var maxhits = (decimal)this.controller._gameenv?._player?.Stats?.MaxHits;
            if (maxhits == 0)
            {
                return;
            }

            decimal cur_hits = maxhits * (numericUpDown6.Value / 100m);
            this.label16.Text = $"{cur_hits:0}/{maxhits}";
        }

        private void numericUpDown12_ValueChanged(object sender, EventArgs e)
        {
            //34
            var maxhits = (decimal)this.controller._gameenv?._player?.Stats?.MaxHits;
            if (maxhits == 0)
            {
                return;
            }

            decimal cur_hits = maxhits * (numericUpDown12.Value / 100m);
            this.label31.Text = $"{cur_hits:0}/{maxhits}";
        }

        private void numericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            //31
            var maxhits = (decimal)this.controller._gameenv?._player?.Stats?.MaxHits;
            if (maxhits == 0)
            {
                return;
            }

            decimal cur_hits = maxhits * (numericUpDown11.Value / 100m);
            this.label31.Text = $"{cur_hits:0}/{maxhits}";
        }

        private void numericUpDown10_ValueChanged(object sender, EventArgs e)
        {
            //28
            var maxhits = (decimal)this.controller._gameenv?._player?.Stats?.MaxHits;
            if (maxhits == 0)
            {
                return;
            }

            decimal cur_hits = maxhits * (numericUpDown10.Value / 100m);
            this.label28.Text = $"{cur_hits:0}/{maxhits}";
        }

        private void numericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            //25
            var maxhits = (decimal)this.controller._gameenv?._player?.Stats?.MaxHits;
            if (maxhits == 0)
            {
                return;
            }

            decimal cur_hits = maxhits * (numericUpDown9.Value / 100m);
            this.label25.Text = $"{cur_hits:0}/{maxhits}";
        }

        private void numericUpDown8_ValueChanged(object sender, EventArgs e)
        {
            //22
            var maxhits = (decimal)this.controller._gameenv?._player?.Stats?.MaxHits;
            if (maxhits == 0)
            {
                return;
            }

            decimal cur_hits = maxhits * (numericUpDown8.Value / 100m);
            this.label22.Text = $"{cur_hits:0}/{maxhits}";
        }

        private void numericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            //19
            var maxhits = (decimal)this.controller._gameenv?._player?.Stats?.MaxHits;
            if (maxhits == 0)
            {
                return;
            }

            decimal cur_hits = maxhits * (numericUpDown7.Value / 100m);
            this.label19.Text = $"{cur_hits:0}/{maxhits}";
        }
    }

    [Serializable]
    public class HealthControlValues
    {
        public decimal RestMax { get; set; }
        public decimal RestIfBelow { get; set; }
        public decimal HealRest { get; set; }
        public decimal HealCombat { get; set; }
        public decimal RunBelow { get; set; }
        public decimal HangBelow { get; set; }
        public int HealPeriod { get; set; }



        public decimal RestMaxMP { get; set; }
        public decimal RestIfBelowMP { get; set; }
        public decimal HealRestMP { get; set; }
        public decimal HealCombatMP { get; set; }
        public decimal RunBelowMP { get; set; }
        public decimal HangBelowMP { get; set; }

        public bool UseMeditate { get; set; }
        public bool MeditateB4Rest { get; set; }

        public string PreRest { get; set; }
        public string PostRest { get; set; }
        public string PartyWait { get; set; }
        public string PartyResume { get; set; }
    }
}
