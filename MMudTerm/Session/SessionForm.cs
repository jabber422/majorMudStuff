using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MMudTerm.Terminal;
using MMudTerm.Session.SessionStateData;
using static Humanizer.In;
using System.Runtime.CompilerServices;
using MMudTerm.Game;

namespace MMudTerm.Session
{
    /// <summary>
    /// The form for a session, MDI child of MMudTerm
    /// 
    /// This file is the "view" that handles all of the input from the session form
    /// </summary>
    public partial class SessionForm : Form
    {
        TerminalWindow m_term;
        internal SessionController m_controller;
        internal SessionDataObject m_sessionData;

        Dictionary<int, SessionGameInfo> m_gameInfos;

        internal TerminalWindow Terminal
        { get { return this.m_term; } }

        #region tool bar icons
        private Bitmap ExtractIconFromBitmap(Bitmap sourceBitmap, Rectangle section)
        {
            // Create a new bitmap object with the size of the section
            Bitmap iconBitmap = new Bitmap(section.Width, section.Height);

            using (Graphics g = Graphics.FromImage(iconBitmap))
            {
                // Draw the specified section of the source bitmap to the new one
                g.DrawImage(sourceBitmap, 0, 0, section, GraphicsUnit.Pixel);
            }

            return iconBitmap;
        }

        Color icon_transparent_color = Color.Magenta;
        private Bitmap GetIcon(int x_mod)
        {
            Bitmap allicons = Properties.Resources.BitmapSessionIcons;

            //x3 is connect button, x4 is disconnect
            Rectangle iconSection = new Rectangle(16 * x_mod, 0, 16, 16); // Set x, y, width, and height accordingly
            Bitmap icon = ExtractIconFromBitmap(allicons, iconSection);
            if(icon_transparent_color == Color.Magenta)
            {
                icon_transparent_color = icon.GetPixel(0, 0);
            }
            icon.MakeTransparent(icon_transparent_color);
            return icon;
        }

        List<ToolStripButton> toolStripItems = new List<ToolStripButton>();

        #endregion
        internal SessionForm(SessionConnectionInfo sciData)
        {
            InitializeComponent();

            Bitmap allicons = Properties.Resources.BitmapSessionIcons;
            
            //x3 is connect button, x4 is disconnect
            Rectangle iconSection = new Rectangle(16*3, 0, 16, 16); // Set x, y, width, and height accordingly
            Bitmap iconBitmap = ExtractIconFromBitmap(allicons, iconSection);

            this.toolStripConnectBtn.Image = GetIcon(3);

            this.toolStripButton_go.Image = GetIcon(11);
            this.toolStripButton_loop.Image = GetIcon(12);
            this.toolStripButton_stop.Image = GetIcon(15);
            this.toolStripButton_all_monitors.Image = GetIcon(17);
            this.toolStripButton_combat.Image = GetIcon(19);
            this.toolStripButton_rest.Image = GetIcon(21);
            this.toolStripButton_buff.Image = GetIcon(22);
            this.toolStripButton_get.Image = GetIcon(24);
            this.toolStripButton_getcoins.Image = GetIcon(25);

            //these get locked by all on/all
            this.toolStripItems.Add(toolStripButton_go);
            this.toolStripItems.Add(toolStripButton_loop);
            this.toolStripItems.Add(toolStripButton_stop);
            this.toolStripItems.Add(toolStripButton_combat);
            this.toolStripItems.Add(toolStripButton_rest);
            this.toolStripItems.Add(toolStripButton_buff);
            this.toolStripItems.Add(toolStripButton_get);
            this.toolStripItems.Add(toolStripButton_getcoins);





            //i need to be able to add or remove toolStripButton
            //when a button is present
            //if true it need to 






            this.m_sessionData = new SessionDataObject(sciData);

            this.m_controller = new SessionController(this.m_sessionData, this);

            this.m_term = new TerminalWindow(this.m_sessionData);
            InitTermWindow();

            this.m_gameInfos = new Dictionary<int, SessionGameInfo>();
            SessionGameInfo m_gameInfo = new SessionGameInfo(this.m_controller);
            m_gameInfos.Add(m_gameInfo.GetHashCode(), m_gameInfo);
            m_gameInfo.FormClosing += M_gameInfo_FormClosing;
            if (this.components == null)
            {
                this.components = new System.ComponentModel.Container();
            }
            this.components.Add(m_gameInfo);
            
            m_gameInfo.Show();

            this.m_controller.EnterTheGame = this.toolStripButton_all_monitors.Checked;
        }

        private void M_gameInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (this.m_gameInfos)
            {
                this.m_gameInfos.Remove(sender.GetHashCode());
            }
        }

        private void InitTermWindow()
        {
            this.m_term.Location = new System.Drawing.Point(3, 3);
            this.m_term.Name = "termWindow";
            this.m_term.Size = new System.Drawing.Size(20, 20);
            this.m_term.BorderStyle = BorderStyle.FixedSingle;
            this.m_term.KeyDown += new KeyEventHandler(term_KeyDown);
            this.m_term.KeyUp += new KeyEventHandler(term_KeyUp);
            this.m_term.KeyPress += new KeyPressEventHandler(term_KeyPress);
            
            this.sessionTermContainer.Controls.Add(this.m_term);
            this.sessionTermContainer.AutoSize = true;
            this.m_term.Init();
        }

        KeyEventArgs cur_key = null;
        private void term_KeyDown(object sender, KeyEventArgs e)
        {
            this.cur_key = e;
        }

        private void term_KeyUp(object sender, KeyEventArgs e)
        {
            this.cur_key = null;
        }

        //List<char> _buffer = new List<char>();
        //if focus is on the session window any key pressed is buffered, enter will send the buffer
        private void term_KeyPress(object sender, KeyPressEventArgs e)
        {
            byte[] msg = null;
            if (this.m_controller.m_macros.IsMacro(this.cur_key.KeyCode))
            {
                string macro = this.m_controller.m_macros.GetMacro(this.cur_key.KeyCode);
                msg = Encoding.ASCII.GetBytes(macro);
            }
            else
            {
                msg = new byte[] { (byte)e.KeyChar };
            }

            this.m_controller.user_has_send_blocked = this.cur_key.KeyCode == Keys.Enter ? false : true;
            this.m_controller.Send(msg);
        }

        string con_state = "Connect";
        private async void toolStripConnectBtn_Click(object sender, EventArgs e)
        {
            if (!this.toolStripConnectBtn.Checked)
            {
                this.toolStripConnectBtn.Enabled = false;
                this.m_controller._gameenv.Monitor_On = this.toolStripButton_all_monitors.Checked;
                var result = await this.m_controller.ConnectAsync();
                this.toolStripConnectBtn.Enabled = true;
                if (result) 
                {
                    this.toolStripConnectBtn.Checked = true;
                    this.toolStripConnectBtn.Image = GetIcon(4);
                }
            }
            else
            {
                if(this.m_controller.Disconnect())
                {
                    this.toolStripConnectBtn.Checked = false;
                }
                this.toolStripConnectBtn.Image = GetIcon(3);
                this.toolStripConnectBtn.Checked = false;
            }
        }

        private void SessionView_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.m_term.CleanUp();
            //TODO: save data pop-up
            this.m_controller.Dispose();
            this.m_sessionData.Dispose();
            
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this.m_sessionData.MummyScriptEnabled = !this.toolStripButtonMummy.Checked;
            this.toolStripButtonMummy.Checked = this.m_sessionData.MummyScriptEnabled;

            if (this.m_sessionData.MummyScriptEnabled)
            {
                //this.m_controller.AddListener(MummyScriptHandler);
            }
            else
            {
                //this.m_controller.RemoveListender(MummyScriptHandler);
            }
                
        }

        internal void UpdateState(string state_name)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateState), state_name);
            }
            else
            {
                this.toolStripStatusLabel1_state.Text = state_name;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SessionGameInfo m_gameInfo = new SessionGameInfo(this.m_controller);
            lock (this.m_gameInfos)
            {
                m_gameInfos.Add(m_gameInfo.GetHashCode(), m_gameInfo);
            }
            m_gameInfo.FormClosing += M_gameInfo_FormClosing;
            m_gameInfo.Show();
        }

        internal void UpdatePlayerStats(Dictionary<string, string> stats)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Dictionary<string, string>>(UpdatePlayerStats), stats);
            }
            else
            {
                if(stats.ContainsKey("Resting"))
                {
                    if (stats["Resting"] == "Resting")
                    {
                        this.toolStripStatusLabel_status.Text = "Resting";
                    }else if(stats["Resting"] == "No")
                    {
                        if (this.toolStripStatusLabel_status.Text == "Resting")
                        {
                            this.toolStripStatusLabel_status.Text = "Idle";
                        }
                    }
                }
            }
        }

        internal void UpdateCombat()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateCombat));
            }
            else
            {
                this.toolStripStatusLabel_status.Text = 
                    this.m_controller._gameenv._player.IsCombatEngaged ? "In Combat" : "Idle";
            }
        }


        public void UpdateTick()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateTick));
            }
            else
            {
                
                if (this.m_controller._gameenv._player.IsResting)
                {
                    this.toolStripStatusLabel_status.Text = "Resting";
                }
                else if(!this.m_controller._gameenv._player.IsCombatEngaged)
                {
                    this.toolStripStatusLabel_status.Text = "Idle";
                }
                double gained_exp = this.m_controller._gameenv._player.GainedExp;
                DateTime _now = DateTime.Now;
                TimeSpan ts = DateTime.Now - this.m_controller._exphr_start;
                double hours = ts.TotalHours;
                double rate = gained_exp / hours;
                this.toolStripStatusLabel__xphr.Text = FormatNumber(rate) + " exp/hr";
                
            }
        }
        public static string FormatNumber(double num)
        {
            if (num >= 1000000)
                return (num / 1000000D).ToString("0.#") + "M";
            if (num >= 10000)
                return (num / 1000D).ToString("0.#") + "K";

            return num.ToString("N0");
        }

        public void Update(EventType token)
        {
            lock (this.m_gameInfos)
            {
                foreach (SessionGameInfo m_gameInfo in this.m_gameInfos.Values)
                {
                    m_gameInfo.Update(token);
                }
            }

            //these are here because this form uses both of them in the status bar
            switch (token)
            {
                case EventType.Combat:
                    UpdateCombat();
                    break;
                case EventType.Tick:
                    UpdateTick();
                    break;
                case EventType.Room:
                    UpdateRoomName();
                    break;
            }
        }

        private void UpdateRoomName()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateRoomName));
            }
            else
            {
                if (this.m_controller._gameenv?._current_room != null)
                {
                    this.toolStripStatusLabel_currentroom.Text = this.m_controller._gameenv._current_room.MegaMudRoomHash;
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //iconBitmap2.RotateFlip(RotateFlipType.Rotate180FlipX);

            this.m_controller._gameenv.Monitor_On = this.toolStripButton_all_monitors.Checked;
            if (this.toolStripButton_all_monitors.Checked)
            {
                foreach(var x in this.toolStripItems)
                {
                    x.Enabled = true;
                }
                this.toolStripButton_all_monitors.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
            }
            else
            {
                foreach (var x in this.toolStripItems)
                {
                    x.Enabled = false;
                }
                this.toolStripButton_all_monitors.Image.RotateFlip(RotateFlipType.Rotate180FlipX);               
            }
        }

        private void toolStripButton_combat_Click(object sender, EventArgs e)
        {
            this.m_controller._gameenv.Monitor_Combat = this.toolStripButton_combat.Checked;
        }

        private void toolStripButton_rest_Click(object sender, EventArgs e)
        {
            this.m_controller._gameenv.Monitor_Rest = this.toolStripButton_rest.Checked;
        }

        private void toolStripButton_buff_Click(object sender, EventArgs e)
        {
            this.m_controller._gameenv.Monitor_Buff = this.toolStripButton_buff.Checked;
        }

        private void toolStripButton_get_Click(object sender, EventArgs e)
        {
            this.m_controller._gameenv.Monitor_Get = this.toolStripButton_get.Checked;
        }

        private void toolStripButton_getcoins_Click(object sender, EventArgs e)
        {
            this.m_controller._gameenv.Monitor_GetCoins = this.toolStripButton_getcoins.Checked;
        }
    }
}
