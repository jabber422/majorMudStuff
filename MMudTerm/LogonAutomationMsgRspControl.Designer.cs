namespace MMudTerm
{
    partial class LogonAutomationMsgRspControl
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
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.textBox_accountname = new System.Windows.Forms.TextBox();
            this.textBox_msg_accountname = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(386, 4);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(22, 21);
            this.checkBox3.TabIndex = 18;
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // textBox_accountname
            // 
            this.textBox_accountname.Location = new System.Drawing.Point(0, 0);
            this.textBox_accountname.Name = "textBox_accountname";
            this.textBox_accountname.Size = new System.Drawing.Size(154, 26);
            this.textBox_accountname.TabIndex = 16;
            // 
            // textBox_msg_accountname
            // 
            this.textBox_msg_accountname.Location = new System.Drawing.Point(160, 0);
            this.textBox_msg_accountname.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.textBox_msg_accountname.Name = "textBox_msg_accountname";
            this.textBox_msg_accountname.Size = new System.Drawing.Size(220, 26);
            this.textBox_msg_accountname.TabIndex = 17;
            this.textBox_msg_accountname.Text = "\"new\":";
            // 
            // LogonAutomationMsgRspControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.textBox_accountname);
            this.Controls.Add(this.textBox_msg_accountname);
            this.Name = "LogonAutomationMsgRspControl";
            this.Size = new System.Drawing.Size(419, 28);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.TextBox textBox_accountname;
        private System.Windows.Forms.TextBox textBox_msg_accountname;
    }
}
