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

namespace SixDoFMouse
{
    public partial class FormVisualizer : Form
    {
        public enum DisplayMode { Form1, Emgu, Text }
        
        private DisplayMode _workingMode;
        public DisplayMode WorkingMode
        {
            get { return _workingMode; }
        }

        private delegate void SetTextCallback(params string[] textArgs);
        private delegate void SetZoomScaleCallback(double img);

        public double ZoomScale
        {
            get { return imageBoxEmgu.ZoomScale; }
            set { SetZoomScale(value); }
        }

        private void SetZoomScale(double zoom)
        {
            if ((_workingMode == DisplayMode.Emgu) && (imageBoxEmgu.InvokeRequired))
            {
                SetZoomScaleCallback d = new SetZoomScaleCallback(SetZoomScale);
                this.Invoke(d, new object[] { zoom });
            }
            else
            {
                imageBoxEmgu.SetZoomScale(zoom, new Point());
            }
        }

        /// <summary>
        /// Safe-thread displaying text on the control
        /// </summary>
        /// <param name="textArgs">Up to 5 string parameters</param>
        public void SetText(params string[] textArgs)
        {
            if (textArgs.Length > 5)
                return;

            if (textBoxData1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { textArgs });
            }
            else
            {
                TextBox[] Boxes = new TextBox[5];
                Boxes[0] = textBoxData1;
                Boxes[1] = textBoxData2;
                Boxes[2] = textBoxData3;
                Boxes[3] = textBoxData4;
                Boxes[4] = textBoxData5;
                for (int i = 0; i < textArgs.Length; i++)
                    Boxes[i].Text = textArgs[i];
            }
        }

        public FormVisualizer(DisplayMode mode, String title, int top = 0, int left = 0)
        {
            _workingMode = mode;
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
                    imageBoxEmgu.Image = new Image<Bgr, byte>(5, 5);
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

        public void SetPicture(Image<Bgr, byte> img)
        {
            imageBoxEmgu.Image = img;
        }

        public void SetPicture(Image<Hsv, byte> img)
        {
            imageBoxEmgu.Image = img;
        }

        protected override void OnResize(EventArgs e)
        {
            ResizePicture();
        }

        void ResizePicture()
        {
            switch (_workingMode)
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
