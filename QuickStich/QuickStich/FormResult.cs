using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuickStichNamespace
{
    public partial class FormResult : Form
    {
        

        public FormResult()
        {
            InitializeComponent();
        }

        public void SetImage(Bitmap image)
        {
            zoomPicBoxResult.Image = image;
        }
    }
}
