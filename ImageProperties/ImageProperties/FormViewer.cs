using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProperties
{
    public partial class FormViewer : Form
    {
        void LoadPicture(String defaultFile = "")
        {
            string strFileName;
            if (defaultFile == "")
            {
                strFileName = ImageProcessing.OpenImageFile();
            }
            else
            {
                strFileName = defaultFile;
            }
            zoomPicBox1.LoadImage(strFileName);
            labelZoom.Text = zoomPicBox1.GetCurrentZoom().ToString();
        }

        void RearrangeControls() // Resizes main picture box and moves buttons to correct locations
        {
            Magnifier.Left = this.ClientRectangle.Width - Magnifier.Width - 10;
            groupBox1.Left = this.ClientRectangle.Width - groupBox1.Width - 10;

            zoomPicBox1.Top = menuStrip1.Height + 5;
            zoomPicBox1.Left = 7;
            zoomPicBox1.Width = this.ClientRectangle.Width - groupBox1.Width - 33;
            zoomPicBox1.Height = this.ClientRectangle.Height - 40;
        }

        public FormViewer()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);

            zoomPicBox1.MagnifierCopy = Magnifier; // Connection between ZoomPicBox and Magnifier objects

            RearrangeControls();

#if DEBUG
            try
            {
                LoadPicture("C:\\Users\\Public\\Pictures\\Sample Pictures\\Hydrangeas.jpg");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("The default file doesn't exists!");
            }
#endif
        }

        void Form1_SizeChanged(object sender, EventArgs e)
        {
            RearrangeControls();
        }

        void zoomPicBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Magnifier.MagnifierZoomImage(e.Location);
            labelX.Text = zoomPicBox1.CurrentState.GetX();
            labelY.Text = zoomPicBox1.CurrentState.GetY();
            labelR.Text = zoomPicBox1.CurrentState.GetR();
            labelG.Text = zoomPicBox1.CurrentState.GetG();
            labelB.Text = zoomPicBox1.CurrentState.GetB();
            labelZoom.Text = zoomPicBox1.GetCurrentZoom().ToString();
            groupBox1.Refresh();
        }

        void zoomPicBox1_MouseLeave(object sender, EventArgs e)
        {
            labelX.Text = "None";
            labelY.Text = "None";
            labelR.Text = "None";
            labelG.Text = "None";
            labelB.Text = "None";
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadPicture();
        }

        private void sobelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(zoomPicBox1.Image.Width, zoomPicBox1.Image.Height);
            if (zoomPicBox1.Image != null)
            {
                ImageProcessing.SobelPointers(ImageProcessing.Grayscale(zoomPicBox1.Image), bmp, zoomPicBox1.Image.Width, zoomPicBox1.Image.Height);
                FormVisualizer viewResult = new FormVisualizer(FormVisualizer.DisplayMode.Form1, "");
                viewResult.SetPicture(bmp);
                viewResult.Show();
            }
        }

        private void cornerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp;
            if (zoomPicBox1.Image != null)
            {
                long milliseconds1 = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                bmp = ImageProcessing.FilterImage(zoomPicBox1.Image, Filters.Matrix3x3.Corner);
                long milliseconds2 = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                MessageBox.Show((milliseconds2 - milliseconds1).ToString());
                FormVisualizer viewResult = new FormVisualizer(FormVisualizer.DisplayMode.Form1, "");
                viewResult.SetPicture(bmp);
                viewResult.Show();
            }
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp;
            if (zoomPicBox1.Image != null)
            {
                bmp = ImageProcessing.Grayscale(zoomPicBox1.Image);
                FormVisualizer form2 = new FormVisualizer(FormVisualizer.DisplayMode.Form1, "");
                form2.SetPicture(bmp);
                form2.Show();
            }
        }

        private void inverColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp;
            if (zoomPicBox1.Image != null)
            {
                bmp = ImageProcessing.InvertColor(zoomPicBox1.Image);
                FormVisualizer form2 = new FormVisualizer(FormVisualizer.DisplayMode.Form1, "");
                form2.SetPicture(bmp);
                form2.Show();
            }
        }

        private void middleColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp;
            if (zoomPicBox1.Image != null)
            {
                bmp = ImageProcessing.MiddleColor(zoomPicBox1.Image);
                FormVisualizer form2 = new FormVisualizer(FormVisualizer.DisplayMode.Form1, "");
                form2.SetPicture(bmp);
                form2.Show();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void testBWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (zoomPicBox1.Image != null)
            {
                //Form3 form3 = new Form3();
                //form3.Test3Images(zoomPicBox1.Image);
                //form3.Show();
            }
        }

        private void webCamTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormWebCameraGDI formWebCam = new FormWebCameraGDI();
            formWebCam.Show();
        }

        private void zoomPicBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxBorder.Text))
            {
                int x = zoomPicBox1.CurrentState.OriginalX;
                int y = zoomPicBox1.CurrentState.OriginalY;
                int borderPixels;

                if (!int.TryParse(textBoxBorder.Text, out borderPixels))
                    return;

                Color c = ImageProcessing.GetColor(zoomPicBox1.Image, new Point(x, y), borderPixels);
                labelRPick.Text = c.R.ToString();
                labelGPick.Text = c.G.ToString();
                labelBPick.Text = c.B.ToString();
            }
        }
    }
}
