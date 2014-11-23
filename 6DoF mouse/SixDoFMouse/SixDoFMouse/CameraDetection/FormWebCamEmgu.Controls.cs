using Microsoft.Win32;
using SixDoFMouse.CameraDetection;
using System;
using System.Windows.Forms;

namespace SixDoFMouse
{
    partial class FormWebCamEmgu
    {
        private void ReleaseData()
        {
            if (_capture != null)
                _capture.Dispose();
        }

        private void FormWebCamEmgu_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteRegistryValues();
            ReleaseData();

            if (USE_RED)
                vizRed.Close();
            if (USE_GREEN)
                vizGreen.Close();
            if (USE_BLUE)
                vizBlue.Close();
            if (USE_YELLOW)
                vizYellow.Close();
            if (SHOW_FILTERS)
                vizFilters.Close();
            vizOriginal.Close();
            if (!vizDataText.IsDisposed)
                vizDataText.Close();
            if (crawlAveragingModule != null)
                crawlAveragingModule.stopwatch.Enabled = false;
            if (TimerDispatchPars != null)
                TimerDispatchPars.Enabled = false;
            checkBoxCharts.Checked = false;

            Application.Exit();
        }

        private void buttonDrawHomography_Click(object sender, EventArgs e)
        {
            MouseRecognitionModule.UseHomographyAveraging = !MouseRecognitionModule.UseHomographyAveraging;
            if (MouseRecognitionModule.UseHomographyAveraging)
                MessageBox.Show("Using averaging with homography of the five founded spots");
        }

        private void trackBarBrightness_Scroll(object sender, EventArgs e)
        {
            bool continueCapture = false;
            if (_captureInProgress)
            {
                ResetCapturing(); // Stop the capture
                continueCapture = true;
            }

            if (_capture != null)
                _capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_BRIGHTNESS, trackBarBrightness.Value);

            labelBrightness.Text = "Brightness: " + trackBarBrightness.Value;
            if (continueCapture)
                ResetCapturing();
        }

        private void buttonCalibrateCenter_Click(object sender, EventArgs e)
        {
            // Start a calibration
            calibrateModule.IsCalibrate = true;

            //calibrateModule.CalibrationCalcParameters();
        }

        private void checkBoxCharts_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCharts.Checked)
            {
                chartPoint = new FormChart(30, -500, 500, "Point 1", ChartUpdatingMode.Timed, 500);
                chartPoint.AddCurve("X Axis");
                chartPoint.AddCurve("Y Axis");
                chartPoint.AddCurve("Z Axis");
                chartPoint.Show();
            }
            else
            {
                chartPoint.Close();
            }
        }

        private void checkBoxShowTextViz_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowTextViz.Checked)
            {
                if (vizDataText.IsDisposed)
                    vizDataText = new FormVisualizer(FormVisualizer.DisplayMode.Text, "Text display");
                vizDataText.Show();
            }
            else
                vizDataText.Close();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            TrackBar trackBar = (TrackBar)sender;
            HSVCoefficients coefficients;

            if (radioButtonRed.Checked)
            {
                coefficients = ImageProcessingModule.RedHSVCoef;
            }
            else if (radioButtonGreen.Checked)
            {
                coefficients = ImageProcessingModule.GreenHSVCoef;
            }
            else if (radioButtonBlue.Checked)
            {
                coefficients = ImageProcessingModule.BlueHSVCoef;
            }
            else
            {
                coefficients = ImageProcessingModule.YellowHSVCoef;
            }

            switch (trackBar.Name)
            {
                case "trackBarHDown":
                    coefficients.HueDown = (byte)trackBarHDown.Value;
                    labelHDown.Text = coefficients.HueDown.ToString();
                    break;
                case "trackBarHUp":
                    coefficients.HueUp = (byte)trackBarHUp.Value;
                    labelHUp.Text = coefficients.HueUp.ToString();
                    break;
                case "trackBarSDown":
                    coefficients.SatDown = (byte)trackBarSDown.Value;
                    labelSDown.Text = coefficients.SatDown.ToString();
                    break;
                case "trackBarSUp":
                    coefficients.SatUp = (byte)trackBarSUp.Value;
                    labelSUp.Text = coefficients.SatUp.ToString();
                    break;
                case "trackBarWhite":
                    coefficients.ValueUp = (byte)trackBarWhite.Value;
                    labelUpLevel.Text = coefficients.ValueUp.ToString();
                    break;
                case "trackBarBlack":
                    coefficients.ValueDown = (byte)(trackBarBlack.Value);
                    labelDownLevel.Text = coefficients.ValueDown.ToString();
                    break;
                default:
                    break;
            }
        }

        private void radioButtonColors_CheckedChanged(object sender, EventArgs e)
        {
            LoadHSVValues();
        }

        /// <summary>
        /// Load the settings for current selected color and HSV track bars
        /// </summary>
        private void LoadHSVValues()
        {
            HSVCoefficients coefficients;
            if (radioButtonRed.Checked)
            {
                coefficients = ImageProcessingModule.RedHSVCoef;
            }
            else if (radioButtonGreen.Checked)
            {
                coefficients = ImageProcessingModule.GreenHSVCoef;
            }
            else if (radioButtonBlue.Checked)
            {
                coefficients = ImageProcessingModule.BlueHSVCoef;
            }
            else
            {
                coefficients = ImageProcessingModule.YellowHSVCoef;
            }

            trackBarHDown.Value = coefficients.HueDown;
            trackBarHUp.Value = coefficients.HueUp;
            trackBarSDown.Value = coefficients.SatDown;
            trackBarSUp.Value = coefficients.SatUp;
            trackBarBlack.Value = coefficients.ValueDown;
            trackBarWhite.Value = coefficients.ValueUp;

            labelHDown.Text = coefficients.HueDown.ToString();
            labelHUp.Text = coefficients.HueUp.ToString();
            labelSDown.Text = coefficients.SatDown.ToString();
            labelSUp.Text = coefficients.SatUp.ToString();
            labelDownLevel.Text = coefficients.ValueDown.ToString();
            labelUpLevel.Text = coefficients.ValueUp.ToString();
        }

        private void buttonGetWhiteColor_Click(object sender, EventArgs e)
        {
            GetWhiteColor = true;
        }

        //private void trackBarBlackWhite_Scroll(object sender, EventArgs e)
        //{
        //    TrackBar trbSender = (TrackBar)sender;
        //    switch (trbSender.Name)
        //    {
        //        case "trackBarWhite":
        //            UpLevel = (int)(trackBarWhite.Value * 16);
        //            labelUpLevel.Text = UpLevel.ToString();
        //            break;
        //        case "trackBarBlack":
        //            DownLevel = (int)(trackBarBlack.Value * 16);
        //            labelDownLevel.Text = DownLevel.ToString();
        //            break;
        //        default:
        //            break;
        //    }
        //}

        public static void ReadWindowState(RegistryKey rk, object obj, string textRepresentation)
        {
            // Try to cast the object as any Form inheritance
            Form objForm = obj as Form;
            if (obj != null)
            {
                objForm.Top = Convert.ToInt32((string)rk.GetValue(textRepresentation + "Top"));
                objForm.Left = Convert.ToInt32((string)rk.GetValue(textRepresentation + "Left"));
                objForm.Width = Convert.ToInt32((string)rk.GetValue(textRepresentation + "Width"));
                objForm.Height = Convert.ToInt32((string)rk.GetValue(textRepresentation + "Height"));

                FormVisualizer viz = objForm as FormVisualizer;
                if ((viz != null) && (viz.WorkingMode == FormVisualizer.DisplayMode.Emgu))
                {
                    double zoom;
                    if (double.TryParse((string)rk.GetValue(textRepresentation + "Zoom"), out zoom))
                        viz.ZoomScale = zoom;
                }
            }
        }

        private void ReadHSVCoefficient(RegistryKey rk, HSVCoefficients coefficients, string colorName)
        {
            string result = (string)rk.GetValue(colorName + "HueDown");
            if (result != null)
                byte.TryParse(result, out coefficients.HueDown);

            result = (string)rk.GetValue(colorName + "HueUp");
            if (result != null)
                byte.TryParse(result, out coefficients.HueUp);

            result = (string)rk.GetValue(colorName + "SatDown");
            if (result != null)
                byte.TryParse(result, out coefficients.SatDown);

            result = (string)rk.GetValue(colorName + "SatUp");
            if (result != null)
                byte.TryParse(result, out coefficients.SatUp);

            result = (string)rk.GetValue(colorName + "ValueDown");
            if (result != null)
                byte.TryParse(result, out coefficients.ValueDown);

            result = (string)rk.GetValue(colorName + "ValueUp");
            if (result != null)
                byte.TryParse(result, out coefficients.ValueUp);
        }

        private void ReadCheckBoxValues(RegistryKey rk, CheckBox cbControl, string registryItem)
        {
            bool checkBoxChecked;
            if (bool.TryParse((string)rk.GetValue(registryItem), out checkBoxChecked))
                cbControl.Checked = checkBoxChecked;
        }

        private void ReadRegistryValues()
        {
            RegistryKey rk = null;
            try
            {
                rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\" + this.ProductName);
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

                    ReadCheckBoxValues(rk, checkBoxShowTextViz, "ShowTextVisualizer");
                    ReadCheckBoxValues(rk, checkBoxSendControlPars, "SendControlPars");
                    ReadCheckBoxValues(rk, checkBoxOldFiltration, "UseOldFiltration");

                    string deviceID = (string)rk.GetValue("DeviceID");
                    switch (deviceID)
                    {
                        case "1":
                            radioButtonDevice1.Checked = true;
                            break;
                        case "2":
                            radioButtonDevice2.Checked = true;
                            break;
                        default:
                            break;
                    }

                    ReadWindowState(rk, this, "Main");
                    ReadWindowState(rk, vizRed, "Red");
                    ReadWindowState(rk, vizGreen, "Green");
                    ReadWindowState(rk, vizBlue, "Blue");
                    ReadWindowState(rk, vizYellow, "Yellow");
                    ReadWindowState(rk, vizFilters, "Filters");
                    ReadWindowState(rk, vizOriginal, "Original");
                    ReadWindowState(rk, vizDataText, "DataText");

                    // Brightness
                    trackBarBrightness.Value = (int)rk.GetValue("Brightness");
                    labelBrightness.Text = "Brightness " + trackBarBrightness.Value.ToString();

                    // Calibration parameters
                    calibrateModule.ReadRegistryCalibration(rk);

                    // Read HSV coefficients
                    ReadHSVCoefficient(rk, ImageProcessingModule.RedHSVCoef, "Red");
                    ReadHSVCoefficient(rk, ImageProcessingModule.GreenHSVCoef, "Green");
                    ReadHSVCoefficient(rk, ImageProcessingModule.BlueHSVCoef, "Blue");
                    ReadHSVCoefficient(rk, ImageProcessingModule.YellowHSVCoef, "Yellow");
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
        }

        private void WriteWindowState(RegistryKey rk, object obj, string textRepresentation)
        {
            // Try to cast the object as any Form inheritance
            Form objForm = obj as Form;
            if (obj != null)
            {
                rk.SetValue(textRepresentation + "Top", objForm.Top.ToString());
                rk.SetValue(textRepresentation + "Left", objForm.Left.ToString());
                rk.SetValue(textRepresentation + "Width", objForm.Width.ToString());
                rk.SetValue(textRepresentation + "Height", objForm.Height.ToString());
                FormVisualizer viz = objForm as FormVisualizer;
                if ((viz != null) && (viz.WorkingMode == FormVisualizer.DisplayMode.Emgu))
                    rk.SetValue(textRepresentation + "Zoom", viz.ZoomScale.ToString());
            }
        }

        private void WriteHSVcoefficient(RegistryKey rk, HSVCoefficients coefficients, string colorName)
        {
            rk.SetValue(colorName + "HueDown", coefficients.HueDown.ToString());
            rk.SetValue(colorName + "HueUp", coefficients.HueUp.ToString());
            rk.SetValue(colorName + "SatDown", coefficients.SatDown.ToString());
            rk.SetValue(colorName + "SatUp", coefficients.SatUp.ToString());
            rk.SetValue(colorName + "ValueDown", coefficients.ValueDown.ToString());
            rk.SetValue(colorName + "ValueUp", coefficients.ValueUp.ToString());
        }

        private void WriteRegistryValues()
        {
            RegistryKey rk = null;
            try
            {
                rk = Registry.LocalMachine.CreateSubKey("SOFTWARE\\" + this.ProductName);
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

                rk.SetValue("Brightness", trackBarBrightness.Value);

                rk.SetValue("ShowTextVisualizer", checkBoxShowTextViz.Checked.ToString());
                rk.SetValue("SendControlPars", checkBoxSendControlPars.Checked.ToString());
                rk.SetValue("UseOldFiltration", checkBoxOldFiltration.Checked.ToString());

                string deviceID;
                if (radioButtonDevice1.Checked)
                    deviceID = "1";
                else
                    deviceID = "2";
                rk.SetValue("DeviceID", deviceID);

                // Save window's and visualizer's state
                WriteWindowState(rk, this, "Main");
                WriteWindowState(rk, vizRed, "Red");
                WriteWindowState(rk, vizGreen, "Green");
                WriteWindowState(rk, vizBlue, "Blue");
                WriteWindowState(rk, vizYellow, "Yellow");
                WriteWindowState(rk, vizFilters, "Filters");
                WriteWindowState(rk, vizOriginal, "Original");
                WriteWindowState(rk, vizDataText, "DataText");

                // Calibration parameters
                calibrateModule.WriteRegistryCalibration(rk);

                // HSV coefficients
                WriteHSVcoefficient(rk, ImageProcessingModule.RedHSVCoef, "Red");
                WriteHSVcoefficient(rk, ImageProcessingModule.GreenHSVCoef, "Green");
                WriteHSVcoefficient(rk, ImageProcessingModule.BlueHSVCoef, "Blue");
                WriteHSVcoefficient(rk, ImageProcessingModule.YellowHSVCoef, "Yellow");
            }
            catch (Exception)
            {
                MessageBox.Show("Error during writing settings in registry!");
            }
            finally
            {
                if (rk != null)
                    rk.Dispose();
            }
        }
    }
}
