using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;

namespace ImageProperties
{
    public class MagnifierBox : PictureBox
    {
        //There are three dependencies from ZoomPicBox class for correct Magnifier
        //  1. ZoomPicBox.Image is needed for drawing portion of its image
        //  2. ZoomPicBox.Zoom
        //  3. ZoomPicBox control location and Width and Height
        //They are set from ZoomPicBox object
        public Bitmap MainImage = null;
        public float MainImageZoom = 1;
        public int ZoomPicBoxTop, ZoomPicBoxLeft;
        public int ZoomPicBoxWidth, ZoomPicBoxHeight;
        public int ZoomPicBoxScrollX, ZoomPicBoxScrollY;

        float[] SelfZoomFactor = GlobalProperties.zoomFactor;
        int SelfZoomIndex;
        Rectangle zoomArea;
        Bitmap newBmp;
        Graphics g;

        public MagnifierBox()
        {
            zoomArea = new Rectangle();
            //MouseWheelHandler.Add(this, MyMouseWheel);
            SelfZoomIndex = 12;
        }

        public void WheelZoom(MouseEventArgs e)
        {
            if (e.Delta < 0) // Zoom out
            {
                if (SelfZoomIndex > 0)
                { SelfZoomIndex--; }
                else
                { return; }
            }
            else // Zoom in
            {
                if (SelfZoomIndex < SelfZoomFactor.Length - 1)
                { SelfZoomIndex++; }
                else
                { return; }
            }
        }

        //
        // Move the Magnifier according mouse location
        //
        //public void MoveMaG(Point e)
        //{
        //    if (Visible)
        //    {
        //        MagnifierZoomImage(e);
        //        Top = (ZoomPicBoxTop + e.Y) - (Height / 2);
        //        Left = (ZoomPicBoxLeft + e.X) - (Width / 2);
        //    }
        //}

        //
        // Create zoom Magnifier
        //
        public void MagnifierZoomImage(Point e)
        {
            Refresh();
            float zX = 0, zY = 0;
            //Initialize on first use
            if (newBmp == null)
            {
                newBmp = new Bitmap(Width, Height);
                g = Graphics.FromImage(newBmp);
                g.PixelOffsetMode = PixelOffsetMode.Half;
            }

            // (-zoomPicBox1.AutoScrollPosition.X + e.X) coordinates of mouse in picture
            // zoomArea.X(Y) upper left corner of retangle
            // This calculate zoom and centre of retangle under mouse cursor!
            if (MainImage != null)
            {
                // If zoomArea.X and zoomArea.Y < 0, they are set to 0, that is happened when mouse is pointing in upper left corner
                zX = Math.Min(((ZoomPicBoxScrollX + e.X) / MainImageZoom) - ((Width / 2) / SelfZoomFactor[SelfZoomIndex]) + 0.5f, 
                    MainImage.Width - this.Width / SelfZoomFactor[SelfZoomIndex]);
                zY = Math.Min(((ZoomPicBoxScrollY + e.Y) / MainImageZoom) - ((Height / 2) / SelfZoomFactor[SelfZoomIndex]) + 0.5f, 
                    MainImage.Height - this.Height / SelfZoomFactor[SelfZoomIndex]);
                zoomArea.X = Math.Max((int)zX, 0);
                zoomArea.Y = Math.Max((int)zY, 0);
                zoomArea.Width = (int)Math.Round(Width / SelfZoomFactor[SelfZoomIndex]);
                zoomArea.Height = (int)Math.Round(Height / SelfZoomFactor[SelfZoomIndex]);

                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(MainImage, ClientRectangle, zoomArea, GraphicsUnit.Pixel);
            }
            this.Image = newBmp;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            WheelZoom(e);
            MagnifierZoomImage(e.Location);
            base.OnMouseWheel(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.Focus();
            base.OnMouseEnter(e);
        }

        //void MyMouseWheel(MouseEventArgs e)
        //{
        //    WheelZoom(e);
        //    MagnifierZoomImage(e.Location);
        //}
    }
}