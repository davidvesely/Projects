using DataGridExample.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataGridExample
{
    public partial class FormMainWindow : Form
    {
        public FormMainWindow()
        {
            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyAboutBox aboutBox = new MyAboutBox();
            aboutBox.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridExampleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSimpleGrid dataGrid = new FormSimpleGrid();
            dataGrid.MdiParent = this;
            dataGrid.WindowState = FormWindowState.Maximized;
            dataGrid.Show();
        }

        private void radAboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RadAboutBox1 aboutBox = new RadAboutBox1();
            aboutBox.ShowDialog();
        }
    }
}
