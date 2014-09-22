using DataGridExample.BusinessLogic;
using DataGridExample.DatabaseModel;
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
    public partial class FormOPR : Telerik.WinControls.UI.RadForm
    {
        private DatabaseLayer dbLayer = new DatabaseLayer();

        public FormOPR()
        {
            InitializeComponent();
        }

        private void FormOrders_Load(object sender, EventArgs e)
        {
            radGridView1.DataSource = dbLayer.GetOPRRows();
            radGridView1.SetColumnDataSource("TYPE_DOC", dbLayer.GetDocType());
            string[] d_k = new string[] { "D", "K" };
            radGridView2.SetColumnDataSource("D_K", d_k);
        }

        private void radButtonSave_Click(object sender, EventArgs e)
        {
            dbLayer.SaveChanges();
        }

        private void radGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (radGridView1.CurrentRow.DataBoundItem != null)
            {
                GridViewRowInfo currentRow = radGridView1.CurrentRow;
                int index = currentRow.Index;
                var cell = radGridView1.Rows[index].Cells["ID"];
                if (cell != null && cell.Value != null)
                {
                    var rowId = (int)cell.Value;
                    radGridView2.DataSource = dbLayer.GetOPRITEMRows(rowId);
                }
            }
        }

        private void radGridView2_UserAddingRow(object sender, GridViewRowCancelEventArgs e)
        {
            int id = (int)radGridView1.SelectedRows[0].Cells["ID"].Value;
            int pos = dbLayer.GetNextPos(id);
            e.Rows[0].Cells["ID"].Value = id;
            e.Rows[0].Cells["POS"].Value = pos;

            OPRITEM entity = ExtensionUtils.ConvertRowToEntity<OPRITEM>(e.Rows[0].Cells);
            dbLayer.InsertOPRITEM(entity);
        }

        private void radGridView1_UserAddingRow(object sender, GridViewRowCancelEventArgs e)
        {
            int id = dbLayer.GetNextID();
            e.Rows[0].Cells["ID"].Value = id;
        }
    }
}
