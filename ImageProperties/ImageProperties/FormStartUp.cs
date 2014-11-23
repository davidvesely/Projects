using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProperties
{
    public partial class FormStartUp : Form
    {
        public FormStartUp()
        {
            InitializeComponent();
        }

        private void buttonViewer_Click(object sender, EventArgs e)
        {
            FormViewer form = new FormViewer();
            form.Show();
        }

        private void buttonWebCam_Click(object sender, EventArgs e)
        {
            FormWebCameraGDI form = new FormWebCameraGDI();
            form.Show();
        }

        private void buttonOpenCV_Click(object sender, EventArgs e)
        {
            FormWebCamEmgu form = new FormWebCamEmgu();
            form.Show();
        }

        private void BarrelBut_Click(object sender, EventArgs e)
        {
            BarrelForm form = new BarrelForm();
            form.Show();
        }
    }
}
