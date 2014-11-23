using CADBest.AutoCADManagerNamespace;
using SixDoFMouse.AlphaForm2;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SixDoFMouse
{
    public partial class FormControls : Form
    {
        //Arrays for creating Radio Buttons
        PictureButton[] Rotation = new PictureButton[4];
        PictureButton[] Movement = new PictureButton[4];
        //The clicked button of one from Rot All to Roz Z and Move All to Move Z
        PictureButton ClickedRotation, ClickedMovement;
        //Array of all buttons for easier scaling of them
        PictureButton[] Buttons = new PictureButton[14];
        Bitmap[] ButtonsDefault = new Bitmap[14];
        Bitmap[] ButtonsPressed = new Bitmap[14];

        int toward = 1;
        float scale;
        double byX, byY, byZ;

        FormWebCamEmgu cameraDetector;
        AutoCADManager acadManager;

        private void TurnOffMovementButtons()
        {
            //Turns off all other move buttons except the one which is active
            //Also turns off 6DOF button
            for (int i = 0; i < 4; i++)
            {
                if (ClickedMovement != Movement[i])
                    Movement[i].TurnOff();
            }
            if (ClickedMovement != pb6DOF)
                pb6DOF.TurnOff();
        }
        private void TurnOffRotationButtons()
        {
            //Turns off all other rotation buttons except the one which is active
            //Also turns off 6DOF button
            for (int i = 0; i < 4; i++)
            {
                if (ClickedRotation != Rotation[i])
                    Rotation[i].TurnOff();
            }
            if (ClickedRotation != pb6DOF)
                pb6DOF.TurnOff();
        }

        void RearrangeControls(float scale)
        {
            int width, height;
            width = (int)(Properties.Resources.background.Width * scale);
            height = (int)(Properties.Resources.background.Height * scale);
            this.Width = width;
            this.Height = height;
            for (int i = 0; i < Buttons.Length; i++)
            {
                //Position update
                Buttons[i].Left = (int)(Buttons[i].Left * scale);
                Buttons[i].Top = (int)(Buttons[i].Top * scale);
                //Picture update
                Buttons[i].DefaultImage = AutoCADManager.ResizeBitmap(scale, ButtonsDefault[i]);
                Buttons[i].PressedImage = AutoCADManager.ResizeBitmap(scale, ButtonsPressed[i]);
                //Size update
                Buttons[i].Width = Buttons[i].DefaultImage.Width;
                Buttons[i].Height = Buttons[i].DefaultImage.Height;
            }
        }

        public FormControls()
        {
            InitializeComponent();
            InitializeArrays();

            scale = AutoCADManager.ReadPrefScale();
            if (scale != 1)
                RearrangeControls(scale);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
                System.Windows.Forms.Application.Exit();
            // Restart the movement timer with key 1 from the keyboard
            else if (e.KeyChar == (char)Keys.D1)
                timerPanelPosition.Enabled = !timerPanelPosition.Enabled;
        }

        private void pbEmpty_Click(object sender, EventArgs e)
        {
            if (pbEmpty.isPressed) // Start the capture
            {
                acadManager = new AutoCADManager();
                acadManager.SelectObjects();
                cameraDetector = new FormWebCamEmgu();
                //cameraDetector.ResetCapturing();
                cameraDetector.Show();
                //cameraDetector.DrawViewPoint += cameraDetector_DrawViewPoint;
                cameraDetector.SendOrientationParameters += cameraDetector_SendOrientationParameters;
                cameraDetector.DrawPolylines += cameraDetector_DrawPolylines;
                                
                //acadManager.vizDataText = cameraDetector.vizDataText;

                
                //cameraDetector.calibrateModule.DrawTestEvent += cameraDetector_DrawTest;
            }
            else // Stop the capture
            {
                if (GlobalProperties.UseTimers)
                    cameraDetector.TimerDispatchPars.Enabled = false;
                cameraDetector.ResetCapturing();
                //cameraDetector.DrawViewPoint -= cameraDetector_DrawViewPoint;
                cameraDetector.SendOrientationParameters -= cameraDetector_SendOrientationParameters;
                cameraDetector.DrawPolylines -= cameraDetector_DrawPolylines;
                //cameraDetector.calibrateModule.DrawTestEvent -= cameraDetector_DrawTest;
                cameraDetector.Close();
                cameraDetector.Dispose();
                cameraDetector = null;
                acadManager = null;
                Close();
            }
        }

        //private void cameraDetector_DrawTest(object sender, DrawPolylineEventArgs e)
        //{
        //    acadManager.DrawPolylines(e.ViewPoint, e.ShouldDeletePrevious, e.PolylineColor);
        //}

        //private void cameraDetector_DrawViewPoint(object sender, DrawPolylineEventArgs e)
        //{
        //    acadManager.DrawPolylines(e.ViewPoint, 
        //        e.ShouldDeletePrevious, 
        //        Color.FromArgb(255, 255, 255));
        //}

        private void cameraDetector_DrawPolylines(object sender, DrawPolylineEventArgs e)
        {
            acadManager.DrawPolylines(e.Polyline, e.ShouldFlush, e.PolylineColor);
        }

        private void cameraDetector_SendOrientationParameters(object sender, SendOrientationEventArgs e)
        {
            acadManager.ModifyViewspace(e.OrientationParameters, false);
        }

        /// <summary>
        /// This timer makes the panel to follow mouse cursor
        /// </summary>
        private void timerPanelPosition_Tick(object sender, EventArgs e)
        {
            Point mouse = PointToClient(MousePosition);
            int x, y;
            x = mouse.X;
            y = mouse.Y;
            if (x < 0)
                this.Left += x;
            else if (x > this.ClientRectangle.Width)
                this.Left += x - this.ClientRectangle.Width;
            if (y < 0)
                this.Top += y;
            else if (y > this.ClientRectangle.Height)
                this.Top += y - this.ClientRectangle.Height;
        }
    }
}