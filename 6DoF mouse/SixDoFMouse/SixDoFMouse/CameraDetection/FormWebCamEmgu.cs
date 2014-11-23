using CADBest.GeometryNamespace;
using Emgu.CV;
using Emgu.CV.Structure;
using SixDoFMouse.CameraDetection;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SixDoFMouse
{
    public partial class FormWebCamEmgu : Form
    {
        #region Members and Constants
        private const int TIMER_DISP_INTERVAL = 100;

        //public bool checkFourP;
        private Capture _capture = null;
        private bool _captureInProgress;
        public bool IsCapturing
        {
            get { return _captureInProgress; }
        }   

        private FormVisualizer vizRed, vizGreen, vizBlue, 
            vizFilters, vizOriginal, vizYellow;
        public static FormVisualizer  vizDataText;
        private FormChart chartPoint;

        private Image<Bgr, byte> _frame;
        private Image<Bgr, byte> imgOriginal;
        private Image<Bgr, byte> imgFiltered;
        private Image<Hsv, byte> imgHSV;
        private Filteringcoefficients Coef;

        private object imgLocker = new object();
        private Image<Bgr, byte> imgB;
        private Image<Bgr, byte> imgG;
        private Image<Bgr, byte> imgR;
        private Image<Bgr, byte> imgY;

        // Rows and cols of current resolution
        private int FrameRows, FrameCols;

        private MouseDataContainer dataMouse;
        public Calibration calibrateModule;
        private MovingAveraging crawlAveragingModule;
        private MouseRecognition MouseRecognitionModule;
        private ImageProcessing ImageProcessingModule = new ImageProcessing();

        // Storage for n-count camera ViewPoints divided in 4 lists
        //private List<List<Point3D>> ArrViewPoint;

        // This is the center of a frame (Width / 2 and Height / 2),
        // intersection of X axis with right side of the frame (e.g. the right middle point)
        // and top middle point, the intersection of Y axis with the top edge
        // Its used for 4-point calibration algorithm, and is initialized every time when capturing starts
        private List<Point3D> CenterProjXY;

        public TimerDispatch TimerDispatchPars;

        // Usage of different colors and visualizers
        private bool USE_RED =      true;
        private bool USE_GREEN =    true;
        private bool USE_BLUE =     true;
        private bool USE_YELLOW =   true;
        private bool SHOW_FILTERS = true;

        public event DrawPolylineHandler DrawPolylines;
        public event CameraSendOrientationHandler SendOrientationParameters;
        private bool GetWhiteColor = false;
        #endregion

        public FormWebCamEmgu()
        {
            InitializeComponent();

            MouseRecognitionModule = new MouseRecognition();
            if (USE_RED)
                vizRed = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Red");
            if (USE_GREEN)
                vizGreen = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Green");
            if (USE_BLUE)
                vizBlue = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Blue");
            if (USE_YELLOW)
                vizYellow = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Yellow");
            if (SHOW_FILTERS)
                vizFilters = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Filtered");
            vizOriginal = new FormVisualizer(FormVisualizer.DisplayMode.Emgu, "Original");
            vizDataText = new FormVisualizer(FormVisualizer.DisplayMode.Text, "Text Data");

            // For testing purposes
             
            

            MouseRecognitionModule.vizDataText = vizDataText;        

            dataMouse = MouseDataContainer.Instance;
            
           
            calibrateModule = new Calibration();
            TimerDispatchPars = new TimerDispatch(TIMER_DISP_INTERVAL);
            TimerDispatchPars.TickMethod += timerDispatch_TickMethod;
            ReadRegistryValues();
            LoadHSVValues();
        }

        /// <summary>
        /// Initialization which is made when Capture button is activated
        /// </summary>
        private void Initialize()
        {
            GetCoefficients();

            CenterProjXY = new List<Point3D>(3);
            CenterProjXY.Add(new Point3D(FrameCols / 2, FrameRows / 2, 0)); // Center of projection
            CenterProjXY.Add(new Point3D(FrameCols, FrameRows / 2, 0)); // X axis middle
            CenterProjXY.Add(new Point3D(FrameCols / 2, FrameRows, 0)); // Y axis middle

            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FPS, 30);
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, FrameCols);
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, FrameRows);
            _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_EXPOSURE, trackBarBrightness.Value);

            CheckResolution();
            
            imgB = new Image<Bgr, byte>(FrameCols, FrameRows);
            imgG = new Image<Bgr, byte>(FrameCols, FrameRows);
            imgR = new Image<Bgr, byte>(FrameCols, FrameRows);
            imgY = new Image<Bgr, byte>(FrameCols, FrameRows);
            imgFiltered = new Image<Bgr, byte>(FrameCols, FrameRows);

            ImageProcessingModule.DataB = imgB.Data;
            ImageProcessingModule.DataG = imgG.Data;
            ImageProcessingModule.DataR = imgR.Data;
            ImageProcessingModule.DataY = imgY.Data;

            crawlAveragingModule = new MovingAveraging(TimerDispatchPars.Interval);
        }

        private void RGBFilter()
        {
            double im;
            byte min;
            byte red, new_red;
            byte green, new_green;
            byte blue, new_blue;

            byte[, ,] data1 = imgR.Data;
            byte[, ,] data2 = imgG.Data;
            byte[, ,] data3 = imgB.Data;
            byte[, ,] dataYellow = imgY.Data;
            byte[, ,] dataFiltered = imgFiltered.Data;

            for (int i = FrameRows - 1; i >= 0; i--)
            {
                for (int j = FrameCols - 1; j >= 0; j--)
                {
                    red = dataFiltered[i, j, 2];
                    green = dataFiltered[i, j, 1];
                    blue = dataFiltered[i, j, 0];

                    // Subtract the Min of R G B colors from the remaining two
                    min = Geometry.Min(red, green, blue);
                    red = (byte)(red - min);
                    green = (byte)(green - min);
                    blue = (byte)(blue - min);

                    dataFiltered[i, j, 2] = red;
                    dataFiltered[i, j, 1] = green;
                    dataFiltered[i, j, 0] = blue;

                    if (USE_RED)
                    {
                        if (checkBox1.Checked)
                        {
                            im = red * Coef.IM_R_K - green - blue + Coef.IM_R_ADD;
                            if (im >= Coef.IM_R_PRAG)
                                new_red = 255;
                            else
                                new_red = 0;
                        }
                        else
                            new_red = red;
                        data1[i, j, 2] = new_red;
                        data1[i, j, 1] = 0;
                        data1[i, j, 0] = 0;
                    }

                    if (USE_GREEN)
                    {
                        if (checkBox1.Checked)
                        {
                            im = green * Coef.IM_G_K - red - blue + Coef.IM_G_ADD;
                            if (im >= Coef.IM_G_PRAG)
                                new_green = 255;
                            else
                                new_green = 0;
                        }
                        else
                            new_green = green;
                        data2[i, j, 2] = 0;
                        data2[i, j, 1] = new_green;
                        data2[i, j, 0] = 0;
                    }

                    if (USE_BLUE)
                    {
                        if (checkBox1.Checked)
                        {
                            im = blue * Coef.IM_B_K - green - red + Coef.IM_B_ADD;
                            if (im >= Coef.IM_B_PRAG)
                                new_blue = 255;
                            else
                                new_blue = 0;
                        }
                        else
                            new_blue = blue;
                        data3[i, j, 2] = 0;
                        data3[i, j, 1] = 0;
                        data3[i, j, 0] = new_blue;
                    }

                    if (USE_YELLOW)
                    {
                        if (checkBox1.Checked)
                        {
                            im = Coef.IM_Y_K * (0.8 * red + green) - 2 * blue + Coef.IM_Y_ADD;
                            if (im >= Coef.IM_Y_PRAG)
                            {
                                new_red = 255;
                                new_green = 255;
                            }
                            else
                            {
                                new_red = 0;
                                new_green = 0;
                            }
                        }
                        else
                        {
                            new_red = red;
                            new_green = green;
                        }
                        dataYellow[i, j, 2] = new_red;
                        dataYellow[i, j, 1] = new_green;
                        dataYellow[i, j, 0] = 0;
                    }
                }
            }
        }

        //public void ColorFiltration()
        //{
        //    Vector3D currentPixel = new Vector3D();
        //    byte[, ,] dataOriginal = imgOriginal.Data;
        //    byte[, ,] dataR = imgR.Data;
        //    byte[, ,] dataG = imgG.Data;
        //    byte[, ,] dataB = imgB.Data;
        //    byte[, ,] dataY = imgY.Data;

        //    for (int i = 0; i < FrameRows; i++) // Rows
        //    {
        //        for (int j = 0; j < FrameCols; j++) // Cols
        //        {
        //            byte R = dataOriginal[i, j, 2];
        //            byte G = dataOriginal[i, j, 1];
        //            byte B = dataOriginal[i, j, 0];
        //            currentPixel.SetCoordinates(R, G, B);
        //            currentPixel.Normalize();
        //            CheckDistance(currentPixel, MouseRecognitionModule.RedVector,
        //                dataR, FilterColors.R, i, j, dMaxTrackbarR);
        //            CheckDistance(currentPixel, MouseRecognitionModule.GreenVector,
        //                dataG, FilterColors.G, i, j, dMaxTrackbarG);
        //            CheckDistance(currentPixel, MouseRecognitionModule.BlueVector,
        //                dataB, FilterColors.B, i, j, dMaxTrackbarB);
        //            CheckDistance(currentPixel, MouseRecognitionModule.YellowVector,
        //                dataY, FilterColors.Y, i, j, dMaxTrackbarY);
        //        }
        //    }
        //}

        //public static void CheckDistance(Vector3D currentVector, Vector3D colorVector,
        //    byte[, ,] data, FilterColors color, int i, int j, double maxTrackBar)
        //{
        //    // Custom distance method without square root
        //    double distance = Geometry.Distance(currentVector, colorVector);

        //    data[i, j, 2] = data[i, j, 1] = data[i, j, 0] = 0;
        //    if (distance < maxTrackBar)
        //    {
        //        if ((color == FilterColors.R) || (color == FilterColors.Y))
        //            data[i, j, 2] = 255;
        //        if ((color == FilterColors.G) || (color == FilterColors.Y))
        //            data[i, j, 1] = 255;
        //        if (color == FilterColors.B)
        //            data[i, j, 0] = 255;
        //    }
        //}

        /// <summary>
        /// Average lists of point3D with weight.
        /// </summary>
        /// <param name="AddWeight">If true add weight</param>
        /// <param name="vp">List of Point3D</param>
        /// <returns>Average list of point3D</returns>
        public List<Point3D> AverageViewPointsWeight(bool AddWeight, params List<Point3D>[] vp )
        {
            List<List<Point3D>> VpList = new List<List<Point3D>>();
            for (int i = 0; i < vp.Length; i++)
            {
                VpList.Add(vp[i]);
            }

            return AverageViewPointsWeight(AddWeight, VpList);
        }

        /// <summary>
        ///  Average lists of point3D with weight.
        /// </summary>
        /// <param name="AddWeight">If true add weight</param>
        /// <param name="vp">List of lists of Point3D</param>
        /// <returns>Average list of point3D</returns>
        public static List<Point3D> AverageViewPointsWeight(bool AddWeight, List<List<Point3D>> vp)
        {
            int n = 0;
            double[] Xvalues = new double[4];
            double[] Yvalues = new double[4];
            double[] Zvalues = new double[4];
            double SumDist = 0;         
            List<Double> Weights = new List<double>();
            for (int i = 0; i < vp.Count ; i++)
            {
                if (vp[i] != null) 
                {
                    if (vp[i][4].X == 0)
                    {
                        vp[i][4].X = 1;
                    }

                    SumDist += (1 / vp[i][4].X);
                    Weights.Add(1 / vp[i][4].X);
                    n++;
                }
            }

            foreach (List<Point3D> List in vp)
            {
                if ((List != null) && (List.Count == 5))      
                {

                    for (int i = 0; i < List.Count - 1; i++)
                    {
                        Xvalues[i] += ((List[i].X * (1 / List[4].X)) / SumDist);
                        Yvalues[i] += ((List[i].Y * (1 / List[4].X)) / SumDist);
                        Zvalues[i] += ((List[i].Z * (1 / List[4].X)) / SumDist);
                    }
                }
            }

            List<Point3D> vpResult = new List<Point3D>();
            for (int i = 0; i < 4; i++)
            {
                double x, y, z;
                x = Xvalues[i];
                y = Yvalues[i];
                z = Zvalues[i];
                vpResult.Add(new Point3D(x, y, z));
            }
            if (SumDist == 0)
                return null;

            //Add Weight
            if (AddWeight == true)
            {
                vpResult.Add(new Point3D(SumDist, n, 0));
                //vizDataText.SetText("X = " + vpResult[0].X.ToString(),
                //              "Y = " + vpResult[0].Y.ToString(),
                //              "Z = " + vpResult[0].Z.ToString(),
                //              "n = " + n.ToString());

            }
            //Statistics(vp, vpResult, SumDist, Weights);
            return vpResult;        
        }


        public static void Statistics(List<List<Point3D>> vp, List<Point3D> MeanS, double SumDist, List<Double> Weights)
        {
            //Standard Deviation
            double stDevX, stDevY, stDevZ;
            //Variance
            double VarianceX = new double();
            double VarianceY = new double();
            double VarianceZ = new double();
            //Third central moment
            double u3X = new double();
            double u3Y = new double();
            double u3Z = new double();
            //Fourth central moment       
            double u4X = new double();
            double u4Y = new double();
            double u4Z = new double();
            //Skew Asimetria
            double skewX, skewY, skewZ;
            //Excess (Kurtosis)
            double ExcessX, ExcessY, ExcessZ;    

            //Variance
            int n = 0;
            for (int i = 0; i < vp.Count; i++)
            {
                if (vp[i] != null)
                {
                    VarianceX += (Math.Pow((vp[i][0].X - MeanS[0].X), 2) * Weights[n]) / SumDist;
                    VarianceY += (Math.Pow((vp[i][0].Y - MeanS[0].Y), 2) * Weights[n]) / SumDist;
                    VarianceZ += (Math.Pow((vp[i][0].Z - MeanS[0].Z), 2) * Weights[n]) / SumDist;
                    n++;
                }
            }

            //Standard Deviation
            stDevX = Math.Sqrt(VarianceX);
            stDevY = Math.Sqrt(VarianceY);
            stDevZ = Math.Sqrt(VarianceZ);

            // Third central moment
            n = 0;
            for (int i = 0; i < vp.Count; i++)
            {
                if (vp[i] != null)
                {
                    u3X += (Math.Pow((vp[i][0].X - MeanS[0].X), 3) * Weights[n]) / SumDist;
                    u3Y += (Math.Pow((vp[i][0].Y - MeanS[0].Y), 3) * Weights[n]) / SumDist;
                    u3Z += (Math.Pow((vp[i][0].Z - MeanS[0].Z), 3) * Weights[n]) / SumDist;
                    n++;
                }
            }

            // Fourth central moment
            n = 0;
            for (int i = 0; i < vp.Count; i++)
            {
                if (vp[i] != null)
                {
                    u4X += (Math.Pow((vp[i][0].X - MeanS[0].X), 4) * Weights[n]) / SumDist;
                    u4Y += (Math.Pow((vp[i][0].Y - MeanS[0].Y), 4) * Weights[n]) / SumDist;
                    u4Z += (Math.Pow((vp[i][0].Z - MeanS[0].Z), 4) * Weights[n]) / SumDist;
                    n++;
                }
            }

            //Skew
            skewX = u3X / (Math.Pow(VarianceX, 3));
            skewY = u3Y / (Math.Pow(VarianceY, 3));
            skewZ = u3Z / (Math.Pow(VarianceZ, 3));

            //Excess
            ExcessX = (u4X / (Math.Pow(VarianceX, 4))) - 3;
            ExcessY = (u4Y / (Math.Pow(VarianceY, 4))) - 3;
            ExcessZ = (u4Z / (Math.Pow(VarianceZ, 4))) - 3;

            vizDataText.SetText("As X" + skewX.ToString(),
                                "As Y" + skewY.ToString(),
                                "As Z" + skewZ.ToString());
        }


        private void timerDispatch_TickMethod(object sender, EventArgs e)
        {
            if ((SendOrientationParameters != null) &&
                checkBoxSendControlPars.Checked &&
                crawlAveragingModule.UpdateStep())
            {
                SendOrientationEventArgs orientationArgs =
                    new SendOrientationEventArgs();
                orientationArgs.OrientationParameters.Add(
                    crawlAveragingModule.CurrentAngle.Clone());
                if (checkBoxPan.Checked)
                {
                    orientationArgs.OrientationParameters.Add(
                        crawlAveragingModule.CurrentTarget.Clone());
                }
                SendOrientationParameters(this, orientationArgs);
            }
        }

        List<List<Point3D>> MeanWeightAverageVp = new List<List<Point3D>>();

        public void ProcessFrame(object sender, EventArgs arg)
        {
            _frame = _capture.RetrieveBgrFrame();
            //_frame = _capture.RetrieveBgrFrame().Flip(Emgu.CV.CvEnum.FLIP.NONE);
            //_frame = _capture.RetrieveBgrFrame().Flip(Emgu.CV.CvEnum.FLIP.HORIZONTAL);
            //_frame = _capture.RetrieveBgrFrame().Flip(Emgu.CV.CvEnum.FLIP.VERTICAL);

            imgOriginal = _frame.Convert<Bgr, byte>();
            //imgFiltered = _frame.Convert<Bgr, byte>();
            imgHSV = _frame.Convert<Hsv, byte>();           
            //if (GetWhiteColor)
            //{
                //MouseRecognitionModule.WhiteVector = 
                //    ImageProcessing.GetWhiteColor(imgOriginal.Data, FrameRows, FrameCols);
                //MouseRecognitionModule.WhiteVector = new Vector3D(216, 198, 222);
                //MessageBox.Show("White averaged color = " + MouseRecognitionModule.WhiteVector.ToString());
                //MouseRecognitionModule.RecalcVectors();
                //MouseRecognitionModule.RotateColorVectors();
            //    GetWhiteColor = false;
            //}

            //ImageProcessingModule.ImageProcess(imgFiltered.Data, FrameRows, FrameCols, DownLevel, UpLevel);
            //imgHSV = imgFiltered.Convert<Hsv, byte>();
            ImageProcessingModule.ImageProcessHSV(imgHSV.Data, FrameRows, FrameCols);
            imgFiltered = imgHSV.Convert<Bgr, byte>();

            if (checkBoxOldFiltration.Checked)
            {
                RGBFilter();
            }
            else
            {
                //if (MouseRecognitionModule.WhiteVector != null)
                //    ColorFiltration();
            }

            List<Point3D> ViewPointYellow = null;
            List<Point3D> ViewPointRed = null;
            List<Point3D> ViewPointBlue = null;
            List<Point3D> ViewPointGreen = null;
         


            if (USE_RED)
            {
                ViewPointRed = MouseRecognitionModule.Detection(
                    imgR, imgOriginal.Data, FilterColors.R, FrameRows, FrameCols, CenterProjXY);
            }

            if (USE_GREEN)
            {
                ViewPointGreen = MouseRecognitionModule.Detection(
                    imgG, imgOriginal.Data, FilterColors.G, FrameRows, FrameCols, CenterProjXY);
            }

            if (USE_BLUE)
            {
                ViewPointBlue = MouseRecognitionModule.Detection(
                    imgB, imgOriginal.Data, FilterColors.B, FrameRows, FrameCols, CenterProjXY);
            }

            if (USE_YELLOW)
            {
                ViewPointYellow = MouseRecognitionModule.Detection(
                    imgY, imgOriginal.Data, FilterColors.Y, FrameRows, FrameCols, CenterProjXY);
            }

            List<Point3D> averagedVP = new List<Point3D>();

            averagedVP = AverageViewPointsWeight(true, ViewPointRed, ViewPointGreen, ViewPointBlue, ViewPointYellow);         
              
            if(averagedVP != null)
            //Visualization.ProjectDescriptor(averagedVP, CenterProjXY, imgOriginal, FrameRows);

            //FourP Weight moving averaging
                MeanWeightAverageVp.Add(averagedVP);

            if (MeanWeightAverageVp.Count > 4)
                MeanWeightAverageVp.RemoveAt(0);

            if ((MeanWeightAverageVp.Count == 4) && (checkBoxWeight2.Checked))
            {
                averagedVP = AverageViewPointsWeight(false, MeanWeightAverageVp);
            }
            //End                       

            if (averagedVP != null)
            {
                SendControlParameters(averagedVP, imgOriginal.Data);
            }

            //if (!TimerDispatch.Enabled)
            //{
                bool PreventFlick = checkBoxTopFlickering.Checked && (averagedVP == null);
                VisualizeImageData(PreventFlick);
            //}
        }

        List<Point3D> CalibrationCopy = new List<Point3D>();
        List<Point3D> currentClone = new List<Point3D>();

        Point3D ySinCosCurr = null;
        Point3D zSinCosCurr = null;
        Point3D xSinCosCurr = null;
        Point3D TranslationCurr = null;
        Point3D[] currParam = null;


        private void SendControlParameters(List<Point3D> ViewPoint, byte[, ,] ImgDataToFlash)
        {
            List<Point3D> ViewDescriptor = new List<Point3D>();
            if (GlobalProperties.ViewPointMode)
            {
                ViewDescriptor.Add(ViewPoint[0]);
                ViewDescriptor.Add(ViewPoint[1]);
                ViewDescriptor.Add(ViewPoint[2]);
            }

            Visualization.ProjectDescriptor(ViewPoint, CenterProjXY, imgR, FrameRows);
            List<Point3D> currentOrientation = calibrateModule.RecalcOrientation(ViewPoint);

            if (GlobalProperties.ViewPointMode)
            {
                if (checkBoxPan.Checked)
                    ViewDescriptor.Add(currentOrientation[0].Clone());
                else
                    ViewDescriptor.Add(new Point3D(0, 0, 0));

                ViewDescriptor.Add(currentOrientation[1]);
                ViewDescriptor.Add(currentOrientation[2]);
                ViewDescriptor.Add(currentOrientation[3]);
            }

            // Recalibrate every frame
            if (calibrateModule.p0translation != null)
            {
                // Move current frame with translation of calibration frame 
                Geometry.p_add(currentOrientation, calibrateModule.p0translation, -1);

                // Rotation of whole current coordinate system with parameters of calibration
                Geometry.p_rotY(currentOrientation, calibrateModule.ySinCosAxis, -1);
                Geometry.p_rotZ(currentOrientation, calibrateModule.zSinCosAxis, -1);
                Geometry.p_rotX(currentOrientation, calibrateModule.xSinCosAxis, -1);

                // Move back current frame with translation of calibration frame 
                Geometry.p_add(currentOrientation, calibrateModule.p0translation, 1);
                CalibrationCopy = Geometry.CloneList(calibrateModule.CalibrationOrientation[0]);     
                
                       
                // Move current frame to origin and calculate align parameters
                TranslationCurr = currentOrientation[0].Clone();
                Geometry.p_add(currentOrientation, currentOrientation[0].Clone(), -1);
                currParam = Geometry.align_calc_pars(
                    currentOrientation[0],
                    currentOrientation[1],
                    currentOrientation[2]);

                // Rotate Current frame coordinate system to coincide with XYZ
                ySinCosCurr = Geometry.calc_rotY(currentOrientation[1]);
                Geometry.p_rotY(currentOrientation, ySinCosCurr, -1);

                zSinCosCurr = Geometry.calc_rotZ(currentOrientation[1]);
                Geometry.p_rotZ(currentOrientation, zSinCosCurr, -1);

                xSinCosCurr = Geometry.calc_rotX(currentOrientation[2]);
                Geometry.p_rotX(currentOrientation, xSinCosCurr, -1);

                //Align current orientation with parameters of calibration and current frame parameters before rotation
                Geometry.align(currentOrientation, currParam, calibrateModule.AlignParamCalibration);

                // Translate back with current frame translation
                Geometry.p_add(currentOrientation, TranslationCurr.Clone(), 1);

                //VisualizeCoordinateSystem(currentOrientation, true);
            }

            OnSendParameters(ViewDescriptor, currentOrientation);

            if (calibrateModule.IsCalibrate)
            {
                calibrateModule.CalcCalibration(ViewPoint, ImgDataToFlash);
            }

            if ((checkBoxCharts.Checked) && (!chartPoint.IsDisposed))
            {
            //    chartPt1.UpdateChart(ViewPoint[1].X, ViewPoint[1].Y, ViewPoint[1].Z);
            //    chartPtD1.UpdateChart(dArr1.X, dArr1.Y, dArr1.Z);
            }

            // Call subscribers for this event
            //if (checkBoxDrawViewPoint.Checked && (DrawViewPoint != null))
            //    DrawViewPoint(this, new DrawViewPointEventArgs(ViewPoint, true));
        }

        private void OnSendParameters(List<Point3D> ViewDescriptor, List<Point3D> currentOrientation)
        {
            // Call subscribers to SendOrientationParameters event
            if ((SendOrientationParameters != null) && (checkBoxSendControlPars.Checked))
            {
                SendOrientationEventArgs orientationArgs =
                    new SendOrientationEventArgs();

                if (GlobalProperties.ViewPointMode)
                    orientationArgs.OrientationParameters = ViewDescriptor;
                else
                {
                    List<Point3D> rotationAngles = Geometry.GetRotationAngles(currentOrientation);
                    // Calculate the corresponding angles for X, Y and Z rotation
                    double xRot = Geometry.CalculateAngle(rotationAngles[0]) * 1;
                    double yRot = Geometry.CalculateAngle(rotationAngles[1]) * -1;
                    double zRot = Geometry.CalculateAngle(rotationAngles[2]) * 1;
                    xRot = Geometry.AngleRound(xRot, 10);
                    yRot = Geometry.AngleRound(yRot, 10);
                    zRot = Geometry.AngleRound(zRot, 10);
                    Point3D angles = new Point3D(xRot, yRot, zRot);

                    //vizDataText.SetText(
                    //    "target X: " + currentOrientation[0].X.ToString(),
                    //    "target Y: " + currentOrientation[0].Y.ToString(),
                    //    "target Z: " + currentOrientation[0].Z.ToString());
                        //"time: " + elapsedTime.ToString());

                    double xTarget = Geometry.ValueRound(currentOrientation[0].X, 10);
                    double yTarget = Geometry.ValueRound(currentOrientation[0].Y, 10);
                    double zTarget = Geometry.ValueRound(currentOrientation[0].Z, 10);
                    //if ((zTarget > 50) && (zTarget < 160))
                    //{
                    //    zTarget = 100;
                    //}

                    //if ((zTarget > 160) && (zTarget < 200))
                    //{
                    //    zTarget = 200;

                    //}
                    //if ((zTarget > 200) && (zTarget < 300))
                    //{
                    //    zTarget = 250;
                    //}
                    //if (zTarget > 300)
                    //{
                    //    zTarget = 300;
                    //}
                    Point3D target = new Point3D(xTarget, yTarget, zTarget);

                    double elapsedTime = 0;
                    if (GlobalProperties.UseTimers)
                        elapsedTime = crawlAveragingModule.FrameTick(angles, target);
                    else
                    {

                        orientationArgs.OrientationParameters.Add(angles);
                        //orientationArgs.OrientationParameters.Add(currentOrientation[0]);     
                        if (checkBoxPan.Checked)
                            orientationArgs.OrientationParameters.Add(target);
                    }



                    //vizDataText.SetText(
                    //    "angle X: " + Geometry.ConvertToDeg(xRot).ToString(),
                    //    "angle Y: " + Geometry.ConvertToDeg(yRot).ToString(),
                    //    "angle Z: " + Geometry.ConvertToDeg(zRot).ToString(),
                    //    "time: " + elapsedTime.ToString());
                }

                if (!GlobalProperties.UseTimers)
                    SendOrientationParameters(this, orientationArgs);
            }
        }

        private void checkBoxWeight_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}