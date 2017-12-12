using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MMudTerm_Protocols.AnsiProtocolCmds;
using MMudTerm_Protocols;
using MMudTerm.Session;

namespace MMudTerm.Terminal
{
    internal partial class TerminalWindow : UserControl
    {
        //Settings vars
        Color _bgColor;
        Font _font;
        Dictionary<int, Brush> _pallet;
        Brush _fColor;
        Brush _bColor;

        //calculated sizes
        SizeF charSize;
        double maxCols;
        double maxRows;
        int _userRows;
        int _userCols;
        
        CircularScreenBuffer grid;

        Graphics canvas; 
        SessionDataObject SessionData;

        delegate void GuiUpdate();
        GuiUpdate refresh;

        System.Timers.Timer heartbeat;
        System.Timers.ElapsedEventHandler ticker;
        bool caratVis = true;

        //ctor
        internal TerminalWindow(SessionDataObject sdo)
        {
            this.SessionData = sdo;
            InitializeComponent();
            this.SetStyle(
                System.Windows.Forms.ControlStyles.UserPaint |
                System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                true);

            refresh = new GuiUpdate(DoRefresh);
            ticker = new System.Timers.ElapsedEventHandler(heartbeat_Tick);

            heartbeat = new System.Timers.Timer(500);
            heartbeat.Elapsed += ticker;
            heartbeat.Start();
        }
        
        //redraw the screen every X milliseconds, causes the carat to blink
        void heartbeat_Tick(object sender, EventArgs e)
        {
            this.Invoke(refresh);
            caratVis = !caratVis;
        }

        //load session config settings
        private void GetConfigSettings()
        {
            this._font = this.SessionData.GetTermFont();
            this._pallet = this.SessionData.GetTermPallet();
            this._bgColor = this.SessionData.GetBackGroundColor();

            this._fColor = this._pallet[37];
            this._bColor = this._pallet[30];

            this._userRows = this.SessionData.GetUserSpecifiedRows();
            this._userCols = this.SessionData.GetUserSpecifiedCols();
        }

        //called on resize of parent or initial load
        internal void Init()
        {
            DoCalculations();
            this.canvas = this.CreateGraphics();
            this.grid = new CircularScreenBuffer((int)this.maxRows, (int)this.maxCols);
        }

        //here we are going to preform some initial calcs
        //we are going to set our columns from app data (80 for now)
        //then do some measurements
        private void DoCalculations()
        {
            GetConfigSettings();
            //measure the char size
            this.canvas = this.CreateGraphics();
            this.charSize = this.canvas.MeasureString("8", this._font);
            //compress our mono spaced lettering
            this.charSize.Width *= 0.7f;
            this.charSize.Height *= 0.9f;
            this.canvas.Dispose();

            //calc the height/width from the char size and the user speced rows/cols           
            int width = (int)Math.Ceiling(this.charSize.Width * this._userCols);
            int height = (int)Math.Ceiling(this.charSize.Height * this._userRows);

            this.Size = new Size(width, height);
            SizeF ss = new SizeF(this.Size.Width, this.Size.Height);
            
            //calc max colums we can display in the parent
            this.maxCols = Math.Floor(ss.Width / this.charSize.Width);
            this.maxRows = Math.Floor(ss.Height / this.charSize.Height);

        }

        #region overrides
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            e.Graphics.TextContrast = 0;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            DrawScreen(e.Graphics);
        }
       
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            heartbeat.Elapsed -= ticker;
            heartbeat.Stop();
            heartbeat.Dispose();
            base.OnHandleDestroyed(e);
        }
        #endregion

        //refresh the screen
        private void DoRefresh()
        {
            this.Refresh();
        }

        //should be the last thing called after commands are entered
        private void DrawScreen(Graphics g)
        {
            TermChar cur;
            PointF pt;

            g.Clear(this._bgColor);

            lock (this.grid)
            {
                for (int i = 0; i <= this.grid.Count; ++i)
                {
                    //TODO: change to TextRender
                    for (int j = 0; j < this.grid.MaxColCount; ++j)
                    {
                        if (this.grid.GetValue(i, j).Char != "\0")
                        {
                            cur = (TermChar)this.grid.GetValue(i, j);
                            pt = CalcDrawPoint(i, j);
                            SetColor(cur);
                            g.FillRectangle(this._bColor, pt.X, pt.Y, this.charSize.Width, this.charSize.Height);
                        }
                    }
                    for (int j = 0; j < this.grid.MaxColCount; ++j)
                    {
                        if (this.grid.GetValue(i, j).Char != "\0")
                        {
                            cur = (TermChar)this.grid.GetValue(i, j).Clone();
                            pt = CalcDrawPoint(i, j);
                            SetColor(cur);
                            g.DrawString(cur.Char, this._font, this._fColor, pt);
                        }
                    }
                }

                if (!caratVis)
                    return;
                TermChar carat = this.grid.Carat;
                SetColor(carat);
                g.DrawString(carat.Char, this._font, this._fColor, 
                    CalcDrawPoint(this.grid.Carat.Row, this.grid.Carat.Col));
            }
        }

        private void SetColor(TermChar tc)
        {
            try
            {
                if ((tc.AnsiGraphic.Fcolor) > 0)
                    this._fColor = this._pallet[tc.AnsiGraphic.Fcolor];
                if ((tc.AnsiGraphic.Bcolor) > 0)
                    this._bColor = this._pallet[tc.AnsiGraphic.Bcolor];
            }
            catch (Exception ex)
            {
            }
        }

        private PointF CalcDrawPoint(int row, int col)
        {
            PointF rtnVal = new PointF(col * this.charSize.Width, row * this.charSize.Height);
            return rtnVal;
        }

        //execute each command in the queue on the screen buffer, then redraw the screen
        internal void HandleCommands(Queue<TermCmd> queue)
        {
            heartbeat.Stop();
            lock (this.grid)
            {
                while (queue.Count > 0)
                {
                    TermCmd cmd = queue.Dequeue();
                    if (cmd is TermStringDataCmd)
                    {
                    }
                    cmd.DoCommand(this.grid);
                }
            }
            //cross thread invoke
            this.Invoke(refresh);
            heartbeat.Start();
        }

        //helping out the GC
        internal void CleanUp()
        {
            this._font.Dispose();
            this._pallet.Clear(); this._pallet = null;
            this._bColor = null;
            this.grid.Dispose();
            this.canvas.Dispose();
        }

        //TODO: removable
        internal void DoGridDebugDump()
        {
            Console.WriteLine("Start dump...");
            int cnt = grid.Count;
            for (int i = 0; i <= grid.Count; ++i)
            {
                for (int j = 0; j < grid.MaxColCount; ++j)
                {
                    TermChar c = grid.GetValue(i, j);
                    if (c.Char == '\0'.ToString()) Console.Write(Encoding.ASCII.GetString(new byte[] { 157 }));
                    Console.Write(c.Char);
                    Console.Write("-");
                }
                Console.Write("\r\n");
            }
            Console.WriteLine("End");
        }
    }
}

