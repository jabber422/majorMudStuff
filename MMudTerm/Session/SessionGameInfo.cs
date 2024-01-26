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
using System.Xml.Linq;
using static System.Windows.Forms.LinkLabel;

namespace MMudTerm.Session
{
    public partial class SessionGameInfo : Form
    {
        public SessionController _controller = null;
        
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
        }

        public void Update(EventType token)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<EventType>(Update), token);
            }
            else
            {
                switch (token)
                {
                    case EventType.Who:
                    case EventType.Top:
                    case EventType.SomeoneEnteredTheGame:
                    case EventType.SomeoneLeftTheGame:
                        
                        this.UpdateWho();
                        break;
                    case EventType.Stats:
                    case EventType.ExperienceGain:
                        this.UpdateStats();
                        break;
                    case EventType.Inventory:
                        this.UpdateInventory();
                        break;
                    case EventType.Room:
                    case EventType.SearchFound:
                    case EventType.DoorOpen:
                    case EventType.DoorClosed:
                    case EventType.RoomSomethingMovedInto:
                    case EventType.RoomSomethingMovedOut:
                    case EventType.EntityDeath:
                        this.UpdateRoom();
                        break;
                    case EventType.RoomLook:
                        this.UpdateLookRoom();
                        break;
                    case EventType.Tick:
                        this.UpdateTick();
                        if(this._controller._gameenv._player.Buffs?.Count > 0) UpdateBuffs();
                        break;
                    case EventType.PickUpCoins:
                    case EventType.DropCoins:
                    case EventType.HidCoins:
                    case EventType.PickUpItem:
                    case EventType.DropItem:
                    case EventType.HidItem:
                    case EventType.SeeHiddenItem:
                        this.UpdateRoom();
                        this.UpdateInventory();
                        break;
                    case EventType.BoughtSomething:
                        this.UpdateInventory();
                        break;
                    case EventType.Combat:
                        this.UpdateCombat();
                        break;
                    case EventType.Gossip:
                        this.richTextBox1.WordWrap = true;
                        this.richTextBox1.Multiline = true;
                        foreach(string s in this._controller._gameenv._gossips)
                        {
                            this.richTextBox1.AppendText(s + "\r\n");
                        }

                        //this won't line wrap... why??
                        //this.richTextBox1.Text = string.Join("\r\n",this._controller._gameenv._gossips);
                        break;
                    //case EventType.BuffSpellCastSuccess_3rdP:
                    case EventType.BuffSpellCastSuccess:
                    case EventType.BuffExpired:
                        UpdateBuffs();
                        break;
                    default:
                        //Console.WriteLine($"Not used: {token}");
                        break;
                }
            }
        }

        private void UpdateBuffs()
        {
            for(int i=0;i< this._controller._gameenv._player.Buffs?.Count;i++)
            {
                Spell spell = this._controller._gameenv._player.Buffs.Values.ElementAt(i);
                var label = this.groupBox_buffs.Controls.Find($"label_buff_{i+1}", true).FirstOrDefault();
                var label_time = this.groupBox_buffs.Controls.Find($"label_buff_{i+1}_time", true).FirstOrDefault();
                label.Text = spell.Name;
                TimeSpan ts = DateTime.Now - spell.CastTime;
                float coef = (float)spell.DurInc;

                float dur = ((spell.Duration * 4) + (coef * (float)this._controller._gameenv._player.Stats.Level));
                label_time.Text = ts.TotalSeconds.ToString("F0") + "  " + dur.ToString("F0");
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
            this.combatSessionsControl1.Update(EventType.Room);
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
            foreach (Entity e in cur_room.AlsoHere)
            {
                if (e == null) { continue; }
                if (e.Verb != "")
                {
                    also_here += $"({e.Verb}) ";
                }

                also_here += e.Name + ", ";
            }
            textBox_alsohere_value.Text = also_here;

            label_exits_value.Text = GetExits(cur_room);

            string hidden_here = "";
            foreach (Item i in cur_room.HiddenItems.Values)
            {
                hidden_here += i.Name + ", ";
            }
            textBox1.Text = hidden_here;

            this.label_mega_hash.Text = cur_room.MegaMudRoomHash.ToString("X");
            this.label_light.Text = cur_room.Light;
            this.label_cause.Text = cur_room.Cause;
        }

        private string GetExits(Room current_rom)
        {
            List<string> tokens = new List<string>();
            foreach (RoomExit re in current_rom.RoomExits)
            {
                if (re.IsDoor)
                {
                    string state = re.IsOpen == true ? "open" : "closed";
                    tokens.Add($"({state}) {re.Exit}");
                }
                else
                {
                    tokens.Add(re.Exit);
                }
            }

            return String.Join(", ", tokens);
        }

        private void UpdateLookRoom()
        {
            Room cur_room = this._controller._gameenv._look_room;
            label_lr_name.Text = cur_room.Name;

            string items = "";
            foreach (Item i in cur_room.VisibleItems.Values)
            {
                if (i.Quantity > 1)
                {
                    items += i.Quantity + " " + i.Name + ", ";
                }
                else
                {
                    items += i.Name + ", ";
                }
            }
            textBox_lr_items.Text = items;

            string also_here = "";
            foreach (Entity e in cur_room.AlsoHere)
            {
                if (e == null) { continue; }
                also_here += e.Name + ", ";
            }
            textBox_lr_alsohere.Text = also_here;
            label_lr_exits.Text = GetExits(cur_room);

            this.label_lr_hash.Text = cur_room.MegaMudRoomHash.ToString("X");

            
        }

    
        private void UpdateCombat()
        {
            this.combatSessionsControl1.Update(EventType.Combat);
        }

        public void UpdateStats()
        {
            PlayerStats stats = this._controller._gameenv._player.Stats;
            
            label9.Text = stats.FirstName + " " + stats.LastName;
            label10.Text = stats.Race;
            label11.Text = stats.Class;
            label24.Text = stats.Level.ToString();
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
            TimeSpan ts = DateTime.Now - this._controller._exphr_start;
            double hours = ts.TotalHours;
            double rate = gained_exp / hours;
            label_exphr_value.Text = FormatNumber(rate);

        }

        public static string FormatNumber(double num)
        {
            if (num >= 1000000)
                return (num / 1000000D).ToString("0.#") + "M";
            if (num >= 10000)
                return (num / 1000D).ToString("0.#") + "K";

            return num.ToString("N0");
        }



        public void UpdateInventory()
        {
            List<Item> items = new List<Item> ();
            Dictionary<string, Item> items_ = this._controller._gameenv._player.Inventory.Items;
            items = items_.Values.ToList ();
            this.tabPage_inv.Controls.Clear();

            
            int yPos = 0;
            foreach (Item item in items)
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
            this._controller._exphr_start = DateTime.Now;
            this.Update(EventType.ExperienceGain);
        }

        //AI wrote this and below, kinda cool and scary
        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView2.Font = new Font(dataGridView1.Font.FontFamily, 12); // Set font size to 12
            dataGridView2.ReadOnly = false;
            dataGridView2.EditMode = DataGridViewEditMode.EditOnEnter;

            var editableList = _controller._gameenv._matcher.RegexListwMacros()
            .Select(kvp => new EditableKeyValuePair { Key = kvp.Key, Value = kvp.Value })
            .ToList();

            dataGridView2.DataSource = editableList;
            dataGridView2.CellEndEdit += dataGridView2_CellEndEdit;

            this.dataGridView2.AutoGenerateColumns = true;
            this.dataGridView2.Refresh();
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid != null)
            {
                var rowIndex = e.RowIndex;
                var newKey = grid.Rows[rowIndex].Cells[0].Value.ToString();
                var newValue = (EventType)grid.Rows[rowIndex].Cells[1].Value;

                var editableList = _controller._gameenv._matcher.RegexListwMacros()
                .Select(kvp => new EditableKeyValuePair { Key = kvp.Key, Value = kvp.Value })
                .ToList();
                // Update the list
                editableList[rowIndex].Key = newKey;
                editableList[rowIndex].Value = newValue;

                // Update the original dictionary
                // Note: This assumes that the keys are unique and handles key changes.
                var oldKey = _controller._gameenv._matcher.RegexListwMacros().ElementAt(rowIndex).Key;
                if (!oldKey.Equals(newKey))
                {
                    _controller._gameenv._matcher.RegexListwMacros().Remove(oldKey);
                }
                _controller._gameenv._matcher.RegexListwMacros()[newKey] = newValue;

                _controller._gameenv._matcher.Reload();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this._controller._gameenv._gossips.Clear();
            this.richTextBox1.Text = "";
        }
    }

    public class EditableKeyValuePair
    {
        public string Key { get; set; }
        public EventType Value { get; set; }
    }

}
