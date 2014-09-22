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
        // Class за работа с базата
        private DatabaseLayer dbLayer = new DatabaseLayer();

        public FormOPR()
        {
            InitializeComponent();
        }

        private void FormOrders_Load(object sender, EventArgs e)
        {
            // Източник на данни за главния Grid
            radGridView1.DataSource = dbLayer.GetOPRRows();
            // Колоната TYPE_DOC е направена като ComboBox, item-ите си ги взима от DatabaseLayer класа
            radGridView1.SetColumnDataSource("TYPE_DOC", dbLayer.GetDocType());
            // ComboBox колона с item-и D и K
            string[] d_k = new string[] { "D", "K" };
            radGridView2.SetColumnDataSource("D_K", d_k);
        }

        private void radButtonSave_Click(object sender, EventArgs e)
        {
            // Запис на промените
            dbLayer.SaveChanges();
        }

        // Event, който се случва при натискане на ред от първият грид
        private void radGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // Ако е натиснат ред
            if (radGridView1.CurrentRow.DataBoundItem != null)
            {
                GridViewRowInfo currentRow = radGridView1.CurrentRow;
                // Взима индекса на натиснатия ред
                int index = currentRow.Index;
                var cell = radGridView1.Rows[index].Cells["ID"];
                if (cell != null && cell.Value != null)
                {
                    // Взима ID-то от таблицата на натиснатия ред
                    var rowId = (int)cell.Value;
                    // Казва на втория грид какво да зареди (дърпа редовете от втория грид със същото ID)
                    radGridView2.DataSource = dbLayer.GetOPRITEMRows(rowId);
                }
            }
        }

        // При добавяне на ред във втория грид
        // Необходими са няколко допълнителни стъпки, тъй като DataSource на грида е заявка от базата
        // и затова грида незнае къде да insert-не новия ред
        private void radGridView2_UserAddingRow(object sender, GridViewRowCancelEventArgs e)
        {
            // Взимат се съответното ID и POS
            int id = (int)radGridView1.SelectedRows[0].Cells["ID"].Value;
            int pos = dbLayer.GetNextPos(id); // Взима се максималната стойност на POS + 1
            e.Rows[0].Cells["ID"].Value = id;
            e.Rows[0].Cells["POS"].Value = pos;

            // Използвам функция за превръщане на данните от новия ред в обект от Database модела
            OPRITEM entity = ExtensionUtils.ConvertRowToEntity<OPRITEM>(e.Rows[0].Cells);
            dbLayer.InsertOPRITEM(entity);
        }

        private void radGridView1_UserAddingRow(object sender, GridViewRowCancelEventArgs e)
        {
            // Тук добавянето на ред е по-лесно, защото се поема автоматично от .NET Framework-а
            // тъй като на грида съм задал да си дърпа данните от цялата таблица
            int id = dbLayer.GetNextID();
            e.Rows[0].Cells["ID"].Value = id;
        }
    }
}
