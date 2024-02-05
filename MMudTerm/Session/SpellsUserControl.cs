using MmeDatabaseReader;
using MMudObjects;
using MMudTerm.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMudTerm.Session
{
    public partial class SpellsUserControl : UserControl
    {
        SessionController controller;

        public SpellsUserControl(SessionController controller)
        {
            InitializeComponent();
            this.controller = controller;
            this.LoadState(this.controller.UserConfig_Spells);
        }

        private void groupBox_other_VisibleChanged(object sender, EventArgs e)
        {
            List<Spell> spelllist = MMudData.GetSpellList(this.controller._gameenv._player);

            List<string> buff_spells = new List<string>();
            foreach (Spell spell in spelllist)
            {
                if (spell.TargetType == EnumTargetType.Self || spell.TargetType == EnumTargetType.SingleOrSelf)
                {
                    buff_spells.Add(spell.Name);
                }
            }
            this.comboBox_bless_1.Items.Clear();
            this.comboBox_bless_2.Items.Clear();
            this.comboBox_bless_3.Items.Clear();
            this.comboBox_bless_4.Items.Clear();
            this.comboBox_bless_5.Items.Clear();
            this.comboBox_bless_6.Items.Clear();
            this.comboBox_bless_7.Items.Clear();
            this.comboBox_bless_8.Items.Clear();
            this.comboBox_bless_9.Items.Clear();
            this.comboBox_bless_10.Items.Clear();
            this.comboBox_bless_1.Items.AddRange(buff_spells.ToArray());
            this.comboBox_bless_2.Items.AddRange(buff_spells.ToArray());
            this.comboBox_bless_3.Items.AddRange(buff_spells.ToArray());
            this.comboBox_bless_4.Items.AddRange(buff_spells.ToArray());
            this.comboBox_bless_5.Items.AddRange(buff_spells.ToArray());
            this.comboBox_bless_6.Items.AddRange(buff_spells.ToArray());
            this.comboBox_bless_7.Items.AddRange(buff_spells.ToArray());
            this.comboBox_bless_8.Items.AddRange(buff_spells.ToArray());
            this.comboBox_bless_9.Items.AddRange(buff_spells.ToArray());
            this.comboBox_bless_10.Items.AddRange(buff_spells.ToArray());

            this.LoadState(this.controller.UserConfig_Spells);
        }

        private void LoadState(SpellUserControlState _state)
        {
            this.textBox_min_tick_rate.Text = _state.textBox_min_tick_rate;
            SetComboBoxSelectedItem(this.comboBox_bless_2, _state.comboBox_bless_2);
            SetComboBoxSelectedItem(this.comboBox_bless_1, _state.comboBox_bless_1);
            SetComboBoxSelectedItem(this.comboBox_mp_full, _state.comboBox_mp_full);
            SetComboBoxSelectedItem(this.comboBox_heal_regen, _state.comboBox_heal_regen);
            SetComboBoxSelectedItem(this.comboBox_hp_full, _state.comboBox_hp_full);
            SetComboBoxSelectedItem(this.comboBox_mana_regen, _state.comboBox_mana_regen);
            SetComboBoxSelectedItem(this.comboBox_heal_hp, _state.comboBox_heal_hp );
            SetComboBoxSelectedItem(this.comboBox_bless_10, _state.comboBox_bless_10);
            SetComboBoxSelectedItem(this.comboBox_bless_9, _state.comboBox_bless_9);
            SetComboBoxSelectedItem(this.comboBox_bless_8, _state.comboBox_bless_8);
            SetComboBoxSelectedItem(this.comboBox_bless_7, _state.comboBox_bless_7);
            SetComboBoxSelectedItem(this.comboBox_bless_6, _state.comboBox_bless_6);
            SetComboBoxSelectedItem(this.comboBox_bless_5, _state.comboBox_bless_5);
            SetComboBoxSelectedItem(this.comboBox_bless_4, _state.comboBox_bless_4);
            SetComboBoxSelectedItem(this.comboBox_bless_3, _state.comboBox_bless_3);
            SetComboBoxSelectedItem(this.comboBox_light, _state.comboBox_light);
            SetComboBoxSelectedItem(this.comboBox_cure_posion, _state.comboBox_cure_posion);
            SetComboBoxSelectedItem(this.comboBox_freedom, _state.comboBox_freedom);
            SetComboBoxSelectedItem(this.comboBox_cure_blind, _state.comboBox_cure_blind);
            SetComboBoxSelectedItem(this.comboBox_cure_disease, _state.comboBox_cure_disease);
        }

        private void SetComboBoxSelectedItem(ComboBox comboBox, string value)
        {
            if (comboBox != null && value != null)
            {
                // Assuming value is the actual string to be selected in the ComboBox
                comboBox.SelectedItem = comboBox.Items.Cast<object>().FirstOrDefault(item => item.ToString() == value);
            }
        }

        private SpellUserControlState SaveState()
        {
            SpellUserControlState _state = new SpellUserControlState();
            _state.comboBox_bless_2 = this.comboBox_bless_2.Text;
            _state.comboBox_bless_1 = this.comboBox_bless_1.Text;
            _state.textBox_min_tick_rate = this.textBox_min_tick_rate.Text;
            _state.comboBox_mp_full = this.comboBox_mp_full.Text;
            _state.comboBox_heal_regen = this.comboBox_heal_regen.Text;
            _state.comboBox_hp_full = this.comboBox_hp_full.Text;
            _state.comboBox_mana_regen = this.comboBox_mana_regen.Text;
            _state.comboBox_heal_hp = this.comboBox_heal_hp.Text;
            _state.comboBox_bless_10 = this.comboBox_bless_10.Text;
            _state.comboBox_bless_9 = this.comboBox_bless_9.Text;
            _state.comboBox_bless_8 = this.comboBox_bless_8.Text;
            _state.comboBox_bless_7 = this.comboBox_bless_7.Text;
            _state.comboBox_bless_6 = this.comboBox_bless_6.Text;
            _state.comboBox_bless_5 = this.comboBox_bless_5.Text;
            _state.comboBox_bless_4 = this.comboBox_bless_4.Text;
            _state.comboBox_bless_3 = this.comboBox_bless_3.Text;
            _state.comboBox_light = this.comboBox_light.Text;
            _state.comboBox_cure_posion = this.comboBox_cure_posion.Text;
            _state.comboBox_freedom = this.comboBox_freedom.Text;
            _state.comboBox_cure_blind = this.comboBox_cure_blind.Text;
            _state.comboBox_cure_disease = this.comboBox_cure_disease.Text;
            return _state;
        }

        private void comboBox_heal_hp_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this.controller.UserConfig_Spells = this.SaveState();
        }

        public void UpdateBuffs()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateBuffs));
            }
            else {
                for (int i = 0; i < 10; i++)
                {
                    var label = this.groupBox_buffs.Controls.Find($"label_buff_{i + 1}", true).FirstOrDefault();
                    var label_time = this.groupBox_buffs.Controls.Find($"label_buff_{i + 1}_time", true).FirstOrDefault();

                    if (this.controller._gameenv._player.Buffs != null)
                    {
                        Spell spell = null;
                        try
                        {
                            spell = this.controller._gameenv._player.Buffs.Values.ElementAt(i);
                            label.Text = spell.Name;
                            TimeSpan ts = DateTime.Now - spell.CastTime;
                            float coef = (float)spell.DurInc;
                            float dur = ((spell.Duration * MajorMudBbsGame.ROUND_TIME_IN_SEC) + (coef * (float)this.controller._gameenv._player.Stats.Level));
                            label_time.Text = ts.TotalSeconds.ToString("F0") + "  " + dur.ToString("F0");
                        }
                        catch (ArgumentOutOfRangeException aex)
                        {
                            label.Text = "";
                            label_time.Text = "";
                        }
                    }
                }
            }
        }
    }

    [Serializable]
    public class SpellUserControlState
    {
        public string comboBox_bless_2 { get; set; }
        public string comboBox_bless_1{get;set;}
        public string textBox_min_tick_rate{get;set;}
        public string comboBox_mp_full{get;set;}
        public string comboBox_heal_regen{get;set;}
        public string comboBox_hp_full{get;set;}
        public string comboBox_mana_regen{get;set;}
        public string comboBox_heal_hp{get;set;}
        public string comboBox_bless_10{get;set;}
        public string comboBox_bless_9{get;set;}
        public string comboBox_bless_8{get;set;}
        public string comboBox_bless_7{get;set;}
        public string comboBox_bless_6{get;set;}
        public string comboBox_bless_5{get;set;}
        public string comboBox_bless_4{get;set;}
        public string comboBox_bless_3{get;set;}
        public string comboBox_light{get;set;}
        public string comboBox_cure_posion{get;set;}
        public string comboBox_freedom{get;set;}
        public string comboBox_cure_blind{get;set;}
        public string comboBox_cure_disease{get;set;}

        public List<string> BuffsToMaintain()
        {
            return new List<string>() {
                this.comboBox_bless_1,
                this.comboBox_bless_2,
                this.comboBox_bless_3,
                this.comboBox_bless_4,
                this.comboBox_bless_5,
                this.comboBox_bless_6,
                this.comboBox_bless_7,
                this.comboBox_bless_8,
                this.comboBox_bless_9,
                this.comboBox_bless_10,
            };
        }
    }
}
