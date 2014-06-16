using DataGridExample.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace DataGridExample.Forms
{
    public partial class FormOrders : Telerik.WinControls.UI.RadForm
    {
        private OrdersLayer ordersLayer;
        public FormOrders()
        {
            InitializeComponent();
            ordersLayer = new OrdersLayer();
        }

        private void FormOrders_Load(object sender, EventArgs e)
        {
            radGridView1.DataSource = ordersLayer.GetOrders();
            GridViewComboBoxColumn statusColumn = radGridView1.Columns["status"] as GridViewComboBoxColumn;
            statusColumn.DataSource = ordersLayer.GetStatus();
            string[] types = new string[] { "Type1", "Type2", "Type3" };
            GridViewComboBoxColumn typeColumn = radGridView1.Columns["type"] as GridViewComboBoxColumn;
            typeColumn.DataSource = types;
        }

        private void radButtonSave_Click(object sender, EventArgs e)
        {
            ordersLayer.SaveChanges();
        }
    }
}
