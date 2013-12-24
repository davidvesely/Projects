using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _01.Cars
{
    public partial class Default : System.Web.UI.Page
    {
        private List<Producer> CreateProducers()
        {
            var producers = new List<Producer>();

            producers.Add(new Producer()
            {
                Name = "Audi",
                Models = new List<Model>()
                {
                    new Model() { Name = "50", Engine = Engines.Petrol, Year = 1990 },
                    new Model() { Name = "60", Engine = Engines.Diesel, Year = 1999 },
                    new Model() { Name = "100", Engine = Engines.Electric, Year = 2000 },
                    new Model() { Name = "200", Engine = Engines.Hybrid, Year = 2005 },
                }
            });

            producers.Add(new Producer()
            {
                Name = "Bmw",
                Models = new List<Model>()
                {
                    new Model() { Name = "Z1", Engine = Engines.Petrol, Year = 1990 },
                    new Model() { Name = "Z2", Engine = Engines.Diesel, Year = 1999 },
                    new Model() { Name = "Z3", Engine = Engines.Electric, Year = 2000 },
                    new Model() { Name = "Z4", Engine = Engines.Hybrid, Year = 2005 },
                }
            });

            producers.Add(new Producer()
            {
                Name = "Opel",
                Models = new List<Model>()
                {
                    new Model() { Name = "Kadet", Engine = Engines.Petrol, Year = 1990 },
                    new Model() { Name = "Zafira", Engine = Engines.Diesel, Year = 1999 },
                    new Model() { Name = "Corsa", Engine = Engines.Electric, Year = 2000 },
                    new Model() { Name = "Tigra", Engine = Engines.Hybrid, Year = 2005 },
                }
            });
            return producers;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var producers = CreateProducers();
            if (!Page.IsPostBack)
            {
                DropDownListProducer.DataSource = producers;
                DropDownListProducer.DataTextField = "Name";
                DropDownListProducer.DataBind();
            }


            int ind = DropDownListProducer.SelectedIndex;
            if (ind >= 0)
            {
                DropDownListModel.DataSource = producers[ind].Models;
                DropDownListModel.DataTextField = "Name";
                DropDownListModel.DataBind();
            }
        }
    }
}