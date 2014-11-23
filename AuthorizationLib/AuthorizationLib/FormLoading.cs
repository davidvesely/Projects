using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AuthorizationLib
{
    [ComVisible(false)]
    public partial class FormLoading : Form
    {
        public FormLoading()
            : this("Loading ...")
        {
        }

        public FormLoading(string LoadingText)
        {
            InitializeComponent();
            LabelText = LoadingText;
        }

        public string LabelText { get; set; }

        private void FormLoading_Load(object sender, EventArgs e)
        {
            /*progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 50;
            progressBar1.Maximum = 10;*/
            progressBar1.Visible = false;
            label1.Text = LabelText;
            label1.Left = this.ClientRectangle.Width / 2 - label1.Width / 2;
            Invalidate();
        } 
    }
}
