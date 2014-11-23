using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ImageProperties
{
    /// <summary>
    /// ZoomPicBox does what it says on the wrapper.
    /// </summary>
    /// <remarks>
    /// PictureBox doesn't lend itself well to overriding. Why not start with something basic and do the job properly?
    /// </remarks>
    public class ZoomPicBox : ScrollableControl
    {
        //All fields are private
        bool isPanning; // Its used by Mouse Move event and is set: 
                        // ON by MouseDown event, OFF by MouseUp event

        int previousX = 0, previousY = 0; // These are used to help Pan method to determine in which direction is moved the mouse
        float[] zoomFactor = GlobalProperties.zoomFactor;
        int zoomIndex = 12;

        //These are used for drawing cursor
        Pen penMouse = new System.Drawing.Pen(Color.Blue, 1.5F); //Pen for drawing the cursor
        SolidBrush brushMouse = new SolidBrush(Color.Blue); //Brush for drawing a single pixel of cursor lines
        //Bitmap DisplayedImage = null; //Current portion of displayed image
        Graphics DisplayedArea = null; //This is used for drawing custom cursor

        public MagnifierBox MagnifierCopy = null; // This is reference to Magnifier, used to copy properties in it

        public CurrentStateClass CurrentState; // Used to extract X, Y, and RGB color of current pixel

        public void SetPanMode(bool mode)
        {
            isPanning = mode;
        }
        
        public void SetPanMode(bool mode, MouseEventArgs e)
        {
            isPanning = mode;
            if (mode)
            {
                previousX = e.X;
                previousY = e.Y;
            }
        }

        public float GetCurrentZoom()
        {
            return zoomFactor[zoomIndex];
        }

        public bool GetPanMode()
        {
            return isPanning;
        }
        Bitmap _image;
        [
        Category("Appearance"),
        Description("The image to be displayed")
        ]
        public Bitmap Image
        {
            get { return _image; }
            set
            {
                _image = value;
                UpdateScaleFactor();
                Invalidate();
            }
        }

        float _zoom = 1.0f;
        [
        Category("Appearance"),
        Description("The zoom factor. Less than 1 to reduce. More than 1 to magnify.")
        ]
        public float Zoom
        {
            get { return _zoom; }
            set
            {
                if (value < 0 || value < 0.00001)
                    value = 0.00001f;
                _zoom = value;
                UpdateScaleFactor();
                Invalidate();
            }
        }

        /// <summary>
        /// Calculates the effective size of the image
        ///after zooming and updates the AutoScrollSize accordingly
        /// </summary>
        private void UpdateScaleFactor()
        {
            if (_image == null)
                this.AutoScrollMinSize = this.Size;
            else
            {
                this.AutoScrollMinSize = new Size(
                  (int)(this._image.Width * _zoom + 0.5f),
                  (int)(this._image.Height * _zoom + 0.5f)
                  );
            }
        }

        InterpolationMode _interpolationMode = InterpolationMode.Default;
        [
        Category("Appearance"),
        Description("The interpolation mode used to smooth the drawing")
        ]
        public InterpolationMode InterpolationMode
        {
            get { return _interpolationMode; }
            set { _interpolationMode = value; }
        }


        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // do nothing.
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //if no image, don't bother
            if (_image == null)
            {
                base.OnPaintBackground(e);
                return;
            }
            //Set up a zoom matrix
            Matrix mx = new Matrix(_zoom, 0, 0, _zoom, 0, 0);
            //now translate the matrix into position for the scrollbars
            mx.Translate(this.AutoScrollPosition.X / _zoom, this.AutoScrollPosition.Y / _zoom);
            //use the transform
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            e.Graphics.Transform = mx;
            //and the desired interpolation mode
            e.Graphics.InterpolationMode = _interpolationMode;
            //Draw the image ignoring the images resolution settings.
            e.Graphics.DrawImage(_image, new Rectangle(0, 0, this._image.Width, this._image.Height), 0, 0, _image.Width, _image.Height, GraphicsUnit.Pixel);
            base.OnPaint(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            SetPanMode(true, e);
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Pan(e);
            GetPixelInfo(e);
            DrawCursor(e);
            base.OnMouseMove(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Cursor.Hide();
            this.Focus();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Cursor.Show();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            SetPanMode(false);
            base.OnMouseUp(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (MagnifierCopy != null)
                SetZoomPicBoxLocation(); // Updates the position and dimenstions of the control needed for magnifier
            DisplayedArea = CreateGraphics();
            //UpdateDisplayedImage();
            base.OnResize(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            ZoomImage(e);
            //The next line is not called, because in addition to zooming scrolls the picture down or up
            //base.OnMouseWheel(e);
        }

        public ZoomPicBox()
        {
            //Double buffer the control
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.ResizeRedraw |
              ControlStyles.UserPaint |
              ControlStyles.DoubleBuffer, true);

            SetPanMode(false); //Pan mode is set ON only on MouseDown event
            CurrentState = new CurrentStateClass(); // Initialize current state (x, y, and color of the pixel)
        }

        void DrawCursor(MouseEventArgs e)
        {
            int OriginalX, OriginalY;
            int ScaledX, ScaledY;
            int i, BrushLenght;

            Refresh();

            //New method for drawing cursor
            if ((_image != null) && (0 < e.X) && (e.X < this.ClientRectangle.Width) && // Checking if mouse is not dragging outside the ZoomPicBox
                (0 < e.Y) && (e.Y < this.ClientRectangle.Height))
            {
                //Coordinates of first pixel on the screen on X axis
                ScaledY = -this.AutoScrollPosition.Y + e.Y;
                OriginalX = (int)(-this.AutoScrollPosition.X / this.Zoom);
                OriginalY = (int)(ScaledY / this.Zoom);

                //Drawing with the color until start of new pixel
                brushMouse.Color = ImageProcessing.InvertColor(GetPixelMod(OriginalX, OriginalY));
                for (i = 0; i < this.ClientRectangle.Width; i++) // Drawing X cursor Line
                {
                    ScaledX = -this.AutoScrollPosition.X + i;
                    if (ScaledX % this.Zoom == 0) // Reaching new pixel and changing the brush color according to
                    {
                        OriginalX = (int)(ScaledX / this.Zoom);
                        brushMouse.Color = ImageProcessing.InvertColor(GetPixelMod(OriginalX, OriginalY));
                        BrushLenght = Math.Max((int)this.Zoom, 1); // If Zoom < 1 Draws 1 pixel, otherwise pixels are Zoom count
                        DisplayedArea.FillRectangle(brushMouse, i, e.Y, BrushLenght, 1); //Draws the equal pixels
                        i += BrushLenght - 1; // Skips the equal pixels (when zoomed)
                    }
                    else
                    {
                        DisplayedArea.FillRectangle(brushMouse, i, e.Y, 1, 1);
                    }
                }

                //Coordinates of first pixel on the screen on Y axis
                ScaledX = -this.AutoScrollPosition.X + e.X;
                OriginalX = (int)(ScaledX / this.Zoom);
                OriginalY = (int)(-this.AutoScrollPosition.Y / this.Zoom);

                //Drawing with the color until start of new pixel
                brushMouse.Color = ImageProcessing.InvertColor(GetPixelMod(OriginalX, OriginalY));
                for (i = 0; i < this.ClientRectangle.Height; i++) // Drawing X cursor Line
                {
                    ScaledY = -this.AutoScrollPosition.Y + i;
                    if (ScaledY % this.Zoom == 0) // Reaching new pixel and changing the brush color according to
                    {
                        OriginalY = (int)(ScaledY / this.Zoom);
                        brushMouse.Color = ImageProcessing.InvertColor(GetPixelMod(OriginalX, OriginalY));
                        BrushLenght = Math.Max((int)this.Zoom, 1); // If Zoom < 1 Draws 1 pixel, otherwise pixels are Zoom count
                        DisplayedArea.FillRectangle(brushMouse, e.X, i, 1, BrushLenght); //Draws the equal pixels
                        i += BrushLenght - 1; // Skips the equal pixels (when zoomed)
                    }
                    else
                    {
                        DisplayedArea.FillRectangle(brushMouse, e.X, i, 1, 1);
                    }
                }
            }
            else if (_image == null)
            {
                penMouse.Color = Color.Black;
                DisplayedArea.DrawLine(penMouse, 0, e.Y, this.ClientRectangle.Width, e.Y); //X cursor line
                DisplayedArea.DrawLine(penMouse, e.X, 0, e.X, this.ClientRectangle.Height); //Y cursor line
                DisplayedArea.DrawRectangle(penMouse, e.X - 5, e.Y - 5, 10, 10);
            }

            //Old method for drawing cursor
            //if ((0 < e.X) && (e.X < DisplayedImage.Width) && // Checking if mouse is not dragging outside the ZoomPicBox
            //    (0 < e.Y) && (e.Y < DisplayedImage.Height))
            //{
            //    for (int i = 0; i < DisplayedImage.Width; i += 5) // Drawing X cursor Line, each 5 pixels are equal to lower the flickering during redrawing cursor
            //    {
            //        brushMouse.Color = ImageProcessing.InvertColor(DisplayedImage.GetPixel(i, e.Y));
            //        DisplayedArea.FillRectangle(brushMouse, i, e.Y, 5, 1);
            //    }
            //    for (int i = 0; i < DisplayedImage.Height; i += 5) // Drawing Y cursor Line
            //    {
            //        brushMouse.Color = ImageProcessing.InvertColor(DisplayedImage.GetPixel(e.X, i));
            //        DisplayedArea.FillRectangle(brushMouse, e.X, i, 1, 5);
            //    }
            //    penMouse.Color = ImageProcessing.InvertColor(DisplayedImage.GetPixel(e.X, e.Y));
            //    DisplayedArea.DrawRectangle(penMouse, e.X - 5, e.Y - 5, 10, 10);
            //}
            //else if (_image == null)
            //{
            //    penMouse.Color = Color.Black;
            //    DisplayedArea.DrawLine(penMouse, 0, e.Y, this.ClientRectangle.Width, e.Y); //X cursor line
            //    DisplayedArea.DrawLine(penMouse, e.X, 0, e.X, this.ClientRectangle.Height); //Y cursor line
            //    DisplayedArea.DrawRectangle(penMouse, e.X - 5, e.Y - 5, 10, 10);
            //}
        }

        Color GetPixelMod(int x, int y)
        {
            if ((x < _image.Width) && (y < _image.Height))
                return _image.GetPixel(x, y);
            else
                return Color.Black;
        }

        void SetZoomPicBoxLocation()
        {
            if (MagnifierCopy != null)
            {
                MagnifierCopy.ZoomPicBoxTop = Top;
                MagnifierCopy.ZoomPicBoxLeft = Left;
                MagnifierCopy.ZoomPicBoxWidth = Width;
                MagnifierCopy.ZoomPicBoxHeight = Height;
                MagnifierCopy.ZoomPicBoxScrollX = -AutoScrollPosition.X;
                MagnifierCopy.ZoomPicBoxScrollY = -AutoScrollPosition.Y;
            }
        }

        public void LoadImage(String fileName)
        {
            try
            {
                if (this.Image != null)
                    this.Image.Dispose();
                this.Image = new Bitmap(fileName);
                if (MagnifierCopy != null)
                    MagnifierCopy.MainImage = this.Image;
                SetZoomPicBoxLocation();
                DisplayedArea = CreateGraphics();
                //UpdateDisplayedImage();
            }
            catch (Exception)
            {
                MessageBox.Show("Exception is thrown during loading image");
            }
        }

        void Pan(MouseEventArgs e)
        {
            if (isPanning)
            {
                //With previousX and previousY the direction of mouse is determined and image is panned in correct direction
                //imageX and imageY are used as coordinates for whole image, which helps the whole Image to be panned

                //IMPORTANT!!! AutoScrollPosition.X or Y always should be negative
                //when you assign positive values, it flips them to negative values, so if you check AutoScrollPosition.X,
                //it will be negative!  Assign it positive, it comes back negative
                //that is some kind of bug of ScrollableControl class
                int imageX = 0, imageY = 0; // Coordinates of cursor on Scaled image
                int panSpeedX = 1, panSpeedY = 1;

                panSpeedX = previousX - e.X;
                panSpeedY = previousY - e.Y;
                
                imageX = -this.AutoScrollPosition.X + panSpeedX;
                imageY = -this.AutoScrollPosition.Y + panSpeedY;
                this.AutoScrollPosition = new Point(imageX, imageY);
                previousX = e.X;
                previousY = e.Y;
                //DrawToBitmap(DisplayedImage, ClientRectangle); // Updates the displayed portion for cursor display
                SetZoomPicBoxLocation();

                //GC.Collect(); // temporary only
            }
        }

        private void GetPixelInfo(MouseEventArgs e)
        {
            try
            {
                float OriginalX, OriginalY;
                OriginalX = (-AutoScrollPosition.X + e.X) / zoomFactor[zoomIndex];
                OriginalY = (-AutoScrollPosition.Y + e.Y) / zoomFactor[zoomIndex];

                if ((this.Image != null) && (OriginalX < this.Image.Width) &&
                    (OriginalY < this.Image.Height) &&
                    (OriginalX >= 0) && (OriginalY >= 0))
                {
                    //Coordinates of the image will be given with X0Y starting in bottom left corner
                    CurrentState.SetState( //x, y, current color
                        Math.Round(OriginalX + 1, 4), // +1 because coordinates always begins from 0
                        Math.Round(this.Image.Height - OriginalY + 1, 4), // Invert the Y axis with Image Height
                        this.Image.GetPixel((int)OriginalX, (int)OriginalY),
                        (int)OriginalX, (int)OriginalY);
                }
                else
                {
                    CurrentState.SetState(0, 0, Color.Empty, 0, 0);
                }
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Out of memory exception");
            }
            catch (NullReferenceException)
            {
                //Do nothing, only checks if Image is loaded, that prevents the program to explode
            }
        }

        void ZoomImage(MouseEventArgs e)
        {
            float oldzoom;
            int newx, newy;

            if (this.Image != null)
            {
                oldzoom = zoomFactor[zoomIndex];
                if (e.Delta < 0) // Zoom out
                {
                    if (zoomIndex > 0)
                    {
                        zoomIndex--;
                    }
                    else
                    {
                        return;
                    }
                }
                else // Zoom in
                {
                    if (zoomIndex < zoomFactor.Length - 1)
                    {
                        zoomIndex++;
                    }
                    else
                    {
                        return;
                    }
                }

                newx = (int)Math.Round((-AutoScrollPosition.X + e.X) / oldzoom * zoomFactor[zoomIndex] - e.X);
                newy = (int)Math.Round((-AutoScrollPosition.Y + e.Y) / oldzoom * zoomFactor[zoomIndex] - e.Y);

                Zoom = zoomFactor[zoomIndex];
                AutoScrollPosition = new Point(newx, newy);
                if (MagnifierCopy != null)
                    MagnifierCopy.MainImageZoom = GetCurrentZoom();
                //DrawToBitmap(DisplayedImage, ClientRectangle); // Updates the displayed portion for cursor display
                DisplayedArea = CreateGraphics();
                DrawCursor(e);
                SetZoomPicBoxLocation();
                //GC.Collect(); // temporary only
            }
        }

        //public void UpdateDisplayedImage()
        //{
        //    if (DisplayedImage != null)
        //        DisplayedImage.Dispose();
        //    DisplayedImage = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
        //    DrawToBitmap(DisplayedImage, ClientRectangle); // Updates the displayed portion for cursor display
        //}
    } // End of class

    public class CurrentStateClass
    {
        double CurrentX, CurrentY; //Current coordinates with 4 numbers after decimal point
        Color CurrentColor;
        //public Point mouseLocation; //Current mouse location on the control

        public CurrentStateClass()
        {
            CurrentX = 0;
            CurrentY = 0;
            //_normalX = 0;
            //mouseLocation = Point.Empty;
            CurrentColor = Color.Empty;
        }

        int _originalX, _originalY;
        public int OriginalX { get { return _originalX; } }
        public int OriginalY { get { return _originalY; } }

        public void SetState(double x, double y, Color c, int originalX, int originalY)
        {
            CurrentX = x;
            CurrentY = y;
            CurrentColor = c;
            _originalX = originalX;
            _originalY = originalY;
        }

        public string GetX()
        {
            if (CurrentX != 0)
            { return CurrentX.ToString(); }
            else
            { return "None"; }
        }

        public string GetY()
        {
            if (CurrentY != 0)
            { return CurrentY.ToString(); }
            else
            { return "None"; }
        }

        public string GetR()
        {
            if (!CurrentColor.IsEmpty)
            { return CurrentColor.R.ToString(); }
            else
            { return "None"; }
        }

        public string GetG()
        {
            if (!CurrentColor.IsEmpty)
            { return CurrentColor.G.ToString(); }
            else
            { return "None"; }
        }

        public string GetB()
        {
            if (!CurrentColor.IsEmpty)
            { return CurrentColor.B.ToString(); }
            else
            { return "None"; }
        }
    } // Structure of the current state under a mouse (x, y, and color)
} // End of namespace