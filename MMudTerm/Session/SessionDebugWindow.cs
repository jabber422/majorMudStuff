using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMudTerm.Session
{
    public partial class SessionDebugWindow : Form
    {
        private SessionController controller;

        

        public SessionDebugWindow(SessionController controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        public void UpdateStats(Dictionary<string, string> stats)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Dictionary<string, string>>(UpdateStats), stats);
            }
            else
            {

                foreach (KeyValuePair<String, string> kvp in stats)
                {
                    string[] vals = null;
                    switch (kvp.Key)
                    {
                        case "Lives/CP":
                            vals = kvp.Value.Split('/');
                            this.label_lives_value.Text = vals[0];
                            this.label_cp_value.Text = "/" + vals[1];
                            break;
                        case "Exp":
                            this.label_exp_value.Text = kvp.Value;
                            break;
                        case "Perception":
                            this.label_perception_value.Text = kvp.Value;
                            break;
                        case "Stealth":
                            this.label_stealth_value.Text = kvp.Value;
                            break;
                        case "Hits":
                            vals = kvp.Value.Split('/');
                            this.label_hits_value.Text = vals[0];
                            this.label_hits_max.Text = "/" + vals[1];
                            break;
                        case "Current Hits":
                            this.label_hits_value.Text = kvp.Value;
                            break;
                        case "Armour":
                            vals = kvp.Value.Split('/');
                            this.label_ac_value.Text = vals[0];
                            this.label_dr_value.Text = "/" + vals[1];
                            break;
                        case "Thievery":
                            this.label_thief_value.Text = kvp.Value;
                            break;
                        case "Mana":
                            vals = kvp.Value.Split('/');
                            this.label_mana_value.Text = vals[0];
                            this.label_mana_max.Text = "/" + vals[1];
                            break;
                        case "Current Mana":
                            this.label_mana_value.Text = kvp.Value;
                            break;
                        case "Spellcasting":
                            this.label_sc_value.Text = kvp.Value;
                            break;
                        case "Traps":
                            this.label_trap_value.Text = kvp.Value;
                            break;
                        case "Picklocks":
                            this.label_pick_value.Text = kvp.Value;
                            break;
                        case "Strength":
                            this.label_str_value.Text = kvp.Value;
                            break;
                        case "Agility":
                            this.label_agi_value.Text = kvp.Value;
                            break;
                        case "Tracking":
                            this.label_track_value.Text = kvp.Value;
                            break;
                        case "Intellect":
                            this.label_int_value.Text = kvp.Value;
                            break;
                        case "Health":
                            this.label_hea_value.Text = kvp.Value;
                            break;
                        case "Arts":
                            this.label_ma_value.Text = kvp.Value;
                            break;
                        case "Willpower":
                            this.label_wil_value.Text = kvp.Value;
                            break;
                        case "Charm":
                            this.label_chr_value.Text = kvp.Value;
                            break;
                        case "MagicRes":
                            this.label_mr_value.Text = kvp.Value;
                            break;
                        case "Level":
                            this.label_level_value.Text = kvp.Value;
                            break;
                        case "Resting":
                            this.label_rest_value.Text = kvp.Value;
                            break;
                        case "Name":
                            this.label_name_value.Text = kvp.Value;
                            break;
                        case "Race":
                            this.label_race_value.Text = kvp.Value;
                            break;
                        case "Class":
                            this.label_class_value.Text = kvp.Value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        internal void UpdateRoom(Dictionary<string, string> room_info)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Dictionary<string, string>>(UpdateRoom), room_info);
            }
            else
            {

                foreach (KeyValuePair<String, string> kvp in room_info)
                {
                    string[] vals = null;
                    switch (kvp.Key)
                    {
                        case "name":
                            this.label_roomname_value.Text = kvp.Value;
                            break;
                        case "desc":
                            this.label_roomdesc_value.Text = kvp.Value;
                            break;
                        case "items":
                            this.textBox_items_value.Text = kvp.Value;
                            break;
                        case "here":
                            this.textBox_alsohere_value.Text = kvp.Value;
                            break;
                        case "exits":
                            this.label_exits_value.Text = kvp.Value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        internal void UpdateInv(Dictionary<string, string> inv)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Dictionary<string, string>>(UpdateInv), inv);
            }
            else
            {

                foreach (KeyValuePair<String, string> kvp in inv)
                {
                    string[] vals = null;
                    switch (kvp.Key)
                    {
                        case "carry":
                            this.textbox_inv_value.Text = kvp.Value;
                            break;
                        case "cash":
                            this.label_wealth_value.Text = kvp.Value;
                            break;
                        case "enc":
                            this.label_enc_value.Text = kvp.Value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        internal void UpdateCombat(bool in_combat)
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new Action<bool> (UpdateCombat), in_combat);
            }
            else
            {
                this.label_incombat_value.Text = in_combat.ToString();
            }
        }

        internal void UpdateForSale(string result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateForSale), result);
            }
            else
            {
                this.textBox_forsale_value.Text = result.ToString();
            }
        }
    }
}

