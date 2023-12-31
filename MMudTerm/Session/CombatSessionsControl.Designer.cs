namespace MMudTerm.Session
{
    partial class CombatSessionsControl
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label_roomname_value = new System.Windows.Forms.Label();
            this.label_roomname = new System.Windows.Forms.Label();
            this.label_also_here_value = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label_also_here_value);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.label_roomname_value);
            this.splitContainer1.Panel1.Controls.Add(this.label_roomname);
            this.splitContainer1.Size = new System.Drawing.Size(870, 639);
            this.splitContainer1.SplitterDistance = 92;
            this.splitContainer1.TabIndex = 0;
            // 
            // label_roomname_value
            // 
            this.label_roomname_value.AutoSize = true;
            this.label_roomname_value.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_roomname_value.Location = new System.Drawing.Point(108, 15);
            this.label_roomname_value.Name = "label_roomname_value";
            this.label_roomname_value.Size = new System.Drawing.Size(72, 19);
            this.label_roomname_value.TabIndex = 83;
            this.label_roomname_value.Text = "Unknown";
            // 
            // label_roomname
            // 
            this.label_roomname.AutoSize = true;
            this.label_roomname.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_roomname.Location = new System.Drawing.Point(3, 15);
            this.label_roomname.Name = "label_roomname";
            this.label_roomname.Size = new System.Drawing.Size(99, 19);
            this.label_roomname.TabIndex = 82;
            this.label_roomname.Text = "Room Name:";
            // 
            // label_also_here_value
            // 
            this.label_also_here_value.AutoSize = true;
            this.label_also_here_value.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_also_here_value.Location = new System.Drawing.Point(108, 34);
            this.label_also_here_value.Name = "label_also_here_value";
            this.label_also_here_value.Size = new System.Drawing.Size(72, 19);
            this.label_also_here_value.TabIndex = 85;
            this.label_also_here_value.Text = "Unknown";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 19);
            this.label2.TabIndex = 84;
            this.label2.Text = "Also Here:";
            // 
            // CombatSessionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "CombatSessionsControl";
            this.Size = new System.Drawing.Size(870, 639);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label_also_here_value;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_roomname_value;
        private System.Windows.Forms.Label label_roomname;
    }
}
