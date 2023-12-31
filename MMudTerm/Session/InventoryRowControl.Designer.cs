namespace MMudTerm.Session
{
    partial class InventoryRowControl
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
            this.button_buy = new System.Windows.Forms.Button();
            this.button_sell = new System.Windows.Forms.Button();
            this.button_look = new System.Windows.Forms.Button();
            this.button_drop = new System.Windows.Forms.Button();
            this.button_hide = new System.Windows.Forms.Button();
            this.label_count_value = new System.Windows.Forms.Label();
            this.label_item_name = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_buy
            // 
            this.button_buy.Location = new System.Drawing.Point(3, 3);
            this.button_buy.Name = "button_buy";
            this.button_buy.Size = new System.Drawing.Size(53, 40);
            this.button_buy.TabIndex = 0;
            this.button_buy.Text = "Buy";
            this.button_buy.UseVisualStyleBackColor = true;
            this.button_buy.Click += new System.EventHandler(this.button_buy_Click);
            // 
            // button_sell
            // 
            this.button_sell.Location = new System.Drawing.Point(62, 3);
            this.button_sell.Name = "button_sell";
            this.button_sell.Size = new System.Drawing.Size(53, 40);
            this.button_sell.TabIndex = 1;
            this.button_sell.Text = "Sell";
            this.button_sell.UseVisualStyleBackColor = true;
            this.button_sell.Click += new System.EventHandler(this.button_sell_Click);
            // 
            // button_look
            // 
            this.button_look.Location = new System.Drawing.Point(121, 3);
            this.button_look.Name = "button_look";
            this.button_look.Size = new System.Drawing.Size(53, 40);
            this.button_look.TabIndex = 2;
            this.button_look.Text = "Look";
            this.button_look.UseVisualStyleBackColor = true;
            this.button_look.Click += new System.EventHandler(this.button_look_Click);
            // 
            // button_drop
            // 
            this.button_drop.Location = new System.Drawing.Point(180, 3);
            this.button_drop.Name = "button_drop";
            this.button_drop.Size = new System.Drawing.Size(53, 40);
            this.button_drop.TabIndex = 3;
            this.button_drop.Text = "Drop";
            this.button_drop.UseVisualStyleBackColor = true;
            this.button_drop.Click += new System.EventHandler(this.button_drop_Click);
            // 
            // button_hide
            // 
            this.button_hide.Location = new System.Drawing.Point(239, 3);
            this.button_hide.Name = "button_hide";
            this.button_hide.Size = new System.Drawing.Size(53, 40);
            this.button_hide.TabIndex = 4;
            this.button_hide.Text = "Hide";
            this.button_hide.UseVisualStyleBackColor = true;
            this.button_hide.Click += new System.EventHandler(this.button_hide_Click);
            // 
            // label_count_value
            // 
            this.label_count_value.AutoSize = true;
            this.label_count_value.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_count_value.Location = new System.Drawing.Point(298, 13);
            this.label_count_value.Name = "label_count_value";
            this.label_count_value.Size = new System.Drawing.Size(43, 23);
            this.label_count_value.TabIndex = 5;
            this.label_count_value.Text = "XXX";
            // 
            // label_item_name
            // 
            this.label_item_name.AutoSize = true;
            this.label_item_name.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_item_name.Location = new System.Drawing.Point(347, 13);
            this.label_item_name.Name = "label_item_name";
            this.label_item_name.Size = new System.Drawing.Size(43, 23);
            this.label_item_name.TabIndex = 6;
            this.label_item_name.Text = "XXX";
            // 
            // InventoryRowControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_item_name);
            this.Controls.Add(this.label_count_value);
            this.Controls.Add(this.button_hide);
            this.Controls.Add(this.button_drop);
            this.Controls.Add(this.button_look);
            this.Controls.Add(this.button_sell);
            this.Controls.Add(this.button_buy);
            this.Name = "InventoryRowControl";
            this.Size = new System.Drawing.Size(1072, 46);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_buy;
        private System.Windows.Forms.Button button_sell;
        private System.Windows.Forms.Button button_look;
        private System.Windows.Forms.Button button_drop;
        private System.Windows.Forms.Button button_hide;
        private System.Windows.Forms.Label label_count_value;
        private System.Windows.Forms.Label label_item_name;
    }
}
