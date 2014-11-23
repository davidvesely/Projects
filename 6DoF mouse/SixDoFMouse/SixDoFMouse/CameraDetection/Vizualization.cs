using CADBest.GeometryNamespace;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace SixDoFMouse.CameraDetection
{
    public static class Visualization
    {
        /// <summary>
        /// Project the big mouse descriptor on given image and draw lines on it
        /// </summary>
        /// <param name="cameraOrientation">Detected camera orientation and view point</param>
        /// <param name="sideNumber">Number of the detected side</param>
        /// <param name="centerProjXY">Center of the image, positive X and Y axis in pixel coordinates</param>
        /// <param name="img">The OpenCV image container, on which the descriptor will be projected</param>
        /// <param name="FrameRows">Height of the image, needed for converting from screen coordinates to normal geometry coordinates</param>
        public static void ProjectDescriptor(List<Point3D> cameraOrientation, List<Point3D> centerProjXY, Image<Bgr, byte> img, int FrameRows)
        {
            List<Point3D> viewPoint = new List<Point3D>(cameraOrientation.GetRange(0, 3));
            List<List<Point3D>> projectedDescriptor = new List<List<Point3D>>(14);

            foreach (Point3D[] side in MouseDataContainer.Instance.ColorSpotSides)
            {
                List<Point3D> clonedSide = new List<Point3D>(Geometry.CloneList(side));
                // Add the first point at the end so that the side is drawn as closed polyline
                clonedSide.Add(clonedSide[0].Clone());
                projectedDescriptor.Add(clonedSide);
            }

            // Add camera view point to projected objects
            projectedDescriptor.Add(Geometry.CloneList(cameraOrientation));
            // Add the X Y Z axis of descriptor
            int axisLength = 50;
            List<Point3D> axis = new List<Point3D>(2);
            // X axis
            axis.Add(new Point3D(0, 0, 0));
            axis.Add(new Point3D(axisLength, 0, 0));
            projectedDescriptor.Add(axis);
            // Y axis
            axis = new List<Point3D>(2);
            axis.Add(new Point3D(0, 0, 0));
            axis.Add(new Point3D(0, axisLength, 0));
            projectedDescriptor.Add(axis);
            // Z axis
            axis = new List<Point3D>(2);
            axis.Add(new Point3D(0, 0, 0));
            axis.Add(new Point3D(0, 0, axisLength));
            projectedDescriptor.Add(axis);
            
            Geometry.PerspectiveProjection(viewPoint, centerProjXY, projectedDescriptor);

            // Converting is needed because screen coordinate system has origin in the upper left corner
            // while Geometry works with origin of coordinate system in the left bottom corner
            ImageProcessing.ConvertXY(projectedDescriptor, FrameRows);
            DrawDescriptorLines(img, projectedDescriptor);

            //DrawPoints(img.Data, projectedDescriptor[1], FrameRows,);
        }

        public static void ProjectDescriptorPoints(List<Point3D> cameraOrientation, int sideNumber, 
            List<Point3D> centerProjXY, byte[,,] img, int FrameRows, int FrameCols, byte colorCross = 255)
        {
            List<Point3D> viewPoint = new List<Point3D>(cameraOrientation.GetRange(0, 3));
            List<List<Point3D>> projectedDescriptor = new List<List<Point3D>>(12);

            foreach (Point3D[] side in MouseDataContainer.Instance.ColorSpotSides)
            {
                List<Point3D> clonedSide = new List<Point3D>(Geometry.CloneList(side));
                projectedDescriptor.Add(clonedSide);
            }
            Geometry.PerspectiveProjection(viewPoint, centerProjXY, projectedDescriptor);

            // Converting is needed because screen coordinate system has origin in the upper left corner
            // while Geometry works with origin of coordinate system in the left bottom corner
            ImageProcessing.ConvertXY(projectedDescriptor, FrameRows);
            DrawPoints(img, projectedDescriptor[sideNumber - 1], FrameRows, FrameCols, colorCross);            
        }

        /// <summary>
        /// Project descriptor points on image
        /// </summary>
        /// <param name="cameraOrientation"></param>
        /// <param name="sideNumber"></param>
        /// <param name="centerProjXY"></param>
        /// <param name="FrameRows"></param>
        /// <returns> List with five points on image coordinate system</returns>
        public static List<Point3D> DescriptorPointsOnImage(List<Point3D> cameraOrientation, int sideNumber, List<Point3D> centerProjXY, int FrameRows)
        {
            List<Point3D> viewPoint = new List<Point3D>(cameraOrientation.GetRange(0, 3));
            List<List<Point3D>> projectedDescriptor = new List<List<Point3D>>();
            projectedDescriptor.Add(new List<Point3D>(Geometry.CloneList(MouseDataContainer.Instance.ColorSpotSides[sideNumber - 1])));
            
            Geometry.PerspectiveProjection(viewPoint, centerProjXY, projectedDescriptor);

            // Converting is needed because screen coordinate system has origin in the upper left corner
            // while Geometry works with origin of coordinate system in the left bottom corner
            ImageProcessing.ConvertXY(projectedDescriptor, FrameRows);
            
            return projectedDescriptor[0];
        }

        private static Point P1 = new Point();
        private static Point P2 = new Point(); // Helper points for drawing lines in image
        private static LineSegment2D Line = new LineSegment2D();
        private static Bgr WhiteColor = new Bgr(Color.White);
        private static Bgr RedColor = new Bgr(Color.Red);
        private static Bgr GreenColor = new Bgr(Color.Green);
        private static Bgr BlueColor = new Bgr(Color.Blue);

        /// <summary>
        /// Draws the descriptor sides on desired image,
        /// additionally coloring X Y Z axis with R G B color
        /// </summary>
        /// <param name="image"></param>
        /// <param name="objects"></param>
        private static void DrawDescriptorLines(Image<Bgr, byte> image, List<List<Point3D>> objects)
        {
            //foreach (List<Point3D> currentObj in objects)
            Bgr currentColor = WhiteColor;
            for (int i = 0; i < objects.Count; i++)
            {
                // Skip drawing the result from 4p camera calibration
                if (i == 12)
                    continue;

                for (int j = 1; j < objects[i].Count; j++)
                {
                    P1.X = (int)Math.Round(objects[i][j - 1].X);
                    P1.Y = (int)Math.Round(objects[i][j - 1].Y);

                    P2.X = (int)Math.Round(objects[i][j].X);
                    P2.Y = (int)Math.Round(objects[i][j].Y);

                    Line.P1 = P1;
                    Line.P2 = P2;
                    switch (i)
                    {
                        case 13:
                            currentColor = RedColor;
                            break;
                        case 14:
                            currentColor = GreenColor;
                            break;
                        case 15:
                            currentColor = BlueColor;
                            break;
                        default:
                            break;
                    }
                    image.Draw(Line, currentColor, 1);
                }
            }
        }

        public static void DrawPoints(byte[, ,] data, List<Point3D> Points, int FrameRows, int FrameCols, byte colorCross = 255)
        {
            List<WeightCenter> points2D = new List<WeightCenter>(Points.Count);
            foreach (Point3D currentP in Points)
            {
                points2D.Add(new WeightCenter((int)currentP.X, (int)currentP.Y, 0));
            }
            VisualizeWeightCenter(points2D, data, FrameRows, FrameCols, colorCross);
        }

        public static void VisualizeStrips(byte[, ,] destination, List<Strip> RowStrips, List<Strip> ColStrips, int rows, int cols)
        {
            foreach (Strip strip in RowStrips)
            {
                for (int i = 0; i < cols; i++)
                {
                    destination[strip.start, i, 2] = destination[strip.start, i, 1] = destination[strip.start, i, 0] = 255;
                    destination[strip.end, i, 2] = destination[strip.end, i, 1] = destination[strip.end, i, 0] = 255;
                }
            }

            foreach (Strip strip in ColStrips)
            {
                for (int i = 0; i < rows; i++)
                {
                    destination[i, strip.start, 2] = destination[i, strip.start, 1] = destination[i, strip.start, 0] = 255;
                    destination[i, strip.end, 2] = destination[i, strip.end, 1] = destination[i, strip.end, 0] = 255;
                }
            }
        }

        public static void VisualizeWeightCenter(WeightCenter w, byte[, ,] destination, int FrameRows, int FrameCols, byte colorCross = 255)
        {
            int wx, wy;
            if ((w.x > 0) && (w.y > 0))
            {
                int crossHairSize = 4;

                wx = w.x;
                wy = w.y;

                // Draw white pixels on X and Y axis
                for (int i = (-1 * crossHairSize); i < crossHairSize; i++)
                {
                    // Checks if a white cross is drawn outside the picture
                    if (((wx + i) > (FrameCols - 1)) || ((wy + i) > (FrameRows - 1)))
                        break;
                    if (((wx + i) <= 1) || ((wy + i) <= 1))
                        break;
                    // pixels are indexed as row -> col, therefore wy and wx are in this order, row = wy, col = wx
                    if((colorCross > 255) || (colorCross < 0))
                        colorCross = 255;
                    if ((wy > FrameRows-1) || (wy < 0))
                        break;
                    if ((wx > FrameCols-1) || (wx < 0))
                        break;
                    destination[wy + i, wx, 2] = destination[wy + i, wx, 1] = destination[wy + i, wx, 0] = colorCross;
                    destination[wy, wx + i, 2] = destination[wy, wx + i, 1] = destination[wy, wx + i, 0] = colorCross;
                }
            }
        }

        public static void VisualizeWeightCenter(List<WeightCenter> weights, byte[, ,] destination, int FrameRows, int FrameCols, byte colorCross = 255)
        {
            foreach (WeightCenter w in weights)
                VisualizeWeightCenter(w, destination, FrameRows, FrameCols, colorCross);
        }
    }
}
