using System;
using System.Text;

namespace _04.StudentRegistration
{
    public partial class Default : System.Web.UI.Page
    {
        protected readonly StringBuilder _specialities = new StringBuilder();

        protected void Page_Load(object sender, EventArgs e)
        {
            int[] indices = ListBoxSpeciality.GetSelectedIndices();
            foreach (int index in indices)
            {
                _specialities.Append(ListBoxSpeciality.Items[index]);
                if (index != indices.Length - 1)
                {
                    _specialities.Append(", ");
                }
            }
        }

        protected void ButtonConfirmation_OnClick(object sender, EventArgs e)
        {
            LiteralConfirm.Text = "Confirm dialog is clicked.";
        }
    }
}