using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // initialize LINQ page
            ShowLinq();
        }

        //-----------------------------------------------------------------------------------
        #region ** Grid, Master Detail

        // save/cancel/refresh changes in the data source
        void _btnSave_Click(object sender, EventArgs e)
        {
            entityDataSource1.SaveChanges();
        }
        void _btnCancel_Click(object sender, EventArgs e)
        {
            entityDataSource1.CancelChanges();
        }
        void _btnRefresh_Click(object sender, EventArgs e)
        {
            entityDataSource1.Refresh();
        }

        // report any errors
        void entityDataSource1_DataError(object sender, EFWinforms.DataErrorEventArgs e)
        {
            MessageBox.Show("Error Detected:\r\n" + e.Exception.Message);
            entityDataSource1.CancelChanges(); 
            e.Handled = true;
        }

        #endregion

        //-----------------------------------------------------------------------------------
        #region ** Chart

        // update chart when list changes
        void chartBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            chart1.DataBind();
        }

        // update filter when text changes
        void _txtMinPrice_Validated(object sender, EventArgs e)
        {
            ApplyFilter();
        }
        void _txtMinPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                ApplyFilter();
                e.Handled = true;
            }
        }

        // apply the filter
        void ApplyFilter()
        {
            // never show discontinued products
            var filter = "(Not Discontinued)";

            // apply minimum price condition
            var minPrice = _txtMinPrice.Text.Trim();
            if (!string.IsNullOrEmpty(minPrice))
            {
                double d;
                if (!double.TryParse(minPrice, out d))
                {
                    MessageBox.Show("Invalid Minimum Unit Price, please try again.");
                }
                else
                {
                    filter += string.Format(" and (UnitPrice >= {0})", minPrice);
                }
            }

            // set the filter
            chartBindingSource.Filter = filter;
        }
        #endregion

        //-----------------------------------------------------------------------------------
        #region ** LINQ

        void ShowLinq()
        {
            // some LINQ query
            var q =
                from Order o in entityDataSource1.EntitySets["Orders"]
                select new
                {
                    OrderID = o.OrderID,
                    ShipName = o.ShipName,
                    ShipAddress = o.ShipAddress,
                    ShipCity = o.ShipCity,
                    ShipCountry = o.ShipCountry,
                    Customer = o.Customer.CompanyName,
                    Address = o.Customer.Address,
                    City = o.Customer.City,
                    Country = o.Customer.Country,
                    SalesPerson = o.Employee.FirstName + " " + o.Employee.LastName,
                    // SalesPerson = o.Employee.FullName,
                    OrderDate = o.OrderDate,
                    RequiredDate = o.RequiredDate,
                    ShippedDate = o.ShippedDate,
                    Amount = 
                    (
                        from Order_Detail od in o.Order_Details
                        select (double)od.UnitPrice * od.Quantity * (1 - od.Discount)
                    ).Sum()
                };

            // create BindingList (sortable/filterable)
            var bindingList = entityDataSource1.CreateView(q);

            // assign BindingList to grid
            _gridLINQ.DataSource = bindingList;
        }

        #endregion
    }
}
