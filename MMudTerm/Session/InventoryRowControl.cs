using MMudObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MMudTerm.Session
{
    public partial class InventoryRowControl : UserControl
    {
        SessionController _controller = null;
        Item _item = null;
        public InventoryRowControl(SessionController controller, CarryableItem item)
        {
            InitializeComponent();
            this._controller = controller;
            this._item = item;
            this.label_count_value.Text = item.Quantity.ToString();
            this.label_item_name.Text = item.Name;
        }

        private void button_look_Click(object sender, EventArgs e)
        {
            this._controller.Send("look " + this._item.Name + "\r\n");
        }

        private void button_drop_Click(object sender, EventArgs e)
        {
            this._controller.Send("drop " + this._item.Name + "\r\n");
        }

        private void button_hide_Click(object sender, EventArgs e)
        {
            this._controller.Send("hide " + this._item.Name + "\r\n");
        }

        private void button_sell_Click(object sender, EventArgs e)
        {
            this._controller.Send("sell " + this._item.Name + "\r\n");
        }

        private void button_buy_Click(object sender, EventArgs e)
        {
            this._controller.Send("buy " + this._item.Name + "\r\n");

        }
    }
}
