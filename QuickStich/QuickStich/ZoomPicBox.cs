using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QuickStichNamespace
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
        float[] zoomFactor = { 0.01f, 0.03f, 0.05f, 0.08f, 0.09f, .1f, .2f, .3f, .4f, .5f, .7f, .8f, 
                                       1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f, 5f, 6f, 8f, 
                                       10f, 12f, 15f, 20f, 35f, 40f, 45f, 50f, 55f, 60f };
        int zoomIndex = 12;

        //These are used for drawing cursor
        Pen penMouse = new System.Drawing.Pen(Color.Blue, 1.5F); //Pen for drawing the cursor
        SolidBrush brushMouse = new SolidBrush(Color.Blue); //Brush for drawing a single pixel of cursor lines
        //Bitmap DisplayedImage = null; //Current portion of displayed image
        Graphics DisplayedArea = null; //This is used for drawing custom cursor

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

        private string _fileName = null;
        public string FileName
        {
            get { return _fileName; }
        }

        private Bitmap _image;

        [Category("Appearance"), Description("The image to be displayed")]
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
        
        [Category("Appearance"), Description("The zoom factor. Less than 1 to reduce. More than 1 to magnify.")]
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

        [Category("Appearance"), Description("The interpolation mode used to smooth the drawing")]
        public InterpolationMode InterpolationMode
        {
            get { return _interpolationMode; }
            set { _interpolationMode = value; }
        }


        protected override void OnPaintBackground(PaintEventArgs prevent)
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
            e.Graphics.DrawImage(_image, 
                new Rectangle(0, 0, this._image.Width, this._image.Height), 
                0, 0, _image.Width, _image.Height, GraphicsUnit.Pixel);

            base.OnPaint(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (this.Parent.ContainsFocus)
            {
                this.Focus();
            }

            // Update for case, when Picture box is placed inside panel
            // to maintain handling of scroll event
            if ((this.Parent is Panel) && (this.Parent.Parent.ContainsFocus))
            {
                this.Focus();
            }

            base.OnMouseEnter(e);
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
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            SetPanMode(false);
            base.OnMouseUp(e);
        }

        protected override void OnResize(EventArgs e)
        {
            DisplayedArea = CreateGraphics();
            base.OnResize(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            ZoomImage(e);
            // The next line is not called, because in addition to
            // zooming scrolls the picture down or up
            // base.OnMouseWheel(e);
        }

        public ZoomPicBox()
        {
            //Double buffer the control
            // Prevents flickering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.ResizeRedraw |
              ControlStyles.UserPaint |
              ControlStyles.DoubleBuffer, true);

            SetPanMode(false); //Pan mode is set ON only on MouseDown event
            CurrentState = new CurrentStateClass(); // Initialize current state (x, y, and color of the pixel)
        }

        Color GetPixelMod(int x, int y)
        {
            if ((x < _image.Width) && (y < _image.Height))
                return _image.GetPixel(x, y);
            else
                return Color.Black;
        }

        public void LoadImage(String fileName)
        {
            try
            {
                if (this.Image != null)
                    this.Image.Dispose();
                this.Image = new Bitmap(fileName);
                _fileName = fileName;
                DisplayedArea = CreateGraphics();
            }
            catch (Exception)
            {
                MessageBox.Show("Exception is thrown during loading image");
            }
        }

        void Pan(MouseEventArgs e)
        {
            if ((isPanning) && (e.Button != MouseButtons.Left) && (e.Button != MouseButtons.Right))
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
                DisplayedArea = CreateGraphics();
            }
        }
    } // End of class

    public class CurrentStateClass
    {
        double CurrentX, CurrentY; //Current coordinates with 4 numbers after decimal point
        Color CurrentColor;

        public CurrentStateClass()
        {
            CurrentX = 0;
            CurrentY = 0;
            CurrentColor = Color.Empty;
        }

        private int _originalX, _originalY;
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
            return _originalX.ToString();
        }

        public string GetY()
        {
            return _originalY.ToString();
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