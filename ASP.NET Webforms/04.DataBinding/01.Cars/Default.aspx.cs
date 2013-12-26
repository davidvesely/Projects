using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _01.Cars
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            var dbEntities = new CarsEntities();

            if (!Page.IsPostBack)
            {
                var queryProducers = from p in dbEntities.Producers select p;
                
                DropDownListProducer.DataSource = queryProducers.ToList();
                DropDownListProducer.DataTextField = "Name";
                DropDownListProducer.DataValueField = "ID";
                DropDownListProducer.DataBind();

                var extras = new List<string>()
            {
                "air conditioner", "cd player", "seat heating", "shibidah"
            };
                CheckBoxListExtras.DataSource = extras;
                CheckBoxListExtras.DataBind();
            }


            int ind = DropDownListProducer.SelectedIndex;
            if (ind < 0) return;
            int producerId = int.Parse(DropDownListProducer.SelectedValue);
            var queryModels = from m in dbEntities.Models
                              where m.ProducerID == producerId
                              select m;
            DropDownListModel.DataSource = queryModels.ToList();
            DropDownListModel.DataTextField = "Name";
            DropDownListModel.DataValueField = "ID";
            DropDownListModel.DataBind();
        }

        protected void ButtonSubmit_OnClick(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            sb.Append("Producer: ");
            sb.Append(DropDownListProducer.SelectedItem.Text);
            sb.Append("<br />");

            sb.Append("Model: ");
            sb.Append(DropDownListModel.SelectedItem.Text);
            sb.Append("<br />");

            sb.Append("Extras: ");
            foreach (ListItem item in CheckBoxListExtras.Items)
            {
                if (item.Selected)
                {
                    sb.Append(item.Text);
                    sb.Append("<br />");
                }
            }

            LiteralSelected.Text = sb.ToString();
        }
    }
}