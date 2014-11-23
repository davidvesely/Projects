using Emgu.CV;
using Emgu.CV.Structure;
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
    public partial class FormVisualizer : Form
    {
        public enum DisplayMode { Form1, Emgu, Text }
        DisplayMode workingMode;

        public string Data1
        {
            get { return textBoxData1.Text; }
            set { textBoxData1.Text = value; }
        }

        public string Data2
        {
            get { return textBoxData2.Text; }
            set { textBoxData2.Text = value; }
        }

        public string Data3
        {
            get { return textBoxData3.Text; }
            set { textBoxData3.Text = value; }
        }

        public string Data4
        {
            get { return textBoxData4.Text; }
            set { textBoxData4.Text = value; }
        }

        public string Data5
        {
            get { return textBoxData5.Text; }
            set { textBoxData5.Text = value; }
        }

        public FormVisualizer(DisplayMode mode, String title, int top = 0, int left = 0)
        {
            workingMode = mode;
            InitializeComponent();
            this.Text = title;
            ResizePicture();
            switch (mode)
            {
                case DisplayMode.Form1:
                    zoomPicBox1.Visible = true;
                    imageBoxEmgu.Visible = false;
                    break;
                case DisplayMode.Emgu:
                    zoomPicBox1.Visible = false;
                    imageBoxEmgu.Visible = true;
                    break;
                case DisplayMode.Text:
                    textBoxData1.Visible = true;
                    textBoxData2.Visible = true;
                    textBoxData3.Visible = true;
                    textBoxData4.Visible = true;
                    textBoxData5.Visible = true;
                    imageBoxEmgu.Visible = false;
                    zoomPicBox1.Visible = false;
                    this.Width = 250;
                    this.Height = 200;
                    break;
                default:
                    break;
            }
            if ((top != 0) && (left != 0))
            {
                this.Top = top;
                this.Left = left;
            }
        }

        public void SetPicture(Bitmap img)
        {
            zoomPicBox1.Image = img;
            ResizePicture();
        }

        public void SetPicture(Image<Bgr, Byte> img)
        {
            imageBoxEmgu.Image = img;
        }

        protected override void OnResize(EventArgs e)
        {
            ResizePicture();
        }

        void ResizePicture()
        {
            switch (workingMode)
            {
                case DisplayMode.Form1:
                    zoomPicBox1.Width = this.ClientRectangle.Width - 26;
                    zoomPicBox1.Height = this.ClientRectangle.Height - 26;
                    zoomPicBox1.Top = 13;
                    zoomPicBox1.Left = 13;
                    break;
                case DisplayMode.Emgu:
                    imageBoxEmgu.Width = this.ClientRectangle.Width - 26;
                    imageBoxEmgu.Height = this.ClientRectangle.Height - 26;
                    imageBoxEmgu.Top = 13;
                    imageBoxEmgu.Left = 13;
                    break;
                default:
                    break;
            }
        }
    }
}
