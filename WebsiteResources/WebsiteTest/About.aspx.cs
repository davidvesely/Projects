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
        protected void Page_Load(object sender, EventArgs e)
        {
            Literal1.Text = Resources.Resource.CustomText;
        }
    }
}