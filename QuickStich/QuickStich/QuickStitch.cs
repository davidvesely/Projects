using CADBest.GeometryNamespace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QuickStichNamespace
{
    // Width is X. Height is Y.

    public class QuickStitch
    {
        public List<Point3D> FirstPoints, SecondPoints;
        private Bitmap FirstBMP, SecondBMP;
        public ProgressBar ProgBar;
        public const int StitchPointsCount = 4;
        public bool LeftCase = false;

        public ImageData MainImageData;
        public List<ImageData> ImageDataCollection;

        public QuickStitch()
        {
            ImageDataCollection = new List<ImageData>();
            
        }

        public void AddMainImage()
        {

        }

        public Bitmap TransformPictures()
        {
            if ((FirstPoints.Count != 4) || (SecondPoints.Count != 4))
                return null;

            if (LeftCase)
            {
                List<Point3D> temp = FirstPoints;
                FirstPoints = SecondPoints;
                SecondPoints = temp;
            }

            //Step 1. Homography second bmp frame with parameters of selected points.
            List<Point3D> SourcePars = Geometry.homography_calc_pars(SecondPoints);
            List<Point3D> DestPars = Geometry.homography_calc_pars(FirstPoints);     
            List<Point3D> SecondFrameHomography = Geometry.homography_calc(GetBitmapFrame(SecondBMP), SourcePars, DestPars);
            List<Point3D> SecondPointsHomography = Geometry.homography_calc(SecondPoints, SourcePars, DestPars);

            

            //Step 2
            //Create new frame around distorted old frame. It will be used for creating new bmp.
            List<Point3D> SecondNewRetangle = PolygonToRectangle(SecondFrameHomography);
            // Crop frame to selected points
            int index;
            if (LeftCase)
            {
                index = CropFirstBmpPoint(SecondPointsHomography);
                SecondNewRetangle[2].X = SecondPointsHomography[index].X;
                SecondNewRetangle[3].X = SecondPointsHomography[index].X;
            }
            else
            {
                index = CropSecondBmpPoint(SecondPointsHomography);
                SecondNewRetangle[0].X = SecondPointsHomography[index].X;
                SecondNewRetangle[1].X = SecondPointsHomography[index].X;
            }            
            
            //Translate new frame to origin of coordinate system
            Point3D Translation = SecondNewRetangle[0].Clone();
            Geometry.p_add(SecondNewRetangle, Translation, -1);
            Geometry.p_add(SecondFrameHomography, Translation, -1);
            Geometry.p_add(SecondPointsHomography, Translation, -1);

            RoundFrame(SecondNewRetangle);
            Bitmap SecondNewBmp = new Bitmap((int)SecondNewRetangle[2].X, (int)SecondNewRetangle[2].Y);

            //Convert pixel to CadBest Points3D.
            List<List<Point3D>> SecondBMPList = BmpToList(SecondNewBmp);
            List<List<Point3D>> HomographySecondToFirst = HomographyGrid(SecondBMPList, SecondFrameHomography, GetBitmapFrame(SecondBMP));

            RecalculateColor(SecondBMP, SecondNewBmp, HomographySecondToFirst);          

            // Translate second frame and four points (homography)
            // and align them with first frame and four points
            Point3D CommonSecond = SecondPointsHomography[0].Clone();
            Geometry.p_add(SecondNewRetangle, CommonSecond, -1.0);
            Point3D CommonFirst = FirstPoints[0].Clone();
            Geometry.p_add(SecondNewRetangle, CommonFirst, 1.0);

            // Create common frame of the result bitmap
            List<Point3D> commonRectangle = new List<Point3D>();
            commonRectangle.AddRange(GetBitmapFrame(FirstBMP));
            commonRectangle.AddRange(SecondNewRetangle);
            List<Point3D> CommonFrame = PolygonToRectangle(commonRectangle);

            // Translate the left upper corner of snapped frame to (0; 0)
            Point3D OriginCommonFrame = CommonFrame[0].Clone();
            Geometry.p_add(CommonFrame, OriginCommonFrame, -1.0);
            // The reference points, that corresponds to first of
            // the four picked points in first original image and
            // second homograph image
            Point3D CommonPoint = FirstPoints[0].Clone();
            Geometry.p_add(CommonPoint, OriginCommonFrame, -1.0);

            // Fill with pixels
            Bitmap CommonBmp = new Bitmap((int)CommonFrame[2].X, (int)CommonFrame[2].Y);

            TranslatePixel(SecondNewBmp, CommonBmp, CommonSecond, CommonPoint, false);
            TranslatePixel(FirstBMP, CommonBmp, CommonFirst, CommonPoint, true);
                                           
            // Display the result bmp in new window
            FormResult res = new FormResult();
            res.SetImage(CommonBmp);
            res.Show();

            return SecondNewBmp;
        }

        /// <summary>
        /// Round corners coordinate of frame.
        /// </summary>
        /// <param name="frame">Frame</param>
        private void RoundFrame(List<Point3D> frame)
        {
            //Create new BMP from new frame.
            int widthSecond = (int)Math.Ceiling(frame[2].X);
            int heightSecond = (int)Math.Ceiling(frame[2].Y);
            // Round the four corner's coordinates
            frame[1].Y = heightSecond;
            frame[2].X = widthSecond;
            frame[2].Y = heightSecond;
            frame[3].X = widthSecond;
        }

        /// <summary>
        /// Homography of list of list of Points3D.
        /// </summary>
        /// <param name="GridBmp"></param>
        /// <param name="SourceFrame"></param>
        /// <param name="DestFrame"></param>
        /// <returns></returns>
        public List<List<Point3D>> HomographyGrid(List<List<Point3D>> GridBmp, List<Point3D> SourceFrame, List<Point3D> DestFrame)
        {
            //Step 3. Homography back new bmp to first picture. Distorted frame to regular frame of first bmp.
            List<Point3D> SourcePars = Geometry.homography_calc_pars(SourceFrame);
            List<Point3D> DestPars = Geometry.homography_calc_pars(DestFrame); //GetBitmapFrame
            List<List<Point3D>> HomographySecondToFirst = new List<List<Point3D>>();
            foreach (List<Point3D> curr in GridBmp)
            {
                HomographySecondToFirst.Add(Geometry.homography_calc(curr, SourcePars, DestPars));
            }
            return HomographySecondToFirst;
        }

        private void RecalculateColor(Bitmap Original, Bitmap Destination, List<List<Point3D>> HomographySecondToFirst)
        {
            //Recalculate new color of the pixel.
            LockBitmap SecondBmpLock = new LockBitmap(Original);
            SecondBmpLock.LockBits();
            LockBitmap SecondBmpResultLock = new LockBitmap(Destination);
            SecondBmpResultLock.LockBits();

            int widthSecondOriginal = Original.Width;
            int heightSecondOriginal = Original.Height;
            ProgBar.Maximum = HomographySecondToFirst.Count;
            ProgBar.Minimum = 0;
            ProgBar.Step = 1;
            ProgBar.Value = 0;
            for (int j = 0; j < HomographySecondToFirst.Count - 2; j++)
            {
                for (int i = 0; i < HomographySecondToFirst[j].Count - 2; i++)
                {
                    //Surrounded pixel from Points
                    List<Point3D> currList = new List<Point3D>();
                    currList.Add(HomographySecondToFirst[j][i]);
                    currList.Add(HomographySecondToFirst[j][i + 1]);
                    currList.Add(HomographySecondToFirst[j + 1][i + 1]);
                    currList.Add(HomographySecondToFirst[j + 1][i]);

                    // Only if the distorted polygon is inside the bitmap
                    if (!CompareTwoRetangle(currList, widthSecondOriginal, heightSecondOriginal))
                    {
                        Color c = RemapPixels.RecalcPixelColor(currList, SecondBmpLock, widthSecondOriginal, heightSecondOriginal);
                        SecondBmpResultLock.SetPixel(i, j, c);
                    }
                }
                ProgBar.PerformStep();
            }

            SecondBmpLock.UnlockBits();
            SecondBmpResultLock.UnlockBits();

        }

        /// <summary>
        /// Calculate the X value of new end of frame.
        /// </summary>
        /// <param name="FourPoints">Selected Four points</param>
        /// <returns>Index of point</returns>
        private int CropFirstBmpPoint(List<Point3D> FourPoints)
        {
            double max = FourPoints[0].X;
            int index = 0;
            for (int i = 0; i < FourPoints.Count; i++)
            {
                if (max <= FourPoints[i].X)
                {
                    max = FourPoints[i].X;
                    index = i;
                }
            }
            return index;       
        }

        /// <summary>
        /// Calculate X value of new origin frame.
        /// </summary>
        /// <param name="FourPoints">Selected Four points</param>
        /// <returns>Index of list</returns>
        private int CropSecondBmpPoint(List<Point3D> FourPoints)
        {
            double min = FourPoints[0].X;
            int index = 0;
            for (int i = 0; i < FourPoints.Count; i++)
            {
                if (min >= FourPoints[i].X)
                {
                    min = FourPoints[i].X;
                    index = i;
                }
            }
            return index;
        }

        /// <summary>
        /// Translate pixel from one picture to other.
        /// </summary>
        /// <param name="Source">Main Image</param>
        /// <param name="Common">New Image</param>
        /// <param name="SoureRef">Point in main image</param>
        /// <param name="CommonRef">Same point in new image(It should be in different location, but is not necessary)</param>
        /// <param name="first">True for first bmp. False if image is second bmp.</param>
        private void TranslatePixel(Bitmap Source, Bitmap Common, Point3D SoureRef, Point3D CommonRef, bool first)
        {
            LockBitmap SourceLock = new LockBitmap(Source);
            LockBitmap CommonLock = new LockBitmap(Common);            
            SourceLock.LockBits();           
            CommonLock.LockBits();
            int width;
            int start;
            if (first)
            {
                int index;
                if (LeftCase)
                {
                    index = CropSecondBmpPoint(FirstPoints);
                    start = (int)FirstPoints[index].X + 1;
                    width = SourceLock.Width;
                }
                else
                {
                    index = CropFirstBmpPoint(FirstPoints);
                    width = (int)FirstPoints[index].X + 1;
                    start = 0;
                }                
            }
            else
            {

                width = SourceLock.Width;
                start = 0;
            }

                        
            int xDiff = (int)(SoureRef.X - CommonRef.X);
            int yDiff = (int)(SoureRef.Y - CommonRef.Y);
            for (int i = start; i < width; i++)
            {
                for (int j = 0; j < SourceLock.Height; j++)
                {
                    Color c = SourceLock.GetPixel(i, j);
                    CommonLock.SetPixel(i - xDiff, j - yDiff, c);
                }
            }
            SourceLock.UnlockBits();
            CommonLock.UnlockBits();
        }

        /// <summary>
        /// Create rectangle around four point polygon. 
        /// </summary>
        /// <param name="FourPoints">Polygon of any count of points</param>
        /// <returns>List of Point3D</returns>
        public List<Point3D> PolygonToRectangle(List<Point3D> FourPoints)
        {
            if ((FourPoints == null) || (FourPoints.Count == 0))
                return null;

            double RightX, LeftX, UpY, DownY;
            LeftX = RightX = FourPoints[0].X;
            UpY = DownY = FourPoints[0].Y;

            foreach (Point3D p in FourPoints)
            {
                if (RightX < p.X)
                    RightX = p.X;
                if (LeftX > p.X)
                    LeftX = p.X;

                if (DownY < p.Y)
                    DownY = p.Y;
                if (UpY > p.Y)
                    UpY = p.Y;
            }

            List<Point3D> result = new List<Point3D>();
            result.Add(new Point3D(LeftX, UpY, 0));
            result.Add(new Point3D(LeftX, DownY, 0));
            result.Add(new Point3D(RightX, DownY, 0));
            result.Add(new Point3D(RightX, UpY, 0));

            return result;
        }

        /// <summary>
        /// Compare if four points polygon is inside or outside in Rectangle(0,0, width, height)
        /// </summary>
        /// <param name="FourPoints">Four points</param>
        /// <param name="width">The width of rectangle</param>
        /// <param name="height">The height of rectangle</param>
        /// <returns>True if is outside. False if is inside</returns>
        public bool CompareTwoRetangle(List<Point3D> FourPoints, int width, int height)
        {
            double RightX = RemapPixels.Max4(FourPoints[0].X, FourPoints[1].X, FourPoints[2].X, FourPoints[3].X);
            double LeftX = RemapPixels.Min4(FourPoints[0].X, FourPoints[1].X, FourPoints[2].X, FourPoints[3].X);
            double UpY = RemapPixels.Min4(FourPoints[0].Y, FourPoints[1].Y, FourPoints[2].Y, FourPoints[3].Y);
            double DownY = RemapPixels.Max4(FourPoints[0].Y, FourPoints[1].Y, FourPoints[2].Y, FourPoints[3].Y);

            if ((LeftX < 0) || (RightX > width) || (UpY < 0) || (DownY > height))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get list of Points3D with coordinate of image picture.
        /// </summary>
        /// <param name="bmp">Image</param>
        /// <returns>List of Points3D</returns>
        private List<Point3D> GetBitmapFrame(Bitmap bmp)
        {
            List<Point3D> bitmapFrame = new List<Point3D>();
            bitmapFrame.Add(new Point3D(0, 0, 0));
            bitmapFrame.Add(new Point3D(0, bmp.Height, 0));
            bitmapFrame.Add(new Point3D(bmp.Width, bmp.Height, 0));
            bitmapFrame.Add(new Point3D(bmp.Width, 0, 0));
            return bitmapFrame;
        }

        /// <summary>
        /// Convert Bmp to List of Points3D.  Lists number is height(rows) of bmp. Every list size is wight of bmp(cols).
        /// </summary>
        /// <param name="bmp">Bitmap image</param>
        /// <returns>List of list of Point3D</returns>
        private List<List<Point3D>> BmpToList(Bitmap bmp)
        {
            List<List<Point3D>> result = new List<List<Point3D>>();
            // Fill the array with points, which refers to
            // each pixel corner (e.g. width = 300px,
            // fill the array with 301 pixel corners, same for height)
            for (int i = 0; i < bmp.Height + 1; i++) //Y rows
            {
                List<Point3D> curr = new List<Point3D>(SecondBMP.Width);
                for (int j = 0; j < bmp.Width + 1; j++)// X cols
                {
                    curr.Add(new Point3D(j, i, 0));// i=rows, j=cols
                }
                result.Add(curr);
            }
            return result;
        }

        /// <summary>
        ///  Transfer point X value from Line to Curve(Circle).
        /// </summary>
        /// <param name="BmpGrid">List of Point3D of image</param>
        /// <param name="radius">Curve Radius</param>
        /// <param name="CenterProjection">Tangent point of line and curve(or perpendicular point from center of the circle to line).</param>
        private void LineToCurve(List<List<Point3D>> BmpGrid, double radius, Point3D CenterProjection)
        {           
            foreach (List<Point3D> currList in BmpGrid)
            {
                foreach (Point3D p in currList)
                {
                    Geometry.LineToCurve(p, radius, CenterProjection);                
                }
            }
        }

        /// <summary>
        ///  Transfer point X value from Curve(Circle) to Line.
        /// </summary>
        /// <param name="BmpGrid">List of Point3D of image</param>
        /// <param name="radius">Curve Radius</param>
        /// <param name="CenterProjection">Tangent point of line and curve(or perpendicular point from center of the circle to line).</param>
        private void CurveToLine(List<List<Point3D>> BmpGrid, double radius, Point3D CenterProjection)
        {
            foreach (List<Point3D> currList in BmpGrid)
            {
                foreach (Point3D p in currList)
                {
                    Geometry.CurveToLine(p, radius, CenterProjection);
                }
            }
        }
    }
}
