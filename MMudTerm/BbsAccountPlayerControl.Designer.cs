namespace MMudTerm
{
    partial class BbsAccountPlayerControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox_bbs_account = new System.Windows.Forms.GroupBox();
            this.button_connect = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox_bbs_account.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Account Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(356, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Message";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Password:";
            // 
            // groupBox_bbs_account
            // 
            this.groupBox_bbs_account.Controls.Add(this.button_connect);
            this.groupBox_bbs_account.Controls.Add(this.label5);
            this.groupBox_bbs_account.Controls.Add(this.label4);
            this.groupBox_bbs_account.Controls.Add(this.label1);
            this.groupBox_bbs_account.Controls.Add(this.label3);
            this.groupBox_bbs_account.Controls.Add(this.label2);
            this.groupBox_bbs_account.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_bbs_account.Location = new System.Drawing.Point(0, 0);
            this.groupBox_bbs_account.Name = "groupBox_bbs_account";
            this.groupBox_bbs_account.Size = new System.Drawing.Size(604, 203);
            this.groupBox_bbs_account.TabIndex = 7;
            this.groupBox_bbs_account.TabStop = false;
            this.groupBox_bbs_account.Text = "Character";
            // 
            // button_connect
            // 
            this.button_connect.Location = new System.Drawing.Point(28, 154);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(83, 35);
            this.button_connect.TabIndex = 18;
            this.button_connect.Text = "Start";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(521, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 20);
            this.label5.TabIndex = 16;
            this.label5.Text = "Enabled";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(188, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Reply";
            // 
            // BbsAccountPlayerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox_bbs_account);
            this.Name = "BbsAccountPlayerControl";
            this.Size = new System.Drawing.Size(604, 203);
            this.groupBox_bbs_account.ResumeLayout(false);
            this.groupBox_bbs_account.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox_bbs_account;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button_connect;
        private System.Windows.Forms.Label label5;
    }
}
