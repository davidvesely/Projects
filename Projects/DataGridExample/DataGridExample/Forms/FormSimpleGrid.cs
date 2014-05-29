using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataGridExample.DatabaseModel;
using DataGridExample.BusinessLogic;

namespace DataGridExample.Forms
{
    public partial class FormSimpleGrid : Form
    {
        HREntities db = new HREntities();

        public FormSimpleGrid()
        {
            InitializeComponent();
        }

        private void radButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                db.Rollback();
                this.radGridView1.MasterTemplate.Refresh(null); 
                MessageBox.Show("Грешка при записване на стойностите!");
            }
        }

        private void FormSimpleGrid_Load(object sender, EventArgs e)
        {
            var query = (from t in db.EMPLOYEES
                        select t).ToList();
            radGridView1.DataSource = query;
        }
    }
}
