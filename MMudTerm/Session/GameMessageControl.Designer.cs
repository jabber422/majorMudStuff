namespace MMudTerm.Session
{
    partial class GameMessageControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox_blind = new System.Windows.Forms.CheckBox();
            this.checkBox_confused = new System.Windows.Forms.CheckBox();
            this.checkBox_poisoned = new System.Windows.Forms.CheckBox();
            this.checkBox_hprgen = new System.Windows.Forms.CheckBox();
            this.checkBox_losinghp = new System.Windows.Forms.CheckBox();
            this.checkBox_diseased = new System.Windows.Forms.CheckBox();
            this.checkBox_lastactionfailed = new System.Windows.Forms.CheckBox();
            this.checkBox_endscombt = new System.Windows.Forms.CheckBox();
            this.checkBox_noattack = new System.Windows.Forms.CheckBox();
            this.checkBox_nomove = new System.Windows.Forms.CheckBox();
            this.checkBox_mprgen = new System.Windows.Forms.CheckBox();
            this.radioButton_ignore = new System.Windows.Forms.RadioButton();
            this.radioButton_checkroom = new System.Windows.Forms.RadioButton();
            this.radioButton_waitwearoff = new System.Windows.Forms.RadioButton();
            this.radioButton_rest_hp = new System.Windows.Forms.RadioButton();
            this.radioButton_hang = new System.Windows.Forms.RadioButton();
            this.radioButton_run = new System.Windows.Forms.RadioButton();
            this.radioButton_restmp = new System.Windows.Forms.RadioButton();
            this.checkBox_disable = new System.Windows.Forms.CheckBox();
            this.checkBox_usechase = new System.Windows.Forms.CheckBox();
            this.checkBox_findinconv = new System.Windows.Forms.CheckBox();
            this.checkBox_findanywhere = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_use = new System.Windows.Forms.ComboBox();
            this.textBox_message = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_endswith = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_response = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button_help = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_ok = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_response);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBox_endswith);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textBox_message);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBox_use);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_name);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(480, 187);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Message";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_lastactionfailed);
            this.groupBox2.Controls.Add(this.checkBox_endscombt);
            this.groupBox2.Controls.Add(this.checkBox_noattack);
            this.groupBox2.Controls.Add(this.checkBox_nomove);
            this.groupBox2.Controls.Add(this.checkBox_mprgen);
            this.groupBox2.Controls.Add(this.checkBox_hprgen);
            this.groupBox2.Controls.Add(this.checkBox_losinghp);
            this.groupBox2.Controls.Add(this.checkBox_diseased);
            this.groupBox2.Controls.Add(this.checkBox_poisoned);
            this.groupBox2.Controls.Add(this.checkBox_confused);
            this.groupBox2.Controls.Add(this.checkBox_blind);
            this.groupBox2.Location = new System.Drawing.Point(3, 196);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(237, 385);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Effect";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox_disable);
            this.groupBox3.Controls.Add(this.checkBox_usechase);
            this.groupBox3.Controls.Add(this.radioButton_hang);
            this.groupBox3.Controls.Add(this.checkBox_findinconv);
            this.groupBox3.Controls.Add(this.radioButton_run);
            this.groupBox3.Controls.Add(this.checkBox_findanywhere);
            this.groupBox3.Controls.Add(this.radioButton_restmp);
            this.groupBox3.Controls.Add(this.radioButton_rest_hp);
            this.groupBox3.Controls.Add(this.radioButton_waitwearoff);
            this.groupBox3.Controls.Add(this.radioButton_checkroom);
            this.groupBox3.Controls.Add(this.radioButton_ignore);
            this.groupBox3.Location = new System.Drawing.Point(246, 196);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(237, 385);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Action";
            // 
            // checkBox_blind
            // 
            this.checkBox_blind.AutoSize = true;
            this.checkBox_blind.Location = new System.Drawing.Point(6, 25);
            this.checkBox_blind.Name = "checkBox_blind";
            this.checkBox_blind.Size = new System.Drawing.Size(81, 24);
            this.checkBox_blind.TabIndex = 0;
            this.checkBox_blind.Text = "Blinded";
            this.checkBox_blind.UseVisualStyleBackColor = true;
            // 
            // checkBox_confused
            // 
            this.checkBox_confused.AutoSize = true;
            this.checkBox_confused.Location = new System.Drawing.Point(6, 55);
            this.checkBox_confused.Name = "checkBox_confused";
            this.checkBox_confused.Size = new System.Drawing.Size(97, 24);
            this.checkBox_confused.TabIndex = 1;
            this.checkBox_confused.Text = "Confused";
            this.checkBox_confused.UseVisualStyleBackColor = true;
            // 
            // checkBox_poisoned
            // 
            this.checkBox_poisoned.AutoSize = true;
            this.checkBox_poisoned.Location = new System.Drawing.Point(6, 85);
            this.checkBox_poisoned.Name = "checkBox_poisoned";
            this.checkBox_poisoned.Size = new System.Drawing.Size(94, 24);
            this.checkBox_poisoned.TabIndex = 2;
            this.checkBox_poisoned.Text = "Poisoned";
            this.checkBox_poisoned.UseVisualStyleBackColor = true;
            // 
            // checkBox_hprgen
            // 
            this.checkBox_hprgen.AutoSize = true;
            this.checkBox_hprgen.Location = new System.Drawing.Point(6, 175);
            this.checkBox_hprgen.Name = "checkBox_hprgen";
            this.checkBox_hprgen.Size = new System.Drawing.Size(162, 24);
            this.checkBox_hprgen.TabIndex = 5;
            this.checkBox_hprgen.Text = "HP\'s Regenerating";
            this.checkBox_hprgen.UseVisualStyleBackColor = true;
            // 
            // checkBox_losinghp
            // 
            this.checkBox_losinghp.AutoSize = true;
            this.checkBox_losinghp.Location = new System.Drawing.Point(6, 145);
            this.checkBox_losinghp.Name = "checkBox_losinghp";
            this.checkBox_losinghp.Size = new System.Drawing.Size(197, 24);
            this.checkBox_losinghp.TabIndex = 4;
            this.checkBox_losinghp.Text = "Losing HPs (on fire, etc)";
            this.checkBox_losinghp.UseVisualStyleBackColor = true;
            // 
            // checkBox_diseased
            // 
            this.checkBox_diseased.AutoSize = true;
            this.checkBox_diseased.Location = new System.Drawing.Point(6, 115);
            this.checkBox_diseased.Name = "checkBox_diseased";
            this.checkBox_diseased.Size = new System.Drawing.Size(95, 24);
            this.checkBox_diseased.TabIndex = 3;
            this.checkBox_diseased.Text = "Diseased";
            this.checkBox_diseased.UseVisualStyleBackColor = true;
            // 
            // checkBox_lastactionfailed
            // 
            this.checkBox_lastactionfailed.AutoSize = true;
            this.checkBox_lastactionfailed.Location = new System.Drawing.Point(6, 325);
            this.checkBox_lastactionfailed.Name = "checkBox_lastactionfailed";
            this.checkBox_lastactionfailed.Size = new System.Drawing.Size(148, 24);
            this.checkBox_lastactionfailed.TabIndex = 10;
            this.checkBox_lastactionfailed.Text = "Last action failed";
            this.checkBox_lastactionfailed.UseVisualStyleBackColor = true;
            // 
            // checkBox_endscombt
            // 
            this.checkBox_endscombt.AutoSize = true;
            this.checkBox_endscombt.Location = new System.Drawing.Point(6, 295);
            this.checkBox_endscombt.Name = "checkBox_endscombt";
            this.checkBox_endscombt.Size = new System.Drawing.Size(125, 24);
            this.checkBox_endscombt.TabIndex = 9;
            this.checkBox_endscombt.Text = "Ends Combat";
            this.checkBox_endscombt.UseVisualStyleBackColor = true;
            // 
            // checkBox_noattack
            // 
            this.checkBox_noattack.AutoSize = true;
            this.checkBox_noattack.Location = new System.Drawing.Point(6, 265);
            this.checkBox_noattack.Name = "checkBox_noattack";
            this.checkBox_noattack.Size = new System.Drawing.Size(150, 24);
            this.checkBox_noattack.TabIndex = 8;
            this.checkBox_noattack.Text = "Attack Prevented";
            this.checkBox_noattack.UseVisualStyleBackColor = true;
            // 
            // checkBox_nomove
            // 
            this.checkBox_nomove.AutoSize = true;
            this.checkBox_nomove.Location = new System.Drawing.Point(6, 235);
            this.checkBox_nomove.Name = "checkBox_nomove";
            this.checkBox_nomove.Size = new System.Drawing.Size(177, 24);
            this.checkBox_nomove.TabIndex = 7;
            this.checkBox_nomove.Text = "Movement prevented";
            this.checkBox_nomove.UseVisualStyleBackColor = true;
            // 
            // checkBox_mprgen
            // 
            this.checkBox_mprgen.AutoSize = true;
            this.checkBox_mprgen.Location = new System.Drawing.Point(6, 205);
            this.checkBox_mprgen.Name = "checkBox_mprgen";
            this.checkBox_mprgen.Size = new System.Drawing.Size(169, 24);
            this.checkBox_mprgen.TabIndex = 6;
            this.checkBox_mprgen.Text = "Mana Regenerating";
            this.checkBox_mprgen.UseVisualStyleBackColor = true;
            // 
            // radioButton_ignore
            // 
            this.radioButton_ignore.AutoSize = true;
            this.radioButton_ignore.Location = new System.Drawing.Point(6, 25);
            this.radioButton_ignore.Name = "radioButton_ignore";
            this.radioButton_ignore.Size = new System.Drawing.Size(73, 24);
            this.radioButton_ignore.TabIndex = 0;
            this.radioButton_ignore.TabStop = true;
            this.radioButton_ignore.Text = "Ignore";
            this.radioButton_ignore.UseVisualStyleBackColor = true;
            // 
            // radioButton_checkroom
            // 
            this.radioButton_checkroom.AutoSize = true;
            this.radioButton_checkroom.Location = new System.Drawing.Point(6, 55);
            this.radioButton_checkroom.Name = "radioButton_checkroom";
            this.radioButton_checkroom.Size = new System.Drawing.Size(203, 24);
            this.radioButton_checkroom.TabIndex = 1;
            this.radioButton_checkroom.TabStop = true;
            this.radioButton_checkroom.Text = "Check who is in the room";
            this.radioButton_checkroom.UseVisualStyleBackColor = true;
            // 
            // radioButton_waitwearoff
            // 
            this.radioButton_waitwearoff.AutoSize = true;
            this.radioButton_waitwearoff.Location = new System.Drawing.Point(6, 85);
            this.radioButton_waitwearoff.Name = "radioButton_waitwearoff";
            this.radioButton_waitwearoff.Size = new System.Drawing.Size(173, 24);
            this.radioButton_waitwearoff.TabIndex = 2;
            this.radioButton_waitwearoff.TabStop = true;
            this.radioButton_waitwearoff.Text = "Wait until it wears off";
            this.radioButton_waitwearoff.UseVisualStyleBackColor = true;
            // 
            // radioButton_rest_hp
            // 
            this.radioButton_rest_hp.AutoSize = true;
            this.radioButton_rest_hp.Location = new System.Drawing.Point(6, 115);
            this.radioButton_rest_hp.Name = "radioButton_rest_hp";
            this.radioButton_rest_hp.Size = new System.Drawing.Size(155, 24);
            this.radioButton_rest_hp.TabIndex = 3;
            this.radioButton_rest_hp.TabStop = true;
            this.radioButton_rest_hp.Text = "Rest until full HP\'s";
            this.radioButton_rest_hp.UseVisualStyleBackColor = true;
            // 
            // radioButton_hang
            // 
            this.radioButton_hang.AutoSize = true;
            this.radioButton_hang.Location = new System.Drawing.Point(6, 204);
            this.radioButton_hang.Name = "radioButton_hang";
            this.radioButton_hang.Size = new System.Drawing.Size(84, 24);
            this.radioButton_hang.TabIndex = 6;
            this.radioButton_hang.TabStop = true;
            this.radioButton_hang.Text = "Hangup";
            this.radioButton_hang.UseVisualStyleBackColor = true;
            // 
            // radioButton_run
            // 
            this.radioButton_run.AutoSize = true;
            this.radioButton_run.Location = new System.Drawing.Point(6, 174);
            this.radioButton_run.Name = "radioButton_run";
            this.radioButton_run.Size = new System.Drawing.Size(131, 24);
            this.radioButton_run.TabIndex = 5;
            this.radioButton_run.TabStop = true;
            this.radioButton_run.Text = "Don\'t rest, run!";
            this.radioButton_run.UseVisualStyleBackColor = true;
            // 
            // radioButton_restmp
            // 
            this.radioButton_restmp.AutoSize = true;
            this.radioButton_restmp.Location = new System.Drawing.Point(6, 144);
            this.radioButton_restmp.Name = "radioButton_restmp";
            this.radioButton_restmp.Size = new System.Drawing.Size(162, 24);
            this.radioButton_restmp.TabIndex = 4;
            this.radioButton_restmp.TabStop = true;
            this.radioButton_restmp.Text = "Rest until full Mana";
            this.radioButton_restmp.UseVisualStyleBackColor = true;
            // 
            // checkBox_disable
            // 
            this.checkBox_disable.AutoSize = true;
            this.checkBox_disable.Location = new System.Drawing.Point(6, 355);
            this.checkBox_disable.Name = "checkBox_disable";
            this.checkBox_disable.Size = new System.Drawing.Size(169, 24);
            this.checkBox_disable.TabIndex = 14;
            this.checkBox_disable.Text = "Disabled (don\'t use)";
            this.checkBox_disable.UseVisualStyleBackColor = true;
            // 
            // checkBox_usechase
            // 
            this.checkBox_usechase.AutoSize = true;
            this.checkBox_usechase.Location = new System.Drawing.Point(6, 325);
            this.checkBox_usechase.Name = "checkBox_usechase";
            this.checkBox_usechase.Size = new System.Drawing.Size(158, 24);
            this.checkBox_usechase.TabIndex = 13;
            this.checkBox_usechase.Text = "Use when chasing";
            this.checkBox_usechase.UseVisualStyleBackColor = true;
            // 
            // checkBox_findinconv
            // 
            this.checkBox_findinconv.AutoSize = true;
            this.checkBox_findinconv.Location = new System.Drawing.Point(6, 295);
            this.checkBox_findinconv.Name = "checkBox_findinconv";
            this.checkBox_findinconv.Size = new System.Drawing.Size(177, 24);
            this.checkBox_findinconv.TabIndex = 12;
            this.checkBox_findinconv.Text = "Find in conversations";
            this.checkBox_findinconv.UseVisualStyleBackColor = true;
            // 
            // checkBox_findanywhere
            // 
            this.checkBox_findanywhere.AutoSize = true;
            this.checkBox_findanywhere.Location = new System.Drawing.Point(6, 265);
            this.checkBox_findanywhere.Name = "checkBox_findanywhere";
            this.checkBox_findanywhere.Size = new System.Drawing.Size(177, 24);
            this.checkBox_findanywhere.TabIndex = 11;
            this.checkBox_findanywhere.Text = "Find anywhere in text";
            this.checkBox_findanywhere.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(94, 19);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(380, 26);
            this.textBox_name.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Use";
            // 
            // comboBox_use
            // 
            this.comboBox_use.FormattingEnabled = true;
            this.comboBox_use.Location = new System.Drawing.Point(94, 51);
            this.comboBox_use.Name = "comboBox_use";
            this.comboBox_use.Size = new System.Drawing.Size(380, 28);
            this.comboBox_use.TabIndex = 3;
            // 
            // textBox_message
            // 
            this.textBox_message.Location = new System.Drawing.Point(94, 85);
            this.textBox_message.Name = "textBox_message";
            this.textBox_message.Size = new System.Drawing.Size(380, 26);
            this.textBox_message.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Message";
            // 
            // textBox_endswith
            // 
            this.textBox_endswith.Location = new System.Drawing.Point(94, 117);
            this.textBox_endswith.Name = "textBox_endswith";
            this.textBox_endswith.Size = new System.Drawing.Size(380, 26);
            this.textBox_endswith.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Ends with";
            // 
            // textBox_response
            // 
            this.textBox_response.Location = new System.Drawing.Point(94, 149);
            this.textBox_response.Name = "textBox_response";
            this.textBox_response.Size = new System.Drawing.Size(380, 26);
            this.textBox_response.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "Response";
            // 
            // button_help
            // 
            this.button_help.Location = new System.Drawing.Point(408, 587);
            this.button_help.Name = "button_help";
            this.button_help.Size = new System.Drawing.Size(75, 30);
            this.button_help.TabIndex = 3;
            this.button_help.Text = "Help";
            this.button_help.UseVisualStyleBackColor = true;
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(327, 587);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 30);
            this.button_cancel.TabIndex = 4;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            // 
            // button_ok
            // 
            this.button_ok.Location = new System.Drawing.Point(246, 587);
            this.button_ok.Name = "button_ok";
            this.button_ok.Size = new System.Drawing.Size(75, 30);
            this.button_ok.TabIndex = 5;
            this.button_ok.Text = "Ok";
            this.button_ok.UseVisualStyleBackColor = true;
            // 
            // GameMessageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_ok);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_help);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "GameMessageControl";
            this.Size = new System.Drawing.Size(497, 623);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBox_lastactionfailed;
        private System.Windows.Forms.CheckBox checkBox_endscombt;
        private System.Windows.Forms.CheckBox checkBox_noattack;
        private System.Windows.Forms.CheckBox checkBox_nomove;
        private System.Windows.Forms.CheckBox checkBox_mprgen;
        private System.Windows.Forms.CheckBox checkBox_hprgen;
        private System.Windows.Forms.CheckBox checkBox_losinghp;
        private System.Windows.Forms.CheckBox checkBox_diseased;
        private System.Windows.Forms.CheckBox checkBox_poisoned;
        private System.Windows.Forms.CheckBox checkBox_confused;
        private System.Windows.Forms.CheckBox checkBox_blind;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButton_hang;
        private System.Windows.Forms.RadioButton radioButton_run;
        private System.Windows.Forms.RadioButton radioButton_restmp;
        private System.Windows.Forms.RadioButton radioButton_rest_hp;
        private System.Windows.Forms.RadioButton radioButton_waitwearoff;
        private System.Windows.Forms.RadioButton radioButton_checkroom;
        private System.Windows.Forms.RadioButton radioButton_ignore;
        private System.Windows.Forms.CheckBox checkBox_disable;
        private System.Windows.Forms.CheckBox checkBox_usechase;
        private System.Windows.Forms.CheckBox checkBox_findinconv;
        private System.Windows.Forms.CheckBox checkBox_findanywhere;
        private System.Windows.Forms.TextBox textBox_response;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_endswith;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_message;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_use;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_help;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Button button_ok;
    }
}
