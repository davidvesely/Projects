using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _01.SimpleASPX
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonShowGreeting_OnClick(object sender, EventArgs e)
        {
            LabelGreeting.Text = "Hello " + TextBoxName.Text;
            LabelAssembly.Text = Assembly.GetExecutingAssembly().Location;
        }
    }
}