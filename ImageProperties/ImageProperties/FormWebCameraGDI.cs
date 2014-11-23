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
using Microsoft.Win32;
using Touchless.Vision.Camera;
using System.Drawing.Imaging;

namespace ImageProperties
{
    public partial class FormWebCameraGDI : Form
    {
        private CameraFrameSource _frameSource;
        private static Bitmap _latestFrame;
        //private static BitmapData _latestFrameData, _destSobelData;
        private static Bitmap _dest1, _dest2, _dest3;//, _destSobel;
        //private static BitmapData _dest1Data, _dest2Data, _dest3Data;

        private int lastTick;
        private int frameRate;

        int CaptureWidthLocal, CaptureHeightLocal;

        byte[] arr1, arr2, arr3;
        byte[] _orig;
        byte[] _SobelArr;
        
        public FormWebCameraGDI()
        {
            InitializeComponent();
        }

        private void ReadRegistryValues()
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\" + Application.ProductName);
                if (rk != null)
                {
                    string width, height;
                    width = (string)rk.GetValue("width");
                    height = (string)rk.GetValue("height");
                    if ((width != null) && (height != null))
                    {
                        textBoxWidth.Text = width;
                        textBoxHeight.Text = height;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void WriteRegistryValues()
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.CreateSubKey("SOFTWARE\\" + Application.ProductName);
                rk.SetValue("width", textBoxWidth.Text);
                rk.SetValue("height", textBoxHeight.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void WebCameraTest_Load(object sender, EventArgs e)
        {
            ReadRegistryValues();
            if (!DesignMode)
            {
                try
                {
                    // Refresh the list of available cameras
                    comboBoxCameras.Items.Clear();
                    foreach (Camera cam in CameraService.AvailableCameras)
                        comboBoxCameras.Items.Add(cam);

                    if (comboBoxCameras.Items.Count > 0)
                        comboBoxCameras.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void WebCameraTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            thrashOldCamera();
            WriteRegistryValues();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Early return if we've selected the current camera
            if (_frameSource != null && _frameSource.Camera == comboBoxCameras.SelectedItem)
                return;


            thrashOldCamera();
            startCapturing();
        }

        private void startCapturing()
        {
            try
            {
                Camera c = (Camera)comboBoxCameras.SelectedItem;
                setFrameSource(new CameraFrameSource(c));
                _frameSource.Camera.CaptureWidth = CaptureWidthLocal = Convert.ToInt32(textBoxWidth.Text);
                _frameSource.Camera.CaptureHeight = CaptureHeightLocal = Convert.ToInt32(textBoxHeight.Text);
                _frameSource.Camera.Fps = 30;
                _frameSource.NewFrame += OnImageCaptured;

                int PixelCount = CaptureWidthLocal * CaptureHeightLocal;
                _orig = new byte[PixelCount * 3];
                arr1 = new byte[PixelCount];
                arr2 = new byte[PixelCount];
                arr3 = new byte[PixelCount];
                _SobelArr = new byte[PixelCount];
                _dest1 = new Bitmap(CaptureWidthLocal, CaptureHeightLocal, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                _dest2 = new Bitmap(CaptureWidthLocal, CaptureHeightLocal, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                _dest3 = new Bitmap(CaptureWidthLocal, CaptureHeightLocal, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                //_destSobel = new Bitmap(CaptureWidthLocal, CaptureHeightLocal, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                pictureBoxDisplay.Paint += new PaintEventHandler(drawLatestImage);
                pictureBox1.Paint += new PaintEventHandler(drawLatestImage1);
                pictureBox2.Paint += new PaintEventHandler(drawLatestImage2);
                pictureBox3.Paint += new PaintEventHandler(drawLatestImage3);
                _frameSource.StartFrameCapture();
            }
            catch (Exception ex)
            {
                comboBoxCameras.Text = "Select A Camera";
                MessageBox.Show(ex.Message);
            }
        }

        void DisplayFPS()
        {
            if (System.Environment.TickCount - lastTick >= 1000)
            {
                labelFPS.Text = frameRate.ToString();
                labelFPS.Refresh();
                frameRate = 0;
                lastTick = System.Environment.TickCount;
            }
            frameRate++;
        }

        private void drawLatestImage(object sender, PaintEventArgs e)
        {
            try
            {
                if (_latestFrame != null)
                {
                    // Draw the latest image from the active camera
                    //DisplayFPS();
                    if (checkBoxVisualization.Checked)
                    {
                        ImageProcessing.SobelPointers(_latestFrame, _dest1, CaptureWidthLocal, CaptureHeightLocal);
                        //pictureBox1.Refresh();
                        //pictureBox3.Refresh();
                    }
                    e.Graphics.DrawImage(_latestFrame, 0, 0, pictureBoxDisplay.Width, pictureBoxDisplay.Height);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " drawing Source");
            }
        }

        private void drawLatestImage1(object sender, PaintEventArgs e)
        {
            try
            {
                if (_latestFrame != null)
                    // Draw the latest image from the active camera
                    e.Graphics.DrawImage(_dest1, 0, 0, pictureBox1.Width, pictureBox1.Height);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " drawing Red Test");
            }
        }

        private void drawLatestImage2(object sender, PaintEventArgs e)
        {
            try
            {
                //if (_latestFrame != null)
                    // Draw the latest image from the active camera
                    //e.Graphics.DrawImage(_destSobel, 0, 0, pictureBox2.Width, pictureBox2.Height);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " drawing Green Test");
            }
        }

        private void drawLatestImage3(object sender, PaintEventArgs e)
        {
            try
            {
                //if (_latestFrame != null)
                    // Draw the latest image from the active camera
                    //e.Graphics.DrawImage(ImageProcessing.Corner(_latestFrame), 0, 0, pictureBox3.Width, pictureBox3.Height);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " drawing Blue Test");
            }
        }

        public void OnImageCaptured(Touchless.Vision.Contracts.IFrameSource frameSource, Touchless.Vision.Contracts.Frame frame, double fps)
        {
            _latestFrame = frame.Image;
            //ProcessFrame();
            pictureBoxDisplay.Invalidate();
            //pictureBox1.Invalidate();
        }

        private void ProcessFrame()
        {
            try
            {
                //_latestFrameCopy = (Bitmap)_latestFrame.Clone();
                //_dest1Data = _dest1.LockBits(
                //    new Rectangle(0, 0, CaptureWidthLocal, CaptureHeightLocal),
                //    ImageLockMode.ReadWrite,
                //    _dest1.PixelFormat
                //);

                //_dest2Data = _dest2.LockBits(
                //    new Rectangle(0, 0, CaptureWidthLocal, CaptureHeightLocal),
                //    ImageLockMode.ReadWrite,
                //    _dest2.PixelFormat
                //);

                //_dest3Data = _dest3.LockBits(
                //    new Rectangle(0, 0, CaptureWidthLocal, CaptureHeightLocal),
                //    ImageLockMode.ReadWrite,
                //    _dest3.PixelFormat
                //);

                //_destSobelData = _destSobel.LockBits(
                //  new Rectangle(0, 0, CaptureWidthLocal, CaptureHeightLocal),
                //  ImageLockMode.ReadWrite,
                //  _destSobel.PixelFormat
                //);

                //_latestFrameData = _latestFrameCopy.LockBits(
                //    new Rectangle(0, 0, CaptureWidthLocal, CaptureHeightLocal),
                //    ImageLockMode.ReadOnly,
                //    //_latestFrameCopy.PixelFormat
                //    PixelFormat.Format24bppRgb
                //);

                //Read bmp data

                

                //ImageProcessing.ReadBmpData(_latestFrameData, _orig, 3, CaptureWidthLocal, CaptureHeightLocal); //Read lock frame
                //ImageProcessing.BlackWhite2(_orig, arr1, arr2, arr3, CaptureWidthLocal, CaptureHeightLocal);    // Black and white. Emo's filter
                //ImageProcessing.WriteBmpData(_dest1Data, arr1, CaptureWidthLocal, CaptureHeightLocal);           // return data to bitmap
                //ImageProcessing.WriteBmpData(_dest2Data, arr2, CaptureWidthLocal, CaptureHeightLocal);
                //ImageProcessing.WriteBmpData(_dest3Data, arr3, CaptureWidthLocal, CaptureHeightLocal);
                //ImageProcessing.Sobel(_orig, _SobelArr, CaptureWidthLocal, CaptureHeightLocal);
                //ImageProcessing.SobelPointers(_latestFrame, _dest1);
                //ImageProcessing.WriteBmpData(_destSobelData, _SobelArr, CaptureWidthLocal, CaptureHeightLocal);
            }
            finally
            {
                //if (_dest1 != null) _dest1.UnlockBits(_dest1Data);   // unlock the picture
                //if (_dest2 != null) _dest2.UnlockBits(_dest2Data);
                //if (_dest3 != null) _dest3.UnlockBits(_dest3Data);
                //if (_latestFrame != null) _latestFrameCopy.UnlockBits(_latestFrameData);
                //if (_destSobel != null) _destSobel.UnlockBits(_destSobelData);
            }
        }

        private void setFrameSource(CameraFrameSource cameraFrameSource)
        {
            if (_frameSource == cameraFrameSource)
                return;

            _frameSource = cameraFrameSource;
        }

        private void thrashOldCamera()
        {
            // Trash the old camera
            if (_frameSource != null)
            {
                _frameSource.NewFrame -= OnImageCaptured;
                _frameSource.Camera.Dispose();
                setFrameSource(null);
                pictureBoxDisplay.Paint -= new PaintEventHandler(drawLatestImage);
                pictureBox1.Paint -= new PaintEventHandler(drawLatestImage1);
                pictureBox2.Paint -= new PaintEventHandler(drawLatestImage2);
                pictureBox3.Paint -= new PaintEventHandler(drawLatestImage3);
            }
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            MessageBox.Show(_latestFrame.Width.ToString() + " " + _latestFrame.Height.ToString());
            // snap camera
            if (_frameSource != null)
                _frameSource.Camera.ShowPropertiesDialog();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            thrashOldCamera();
        }

        private void labelFPS_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxCameras_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
