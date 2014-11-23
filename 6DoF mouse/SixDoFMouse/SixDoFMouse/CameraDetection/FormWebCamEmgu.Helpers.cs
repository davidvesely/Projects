using CADBest.GeometryNamespace;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace SixDoFMouse
{
    partial class FormWebCamEmgu
    {
        private void VisualizeCoordinateSystem(List<Point3D> currentOrientation, bool ToErasePrevious)
        {
            if (DrawPolylines != null)
            {
                Color[] coordColors = new Color[3];
                coordColors[0] = Color.Red;
                coordColors[1] = Color.Green;
                coordColors[2] = Color.Blue;

                List<List<Point3D>> axises = new List<List<Point3D>>();
                for (int i = 1; i <= 3; i++)
                {
                    List<Point3D> axis = new List<Point3D>();
                    axis.Add(currentOrientation[0]);
                    axis.Add(currentOrientation[i]);
			        axises.Add(axis);
                }

                for (int i = 0; i < axises.Count; i++)
                {
                    DrawPolylineEventArgs e = 
                        new DrawPolylineEventArgs(
                            axises[i], ToErasePrevious, coordColors[i]);
                    DrawPolylines(this, e);
                    ToErasePrevious = false;
                }
            }
        }

        private void SaveScreenshots()
        {
            if (USE_RED)
                imgR.Save("WebcamRed.bmp");
            if (USE_GREEN)
                imgG.Save("WebcamGreen.bmp");
            if (USE_BLUE)
                imgB.Save("WebcamBlue.bmp");
            if (USE_YELLOW)
                imgY.Save("WebcamYellow.bmp");
            _frame.Save("WebcamOriginal.bmp");
            System.Windows.MessageBox.Show("Screenshots are taken successfully!");
        }

        private void GetCoefficients()
        {
            Coef.IM_R_K = trbR_K.Value;
            Coef.IM_R_ADD = (int)trbR_Add.Value;
            Coef.IM_R_PRAG = (int)trbR_Prag.Value;

            Coef.IM_G_K = trbG_K.Value;
            Coef.IM_G_ADD = (int)trbG_Add.Value;
            Coef.IM_G_PRAG = (int)trbG_Prag.Value;

            Coef.IM_B_K = trbB_K.Value;
            Coef.IM_B_ADD = (int)trbB_Add.Value;
            Coef.IM_B_PRAG = (int)trbB_Prag.Value;

            Coef.IM_Y_K = trbY_K.Value;
            Coef.IM_Y_ADD = (int)trbY_Add.Value;
            Coef.IM_Y_PRAG = (int)trbY_Prag.Value;

            int width, height;
            if (!int.TryParse(textBoxWidth.Text, out width) ||
                !int.TryParse(textBoxHeight.Text, out height))
                return;

            FrameRows = height;
            FrameCols = width;
        }

        private void VisualizeImageData(bool PreventFlick)
        {
            // Prevent from drawing a frame when ViewPoint is not obtained
            // and descriptor should be projected without flickering
            if ((USE_RED) && (!PreventFlick))
                vizRed.SetPicture(imgR);
            if (USE_GREEN)
                vizGreen.SetPicture(imgG);
            if (USE_BLUE)
                vizBlue.SetPicture(imgB);
            if (USE_YELLOW)
                vizYellow.SetPicture(imgY);
            if (SHOW_FILTERS)
                vizFilters.SetPicture(imgHSV);
            //if (checkBoxPan.Checked)
            //    imgOriginal = imgOriginal.Flip(Emgu.CV.CvEnum.FLIP.HORIZONTAL);
            vizOriginal.SetPicture(imgOriginal);
        }

        public void ResetCapturing()
        {
            if (_captureInProgress)
            {
                //stop the capture
                buttonCapture.Text = "Start Capture";
                _capture.Pause();
                _capture.ImageGrabbed -= ProcessFrame;
                if (GlobalProperties.UseTimers)
                    TimerDispatchPars.Enabled = false;
            }
            else
            {
                //start the capture
                buttonCapture.Text = "Stop";
                try
                {
                    int choice;
                    if (radioButtonDevice1.Checked)
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

                if (USE_RED)
                    vizRed.Show();
                if (USE_GREEN)
                    vizGreen.Show();
                if (USE_BLUE)
                    vizBlue.Show();
                if (USE_YELLOW)
                    vizYellow.Show();
                if (SHOW_FILTERS)
                    vizFilters.Show();
                vizOriginal.Show();
                if (GlobalProperties.UseTimers)
                    TimerDispatchPars.Enabled = true;
            }
            _captureInProgress = !_captureInProgress;
        }

        private void CheckResolution()
        {
            // Workaround for incorrect width and height
            double width = _capture.GetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH);
            double height = _capture.GetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT);

            if ((FrameRows != height) || (FrameCols != width))
            {
                string msg = string.Format("Resolution is set to default. \n Width: {0}, Height: {1}", width, height);
                System.Windows.Forms.MessageBox.Show(msg);

                FrameRows = (int)height;
                FrameCols = (int)width;
            }
        }
    }
}