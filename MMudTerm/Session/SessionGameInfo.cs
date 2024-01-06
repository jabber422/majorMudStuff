using MMudObjects;
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
using System.Xml.Linq;
using static System.Windows.Forms.LinkLabel;

namespace MMudTerm.Session
{
    public partial class SessionGameInfo : Form
    {
        public SessionController _controller = null;
        DateTime _exphr_start;
        CombatSessionsControl combatSessionsControl1 = null;

        public SessionGameInfo(SessionController controller)
        {
            this._controller = controller;
            this.combatSessionsControl1 = new CombatSessionsControl(this._controller);
            this.combatSessionsControl1.Location = new System.Drawing.Point(11, 6);
            this.combatSessionsControl1.Name = "combatSessionsControl1";
            this.combatSessionsControl1.Size = new System.Drawing.Size(940, 389);
            this.combatSessionsControl1.TabIndex = 0;

            InitializeComponent();
            this.tabPage_combat.Controls.Add(this.combatSessionsControl1);
            _exphr_start = DateTime.Now;
        }

        public void Update(string token)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(Update), token);
            }
            else
            {
                switch (token)
                {
                    case "who":
                    case "top":
                        this.UpdateWho();
                        break;
                    case "stats":
                    case "exp":
                        this.UpdateStats();
                        break;
                    case "inv":
                        this.UpdateInventory();
                        break;
                    case "room":
                    case "search_found":
                        this.UpdateRoom();
                        break;
                    case "in_combat":
                        this.UpdateInCombat();
                        break;
                    case "tick":
                        this.UpdateTick();
                        break;
                    case "pickup_cash":
                    case "drop_cash":
                    case "hid_cash":
                    case "hid_item":
                    case "pickup_item":
                    case "drop_item":
                        this.UpdateRoom();
                        this.UpdateInventory();
                        break;
                    case "bought_something":
                        this.UpdateInventory();
                        break;
                    case "combat":
                        this.UpdateCombat();
                        break;
                }
            }
        }

        private void UpdateTick()
        {
            PlayerStats stats = this._controller._gameenv._player.Stats;
            label12.Text = stats.CurHits.ToString();
            label48.Text = stats.CurMana.ToString();
        }

        private void UpdateRoom()
        {
            this.combatSessionsControl1.Update("room");
            Room cur_room = this._controller._gameenv._current_room;
            label_roomname_value.Text = cur_room.Name;

            string items = "";
            foreach (Item i in cur_room.VisibleItems.Values)
            {
                if (i.Quantity > 1) {
                    items += i.Quantity + " " + i.Name + ", ";
                }
                else
                {
                    items += i.Name + ", ";
                }
            }
            textBox_items_value.Text = items;
            
            string also_here = "";
            foreach(Entity e in cur_room.AlsoHere)
            {
                also_here += e.Name + ", ";
            }
            textBox_alsohere_value.Text = also_here;

            label_exits_value.Text = String.Join(",", cur_room.RoomExits);

            string hidden_here = "";
            foreach (Item i in cur_room.HiddenItems.Values)
            {
                hidden_here += i.Name + ", ";
            }
            textBox1.Text = hidden_here;
        }

        private void UpdateInCombat()
        {
            //this.label56.Text = this._controller._gameenv._player.InCombat.ToString();
        }

        private void UpdateCombat()
        {
            this.combatSessionsControl1.Update("combat");
        }

        public void UpdateStats()
        {
            PlayerStats stats = this._controller._gameenv._player.Stats;
            
            label9.Text = stats.FirstName + " " + stats.LastName;
            label10.Text = stats.Race;
            label11.Text = stats.Class;
            label12.Text = stats.CurHits.ToString();
            label47.Text = stats.MaxHits.ToString();
            label48.Text = stats.CurMana.ToString();
            label49.Text = stats.MaxMana.ToString();
            label13.Text = stats.Strength.ToString();
            label14.Text = stats.Intellect.ToString();
            label15.Text = stats.Willpower.ToString();
            label26.Text = stats.Agility.ToString();
            label27.Text = stats.Health.ToString();
            label28.Text = stats.Charm.ToString();
            label38.Text = stats.Lives.ToString();
            label50.Text = "/ " +stats.CP.ToString();
            label39.Text = stats.Perception.ToString();
            label40.Text = stats.Stealth.ToString();
            label41.Text = stats.Thievery.ToString();
            label42.Text = stats.Traps.ToString();
            label43.Text = stats.Tracking.ToString();
            label44.Text = stats.Martial_Arts.ToString();
            label45.Text = stats.MagicRes.ToString();
            label46.Text = stats.Picklocks.ToString();
            label23.Text = stats.Exp.ToString();
            label51.Text = stats.AC.ToString();
            label52.Text = "/ " + stats.DR.ToString();
            label_total_exp_value.Text = stats.Exp.ToString();
            label_gained_exp_value.Text = this._controller._gameenv._player.GainedExp.ToString();
            double gained_exp = this._controller._gameenv._player.GainedExp;
            DateTime _now = DateTime.Now;
            TimeSpan ts = DateTime.Now - this._exphr_start;
            double hours = ts.TotalHours;
            double rate = gained_exp / hours;
            label_exphr_value.Text = FormatNumber(rate);

        }

        public static string FormatNumber(double num)
        {
            if (num >= 1000000)
                return (num / 1000000D).ToString("0.#") + "M";
            if (num >= 1000)
                return (num / 1000D).ToString("0.#") + "K";

            return num.ToString("N0");
        }

        public void UpdateInventory()
        {
            List<CarryableItem> items = new List<CarryableItem> ();
            Dictionary<string, CarryableItem> items_ = this._controller._gameenv._player.Inventory.Items;
            items = items_.Values.ToList ();
            this.tabPage_inv.Controls.Clear();

            
            int yPos = 0;
            foreach (CarryableItem item in items)
            {
                InventoryRowControl new_item = new InventoryRowControl(this._controller, item);
                new_item.Location = new Point(0, yPos);
                //Label label = new Label
                //{
                //    Text = item.Name,
                //    AutoSize = true,
                //    Location = new Point(10, yPos),
                //    Cursor = Cursors.Hand
                //};
                
                //if (item.Quantity > 1)
                //{
                //    label.Text = item.Quantity.ToString() + " " + item.Name;
                //}

                //if (item.Quantity > 1 && item is EquipableItem && (item as EquipableItem).Equiped)
                //{
                //    label.Text += " (One Equipped - " + (item as EquipableItem).Location + ")";
                //}

                //if (item.Quantity == 1 && item is EquipableItem && (item as EquipableItem).Equiped)
                //{
                //    label.Text += " (Equipped - " + (item as EquipableItem).Location + ")";
                //}

                //label.Click += Label_Click;
                //this.tabPage_inv.Controls.Add(label);
                this.tabPage_inv.Controls.Add(new_item);
                yPos += new_item.Height + 5;
            }
            this.UpdateWealth();

        }

        private void UpdateWealth()
        {
            Purse purse = this._controller._gameenv._player.Inventory.GetPurse();
            this.label64.Text = purse.runic.ToString();
            this.label68.Text = purse.platinum.ToString();
            this.label65.Text = purse.gold.ToString();
            this.label66.Text = purse.silver.ToString();
            this.label67.Text = purse.copper.ToString();
            this.label58.Text = purse.wealth.ToString();
        }

        private void Label_Click(object sender, EventArgs e)
        {
            Label clickedLabel = sender as Label;
            if (clickedLabel != null)
            {
                MessageBox.Show($"You clicked: {clickedLabel.Text}");
                // Implement your click logic here
            }
        }

        public void UpdateWho()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Online",
                HeaderText = "Online",
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Rank",
                HeaderText = "Rank",
                ValueType = typeof(int),
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Alignment",
                HeaderText = "Alignment",
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FirstName", 
                HeaderText = "First Name",      
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LastName",
                HeaderText = "Last Name",
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Title",
                HeaderText = "Title",
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "GangName",
                HeaderText = "Gang",
            });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Exp",
                HeaderText = "Exp",
            });

            this.dataGridView1.AllowUserToResizeColumns = true;
            this.dataGridView1.ReadOnly = false;



            this.dataGridView1.DataSource = new BindingList<Player>(this._controller._gameenv._players); 
            foreach (DataGridViewColumn column in this.dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            this.dataGridView1.Refresh();
        }

        private void label53_Click(object sender, EventArgs e)
        {

        }

        private void label54_Click(object sender, EventArgs e)
        {

        }

        private void label55_Click(object sender, EventArgs e)
        {

        }

        private void button_exp_reset_Click(object sender, EventArgs e)
        {
            this._controller._gameenv._player.GainedExp = 0;
            _exphr_start = DateTime.Now;
            this.Update("exp");
        }
    }
}
