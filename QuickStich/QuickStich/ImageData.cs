using CADBest.GeometryNamespace;
using System.Collections.Generic;
using System.Drawing;

namespace QuickStichNamespace
{
    public class ImageData
    {
        public Bitmap Image;
        public readonly string FilePath;
        public bool IsMain;
        public List<Point3D> FourPointsLeft, FourPointsRight;
        public List<Point3D> FrameHomography;
        public List<Point3D> FourPointsLeftHomography, FourPointsRightHomography;
        public int CropLeft, CropRight;

        private readonly int width, height;
        public int Width
        {
            get
            { 
                return width;
            }
        }
        public int Height
        {
            get 
            { 
                return height;
            }
        }

        public ImageData(string file, Bitmap image)
        {
            FilePath = file;
            Image = image;
            width = Image.Width;
            height = Image.Height;
        }
    }
}