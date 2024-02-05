namespace MMudTerm.Session
{
    partial class MessagesResponsesControl
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_msg = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader_use = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_new = new System.Windows.Forms.Button();
            this.button_modify = new System.Windows.Forms.Button();
            this.button_remove = new System.Windows.Forms.Button();
            this.button_findnext = new System.Windows.Forms.Button();
            this.button_find = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader_name,
            this.columnHeader_msg,
            this.columnHeader_use});
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(491, 367);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader_name
            // 
            this.columnHeader_name.Text = "Name";
            this.columnHeader_name.Width = 116;
            // 
            // columnHeader_msg
            // 
            this.columnHeader_msg.Text = "Message";
            this.columnHeader_msg.Width = 305;
            // 
            // columnHeader_use
            // 
            this.columnHeader_use.Text = "Use";
            // 
            // button_new
            // 
            this.button_new.Location = new System.Drawing.Point(500, 3);
            this.button_new.Name = "button_new";
            this.button_new.Size = new System.Drawing.Size(88, 29);
            this.button_new.TabIndex = 1;
            this.button_new.Text = "New...";
            this.button_new.UseVisualStyleBackColor = true;
            // 
            // button_modify
            // 
            this.button_modify.Location = new System.Drawing.Point(500, 38);
            this.button_modify.Name = "button_modify";
            this.button_modify.Size = new System.Drawing.Size(88, 29);
            this.button_modify.TabIndex = 2;
            this.button_modify.Text = "Modify...";
            this.button_modify.UseVisualStyleBackColor = true;
            // 
            // button_remove
            // 
            this.button_remove.Location = new System.Drawing.Point(500, 73);
            this.button_remove.Name = "button_remove";
            this.button_remove.Size = new System.Drawing.Size(88, 29);
            this.button_remove.TabIndex = 3;
            this.button_remove.Text = "Remove";
            this.button_remove.UseVisualStyleBackColor = true;
            // 
            // button_findnext
            // 
            this.button_findnext.Location = new System.Drawing.Point(500, 274);
            this.button_findnext.Name = "button_findnext";
            this.button_findnext.Size = new System.Drawing.Size(88, 29);
            this.button_findnext.TabIndex = 5;
            this.button_findnext.Text = "Find Next";
            this.button_findnext.UseVisualStyleBackColor = true;
            // 
            // button_find
            // 
            this.button_find.Location = new System.Drawing.Point(500, 239);
            this.button_find.Name = "button_find";
            this.button_find.Size = new System.Drawing.Size(88, 29);
            this.button_find.TabIndex = 4;
            this.button_find.Text = "Find...";
            this.button_find.UseVisualStyleBackColor = true;
            // 
            // MessagesResponsesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button_findnext);
            this.Controls.Add(this.button_find);
            this.Controls.Add(this.button_remove);
            this.Controls.Add(this.button_modify);
            this.Controls.Add(this.button_new);
            this.Controls.Add(this.listView1);
            this.Name = "MessagesResponsesControl";
            this.Size = new System.Drawing.Size(592, 378);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader_name;
        private System.Windows.Forms.ColumnHeader columnHeader_msg;
        private System.Windows.Forms.ColumnHeader columnHeader_use;
        private System.Windows.Forms.Button button_new;
        private System.Windows.Forms.Button button_modify;
        private System.Windows.Forms.Button button_remove;
        private System.Windows.Forms.Button button_findnext;
        private System.Windows.Forms.Button button_find;
    }
}
