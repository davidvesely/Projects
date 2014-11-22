using CADBest.GeometryNamespace;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QuickStichNamespace
{
    public partial class FormMain : Form
    {
        private QuickStitch stitchModule;
        private FormStitchEngine engine;
        private Pen pointsPen = new Pen(Color.Red);
        private const int PointVisualizeRadius = 10;
        
        public FormMain()
        {
            InitializeComponent();
            engine = new FormStitchEngine();
            stitchModule = new QuickStitch();
            buttonAddLeft.Visible = false;
            buttonAddRight.Visible = false;
            ResizePictureBoxes();
        }

        private void LoadImages(string fileName1, string fileName2)
        {
            //LoadImagePicBox(picBoxFirst, fileName1);
            //LoadImagePicBox(picBoxSecond, fileName2);
            //stitchModule = new QuickStitch(picBoxFirst.Image, picBoxSecond.Image);
            //stitchModule.ProgBar = progressBar1;
        }

        private void ResizePictureBoxes()
        {
            int spaceBetweenPictures = 20;
            int margin = 13;
            int bottomSpace = 45;
            
            // Width and height
            int width = this.ClientRectangle.Width / 2 - margin - spaceBetweenPictures / 2;
            int height = this.ClientRectangle.Height - margin - bottomSpace;
            //picBoxFirst.Size = new Size(width, height);
            //picBoxSecond.Size = new Size(width, height);

            // Location of second picture box (first is not moved)
            //picBoxSecond.Left = picBoxFirst.Right + spaceBetweenPictures;
        }

        //private void WriteRegistryData()
        //{
        //    if ((stitchModule == null) ||
        //        (stitchModule.FirstPoints.Count != QuickStitch.StitchPointsCount) ||
        //        (stitchModule.SecondPoints.Count != QuickStitch.StitchPointsCount))
        //    {
        //        return;
        //    }

        //    RegistryKey rk = null;
        //    try
        //    {
        //        rk = Registry.LocalMachine.CreateSubKey("SOFTWARE\\CADBEST\\QuickStitch");
        //        List<Point3D> points = stitchModule.FirstPoints;
        //        for (int i = 0; i < points.Count; i++)
        //        {
        //            rk.SetValue(string.Format("FirstPoints{0}", i), points[i].ToString());
        //        }

        //        points = stitchModule.SecondPoints;
        //        for (int i = 0; i < points.Count; i++)
        //        {
        //            rk.SetValue(string.Format("SecondPoints{0}", i), points[i].ToString());
        //        }

        //        if (picBoxFirst.FileName != null)
        //            rk.SetValue("FirstBmp", picBoxFirst.FileName);
        //        if (picBoxSecond.FileName != null)
        //            rk.SetValue("SecondBmp", picBoxSecond.FileName);
        //    }
        //    finally
        //    {
        //        rk.Dispose();
        //    }
        //}

        //private void ReadRegistryData()
        //{
        //    RegistryKey rk = null;
        //    try
        //    {
        //        rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\CADBEST\\QuickStitch");
        //        string fileName1 = (string)rk.GetValue("FirstBmp");
        //        string fileName2 = (string)rk.GetValue("SecondBmp");

        //        if ((fileName1 == null) || (fileName2 == null))
        //            return;

        //        LoadImages(fileName1, fileName2);

        //        // Load points
        //        for (int i = 0; i < QuickStitch.StitchPointsCount; i++)
        //        {
        //            string point = (string)rk.GetValue(string.Format("FirstPoints{0}", i));
        //            stitchModule.FirstPoints.Add(new Point3D(point));
        //        }

        //        for (int i = 0; i < QuickStitch.StitchPointsCount; i++)
        //        {
        //            string point = (string)rk.GetValue(string.Format("SecondPoints{0}", i));
        //            stitchModule.SecondPoints.Add(new Point3D(point));
        //        }
        //    }
        //    finally
        //    {
        //        rk.Dispose();
        //    }
        //}

        private void PicBox_Paint(object sender, PaintEventArgs e)
        {
            if (stitchModule == null)
                return;
            ZoomPicBox picBox = sender as ZoomPicBox;
            if (picBox == null)
                return;

            List<Point3D> points;
            switch (picBox.Name)
            {
                case "picBoxFirst":
                    points = stitchModule.FirstPoints;
                    break;
                case "picBoxSecond":
                    points = stitchModule.SecondPoints;
                    break;
                default:
                    points = null;
                    break;
            }

            foreach (Point3D p in points)
            {
                // Draw circle at every clicked spot in the image
                // with center at the clicked spot, and radius set
                // by the constant
                e.Graphics.DrawEllipse(
                    pointsPen, 
                    (float)(p.X - PointVisualizeRadius / 2f),
                    (float)(p.Y - PointVisualizeRadius / 2f),
                    PointVisualizeRadius,
                    PointVisualizeRadius);
               
            }
        }

        private void PicBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (stitchModule == null)
                return;

            ZoomPicBox picBox = sender as ZoomPicBox;
            if ((picBox == null) || (picBox.Image == null))// ||  (e.Button != MouseButtons.Left))
                return;

            List<Point3D> points;
            switch (picBox.Name)
            {
                case "picBoxFirst":
                    points = stitchModule.FirstPoints;
                    break;
                case "picBoxSecond":
                    points = stitchModule.SecondPoints;
                    break;
                default:
                    points = null;
                    break;
            }

            // Left mouse button - add points
            if (e.Button == MouseButtons.Left)
            {
                if ((points != null) && (points.Count < 4))
                {
                    points.Add(new Point3D(picBox.CurrentState.OriginalX,
                        picBox.CurrentState.OriginalY, 0));
                }
            }

            // Right mouse button - remove points
            if ((e.Button == MouseButtons.Right) &&
                (points != null) && (points.Count != 0))
            {
                points.RemoveAt(points.Count - 1);                
            }

            picBox.Refresh();

        }

        private void ButtonProcess_Click(object sender, EventArgs e)
        {
            stitchModule.TransformPictures();
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            //ResizePictureBoxes();

        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //WriteRegistryData();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
//#if DEBUG
//            ReadRegistryData();
//#endif
        }

        private void buttonLoadMain_Click(object sender, EventArgs e)
        {
            engine.AddImage(panelImages, stitchModule, Side.Main);
            buttonLoadMain.Visible = false;
            buttonAddLeft.Visible = true;
            buttonAddRight.Visible = true;
        }

        private void buttonAddImage_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
            {
                return;
            }

            switch (button.Name)
            {
                case "buttonAddLeft":
                    engine.AddImage(panelImages, stitchModule, Side.Left);
                    break;
                case "buttonAddRight":
                    engine.AddImage(panelImages, stitchModule, Side.Right);
                    break;
                default:
                    break;
            }
        }

        private void buttonStitch_Click(object sender, EventArgs e)
        {
            // Turn on prepare mode for stitching: Select two images for stitch
            engine.IsStitchPrepareMode = true;
        }
    }
}
