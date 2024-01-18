namespace MMudTerm.Session
{
    partial class GotoLoopSelectForm
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.button_walk = new System.Windows.Forms.Button();
            this.button_run = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Location = new System.Drawing.Point(12, 12);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(375, 394);
            this.treeView1.TabIndex = 0;
            // 
            // button_walk
            // 
            this.button_walk.Location = new System.Drawing.Point(12, 412);
            this.button_walk.Name = "button_walk";
            this.button_walk.Size = new System.Drawing.Size(75, 26);
            this.button_walk.TabIndex = 1;
            this.button_walk.Text = "Walk";
            this.button_walk.UseVisualStyleBackColor = true;
            this.button_walk.Click += new System.EventHandler(this.button_walk_Click);
            // 
            // button_run
            // 
            this.button_run.Location = new System.Drawing.Point(93, 412);
            this.button_run.Name = "button_run";
            this.button_run.Size = new System.Drawing.Size(75, 26);
            this.button_run.TabIndex = 2;
            this.button_run.Text = "Run";
            this.button_run.UseVisualStyleBackColor = true;
            // 
            // GotoLoopSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 450);
            this.Controls.Add(this.button_run);
            this.Controls.Add(this.button_walk);
            this.Controls.Add(this.treeView1);
            this.Name = "GotoLoopSelectForm";
            this.Text = "GotoLoopSelectForm";
            this.Load += new System.EventHandler(this.GotoLoopSelectForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button button_walk;
        private System.Windows.Forms.Button button_run;
    }
}