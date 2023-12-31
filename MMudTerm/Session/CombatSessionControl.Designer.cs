namespace MMudTerm.Session
{
    partial class CombatSessionControl
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
            this.label_target_name = new System.Windows.Forms.Label();
            this.label_ongoing_value = new System.Windows.Forms.Label();
            this.label_rcv_dmg_value = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label_rcvr_damage_from_player_value = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label_taken_damage_from_player_value = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label_taken_damage_value = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Target:";
            // 
            // label_target_name
            // 
            this.label_target_name.AutoSize = true;
            this.label_target_name.Location = new System.Drawing.Point(70, 4);
            this.label_target_name.Name = "label_target_name";
            this.label_target_name.Size = new System.Drawing.Size(51, 20);
            this.label_target_name.TabIndex = 1;
            this.label_target_name.Text = "label2";
            // 
            // label_ongoing_value
            // 
            this.label_ongoing_value.AutoSize = true;
            this.label_ongoing_value.Dock = System.Windows.Forms.DockStyle.Right;
            this.label_ongoing_value.Location = new System.Drawing.Point(277, 0);
            this.label_ongoing_value.Name = "label_ongoing_value";
            this.label_ongoing_value.Size = new System.Drawing.Size(51, 20);
            this.label_ongoing_value.TabIndex = 2;
            this.label_ongoing_value.Text = "label2";
            // 
            // label_rcv_dmg_value
            // 
            this.label_rcv_dmg_value.AutoSize = true;
            this.label_rcv_dmg_value.Location = new System.Drawing.Point(165, 47);
            this.label_rcv_dmg_value.Name = "label_rcv_dmg_value";
            this.label_rcv_dmg_value.Size = new System.Drawing.Size(51, 20);
            this.label_rcv_dmg_value.TabIndex = 4;
            this.label_rcv_dmg_value.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Damage Received:";
            // 
            // label_rcvr_damage_from_player_value
            // 
            this.label_rcvr_damage_from_player_value.AutoSize = true;
            this.label_rcvr_damage_from_player_value.Location = new System.Drawing.Point(165, 78);
            this.label_rcvr_damage_from_player_value.Name = "label_rcvr_damage_from_player_value";
            this.label_rcvr_damage_from_player_value.Size = new System.Drawing.Size(51, 20);
            this.label_rcvr_damage_from_player_value.TabIndex = 6;
            this.label_rcvr_damage_from_player_value.Text = "label4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 78);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(157, 20);
            this.label5.TabIndex = 5;
            this.label5.Text = "Damage from Player:";
            // 
            // label_taken_damage_from_player_value
            // 
            this.label_taken_damage_from_player_value.AutoSize = true;
            this.label_taken_damage_from_player_value.Location = new System.Drawing.Point(165, 139);
            this.label_taken_damage_from_player_value.Name = "label_taken_damage_from_player_value";
            this.label_taken_damage_from_player_value.Size = new System.Drawing.Size(51, 20);
            this.label_taken_damage_from_player_value.TabIndex = 10;
            this.label_taken_damage_from_player_value.Text = "label6";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(157, 20);
            this.label7.TabIndex = 9;
            this.label7.Text = "Damage from Player:";
            // 
            // label_taken_damage_value
            // 
            this.label_taken_damage_value.AutoSize = true;
            this.label_taken_damage_value.Location = new System.Drawing.Point(165, 108);
            this.label_taken_damage_value.Name = "label_taken_damage_value";
            this.label_taken_damage_value.Size = new System.Drawing.Size(51, 20);
            this.label_taken_damage_value.TabIndex = 8;
            this.label_taken_damage_value.Text = "label8";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(4, 108);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(122, 20);
            this.label9.TabIndex = 7;
            this.label9.Text = "Damage Taken:";
            // 
            // CombatSessionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_taken_damage_from_player_value);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label_taken_damage_value);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label_rcvr_damage_from_player_value);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label_rcv_dmg_value);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label_ongoing_value);
            this.Controls.Add(this.label_target_name);
            this.Controls.Add(this.label1);
            this.Name = "CombatSessionControl";
            this.Size = new System.Drawing.Size(328, 204);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_target_name;
        private System.Windows.Forms.Label label_ongoing_value;
        private System.Windows.Forms.Label label_rcv_dmg_value;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_rcvr_damage_from_player_value;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label_taken_damage_from_player_value;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label_taken_damage_value;
        private System.Windows.Forms.Label label9;
    }
}
