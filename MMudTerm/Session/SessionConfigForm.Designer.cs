namespace MMudTerm
{
    partial class SessionConfigForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.addressTextBox = new System.Windows.Forms.ComboBox();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.colsTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rowsTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.start = new System.Windows.Forms.Button();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.autoConnectCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP/Hostname";
            // 
            // addressTextBox
            // 
            this.addressTextBox.Items.AddRange(new object[] {
            "127.0.0.1",
            "dadosbladebbs.dyndns.org"});
            this.addressTextBox.Location = new System.Drawing.Point(89, 70);
            this.addressTextBox.Name = "addressTextBox";
            this.addressTextBox.Size = new System.Drawing.Size(148, 21);
            this.addressTextBox.TabIndex = 1;
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(89, 96);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(148, 20);
            this.portTextBox.TabIndex = 3;
            this.portTextBox.Text = "12345";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 99);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";
            // 
            // colsTextBox
            // 
            this.colsTextBox.Location = new System.Drawing.Point(64, 217);
            this.colsTextBox.Name = "colsTextBox";
            this.colsTextBox.Size = new System.Drawing.Size(32, 20);
            this.colsTextBox.TabIndex = 7;
            this.colsTextBox.Text = "80";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 220);
            this.label3.Name = "label3";
            this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Cols";
            // 
            // rowsTextBox
            // 
            this.rowsTextBox.Location = new System.Drawing.Point(64, 191);
            this.rowsTextBox.Name = "rowsTextBox";
            this.rowsTextBox.Size = new System.Drawing.Size(32, 20);
            this.rowsTextBox.TabIndex = 5;
            this.rowsTextBox.Text = "40";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 194);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Rows";
            // 
            // start
            // 
            this.start.Location = new System.Drawing.Point(12, 243);
            this.start.Name = "start";
            this.start.Size = new System.Drawing.Size(75, 23);
            this.start.TabIndex = 8;
            this.start.Text = "Start";
            this.start.UseVisualStyleBackColor = true;
            this.start.Click += new System.EventHandler(this.button1_Click);
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(89, 44);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(148, 20);
            this.nameTextBox.TabIndex = 10;
            this.nameTextBox.Text = "Local";
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(13, 47);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.nameLabel.Size = new System.Drawing.Size(35, 13);
            this.nameLabel.TabIndex = 9;
            this.nameLabel.Text = "Name";
            // 
            // autoConnectCheckBox
            // 
            this.autoConnectCheckBox.AutoSize = true;
            this.autoConnectCheckBox.Checked = true;
            this.autoConnectCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoConnectCheckBox.Location = new System.Drawing.Point(140, 193);
            this.autoConnectCheckBox.Name = "autoConnectCheckBox";
            this.autoConnectCheckBox.Size = new System.Drawing.Size(91, 17);
            this.autoConnectCheckBox.TabIndex = 11;
            this.autoConnectCheckBox.Text = "Auto-Connect";
            this.autoConnectCheckBox.UseVisualStyleBackColor = true;
            // 
            // SessionConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 278);
            this.Controls.Add(this.autoConnectCheckBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.start);
            this.Controls.Add(this.colsTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rowsTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.addressTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SessionConfigForm";
            this.Text = "NewSessionDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox addressTextBox;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox colsTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox rowsTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button start;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.CheckBox autoConnectCheckBox;
    }
}