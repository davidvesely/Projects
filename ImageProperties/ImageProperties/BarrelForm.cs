using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProperties
{
    public partial class BarrelForm : Form
    {
        public BarrelForm()
        {
            InitializeComponent();
        }

        void BarelDistorsion()
        {
            Bitmap bmp = BarrelzoomPicBox.Image;
            Bitmap newBitmap = new Bitmap(bmp.Width, bmp.Height);
            Color c1;
            //FastBitmap originalFast = new FastBitmap(bmp);
            //FastBitmap destinationFast = new FastBitmap(newBitmap);
            //originalFast.LockImage();
            //destinationFast.LockImage();          
            

            int xu, yu;      // undistorted points
            //float xd, yd;      //distorted points
            int xc, yc;      // distortion center (center of image)
            float k = 0.000001f ;           // radial distortion coefficient
            float p = 0.000001f;           //  tangential distortion coefficients
            float r;
            //float r2;
            // Find the center of distortion
            xc = bmp.Width / 2;  
            yc = bmp.Height / 2;

            for (int xd = 0; xd < bmp.Width; xd++)
            {
                for (int yd = 0; yd < bmp.Height; yd++)
                {
                    c1 = bmp.GetPixel(xd,yd);
                    r =((xd - xc) * (xd - xc) + (yd - yc) * (yd - yc)); //r^2
                   // r2 = (float)Math.Sqrt(r);
                    if (xc > xd)
                    {
                        xu = (int)Math.Round(( xd * (1 + k * r) + (p * (r + 2 * (xc - xd) * (xc - xd)))));                        
                    }
                    else// if (xc <= xd )
                    {
                        xu = (int)Math.Round(( xd * (1 + k * r) + (p * (r + 2 * (xc - xd) * (xc - xd)))));
                    }
                  

                    if (yd > yc)
                    {
                        yu = (int)Math.Round(( yd * (1 + k * r) + (p * (r + 2 * (yc - yd) * (yc - yd)))));
                    }
                    else// if (yc <= yd)
                    {
                        yu = (int)Math.Round(( yd * (1 + k * r) + (p * (r + 2 * (yc - yd) * (yc - yd)))));
                    }

                    if (xu < newBitmap.Width && yu < newBitmap.Height)
                    {
                        newBitmap.SetPixel(xu, yu, c1);
                    }               
                    //newBitmap.SetPixel(xu, yu, c1);
                  
                }
            }


            BarrelzoomPicBox.Image = newBitmap;



            //originalFast.UnlockImage();
            //destinationFast.UnlockImage();


        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BarrelzoomPicBox.LoadImage(ImageProcessing.OpenImageFile());
        }

        private void barelToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            BarelDistorsion();
        }

        private void BarrelzoomPicBox_MouseMove(object sender, MouseEventArgs e)
        {
            labelX.Text = BarrelzoomPicBox.CurrentState.GetX();
            labelY.Text = BarrelzoomPicBox.CurrentState.GetY();
            labelX.Refresh();
            labelY.Refresh();
        }
    }
}
