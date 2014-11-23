using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SixDoFMouse.AlphaForm2
{
    class PictureButton : PictureBox
    {
        public PictureButton() : base()
        {
            this.Margin = new Padding(0);
        }

        private Image _defaultImage;
        public Image DefaultImage
        {
            get { return _defaultImage; }
            set 
            { 
                _defaultImage = Image = value;
                this.Size = _defaultImage.Size;
            }
        }
        public Image PressedImage { get; set; }
        public Boolean IsSingleButton { get; set; }
        public bool isPressed = false;

        public void TurnOff()
        {
            isPressed = false;
            this.Image = DefaultImage;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            isPressed = !isPressed;
            switch (isPressed)
            {
                case true:
                    this.Image = PressedImage;
                    break;
                case false:
                    this.Image = DefaultImage;
                    break;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (IsSingleButton)
            {
                this.Image = DefaultImage;
                isPressed = false;
            }
            base.OnMouseUp(e);
        }
    }
}
