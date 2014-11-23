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
    public partial class FormStaticFrame : Form
    {
        private Image<Bgr, byte> imageCopy;
        private Color BaseColor = Color.Empty;

        private MouseRecognition MouseRecognitionModule;
        private List<Point3D> SinCosW;

        private FormVisualizer vizR, vizG, vizB, vizY;

        public FormStaticFrame()
        {
            InitializeComponent();
            MouseRecognitionModule = new MouseRecognition();
        }

        private void VisualizeResults(Image<Bgr, byte> imgR, 
            Image<Bgr, byte> imgG, Image<Bgr, byte> imgB, Image<Bgr, byte> imgY)
        {
            if ((vizR == null) || (vizG == null) ||
                (vizB == null) || (vizY == null))
            {
                SetVisualizerPositions();
            }

            vizR.SetPicture(imgR);
            vizG.SetPicture(imgG);
            vizB.SetPicture(imgB);
            vizY.SetPicture(imgY);
        }

        private void SetVisualizerPositions()
        {
            vizR = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Red");
            vizG = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Green");
            vizB = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Blue");
            vizY = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Yellow");

            RegistryKey rk = null;
            try
            {
                rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\" + this.ProductName);
                if ((rk != null) && ((string)rk.GetValue("R_K", "-1") != "-1"))
                {
                    FormWebCamEmgu.ReadWindowState(rk, vizR, "Red");
                    FormWebCamEmgu.ReadWindowState(rk, vizG, "Green");
                    FormWebCamEmgu.ReadWindowState(rk, vizB, "Blue");
                    FormWebCamEmgu.ReadWindowState(rk, vizY, "Yellow");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Problem occurred while reading registry saved properties");
            }
            finally
            {
                if (rk != null)
                    rk.Dispose();
            }

            vizR.Show();
            vizG.Show();
            vizB.Show();
            vizY.Show();
        }

        public void SetPicture(string fileName)
        {
            zoomPicBox1.LoadImage(fileName);
            imageCopy = new Image<Bgr, byte>(fileName);
        }

        private void buttonCalcWhite_Click(object sender, EventArgs e)
        {
            labelWhiteT.Text = textBoxTColor.Text;
            Vector3D whiteTvect = ConvertPoint3D(textBoxTColor.Text);
            whiteTvect.Normalize();
            textBoxTColor.Text = whiteTvect.ToString();
            MouseRecognitionModule.WhiteTVector = whiteTvect;

            Vector3D whiteVect;
            if (checkBoxGetColorFromImg.Checked)
            {
                whiteVect = ImageProcessing.GetWhiteColor(imageCopy.Data, imageCopy.Rows, imageCopy.Cols);
                labelWhite.Text = whiteVect.ToString();
            }
            else
            {
                whiteVect = ConvertPoint3D(textBoxWhite.Text);
                if (whiteVect == null)
                    return;
            }

            whiteVect.Normalize();
            textBoxWhite.Text = whiteVect.ToString();
            MouseRecognitionModule.WhiteVector = whiteVect;

            SinCosW = MouseRecognitionModule.RotateColorVectors();
            MouseRecognitionModule.RotateSingleVector(whiteVect, SinCosW);
            textBoxWhiteNew.Text = MouseRecognitionModule.WhiteVector.ToString();
            double degX = Geometry.ConvertToDeg(Geometry.CalculateAngle(SinCosW[0]));
            double degY = Geometry.ConvertToDeg(Geometry.CalculateAngle(SinCosW[1]));
            double degZ = Geometry.ConvertToDeg(Geometry.CalculateAngle(SinCosW[2]));
            labelXSinCos.Text = SinCosW[0].ToString() + ", Deg: " + Math.Round(degX, 5).ToString();
            labelYSinCos.Text = SinCosW[1].ToString() + ", Deg: " + Math.Round(degY, 5).ToString();
            labelZSinCos.Text = SinCosW[2].ToString() + ", Deg: " + Math.Round(degZ, 5).ToString();
        }

        private Vector3D ConvertPoint3D(string input)
        {
            string colorStr = input;
            colorStr = colorStr.Replace(" ", string.Empty);
            string[] colorSplit = colorStr.Split(',');
            if (colorSplit.Length != 3)
            {
                MessageBox.Show("Please provide correct values for this color!");
                return null;
            }

            double[] colorArr = new double[3];
            for (int i = 0; i < colorSplit.Length; i++)
            {
                if (!double.TryParse(colorSplit[i], out colorArr[i]))
                {
                    MessageBox.Show("Please provide correct values for this color!");
                    return null;
                }
            }

            return new Vector3D(colorArr);
        }

        private void buttonCalcColors_Click(object sender, EventArgs e)
        {
            if (SinCosW == null)
                return;

            MouseRecognitionModule.RedVector = CalcColorVector(textBoxRedOld, textBoxRedNew);
            MouseRecognitionModule.GreenVector = CalcColorVector(textBoxGreenOld, textBoxGreenNew);
            MouseRecognitionModule.BlueVector = CalcColorVector(textBoxBlueOld, textBoxBlueNew);
            MouseRecognitionModule.YellowVector = CalcColorVector(textBoxYellowOld, textBoxYellowNew);

            textBoxRDist.BackColor = textBoxRedOld.BackColor;
            textBoxGDist.BackColor = textBoxGreenOld.BackColor;
            textBoxBDist.BackColor = textBoxBlueOld.BackColor;
            textBoxYDist.BackColor = textBoxYellowOld.BackColor;

            numericUpDownR.BackColor = textBoxRedOld.BackColor;
            numericUpDownG.BackColor = textBoxGreenOld.BackColor;
            numericUpDownB.BackColor = textBoxBlueOld.BackColor;
            numericUpDownY.BackColor = textBoxYellowOld.BackColor;

            textBoxRDist.ForeColor = Color.White;
            textBoxGDist.ForeColor = Color.White;
            textBoxBDist.ForeColor = Color.White;
            textBoxYDist.ForeColor = Color.White;
        }

        private Vector3D CalcColorVector(TextBox oldColorBox, TextBox newColorBox)
        {
            Vector3D colorVector = ConvertPoint3D(oldColorBox.Text);
            if (colorVector == null)
                throw new GeometryException("Test program error");

            colorVector.Normalize();
            byte R = (byte)(colorVector.X * 255.0);
            byte G = (byte)(colorVector.Y * 255.0);
            byte B = (byte)(colorVector.Z * 255.0);
            oldColorBox.Text = colorVector.ToString();
            oldColorBox.BackColor = Color.FromArgb(R, G, B);

            MouseRecognitionModule.RotateSingleVector(colorVector, SinCosW);
            newColorBox.Text = colorVector.ToString();
            R = (byte)(colorVector.X * 255.0);
            G = (byte)(colorVector.Y * 255.0);
            B = (byte)(colorVector.Z * 255.0);
            newColorBox.BackColor = Color.FromArgb(R, G, B);

            oldColorBox.ForeColor = newColorBox.ForeColor = Color.White;
            return colorVector;
        }

        private void zoomPicBox1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = zoomPicBox1.CurrentState.OriginalX;
            int y = zoomPicBox1.CurrentState.OriginalY;
            WeightCenter coordinates = new WeightCenter(x, y, 0);
            Color pixelColor = ImageProcessing.GetColor(imageCopy.Data,
                coordinates, (int)numericUpDownBorderPixels.Value);
            labelPixelColor.Text = string.Format("{0} {1} {2}", pixelColor.R, pixelColor.G, pixelColor.B);
            textBoxPixelColor.BackColor = pixelColor;
            Vector3D pixelVector = new Vector3D(pixelColor);
            pixelVector.Normalize();
            textBoxPixelColor.Text = pixelVector.ToString();

            if (MouseRecognitionModule.IsInitialized)
            {
                Vector3D redVector, greenVector, blueVector, yellowVector;
                redVector = MouseRecognitionModule.RedVector;
                greenVector = MouseRecognitionModule.GreenVector;
                blueVector = MouseRecognitionModule.BlueVector;
                yellowVector = MouseRecognitionModule.YellowVector;
                Vector3D currentVector = new Vector3D(pixelColor);
                currentVector.Normalize();

                // Distances
                double distance = Geometry.Distance(currentVector, redVector);
                textBoxRDist.Text = distance.ToString();
                distance = Geometry.Distance(currentVector, greenVector);
                textBoxGDist.Text = distance.ToString();
                distance = Geometry.Distance(currentVector, blueVector);
                textBoxBDist.Text = distance.ToString();
                distance = Geometry.Distance(currentVector, yellowVector);
                textBoxYDist.Text = distance.ToString();
            }
        }

        private void FormStaticFrame_Resize(object sender, EventArgs e)
        {
            zoomPicBox1.Width = groupBoxWhite.Left - (zoomPicBox1.Left * 2);
            zoomPicBox1.Height = this.ClientRectangle.Height - (zoomPicBox1.Top * 2);
        }

        private void buttonVisualize_Click(object sender, EventArgs e)
        {
            //if (MouseRecognitionModule.RedVector == null)
            //    return;

            //int width, height;
            //width = imageCopy.Width;
            //height = imageCopy.Height;
            //Image<Bgr, byte> imgR, imgG, imgB, imgY;
            //imgR = new Image<Bgr, byte>(width, height);
            //imgG = new Image<Bgr, byte>(width, height);
            //imgB = new Image<Bgr, byte>(width, height);
            //imgY = new Image<Bgr, byte>(width, height);
            //byte[,,] dataOriginal = imageCopy.Data;
            //byte[, ,] dataR = imgR.Data;
            //byte[, ,] dataG = imgG.Data;
            //byte[, ,] dataB = imgB.Data;
            //byte[, ,] dataY = imgY.Data;

            //double checkR = (double)numericUpDownR.Value;
            //double checkG = (double)numericUpDownG.Value;
            //double checkB = (double)numericUpDownB.Value;
            //double checkY = (double)numericUpDownY.Value;

            //Vector3D currentPixel = new Vector3D();
            //for (int i = 0; i < height; i++) // Rows
            //{
            //    for (int j = 0; j < width; j++) // Cols
            //    {
            //        byte R = dataOriginal[i, j, 2];
            //        byte G = dataOriginal[i, j, 1];
            //        byte B = dataOriginal[i, j, 0];
            //        currentPixel.SetCoordinates(R, G, B);
            //        currentPixel.Normalize();
            //        FormWebCamEmgu.CheckDistance(currentPixel, MouseRecognitionModule.RedVector,
            //            dataR, FilterColors.R, i, j, checkR);
            //        FormWebCamEmgu.CheckDistance(currentPixel, MouseRecognitionModule.GreenVector,
            //            dataG, FilterColors.G, i, j, checkG);
            //        FormWebCamEmgu.CheckDistance(currentPixel, MouseRecognitionModule.BlueVector,
            //            dataB, FilterColors.B, i, j, checkB);
            //        FormWebCamEmgu.CheckDistance(currentPixel, MouseRecognitionModule.YellowVector,
            //            dataY, FilterColors.Y, i, j, checkY);
            //    }
            //}

            //VisualizeResults(imgR, imgG, imgB, imgY);
        }

        private void FormStaticFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (vizR != null)
                vizR.Close();
            if (vizG != null)
                vizG.Close();
            if (vizB != null)
                vizB.Close();
            if (vizY != null)
                vizY.Close();
        }

        private void buttonNormalize_Click(object sender, EventArgs e)
        {
            Image<Bgr, byte> image = imageCopy.Convert<Bgr, byte>();
            ImageProcessing imgProcessModule = new ImageProcessing();
            imgProcessModule.ImageProcess(image.Data, image.Rows, image.Cols, (int)numericDownPrag.Value, (int)numericUpPrag.Value);
            FormVisualizer imageViz = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Test");
            imageViz.SetPicture(image);
            imageViz.Show();
        }
    }
}
