using CADBest.GeometryNamespace;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace QuickStichNamespace
{
    public static class RemapPixels
    {
        private const double precision = 0.000001;

        public static Color RecalcPixelColor(List<Point3D> polygon, LockBitmap bitmap, int width, int height)
        {
            if (polygon.Count != 4)
                return Color.Empty;

            int xMin = (int)Min4(polygon[0].X, polygon[1].X, polygon[2].X, polygon[3].X);
            int xMax = (int)Max4(polygon[0].X, polygon[1].X, polygon[2].X, polygon[3].X) + 1;
            int yMin = (int)Min4(polygon[0].Y, polygon[1].Y, polygon[2].Y, polygon[3].Y);
            int yMax = (int)Max4(polygon[0].Y, polygon[1].Y, polygon[2].Y, polygon[3].Y) + 1;

            double RSum = 0, GSum = 0, BSum = 0, sa = 0;
            Point3D u1 = new Point3D();
            Point3D u2 = new Point3D();
            for (int xi = xMin; xi <= xMax - 1; xi++)
            {
                for (int yi = yMin; yi <= yMax - 1; yi++)
                {
                    u1.SetCoordinates(xi, yi, 0);
                    u2.SetCoordinates(xi + 1, yi + 1, 0);
                    Color c;
                    if ((xi < 1) || (xi > width) || (yi < 1) || (yi > height))
                    {
                        c = Color.White;
                    }
                    else
                    {
                        c = bitmap.GetPixel(xi - 1, yi - 1);
                    }
                    double s = Math.Abs(Surface4x2(polygon[0], polygon[1], polygon[2], polygon[3], u1, u2));
                    sa += s;
                    RSum += c.R * s;
                    GSum += c.G * s;
                    BSum += c.B * s;
                }
            }

            if (sa == 0)
                sa = 1;
            int R = (int)Math.Round(RSum / sa);
            int G = (int)Math.Round(GSum / sa);
            int B = (int)Math.Round(BSum / sa);

            return Color.FromArgb(R, G, B);
        }

        private static List<Point3D> MakePoly(List<Point3D> points, Point3D pnt, PolySide side, Coordinate coord)
        {
            double firstOne = 0; // xu or yu
            switch (coord)
            {
                case Coordinate.X:
                    firstOne = pnt.X;
                    break;
                case Coordinate.Y:
                    firstOne = pnt.Y;
                    break;
            }

            // Count of output points
            int n = 0;
            // Count of input points
            int pn = (int)points[0].X;
            points.Add(points[1].Clone());

            List<Point3D> poly = new List<Point3D>();
            // This point is usually used for storing the count of members
            poly.Add(new Point3D());

            for (int i = 1; i <= pn; i++)
            {
                // yu or xu
                //double otherOne = CalcKoor(firstOne, points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y);
                //double statement1 = (firstOne - points[i].X);
                //double statement2 = (firstOne - points[i + 1].X);
                double otherOne = 0, statement1 = 0, statement2 = 0;
                switch (coord)
                {
                    case Coordinate.X:
                        otherOne = CalcKoor(firstOne, points[i].X, points[i].Y, points[i + 1].X, points[i + 1].Y);
                        statement1 = (firstOne - points[i].X);
                        statement2 = (firstOne - points[i + 1].X);
                        break;
                    case Coordinate.Y:
                        otherOne = CalcKoor(firstOne, points[i].Y, points[i].X, points[i + 1].Y, points[i + 1].X);
                        statement1 = (firstOne - points[i].Y);
                        statement2 = (firstOne - points[i + 1].Y);
                        break;
                }

                // Check if statement1 * statement2 < 0, in other words
                // only one of them should be negative, so XOR is used here
                if ((statement1 < 0) ^ (statement2 < 0))
                {
                    n++;
                    if (coord == Coordinate.X)
                        poly.Add(new Point3D(firstOne, otherOne, 0));
                    else
                        poly.Add(new Point3D(otherOne, firstOne, 0));
                }

                bool check2 = false;
                switch (side)
                {
                    case PolySide.Left:
                        if (coord == Coordinate.X)
                            check2 = (points[i + 1].X <= firstOne);
                        else
                            check2 = (points[i + 1].Y <= firstOne);
                        break;
                    case PolySide.Right:
                        if (coord == Coordinate.X)
                            check2 = (points[i + 1].X >= firstOne);
                        else
                            check2 = (points[i + 1].Y >= firstOne);
                        break;
                }

                if (check2)
                {
                    n++;
                    poly.Add(new Point3D(points[i + 1].X, points[i + 1].Y, 0));
                }
            }

            poly[0].X = n;
            return poly;
        }

        private static double Surface4x2(
            Point3D p1, Point3D p2, Point3D p3,
            Point3D p4, Point3D u1, Point3D u2)
        {
            List<Point3D> pr = new List<Point3D>(5);
            pr.Add(new Point3D(4, 0, 0)); // Count of elements
            pr.Add(p1);
            pr.Add(p2);
            pr.Add(p3);
            pr.Add(p4);

            pr = MakePoly(pr, u2, PolySide.Left, Coordinate.X);
            if (pr[0].X < 3)
                return 0;

            pr = MakePoly(pr, u1, PolySide.Right, Coordinate.X);
            if (pr[0].X < 3)
                return 0;

            pr = MakePoly(pr, u1, PolySide.Right, Coordinate.Y);
            if (pr[0].X < 3)
                return 0;

            pr = MakePoly(pr, u2, PolySide.Left, Coordinate.Y);
            if (pr[0].X < 3)
                return 0;

            return CalcSurfacePoly(pr);
        }

        private static double CalcSurfacePoly(List<Point3D> points)
        {
            int pn = (int)points[0].X;
            //points[pn + 1] = points[1];
            points.Add(points[1].Clone());
            double s = 0;
            for (int i = 1; i <= pn; i++)
            {
                s = s + points[i].X * points[i + 1].Y - points[i + 1].X * points[i].Y;
            }
            return s / 2.0;
        }

        /// <summary>
        /// Need summary - what is calculating?
        /// </summary>
        private static double CalcKoor(double xc, double x1, double y1, double x2, double y2)
        {
            // Check for equality of x1 and x2
            if (Math.Abs(x1 - x2) < precision)
            {
                return x1;
            }
            else
            {
                return y1 + (xc - x1) * (y2 - y1) / (x2 - x1);
            }
        }

        public static double Min4(double v1, double v2, double v3, double v4)
        {
            double min;
            min = Math.Min(v1, v2);
            min = Math.Min(min, v3);
            min = Math.Min(min, v4);
            return min;
        }

        public static double Max4(double v1, double v2, double v3, double v4)
        {
            double max;
            max = Math.Max(v1, v2);
            max = Math.Max(max, v3);
            max = Math.Max(max, v4);
            return max;
        }

        private enum PolySide
        {
            Left,
            Right
        }

        private enum Coordinate
        {
            X,
            Y
        }
    }
}
