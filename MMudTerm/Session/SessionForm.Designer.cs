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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripConnectBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonProxy = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLogon = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonEnter = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMummy = new System.Windows.Forms.ToolStripButton();
            this.sessionTermContainer = new System.Windows.Forms.Panel();
            this.buttonCombatEngaged = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripConnState});
            this.statusStrip1.Location = new System.Drawing.Point(0, 215);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(414, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.Stretch = false;
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripConnState
            // 
            this.toolStripConnState.Name = "toolStripConnState";
            this.toolStripConnState.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripConnectBtn,
            this.toolStripButtonProxy,
            this.toolStripButtonLogon,
            this.toolStripButtonEnter,
            this.toolStripButtonMummy});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(414, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripConnectBtn
            // 
            this.toolStripConnectBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripConnectBtn.Image = ((System.Drawing.Image)(resources.GetObject("toolStripConnectBtn.Image")));
            this.toolStripConnectBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripConnectBtn.Name = "toolStripConnectBtn";
            this.toolStripConnectBtn.Size = new System.Drawing.Size(56, 22);
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
            this.toolStripButtonProxy.Size = new System.Drawing.Size(40, 22);
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
            this.toolStripButtonLogon.Size = new System.Drawing.Size(45, 22);
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
            this.toolStripButtonEnter.Size = new System.Drawing.Size(38, 22);
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
            this.toolStripButtonMummy.Size = new System.Drawing.Size(57, 22);
            this.toolStripButtonMummy.Text = "Mummy";
            this.toolStripButtonMummy.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // sessionTermContainer
            // 
            this.sessionTermContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sessionTermContainer.Location = new System.Drawing.Point(0, 25);
            this.sessionTermContainer.Name = "sessionTermContainer";
            this.sessionTermContainer.Size = new System.Drawing.Size(414, 190);
            this.sessionTermContainer.TabIndex = 2;
            // 
            // buttonCombatEngaged
            // 
            this.buttonCombatEngaged.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonCombatEngaged.Enabled = false;
            this.buttonCombatEngaged.Location = new System.Drawing.Point(0, 216);
            this.buttonCombatEngaged.Name = "buttonCombatEngaged";
            this.buttonCombatEngaged.Size = new System.Drawing.Size(75, 21);
            this.buttonCombatEngaged.TabIndex = 3;
            this.buttonCombatEngaged.Text = "Combat";
            this.buttonCombatEngaged.UseVisualStyleBackColor = false;
            // 
            // SessionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(414, 237);
            this.Controls.Add(this.buttonCombatEngaged);
            this.Controls.Add(this.sessionTermContainer);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
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
        private System.Windows.Forms.Button buttonCombatEngaged;
    }
}
