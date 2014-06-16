using DataGridExample.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataGridExample.Forms
{
    public partial class FormUsers : Form
    {
        private OrdersLayer ordersManager;

        public FormUsers()
        {
            InitializeComponent();
            ordersManager = new OrdersLayer();
        }

        private void FormOrdersMain_Load(object sender, EventArgs e)
        {
            radGridView1.DataSource = ordersManager.GetUsers();
        }

        private void radButtonSave_Click(object sender, EventArgs e)
        {
            ordersManager.SaveChanges();
        }
    }
}
