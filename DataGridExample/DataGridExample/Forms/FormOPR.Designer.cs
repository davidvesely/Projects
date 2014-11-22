namespace DataGridExample.Forms
{
    partial class FormOPR
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
            //ordersLayer.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn1 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewDateTimeColumn gridViewDateTimeColumn1 = new Telerik.WinControls.UI.GridViewDateTimeColumn();
            Telerik.WinControls.UI.GridViewDateTimeColumn gridViewDateTimeColumn2 = new Telerik.WinControls.UI.GridViewDateTimeColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewDateTimeColumn gridViewDateTimeColumn3 = new Telerik.WinControls.UI.GridViewDateTimeColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn2 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewDateTimeColumn gridViewDateTimeColumn4 = new Telerik.WinControls.UI.GridViewDateTimeColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn5 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn3 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn4 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewComboBoxColumn gridViewComboBoxColumn1 = new Telerik.WinControls.UI.GridViewComboBoxColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn5 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn6 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewComboBoxColumn gridViewComboBoxColumn2 = new Telerik.WinControls.UI.GridViewComboBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn6 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn7 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn7 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn8 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn9 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn10 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn8 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewDecimalColumn gridViewDecimalColumn11 = new Telerik.WinControls.UI.GridViewDecimalColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn9 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn10 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn11 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
            this.radGridView1 = new Telerik.WinControls.UI.RadGridView();
            this.oPRBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.radButtonSave = new Telerik.WinControls.UI.RadButton();
            this.radGridView2 = new Telerik.WinControls.UI.RadGridView();
            this.oPRITEMBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.myButton1 = new DataGridExample.MyControls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.oPRBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonSave)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView2.MasterTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.oPRITEMBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // radGridView1
            // 
            this.radGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radGridView1.Location = new System.Drawing.Point(12, 12);
            // 
            // radGridView1
            // 
            this.radGridView1.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            gridViewDecimalColumn1.DataType = typeof(int);
            gridViewDecimalColumn1.FieldName = "ID";
            gridViewDecimalColumn1.HeaderText = "ID";
            gridViewDecimalColumn1.IsAutoGenerated = true;
            gridViewDecimalColumn1.Name = "ID";
            gridViewTextBoxColumn1.FieldName = "NOM_DOC";
            gridViewTextBoxColumn1.HeaderText = "NOM_DOC";
            gridViewTextBoxColumn1.IsAutoGenerated = true;
            gridViewTextBoxColumn1.Name = "NOM_DOC";
            gridViewTextBoxColumn1.Width = 54;
            gridViewDateTimeColumn1.DataType = typeof(System.Nullable<System.DateTime>);
            gridViewDateTimeColumn1.FieldName = "DATE_DOC";
            gridViewDateTimeColumn1.HeaderText = "DATE_DOC";
            gridViewDateTimeColumn1.IsAutoGenerated = true;
            gridViewDateTimeColumn1.Name = "DATE_DOC";
            gridViewDateTimeColumn1.Width = 54;
            gridViewDateTimeColumn2.DataType = typeof(System.Nullable<System.DateTime>);
            gridViewDateTimeColumn2.FieldName = "ENTER_DATE";
            gridViewDateTimeColumn2.HeaderText = "ENTER_DATE";
            gridViewDateTimeColumn2.IsAutoGenerated = true;
            gridViewDateTimeColumn2.Name = "ENTER_DATE";
            gridViewDateTimeColumn2.Width = 54;
            gridViewTextBoxColumn2.FieldName = "ENTER_USR_CODE";
            gridViewTextBoxColumn2.HeaderText = "ENTER_USR_CODE";
            gridViewTextBoxColumn2.IsAutoGenerated = true;
            gridViewTextBoxColumn2.Name = "ENTER_USR_CODE";
            gridViewTextBoxColumn2.Width = 54;
            gridViewDateTimeColumn3.DataType = typeof(System.Nullable<System.DateTime>);
            gridViewDateTimeColumn3.FieldName = "ACC_DATE";
            gridViewDateTimeColumn3.HeaderText = "ACC_DATE";
            gridViewDateTimeColumn3.IsAutoGenerated = true;
            gridViewDateTimeColumn3.Name = "ACC_DATE";
            gridViewDateTimeColumn3.Width = 54;
            gridViewTextBoxColumn3.FieldName = "ACC_USR_CODE";
            gridViewTextBoxColumn3.HeaderText = "ACC_USR_CODE";
            gridViewTextBoxColumn3.IsAutoGenerated = true;
            gridViewTextBoxColumn3.Name = "ACC_USR_CODE";
            gridViewTextBoxColumn3.Width = 54;
            gridViewDecimalColumn2.DataType = typeof(System.Nullable<int>);
            gridViewDecimalColumn2.FieldName = "PAPKA";
            gridViewDecimalColumn2.HeaderText = "PAPKA";
            gridViewDecimalColumn2.IsAutoGenerated = true;
            gridViewDecimalColumn2.Name = "PAPKA";
            gridViewDecimalColumn2.Width = 54;
            gridViewTextBoxColumn4.FieldName = "DESCRIPTION";
            gridViewTextBoxColumn4.HeaderText = "DESCRIPTION";
            gridViewTextBoxColumn4.IsAutoGenerated = true;
            gridViewTextBoxColumn4.Name = "DESCRIPTION";
            gridViewTextBoxColumn4.Width = 54;
            gridViewDateTimeColumn4.DataType = typeof(System.Nullable<System.DateTime>);
            gridViewDateTimeColumn4.FieldName = "VALEUR";
            gridViewDateTimeColumn4.HeaderText = "VALEUR";
            gridViewDateTimeColumn4.IsAutoGenerated = true;
            gridViewDateTimeColumn4.Name = "VALEUR";
            gridViewDateTimeColumn4.Width = 54;
            gridViewTextBoxColumn5.FieldName = "STATUS";
            gridViewTextBoxColumn5.HeaderText = "STATUS";
            gridViewTextBoxColumn5.IsAutoGenerated = true;
            gridViewTextBoxColumn5.Name = "STATUS";
            gridViewTextBoxColumn5.Width = 54;
            gridViewDecimalColumn3.DataType = typeof(System.Nullable<decimal>);
            gridViewDecimalColumn3.FieldName = "NOM_PAPKA";
            gridViewDecimalColumn3.HeaderText = "NOM_PAPKA";
            gridViewDecimalColumn3.IsAutoGenerated = true;
            gridViewDecimalColumn3.Name = "NOM_PAPKA";
            gridViewDecimalColumn3.Width = 54;
            gridViewDecimalColumn4.DataType = typeof(System.Nullable<decimal>);
            gridViewDecimalColumn4.FieldName = "SEQ";
            gridViewDecimalColumn4.HeaderText = "SEQ";
            gridViewDecimalColumn4.IsAutoGenerated = true;
            gridViewDecimalColumn4.Name = "SEQ";
            gridViewDecimalColumn4.Width = 54;
            gridViewComboBoxColumn1.FieldName = "TYPE_DOC";
            gridViewComboBoxColumn1.HeaderText = "TYPE_DOC";
            gridViewComboBoxColumn1.Name = "TYPE_DOC";
            gridViewComboBoxColumn1.Width = 45;
            this.radGridView1.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewDecimalColumn1,
            gridViewTextBoxColumn1,
            gridViewDateTimeColumn1,
            gridViewDateTimeColumn2,
            gridViewTextBoxColumn2,
            gridViewDateTimeColumn3,
            gridViewTextBoxColumn3,
            gridViewDecimalColumn2,
            gridViewTextBoxColumn4,
            gridViewDateTimeColumn4,
            gridViewTextBoxColumn5,
            gridViewDecimalColumn3,
            gridViewDecimalColumn4,
            gridViewComboBoxColumn1});
            this.radGridView1.MasterTemplate.DataSource = this.oPRBindingSource;
            this.radGridView1.Name = "radGridView1";
            this.radGridView1.Size = new System.Drawing.Size(750, 307);
            this.radGridView1.TabIndex = 0;
            this.radGridView1.Text = "radGridView1";
            this.radGridView1.ThemeName = "ControlDefault";
            this.radGridView1.SelectionChanged += new System.EventHandler(this.radGridView1_SelectionChanged);
            this.radGridView1.UserAddingRow += new Telerik.WinControls.UI.GridViewRowCancelEventHandler(this.radGridView1_UserAddingRow);
            // 
            // oPRBindingSource
            // 
            this.oPRBindingSource.DataSource = typeof(DataGridExample.DatabaseModel.OPR);
            // 
            // radButtonSave
            // 
            this.radButtonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.radButtonSave.Location = new System.Drawing.Point(687, 588);
            this.radButtonSave.Name = "radButtonSave";
            this.radButtonSave.Size = new System.Drawing.Size(75, 24);
            this.radButtonSave.TabIndex = 1;
            this.radButtonSave.Text = "Save";
            this.radButtonSave.Click += new System.EventHandler(this.radButtonSave_Click);
            // 
            // radGridView2
            // 
            this.radGridView2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.radGridView2.Location = new System.Drawing.Point(12, 326);
            // 
            // radGridView2
            // 
            this.radGridView2.MasterTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
            gridViewDecimalColumn5.DataType = typeof(int);
            gridViewDecimalColumn5.FieldName = "ID";
            gridViewDecimalColumn5.HeaderText = "ID";
            gridViewDecimalColumn5.IsAutoGenerated = true;
            gridViewDecimalColumn5.Name = "ID";
            gridViewDecimalColumn5.Width = 49;
            gridViewDecimalColumn6.DataType = typeof(System.Nullable<short>);
            gridViewDecimalColumn6.FieldName = "STATIA";
            gridViewDecimalColumn6.HeaderText = "STATIA";
            gridViewDecimalColumn6.IsAutoGenerated = true;
            gridViewDecimalColumn6.Name = "STATIA";
            gridViewDecimalColumn6.Width = 54;
            gridViewComboBoxColumn2.FieldName = "D_K";
            gridViewComboBoxColumn2.HeaderText = "D_K";
            gridViewComboBoxColumn2.Name = "D_K";
            gridViewComboBoxColumn2.Width = 47;
            gridViewTextBoxColumn6.FieldName = "SMETKA";
            gridViewTextBoxColumn6.HeaderText = "SMETKA";
            gridViewTextBoxColumn6.IsAutoGenerated = true;
            gridViewTextBoxColumn6.Name = "SMETKA";
            gridViewTextBoxColumn6.Width = 54;
            gridViewTextBoxColumn7.FieldName = "PARTIDA";
            gridViewTextBoxColumn7.HeaderText = "PARTIDA";
            gridViewTextBoxColumn7.IsAutoGenerated = true;
            gridViewTextBoxColumn7.Name = "PARTIDA";
            gridViewTextBoxColumn7.Width = 54;
            gridViewDecimalColumn7.DataType = typeof(System.Nullable<decimal>);
            gridViewDecimalColumn7.FieldName = "ST_VAL";
            gridViewDecimalColumn7.HeaderText = "ST_VAL";
            gridViewDecimalColumn7.IsAutoGenerated = true;
            gridViewDecimalColumn7.Name = "ST_VAL";
            gridViewDecimalColumn7.Width = 54;
            gridViewDecimalColumn8.DataType = typeof(System.Nullable<decimal>);
            gridViewDecimalColumn8.FieldName = "ST_LV";
            gridViewDecimalColumn8.HeaderText = "ST_LV";
            gridViewDecimalColumn8.IsAutoGenerated = true;
            gridViewDecimalColumn8.Name = "ST_LV";
            gridViewDecimalColumn8.Width = 54;
            gridViewDecimalColumn9.DataType = typeof(System.Nullable<decimal>);
            gridViewDecimalColumn9.FieldName = "K_VO";
            gridViewDecimalColumn9.HeaderText = "K_VO";
            gridViewDecimalColumn9.IsAutoGenerated = true;
            gridViewDecimalColumn9.Name = "K_VO";
            gridViewDecimalColumn9.Width = 54;
            gridViewDecimalColumn10.DataType = typeof(System.Nullable<decimal>);
            gridViewDecimalColumn10.FieldName = "PRICE";
            gridViewDecimalColumn10.FormatString = "{0:c}";
            gridViewDecimalColumn10.HeaderText = "PRICE";
            gridViewDecimalColumn10.IsAutoGenerated = true;
            gridViewDecimalColumn10.Name = "PRICE";
            gridViewDecimalColumn10.Width = 54;
            gridViewTextBoxColumn8.FieldName = "TXT";
            gridViewTextBoxColumn8.HeaderText = "TXT";
            gridViewTextBoxColumn8.IsAutoGenerated = true;
            gridViewTextBoxColumn8.Name = "TXT";
            gridViewTextBoxColumn8.Width = 54;
            gridViewDecimalColumn11.DataType = typeof(short);
            gridViewDecimalColumn11.FieldName = "POS";
            gridViewDecimalColumn11.HeaderText = "POS";
            gridViewDecimalColumn11.IsAutoGenerated = true;
            gridViewDecimalColumn11.Name = "POS";
            gridViewDecimalColumn11.Width = 49;
            gridViewTextBoxColumn9.FieldName = "M_KA";
            gridViewTextBoxColumn9.HeaderText = "M_KA";
            gridViewTextBoxColumn9.IsAutoGenerated = true;
            gridViewTextBoxColumn9.Name = "M_KA";
            gridViewTextBoxColumn9.Width = 54;
            gridViewTextBoxColumn10.FieldName = "DK_STATUS";
            gridViewTextBoxColumn10.HeaderText = "DK_STATUS";
            gridViewTextBoxColumn10.IsAutoGenerated = true;
            gridViewTextBoxColumn10.Name = "DK_STATUS";
            gridViewTextBoxColumn10.Width = 54;
            gridViewTextBoxColumn11.DataType = typeof(DataGridExample.DatabaseModel.DNEVNIK_DDS);
            gridViewTextBoxColumn11.FieldName = "DNEVNIK_DDS";
            gridViewTextBoxColumn11.HeaderText = "DNEVNIK_DDS";
            gridViewTextBoxColumn11.IsAutoGenerated = true;
            gridViewTextBoxColumn11.Name = "DNEVNIK_DDS";
            gridViewTextBoxColumn11.Width = 58;
            this.radGridView2.MasterTemplate.Columns.AddRange(new Telerik.WinControls.UI.GridViewDataColumn[] {
            gridViewDecimalColumn5,
            gridViewDecimalColumn6,
            gridViewComboBoxColumn2,
            gridViewTextBoxColumn6,
            gridViewTextBoxColumn7,
            gridViewDecimalColumn7,
            gridViewDecimalColumn8,
            gridViewDecimalColumn9,
            gridViewDecimalColumn10,
            gridViewTextBoxColumn8,
            gridViewDecimalColumn11,
            gridViewTextBoxColumn9,
            gridViewTextBoxColumn10,
            gridViewTextBoxColumn11});
            this.radGridView2.MasterTemplate.DataSource = this.oPRITEMBindingSource;
            this.radGridView2.Name = "radGridView2";
            this.radGridView2.Size = new System.Drawing.Size(750, 256);
            this.radGridView2.TabIndex = 2;
            this.radGridView2.Text = "radGridView2";
            this.radGridView2.UserAddingRow += new Telerik.WinControls.UI.GridViewRowCancelEventHandler(this.radGridView2_UserAddingRow);
            // 
            // oPRITEMBindingSource
            // 
            this.oPRITEMBindingSource.DataSource = typeof(DataGridExample.DatabaseModel.OPRITEM);
            // 
            // myButton1
            // 
            this.myButton1.Location = new System.Drawing.Point(373, 588);
            this.myButton1.Name = "myButton1";
            this.myButton1.Size = new System.Drawing.Size(75, 23);
            this.myButton1.TabIndex = 3;
            this.myButton1.Text = "myButton1";
            this.myButton1.UseVisualStyleBackColor = true;
            // 
            // FormOPR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 617);
            this.Controls.Add(this.myButton1);
            this.Controls.Add(this.radGridView2);
            this.Controls.Add(this.radButtonSave);
            this.Controls.Add(this.radGridView1);
            this.Name = "FormOPR";
            // 
            // 
            // 
            this.RootElement.ApplyShapeToControl = true;
            this.Text = "FormOrders";
            this.Load += new System.EventHandler(this.FormOrders_Load);
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.oPRBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radButtonSave)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView2.MasterTemplate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.oPRITEMBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Telerik.WinControls.UI.RadGridView radGridView1;
        private Telerik.WinControls.UI.RadButton radButtonSave;
        private Telerik.WinControls.UI.RadGridView radGridView2;
        private System.Windows.Forms.BindingSource oPRBindingSource;
        private System.Windows.Forms.BindingSource oPRITEMBindingSource;
        private MyControls.MyButton myButton1;
    }
}
