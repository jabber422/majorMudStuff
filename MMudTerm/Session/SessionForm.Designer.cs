namespace MMudTerm.Session
{
    partial class SessionForm
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SessionForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripConnState = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripConnectBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonProxy = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLogon = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEnter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMummy = new System.Windows.Forms.ToolStripButton();
            this.sessionTermContainer = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripConnState,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 694);
            this.statusStrip1.MinimumSize = new System.Drawing.Size(0, 40);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 21, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1006, 40);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.Stretch = false;
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripConnState
            // 
            this.toolStripConnState.Name = "toolStripConnState";
            this.toolStripConnState.Size = new System.Drawing.Size(0, 33);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(179, 33);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(87, 33);
            this.toolStripStatusLabel2.Text = "Unknown";
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripConnectBtn,
            this.toolStripButtonProxy,
            this.toolStripButtonLogon,
            this.toolStripButtonEnter,
            this.toolStripButtonMummy});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(1006, 34);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripConnectBtn
            // 
            this.toolStripConnectBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripConnectBtn.Image = ((System.Drawing.Image)(resources.GetObject("toolStripConnectBtn.Image")));
            this.toolStripConnectBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripConnectBtn.Name = "toolStripConnectBtn";
            this.toolStripConnectBtn.Size = new System.Drawing.Size(81, 29);
            this.toolStripConnectBtn.Text = "Connect";
            this.toolStripConnectBtn.Click += new System.EventHandler(this.toolStripConnectBtn_Click);
            // 
            // toolStripButtonProxy
            // 
            this.toolStripButtonProxy.Checked = true;
            this.toolStripButtonProxy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonProxy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonProxy.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonProxy.Image")));
            this.toolStripButtonProxy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonProxy.Name = "toolStripButtonProxy";
            this.toolStripButtonProxy.Size = new System.Drawing.Size(60, 29);
            this.toolStripButtonProxy.Text = "Proxy";
            this.toolStripButtonProxy.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButtonLogon
            // 
            this.toolStripButtonLogon.Checked = true;
            this.toolStripButtonLogon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonLogon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonLogon.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLogon.Image")));
            this.toolStripButtonLogon.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLogon.Name = "toolStripButtonLogon";
            this.toolStripButtonLogon.Size = new System.Drawing.Size(67, 29);
            this.toolStripButtonLogon.Text = "Logon";
            this.toolStripButtonLogon.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButtonEnter
            // 
            this.toolStripButtonEnter.Checked = true;
            this.toolStripButtonEnter.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonEnter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonEnter.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonEnter.Image")));
            this.toolStripButtonEnter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEnter.Name = "toolStripButtonEnter";
            this.toolStripButtonEnter.Size = new System.Drawing.Size(56, 29);
            this.toolStripButtonEnter.Text = "Enter";
            this.toolStripButtonEnter.Click += new System.EventHandler(this.toolStripButton3_Click);
            // 
            // toolStripButtonMummy
            // 
            this.toolStripButtonMummy.Checked = true;
            this.toolStripButtonMummy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonMummy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonMummy.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMummy.Image")));
            this.toolStripButtonMummy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMummy.Name = "toolStripButtonMummy";
            this.toolStripButtonMummy.Size = new System.Drawing.Size(83, 29);
            this.toolStripButtonMummy.Text = "Mummy";
            this.toolStripButtonMummy.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // sessionTermContainer
            // 
            this.sessionTermContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionTermContainer.Location = new System.Drawing.Point(0, 34);
            this.sessionTermContainer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.sessionTermContainer.Name = "sessionTermContainer";
            this.sessionTermContainer.Size = new System.Drawing.Size(1006, 660);
            this.sessionTermContainer.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(867, 0);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 32);
            this.button1.TabIndex = 4;
            this.button1.Text = "Debug";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SessionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1006, 734);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.sessionTermContainer);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SessionForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SessionView_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripConnectBtn;
        private System.Windows.Forms.Panel sessionTermContainer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripConnState;
        private System.Windows.Forms.ToolStripButton toolStripButtonProxy;
        private System.Windows.Forms.ToolStripButton toolStripButtonLogon;
        private System.Windows.Forms.ToolStripButton toolStripButtonMummy;
        private System.Windows.Forms.ToolStripButton toolStripButtonEnter;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    }
}
