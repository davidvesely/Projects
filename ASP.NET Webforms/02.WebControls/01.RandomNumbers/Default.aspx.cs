using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _01.RandomNumbers
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonGenerate_OnClick(object sender, EventArgs e)
        {
            var rnd = new Random();
            int rangeStart = int.Parse(TextBoxRangeStart.Text);
            int rangeEnd = int.Parse(TextBoxRangeEnd.Text);
            int random = rnd.Next(rangeStart, rangeEnd);
            LabelResult.Text = random.ToString(CultureInfo.InvariantCulture);
        }
    }
}