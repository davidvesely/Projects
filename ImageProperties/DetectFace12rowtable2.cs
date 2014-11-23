using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CADBest.Geometry;

namespace ImageProperties
{
    public partial class FormWebCamEmgu : Form
    {
        private Capture _capture = null;
        private bool _captureInProgress;
        private FastDetector fastDetect = null;

        private FormVisualizer vizRed, vizGreen, vizBlue, vizFilters, vizOriginal, vizYellow;

        private Image<Bgr, Byte> _frame;
        private Image<Bgr, Byte> imgFiltered;

        double IM1_R = 1, IM1_G = 1, IM1_B = 1;
        double IM2_G = 1, IM2_R = 1, IM2_B = 1;
        double IM3_B = 1, IM3_R = 1, IM3_G = 1;
        double IM_Y_K = 1;
        int IM_R_PRAG = 512, IM_R_ADD = 512;
        int IM_G_PRAG = 512, IM_G_ADD = 512;
        int IM_B_PRAG = 512, IM_B_ADD = 512;
        int IM_Y_PRAG = 512, IM_Y_ADD = 512;

        Image<Bgr, Byte> imgB;
        Image<Bgr, Byte> imgG;
        Image<Bgr, Byte> imgR;
        Image<Bgr, Byte> imgY;
        Image<Bgr, Byte> imgAll;

        int FrameRows, FrameCols; // Rows and cols of current resolution
        //Image<Bgr, Byte> imgRblur;
        //Image<Gray, Byte> imgRcorner;

        //private VideoWriter vw = new VideoWriter("test.avi", 30, 800, 600, true);

        public FormWebCamEmgu()
        {
            InitializeComponent();
            vizRed = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Red");
            vizGreen = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Green");
            vizBlue = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Blue");
            vizFilters = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Filtered");
            vizOriginal = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Original");
            vizYellow = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Yellow");
            ReadRegistryValues();
        }

        private void Initialize()
        {
            int width, height;
            width = Convert.ToInt32(textBoxWidth.Text);
            height = Convert.ToInt32(textBoxHeight.Text);
            FrameRows = height;
            FrameCols = width;
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, width);
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, height);
            imgB = new Image<Bgr, Byte>(width, height);
            imgG = new Image<Bgr, Byte>(width, height);
            imgR = new Image<Bgr, Byte>(width, height);
            imgY = new Image<Bgr, Byte>(width, height);
            imgAll = new Image<Bgr, Byte>(width, height);
            
            //imgRblur = new Image<Bgr, Byte>(width, height);
            //imgRcorner = new Image<Gray, Byte>(width, height);
            
            //if (textBoxTreshold.Text != "")
            //    fastDetect = new FastDetector(Convert.ToInt32(textBoxTreshold.Text), true);
            //else
            //    fastDetect = new FastDetector(20, true);
        }

        private void FASTedges(Image<Bgr, byte> source, Bgr color)
        {
            MKeyPoint[] keyPoints = fastDetect.DetectKeyPoints(source.Convert<Gray, byte>(), null);
            foreach (MKeyPoint keypt in keyPoints)
            {
                PointF pt = keypt.Point;
                source.Draw(new CircleF(pt, 2), color, 2);
            }
        }

        private void SaveScreenshots()
        {
            imgR.Save("WebcamRed.bmp");
            imgG.Save("WebcamGreen.bmp");
            imgB.Save("WebcamBlue.bmp");
            imgY.Save("WebcamYellow.bmp");
            imgAll.Save("WebcamAll.bmp");
            _frame.Save("WebcamOriginal.bmp");
            MessageBox.Show("Screenshots are taken successfully!");
        }

        private void GetCoefficients()
        {
            IM1_R = trbR_K.Value;
            IM_R_ADD = (int)trbR_Add.Value;
            IM_R_PRAG = (int)trbR_Prag.Value;

            IM2_G = trbG_K.Value;
            IM_G_ADD = (int)trbG_Add.Value;
            IM_G_PRAG = (int)trbG_Prag.Value;

            IM3_B = trbB_K.Value;
            IM_B_ADD = (int)trbB_Add.Value;
            IM_B_PRAG = (int)trbB_Prag.Value;

            IM_Y_K = trbY_K.Value;
            IM_Y_ADD = (int)trbY_Add.Value;
            IM_Y_PRAG = (int)trbY_Prag.Value;
        }

        private void RGBFilter()
        {
            double im;
            byte min;
            byte red, new_red;
            byte green, new_green;
            byte blue, new_blue;

            byte[,,] data1 = imgR.Data;
            byte[,,] data2 = imgG.Data;
            byte[,,] data3 = imgB.Data;
            byte[,,] dataYellow = imgY.Data;
            byte[, ,] dataOriginal = imgFiltered.Data;
            byte[,,] dataAll = imgAll.Data;

            for (int i = imgFiltered.Rows - 1; i >= 0; i--)
            {
                for (int j = imgFiltered.Cols - 1; j >= 0; j--)
                {
                    red = dataOriginal[i, j, 2]; //Read to the Red Spectrum
                    green = dataOriginal[i, j, 1]; //Read to the Green Spectrum
                    blue = dataOriginal[i, j, 0]; //Read to the BlueSpectrum

                    min = red;
                    if (min > green)
                        min = green;
                    if (min > blue)
                        min = blue;
                    red = (byte)(red - min);
                    green = (byte)(green - min);
                    blue = (byte)(blue - min);

                    dataOriginal[i, j, 2] = red; //Read to the Red Spectrum
                    dataOriginal[i, j, 1] = green; //Read to the Green Spectrum
                    dataOriginal[i, j, 0] = blue; //Read to the BlueSpectrum

                    if (checkBox1.Checked)
                    {
                        im = red * IM1_R - green * IM1_G - blue * IM1_B + IM_R_ADD;
                        if (im >= IM_R_PRAG)
                            new_red = 255;
                        else
                            new_red = 0;
                    }
                    else
                        new_red = red;
                    data1[i, j, 2] = new_red;
                    data1[i, j, 1] = 0;
                    data1[i, j, 0] = 0;

                    if (checkBox1.Checked)
                    {
                        im = green * IM2_G - red * IM2_R - blue * IM2_B + IM_G_ADD;
                        if (im >= IM_G_PRAG)
                            new_green = 255;
                        else
                            new_green = 0;
                    }
                    else
                        new_green = green;
                    data2[i, j, 2] = 0;
                    data2[i, j, 1] = new_green;
                    data2[i, j, 0] = 0;

                    if (checkBox1.Checked)
                    {
                        im = blue * IM3_B - green * IM3_G - red * IM3_R + IM_B_ADD;
                        if (im >= IM_B_PRAG)
                            new_blue = 255;
                        else
                            new_blue = 0;
                    }
                    else
                        new_blue = blue;
                    data3[i, j, 2] = 0;
                    data3[i, j, 1] = 0;
                    data3[i, j, 0] = new_blue;

                    dataAll[i, j, 2] = new_red;
                    dataAll[i, j, 1] = new_green;
                    dataAll[i, j, 0] = new_blue;

                    //Yellow
                    if (checkBox1.Checked)
                    {
                        //im = blue * IM3_B - green * IM3_G - red * IM3_R + IM_B_ADD;
                        im = IM_Y_K * (0.8 * red + green) - 2*blue + IM_Y_ADD;
                        if (im >= IM_Y_PRAG)
                        {
                            new_red = 255;
                            new_green = 255;
                            dataAll[i, j, 2] = new_red;
                            dataAll[i, j, 1] = new_green;
                        }
                        else
                        {
                            new_red = 0;
                            new_green = 0;
                        }
                    }
                    else
                    {
                        new_blue = blue;
                        new_green = green;
                    }
                    dataYellow[i, j, 2] = new_red;
                    dataYellow[i, j, 1] = new_green;
                    dataYellow[i, j, 0] = 0;
                }
            }
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            _frame = _capture.RetrieveBgrFrame();//.Flip(Emgu.CV.CvEnum.FLIP.HORIZONTAL);
            Image<Bgr, byte> imgOriginal = _frame.Convert<Bgr, Byte>();
            imgFiltered = _frame.Convert<Bgr, Byte>();

            GetCoefficients();
            RGBFilter();
            FindSpots(imgR.Data, imgOriginal.Data, imgFiltered.Rows, imgFiltered.Cols, FilterColors.Red);
            FindSpots(imgG.Data, imgOriginal.Data, imgFiltered.Rows, imgFiltered.Cols, FilterColors.Green);
            FindSpots(imgB.Data, imgOriginal.Data, imgFiltered.Rows, imgFiltered.Cols, FilterColors.Blue);
            FindSpots(imgY.Data, imgOriginal.Data, imgFiltered.Rows, imgFiltered.Cols, FilterColors.Red);

            vizRed.SetPicture(imgR);
            vizGreen.SetPicture(imgG);
            vizBlue.SetPicture(imgB);
            vizYellow.SetPicture(imgY);
            vizFilters.SetPicture(imgFiltered);
            vizOriginal.SetPicture(imgOriginal);
        }

        #region WebCam and UI
        private void FormWebCamEmgu_Load(object sender, EventArgs e)
        {
            //Read coefficients from registry
            //ReadRegistryValues();
        }

        private void buttonScreenshots_Click(object sender, EventArgs e)
        {
            SaveScreenshots();
        }

        private void buttonLoadFrame_Click(object sender, EventArgs e)
        {
            LoadFrame();
        }

        private void buttonCapture_Click(object sender, EventArgs e)
        {
            if (_captureInProgress)
            {  //stop the capture
                buttonCapture.Text = "Start Capture";
                _capture.Pause();
                _capture.ImageGrabbed -= ProcessFrame;
            }
            else
            {
                //start the capture
                buttonCapture.Text = "Stop";
                try
                {
                    int choice;
                    if (radioButton1.Checked)
                        choice = 0;
                    else
                        choice = 1;
                    _capture = new Capture(choice);
                    _capture.ImageGrabbed += ProcessFrame;
                }
                catch (NullReferenceException excpt)
                {
                    MessageBox.Show(excpt.Message);
                }
                Initialize();
                _capture.Start();
            }

            _captureInProgress = !_captureInProgress;

            vizRed.Show();
            vizGreen.Show();
            vizBlue.Show();
            vizFilters.Show();
            vizOriginal.Show();
            vizYellow.Show();
        }

        private void ReleaseData()
        {
            if (_capture != null)
                _capture.Dispose();
        }

        private void FormWebCamEmgu_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Writing coefficients to memory
            WriteRegistryValues();

            ReleaseData();
            Application.Exit();
        }

        private void ReadRegistryValues()
        {
            RegistryKey rk = null;
            try
            {
                rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\" + Application.ProductName);
                if ((rk != null) && ((string)rk.GetValue("R_K", "-1") != "-1"))
                {
                    trbR_K.Value = Convert.ToDouble((string)rk.GetValue("R_K"));
                    trbG_K.Value = Convert.ToDouble((string)rk.GetValue("G_K"));
                    trbB_K.Value = Convert.ToDouble((string)rk.GetValue("B_K"));
                    trbY_K.Value = Convert.ToDouble((string)rk.GetValue("Y_K"));

                    trbR_Add.Value = Convert.ToDouble((string)rk.GetValue("R_Add"));
                    trbG_Add.Value = Convert.ToDouble((string)rk.GetValue("G_Add"));
                    trbB_Add.Value = Convert.ToDouble((string)rk.GetValue("B_Add"));
                    trbY_Add.Value = Convert.ToDouble((string)rk.GetValue("Y_Add"));

                    trbR_Prag.Value = Convert.ToDouble((string)rk.GetValue("R_Prag"));
                    trbG_Prag.Value = Convert.ToDouble((string)rk.GetValue("G_Prag"));
                    trbB_Prag.Value = Convert.ToDouble((string)rk.GetValue("B_Prag"));
                    trbY_Prag.Value = Convert.ToDouble((string)rk.GetValue("Y_Prag"));

                    textBoxWidth.Text = (string)rk.GetValue("CaptureWidth");
                    textBoxHeight.Text = (string)rk.GetValue("CaptureHeight");

                    //Main Window
                    this.Top = Convert.ToInt32((string)rk.GetValue("MainTop"));
                    this.Left = Convert.ToInt32((string)rk.GetValue("MainLeft"));
                    this.Width = Convert.ToInt32((string)rk.GetValue("MainWidth"));
                    this.Height = Convert.ToInt32((string)rk.GetValue("MainHeight"));
                    //Red Window
                    vizRed.Top = Convert.ToInt32((string)rk.GetValue("RedTop"));
                    vizRed.Left = Convert.ToInt32((string)rk.GetValue("RedLeft"));
                    vizRed.Width = Convert.ToInt32((string)rk.GetValue("RedWidth"));
                    vizRed.Height = Convert.ToInt32((string)rk.GetValue("RedHeight"));
                    //Green Window
                    vizGreen.Top = Convert.ToInt32((string)rk.GetValue("GreenTop"));
                    vizGreen.Left = Convert.ToInt32((string)rk.GetValue("GreenLeft"));
                    vizGreen.Width = Convert.ToInt32((string)rk.GetValue("GreenWidth"));
                    vizGreen.Height = Convert.ToInt32((string)rk.GetValue("GreenHeight"));
                    //Blue Window
                    vizBlue.Top = Convert.ToInt32((string)rk.GetValue("BlueTop"));
                    vizBlue.Left = Convert.ToInt32((string)rk.GetValue("BlueLeft"));
                    vizBlue.Width = Convert.ToInt32((string)rk.GetValue("BlueWidth"));
                    vizBlue.Height = Convert.ToInt32((string)rk.GetValue("BlueHeight"));
                    //Filters Window
                    vizFilters.Top = Convert.ToInt32((string)rk.GetValue("FiltersTop"));
                    vizFilters.Left = Convert.ToInt32((string)rk.GetValue("FiltersLeft"));
                    vizFilters.Width = Convert.ToInt32((string)rk.GetValue("FiltersWidth"));
                    vizFilters.Height = Convert.ToInt32((string)rk.GetValue("FiltersHeight"));
                    //Original Window
                    vizOriginal.Top = Convert.ToInt32((string)rk.GetValue("OriginalTop"));
                    vizOriginal.Left = Convert.ToInt32((string)rk.GetValue("OriginalLeft"));
                    vizOriginal.Width = Convert.ToInt32((string)rk.GetValue("OriginalWidth"));
                    vizOriginal.Height = Convert.ToInt32((string)rk.GetValue("OriginalHeight"));
                    //Yellow Window
                    vizYellow.Top = Convert.ToInt32((string)rk.GetValue("YellowTop"));
                    vizYellow.Left = Convert.ToInt32((string)rk.GetValue("YellowLeft"));
                    vizYellow.Width = Convert.ToInt32((string)rk.GetValue("YellowWidth"));
                    vizYellow.Height = Convert.ToInt32((string)rk.GetValue("YellowHeight"));
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                rk.Dispose();
            }
        }

        private void WriteRegistryValues()
        {
            RegistryKey rk = null;
            try
            {
                rk = Registry.LocalMachine.CreateSubKey("SOFTWARE\\" + Application.ProductName);
                rk.SetValue("R_K", trbR_K.Value);
                rk.SetValue("G_K", trbG_K.Value);
                rk.SetValue("B_K", trbB_K.Value);
                rk.SetValue("Y_K", trbY_K.Value);

                rk.SetValue("R_Add", trbR_Add.Value);
                rk.SetValue("G_Add", trbG_Add.Value);
                rk.SetValue("B_Add", trbB_Add.Value);
                rk.SetValue("Y_Add", trbY_Add.Value);

                rk.SetValue("R_Prag", trbR_Prag.Value);
                rk.SetValue("G_Prag", trbG_Prag.Value);
                rk.SetValue("B_Prag", trbB_Prag.Value);
                rk.SetValue("Y_Prag", trbY_Prag.Value);

                rk.SetValue("CaptureWidth", textBoxWidth.Text);
                rk.SetValue("CaptureHeight", textBoxHeight.Text);

                //Main Window
                rk.SetValue("MainTop", this.Top.ToString());
                rk.SetValue("MainLeft", this.Left.ToString());
                rk.SetValue("MainWidth", this.Width.ToString());
                rk.SetValue("MainHeight", this.Height.ToString());
                //Red Window
                rk.SetValue("RedTop", vizRed.Top.ToString());
                rk.SetValue("RedLeft", vizRed.Left.ToString());
                rk.SetValue("RedWidth", vizRed.Width.ToString());
                rk.SetValue("RedHeight", vizRed.Height.ToString());
                //Green Window
                rk.SetValue("GreenTop", vizGreen.Top.ToString());
                rk.SetValue("GreenLeft", vizGreen.Left.ToString());
                rk.SetValue("GreenWidth", vizGreen.Width.ToString());
                rk.SetValue("GreenHeight", vizGreen.Height.ToString());
                //Blue Window
                rk.SetValue("BlueTop", vizBlue.Top.ToString());
                rk.SetValue("BlueLeft", vizBlue.Left.ToString());
                rk.SetValue("BlueWidth", vizBlue.Width.ToString());
                rk.SetValue("BlueHeight", vizBlue.Height.ToString());
                //Filters Window
                rk.SetValue("FiltersTop", vizFilters.Top.ToString());
                rk.SetValue("FiltersLeft", vizFilters.Left.ToString());
                rk.SetValue("FiltersWidth", vizFilters.Width.ToString());
                rk.SetValue("FiltersHeight", vizFilters.Height.ToString());
                //Original Window
                rk.SetValue("OriginalTop", vizOriginal.Top.ToString());
                rk.SetValue("OriginalLeft", vizOriginal.Left.ToString());
                rk.SetValue("OriginalWidth", vizOriginal.Width.ToString());
                rk.SetValue("OriginalHeight", vizOriginal.Height.ToString());
                //Yellow Window
                rk.SetValue("YellowTop", vizYellow.Top.ToString());
                rk.SetValue("YellowLeft", vizYellow.Left.ToString());
                rk.SetValue("YellowWidth", vizYellow.Width.ToString());
                rk.SetValue("YellowHeight", vizYellow.Height.ToString());
            }
            catch (Exception)
            {
            }
            finally
            {
                rk.Dispose();
            }
        }
        #endregion

        #region Detection of spots
        struct Strip
        {
            public int start;
            public int end;
            public Strip(int aStart, int aEnd)
            {
                start = aStart;
                end = aEnd;
            }
        }

        struct WeightCenter
        {
            public int x, y;
            public int rowStart;
            public WeightCenter(int aX, int aY, int aRowStart)
            {
                x = aX;
                y = aY;
                rowStart = aRowStart;
            }

            public static readonly WeightCenter Empty = new WeightCenter();
        }

        enum FilterColors { Blue = 0, Green = 1, Red = 2, Yellow = 3 };

        private List<Strip> FindStrips(byte[,,] source, int strip_direction, int elements, FilterColors color, bool isRows) //Check columns or rows for strips with pixels
        {
            List<Strip> strip = new List<Strip>(20);
            int j;
            int start, end;
            for (int i = 0; i < strip_direction; i++)
            {
                j = 0;
                switch (isRows)
                {
                    case true:
                        while ((j < elements) && (source[i, j, (int)color] != 255)) // Find pixels and stop until first founded on the current row
                            j++;
                        break;
                    case false:
                        while ((j < elements) && (source[j, i, (int)color] != 255)) // Find pixels and stop until first founded on the current col
                            j++;
                        break;
                }
                if (j < elements) //If a pixel is found
                {
                    start = i; //Mark its start // strip[currentStrip].
                    do //Find the last row with pixels
                    {
                        j = 0;
                        i++;
                        if (i == strip_direction)
                            break;
                        switch (isRows)
                        {
                            case true:
                                while ((j < elements) && (source[i, j, (int)color] != 255)) // Find pixels and stop until first founded on the current row
                                    j++;
                                break;
                            case false:
                                while ((j < elements) && (source[j, i, (int)color] != 255)) // Find pixels and stop until first founded on the current col
                                    j++;
                                break;
                        }
                    } while (j < elements); // if j == cols -> therefore we've found an empty row
                    end = i - 1; //Mark the end of a strip with last row with pixels // strip[currentStrip].
                    if ((end - start) > 3) //If the strip is with normal width > 5 rows jump to next strip
                        strip.Add(new Strip(start, end));
                }
            }

            return strip;
        }

        private List<WeightCenter> FindSpots(byte[, ,] source, byte[, ,] original, int rows, int cols, FilterColors color)
        {
            int x1, y1, x2, y3;
            List<WeightCenter> weights = new List<WeightCenter>(20);
            WeightCenter w;
            List<Strip> RowStrips = FindStrips(source, rows, cols, color, true); //True for rows
            List<Strip> ColStrips = FindStrips(source, cols, rows, color, false); //False for cols

            foreach(Strip row in RowStrips)
            {
                foreach (Strip col in ColStrips)
                {
                    x1 = col.start;
                    y1 = row.start;
                    x2 = col.end;
                    y3 = row.end;

                    w = CalculateWeightCenter(source, x1, y1, x2 - x1 + 1, y3 - y1 + 1, color);
                    if ((w.x != 0) && (w.y != 0))
                    {
                        weights.Add(w);
                    }
                }
            }

            if (weights.Count == 5)
                DetectFace(source, original, weights, rows, cols, color);

            VisualizeStrips(source, RowStrips, ColStrips, rows, cols);
            //VisualizeWeightCenter(weights, rows, cols, source);

            return weights;
        } // End FindSpots

        private WeightCenter CalculateWeightCenter(byte[, ,] source, int x, int y, int width,
            int height, FilterColors color)
        {
            int sum_x = 0, sum_y = 0, n = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (source[i + y, j + x, (byte)color] == 255)
                    {
                        sum_x += j;
                        sum_y += i;
                        n++;
                    }
                }
            }

            WeightCenter w;
            if (n > 0)
            {
                w = new WeightCenter(
                    x + (int)sum_x / n,
                    y + (int)sum_y / n, y);
            }
            else
                w = WeightCenter.Empty;
            return w;
        }

        // Returns the correct order of weight centers if exactly five spots are detected
        private List<Point3D> DetectFace(byte[, ,] data, byte[, ,] original, List<WeightCenter> weights, int rows, int cols, FilterColors color)
        {
            // Five points from the sides descriptor - destination for homography
            Point3D[] destP = new Point3D[5];
            // Side 6
            //destP[0] = new Point3D(-11.8513, -16.3120, -31.4164);
            //destP[1] = new Point3D(-19.1759, 6.2306, -31.4164);
            //destP[2] = new Point3D(0.0000, 20.1627, -31.4164);
            //destP[3] = new Point3D(19.1759, 6.2306, -31.4164);
            //destP[4] = new Point3D(11.8514, -16.3120, -31.4164);

            // Side 1
            destP[0] = new Point3D(11.8514, 16.3119, 31.4164);
            destP[1] = new Point3D(19.1759, -6.2307, 31.4164);
            destP[2] = new Point3D(0.0000, -20.1627, 31.4164);
            destP[3] = new Point3D(-19.1759, -6.2306, 31.4164);
            destP[4] = new Point3D(-11.8513,16.3120,31.4164);
            
            int[,] indexWeightsDetect = {
                { 0, 1, 2, 3 },
                { 0, 1, 3, 2 },
                { 0, 2, 1, 3 },
                { 0, 2, 3, 1 },
                { 0, 3, 1, 2 },
                { 0, 3, 2, 1 }
            };

            Point3D[] destPars_fourthP = Geometry.homography_calc_pars(
                new Point3D[] { destP[0], destP[1], destP[2], destP[4] });
            Point3D[] destPars_fifthP = Geometry.homography_calc_pars(
                new Point3D[] { destP[0], destP[1], destP[2], destP[3] });
            Point3D[][] destPars = new Point3D[][] { destPars_fourthP, destPars_fifthP };
            double distance;
            double distanceMin = 10000;
            int jMin = 0;
            int[] indexes = new int[5];
            List<Point3D> homographyRes, weightListCurrent;
            List<Point3D> homographyMin = null, weightListMin = null;
            Point3D[] hmMinSourcePars = null, hmMinDestPars = null;

            List<Point3D> weightsList = new List<Point3D>(15);
            foreach (WeightCenter item in weights)
            {
                weightsList.Add(new Point3D(item.x, item.y, 0));
            }

            //if ((weights[0].rowStart != weights[1].rowStart) &&
            //    (weights[1].rowStart == weights[2].rowStart) ||
            //    (weights[3].rowStart == weights[4].rowStart))
            //{
            //    weightsList.Reverse();
            //}

            //ConvertXY(weightsList, rows);

            for (int i = 0; i < 6; i++)
            {
                weightListCurrent = new List<Point3D>(10);
                for (int j = 0; j < 4; j++)
                {
                    weightListCurrent.Add(weightsList[indexWeightsDetect[i, j]]);
                }
                Point3D[] sourcePars = Geometry.homography_calc_pars(weightListCurrent.ToArray());
                weightListCurrent.Add(weightsList[4]);

                for (int j = 0; j <= 1; j++)
                {
                    homographyRes = Geometry.homography_calc(weightListCurrent, sourcePars, destPars[j]);
                    distance = Geometry.Distance(homographyRes[4], destP[j + 3]);
                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        homographyMin = homographyRes;
                        weightListMin = weightListCurrent;
                        hmMinSourcePars = sourcePars;
                        hmMinDestPars = destPars[j];
                        jMin = j;
                        indexes[0] = indexWeightsDetect[i, 0];
                        indexes[1] = indexWeightsDetect[i, 1];
                        indexes[2] = indexWeightsDetect[i, 2];
                        // For correct order of points
                        if (j == 0) // This should be fixed in future
                        {
                            Point3D pTemp = homographyMin[3];
                            homographyMin[3] = homographyMin[4];
                            homographyMin[4] = pTemp;

                            pTemp = weightListMin[3];
                            weightListMin[3] = weightListMin[4];
                            weightListMin[4] = pTemp;

                            indexes[3] = 4;
                            indexes[4] = indexWeightsDetect[i, 3];
                        }
                        else
                        {
                            indexes[4] = 4;
                            indexes[3] = indexWeightsDetect[i, 3];
                        }
                    }
                }
            }

            //MessageBox.Show(distanceMin.ToString());

            if (distanceMin < 1)
            {
                List<Point3D> binarPlaces = new List<Point3D>(5);
                binarPlaces.Add(new Point3D(-3.6793, 16.9995, 0.0000));
                binarPlaces.Add(new Point3D(-17.3045, 1.7539, 0.0000));
                binarPlaces.Add(new Point3D(-7.0154, -15.9155, 0.0000));
                binarPlaces.Add(new Point3D(12.9687, -11.5903, 0.0000));
                binarPlaces.Add(new Point3D(15.0305, 8.7524, 0.0000));
                //List<Point3D> binarPlacesHomography;
                // If true is returned, recalc parameters
                Point3D[] sourceBinPars, destBinPars;
                if (CheckOrder(weightListMin, rows))
                {
                    //binarPlacesHomography = Geometry.homography_calc(binarPlaces, hmMinDestPars, hmMinSourcePars);

                }
                else
                {
                    //binarPlacesHomography = Geometry.homography_calc(binarPlaces, hmMinDestPars, hmMinSourcePars);

                }
                List<Point3D> binarPlacesHomography = Geometry.homography_calc(binarPlaces, hmMinDestPars, hmMinSourcePars);
                //WeightCenter binFound = FindBinaryPlace(binarPlacesHomography, original);

                if (checkBoxWriteDetected.Checked)
                {
                    using (StreamWriter str = new StreamWriter("homography.txt", true))
                    {
                        //List<Point3D> weightListConverted = Geometry.CloneList(weightsList);
                        //ConvertXY(weightListConverted, rows);
                        //foreach (Point3D pt in weightListConverted)
                        //{
                        //    str.WriteLine(pt.X.ToString() + "," + pt.Y.ToString() + "," + pt.Z.ToString());
                        //}
                        //str.WriteLine();

                        //foreach (Point3D pt in homographyMin)
                        //{
                        //    str.WriteLine(pt.X.ToString() + "," + pt.Y.ToString() + "," + pt.Z.ToString());
                        //}
                        //str.WriteLine();

                        List<Point3D> weightListConverted = Geometry.CloneList(weightListMin);
                        ConvertXY(weightListConverted, rows);
                        foreach (Point3D pt in weightListConverted)
                        {
                            str.WriteLine(pt.X.ToString() + "," + pt.Y.ToString() + "," + pt.Z.ToString());
                        }
                        str.WriteLine();
                    }
                }

                if (checkBoxVizDetectedSpots.Checked)
                {
                    //DrawPoints(data, weightListMin, rows, cols);
                    //ConvertYX(binarPlacesHomography, rows);
                    DrawPoints(original, binarPlacesHomography, rows, cols);
                    //VisualizeWeightCenter(binFound, rows, cols, original);
                }
                //List<List<Point3D>> result = new List<List<Point3D>>(3);
                //result.Add(weightListMin);
                return binarPlacesHomography;
            }
            else
                return null;
        }

        private WeightCenter FindBinaryPlace(List<Point3D> binarPlaces, byte[, ,] original)
        {
            int sumMin = 255 * 3;
            int sum;
            byte R, G, B; // b0g1r2
            Point3D pMin = null;
            foreach (Point3D p in binarPlaces)
            {
                R = original[(int)p.Y, (int)p.X, 2];
                G = original[(int)p.Y, (int)p.X, 1];
                B = original[(int)p.Y, (int)p.X, 0];
                sum = R + G + B;
                if (sum < sumMin)
                {
                    sumMin = sum;
                    pMin = p;
                }
            }

            return new WeightCenter((int)pMin.X, (int)pMin.Y, 0);
        }

        // Return true for recalc parameters
        private bool CheckOrder(List<Point3D> source, int rows)
        {
            List<Point3D> weightListMin2 = Geometry.CloneList(source);
            ConvertXY(weightListMin2, rows);
            Point3D pMinus = new Point3D(weightListMin2[0]);
            Geometry.p_add(weightListMin2, pMinus, -1d);
            Point3D rotZsc = Geometry.calc_rotZ(weightListMin2[1]);
            Geometry.p_rotZ(weightListMin2, rotZsc, -1d);

            if (weightListMin2[2].Y < 0)
            {
                source.Reverse(1, source.Count - 1);
                return true;
            }
            else
                return false;
        }
        #endregion

        #region Visualization
        private void DrawPoints(byte[,,] data, List<Point3D> Points, int rows, int cols)
        {
            List<WeightCenter> points2D = new List<WeightCenter>(Points.Count);
            foreach (Point3D currentP in Points)
            {
                points2D.Add(new WeightCenter((int)currentP.X, (int)currentP.Y, 0));
            }
            VisualizeWeightCenter(points2D, rows, cols, data);
        }

        private void VisualizeStrips(byte[, ,] destination, List<Strip> RowStrips, List<Strip> ColStrips, int rows, int cols)
        {
            foreach (Strip strip in RowStrips)
            {
                for (int i = 0; i < cols; i++)
                {
                    destination[strip.start, i, 2] = destination[strip.start, i, 1] = destination[strip.start, i, 0] = 255;
                    destination[strip.end, i, 2] = destination[strip.end, i, 1] = destination[strip.end, i, 0] = 255;
                }
            }

            foreach (Strip strip in ColStrips)
            {
                for (int i = 0; i < rows; i++)
                {
                    destination[i, strip.start, 2] = destination[i, strip.start, 1] = destination[i, strip.start, 0] = 255;
                    destination[i, strip.end, 2] = destination[i, strip.end, 1] = destination[i, strip.end, 0] = 255;
                }
            }
        }

        private void VisualizeWeightCenter(WeightCenter w, int rows, int cols, byte[, ,] destination)
        {
            int wx, wy;
            // Visualize weight centres
            if ((w.x > 0) && (w.y > 0))
            {
                wx = w.x;
                wy = w.y;

                //Draw white pixels on X and Y axis
                for (int i = -20; i < 20; i++)
                {
                    //Checks if a white cross is drawn outside the picture
                    if (((wx + i) > (cols - 1)) || ((wy + i) > (rows - 1)))
                        break;
                    if (((wx + i) <= 0) || ((wy + i) <= 0))
                        break;
                    //pixels are indexed as row -> col, therefore wy and wx are in this order, row = wy, col = wx
                    destination[wy + i, wx, 2] = destination[wy + i, wx, 1] = destination[wy + i, wx, 0] = 255;
                    destination[wy, wx + i, 2] = destination[wy, wx + i, 1] = destination[wy, wx + i, 0] = 255;
                }
            }
        }

        private void VisualizeWeightCenter(List<WeightCenter> weights, int rows, int cols, byte[, ,] destination)
        {
            foreach (WeightCenter w in weights)
                VisualizeWeightCenter(w, rows, cols, destination);
        }

        private void ConvertXY(List<Point3D> source, int rows)
        {
            for (int i = 0; i < source.Count; i++)
                ConvertXY(source[i], rows);
        }

        private void ConvertXY(Point3D sourceP, int rows)
        {
            //double xTemp = sourceP.X;
            //sourceP.X = rows - sourceP.Y;
            //sourceP.Y = xTemp;

            sourceP.Y = rows - sourceP.Y;
        }

        private void ConvertYX(List<Point3D> source, int rows)
        {
            for (int i = 0; i < source.Count; i++)
                ConvertYX(source[i], rows);
        }

        private void ConvertYX(Point3D sourceP, int rows)
        {
            double yTemp = sourceP.Y;
            sourceP.Y = rows - sourceP.X;
            sourceP.X = yTemp;
        }
        #endregion

        // Test method for single frame from file
        private void LoadFrame()
        {
            string strFileName = ImageProcessing.OpenImageFile();
            FormVisualizer visualizer = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Red");
            Image<Bgr, byte> image = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(strFileName));
            strFileName = ImageProcessing.OpenImageFile();
            Image<Bgr, byte> imageOriginal = new Image<Bgr, byte>((Bitmap)Bitmap.FromFile(strFileName));

            List<WeightCenter> weights = FindSpots(image.Data, imageOriginal.Data, image.Rows, image.Cols, FilterColors.Red);
            //List<Point3D> detectedSpots = DetectFace(image.Data, weights, image.Rows, image.Cols, FilterColors.Red);
            //IdentifySide(imageOriginal.Data, detectedSpots[0], detectedSpots[1].ToArray(), detectedSpots[2].ToArray());

            //DrawPoints(imageOriginal.Data, detectedSpots, image.Rows, image.Cols);

            

            visualizer.Show();
            visualizer.SetPicture(imageOriginal);
        }

        //private void IdentifySide(byte[,,] original, List<Point3D> detectedSpots, Point3D[] sourcePars, Point3D[] destPars)
        //{

        //}
    }
}