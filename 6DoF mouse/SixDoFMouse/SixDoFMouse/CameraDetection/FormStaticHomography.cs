using CADBest.GeometryNamespace;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SixDoFMouse.CameraDetection
{
    public partial class FormStaticHomography : Form
    {
        private Image<Bgr, byte> ImageOriginal, ImgR;
        private FilterColors WorkingColor = FilterColors.R;
        private List<Point3D> CenterProjXY;
        private MouseRecognition MouseRecognitionModule;
        int FrameRows, FrameCols;

        public FormStaticHomography(string fileName, string fileNameFiltered)
        {
            InitializeComponent();
            MouseRecognitionModule = new MouseRecognition();
            SetPicture(fileName, fileNameFiltered);
            FrameRows = ImageOriginal.Rows;
            FrameCols = ImageOriginal.Cols;
            CenterProjXY = new List<Point3D>(3);
            CenterProjXY.Add(new Point3D(FrameCols / 2, FrameRows / 2, 0)); // Center of projection
            CenterProjXY.Add(new Point3D(FrameCols, FrameRows / 2, 0)); // X axis middle
            CenterProjXY.Add(new Point3D(FrameCols / 2, FrameRows, 0)); // Y axis middle
            //
            // Processing
            //
            ProcessImage();
            imageBox1.Image = ImageOriginal;
        }

        private void ProcessImage()
        {
            List<Point3D> ViewPoint = MouseRecognitionModule.Detection(
                ImgR, ImageOriginal.Data, WorkingColor, FrameRows, FrameCols, CenterProjXY);
            if (ViewPoint != null)
                MessageBox.Show("ViewPoint is found");
        }

        public void SetPicture(string fileName, string fileNameFiltered)
        {
            ImageOriginal = new Image<Bgr, byte>(fileName);
            ImgR = new Image<Bgr, byte>(fileNameFiltered);
        }

        private void FormStaticHomography_Resize(object sender, EventArgs e)
        {
            imageBox1.Width = this.ClientRectangle.Width - (imageBox1.Left * 2);
            imageBox1.Height = this.ClientRectangle.Height - (imageBox1.Top * 2);
        }
    }
}
