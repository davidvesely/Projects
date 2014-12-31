using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebsiteTest
{
    public partial class About : Page
    {
        private class DataClass
        {
            public int ID { get; set; }
            public string Name { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Literal1.Text = Resources.Resource.CustomText;

            List<DataClass> data = new List<DataClass>()
            {
                new DataClass() { ID = 1, Name = "Name1" },
                new DataClass() { ID = 2, Name = "Name2" },
                new DataClass() { ID = 3, Name = "Name3" },
                new DataClass() { ID = 4, Name = "Name4" },
                new DataClass() { ID = 5, Name = "Name5" },
            };

            ASPxGridView1.DataSource = data;
            ASPxGridView1.DataBind();
        }
    }
}