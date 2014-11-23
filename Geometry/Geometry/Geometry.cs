using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CADBest.GeometryNamespace
{
    /// <summary>
    /// There are stored main methods for controlling geometry
    /// </summary>
    public static class Geometry
    {
        #region Constants
        private static readonly Point3D p000 = new Point3D(0.0, 0.0, 0.0);
        private static readonly Point3D px00 = new Point3D(1.0, 0.0, 0.0);
        private static readonly Point3D p0y0 = new Point3D(0.0, 1.0, 0.0);
        private static readonly Point3D p00z = new Point3D(0.0, 0.0, 1.0);
        private static readonly Point3D pxy0 = new Point3D(1.0, 1.0, 0.0);
        private static readonly Point3D[] p0xy = new Point3D[] { p000, px00, p0y0 };
        private static readonly Point3D[] OriginAlignParametersOXY = 
            new Point3D[] { p000, p0y0, p0y0, p0y0 };
        private static readonly Point3D[] OriginAlignParametersOYX = 
            new Point3D[] { p000, px00, p0y0, new Point3D(0.0, -1.0, 0.0) };
        private static readonly Point3D[] OriginAlignParametersOZX = 
            new Point3D[] { p000, p0y0, px00, new Point3D(-1.0, 0.0, 0.0) };
        #endregion

        #region calc_rot
        /// <summary>
        /// Calculate Sin and Cos Point orientation in YZ plane for given point
        /// </summary>
        /// <returns>New Point with coordinates (Sin, Cos, 0)</returns>
        public static Point3D calc_rotX(Point3D P1)
        {
            double d, SinAlfa, CosAlfa;
            d = Math.Sqrt(P1.Z * P1.Z + P1.Y * P1.Y);
            if (d < 1e-7)
            {
                SinAlfa = 0;
                CosAlfa = 1;
            }
            else
            {
                SinAlfa = P1.Z / d;
                CosAlfa = P1.Y / d;
            }
            Point3D P1Rot = new Point3D(SinAlfa, CosAlfa, 0.0d);
            return P1Rot;
        }

        /// <summary>
        /// Calculate Sin and Cos Point orientation in XZ plane for given point
        /// </summary>
        /// <returns>New Point with coordinates (Sin, Cos, 0)</returns>
        public static Point3D calc_rotY(Point3D P1)
        {
            double d, SinAlfa, CosAlfa;
            d = Math.Sqrt(P1.X * P1.X + P1.Z * P1.Z);
            if (d < 1e-7)
            {
                SinAlfa = 0;
                CosAlfa = 1;
            }
            else
            {
                SinAlfa = - P1.Z / d; // should be with minus
                CosAlfa = P1.X / d;
            }
            Point3D P1Rot = new Point3D(SinAlfa, CosAlfa, 0.0d);
            return P1Rot;
        }

        /// <summary>
        /// Calculate Sin and Cos Point orientation in XY plane for given point
        /// </summary>
        /// <returns>New Point with coordinates (Sin, Cos, 0)</returns>
        public static Point3D calc_rotZ(Point3D P1)
        {
            double d, SinAlfa, CosAlfa;
            d = Math.Sqrt(P1.X * P1.X + P1.Y * P1.Y);
            if (d < 1e-7)
            {
                SinAlfa = 0;
                CosAlfa = 1;
            }
            else
            {
                SinAlfa = P1.Y / d;
                CosAlfa = P1.X / d;
            }
            Point3D P1Rot = new Point3D(SinAlfa, CosAlfa, 0.0d);
            return P1Rot;
        }
        #endregion

        #region p_rot
        /// <summary>
        /// Rotate point around X axis with provided Sin and Cos
        /// This method directly edits the given point
        /// </summary>
        /// <param name="P1">Point which is rotated</param>
        /// <param name="sincos">Sin and Cos Point3D object</param>
        /// <param name="Direction">Direction (-1 clockwise / +1 couter-clockwise)</param>
        public static void p_rotX(Point3D P1, Point3D sincos, double Direction)
        {
            // z'= z*CosAlfa - y*SinAlfa;
            // y'= z*SinAlfa + y*CosAlfa;  where (x′, y′) are the co-ordinates of the point after rotation

            double sin, cos, y; // y is used as temp value, because in last two rows the calculations must be performed with original values
            sin = sincos.X;
            cos = sincos.Y;
            y = P1.Y;
            P1.Y = (y * cos - Direction * P1.Z * sin); // Calcs with the original coordinate values
            P1.Z = (P1.Z * cos + Direction * y * sin); // Calcs with the original coordinate values
        }

        /// <summary>
        /// Rotate points around X axis with provided Sin and Cos
        /// This method directly edits the given points
        /// </summary>
        /// <param name="Points">List of points which are rotated</param>
        /// <param name="sincos">Sin and Cos Point3D object</param>
        /// <param name="Direction">Direction (-1 clockwise / +1 couter-clockwise)</param>
        public static void p_rotX(List<Point3D> Points, Point3D sincos, double Direction)
        {
            for (int i = 0; i < Points.Count; i++)
                p_rotX(Points[i], sincos, Direction);
        }

        /// <summary>
        /// Rotate point around Y axis with provided Sin and Cos
        /// This method directly edits the given point
        /// </summary>
        /// <param name="P1">Point which is rotated</param>
        /// <param name="sincos">Sin and Cos Point3D object</param>
        /// <param name="Direction">Direction (-1 clockwise / +1 couter-clockwise)</param>
        public static void p_rotY(Point3D P1, Point3D sincos, double Direction)
        {
            // x'= x*CosAlfa - z*SinAlfa;
            // z'= x*SinAlfa + z*CosAlfa;  where (x′, y′) are the co-ordinates of the point after rotation

            double sin, cos, x;
            sin = sincos.X;
            cos = sincos.Y;
            x = P1.X;
            // minus from X calculation should be on Z calculation, but calc_rotY should be fixed too
            // minus is fixed
            P1.X = (x * cos + Direction * P1.Z * sin); // Calcs with the original coordinate values
            P1.Z = (P1.Z * cos - Direction * x * sin); // Calcs with the original coordinate values
        }

        /// <summary>
        /// Rotate points around Y axis with provided Sin and Cos
        /// This method directly edits the given points
        /// </summary>
        /// <param name="Points">List of points which are rotated</param>
        /// <param name="sincos">Sin and Cos Point3D object</param>
        /// <param name="Direction">Direction (-1 clockwise / +1 couter-clockwise)</param>
        public static void p_rotY(List<Point3D> Points, Point3D sincos, double Direction)
        {
            for (int i = 0; i < Points.Count; i++)
                p_rotY(Points[i], sincos, Direction);
        }

        /// <summary>
        /// Rotate point around Z axis with provided sincos Point
        /// This method directly edits the given point
        /// </summary>
        /// <param name="P1">Point which is rotated</param>
        /// <param name="sincos">Sin and Cos Point3D object</param>
        /// <param name="Direction">Direction (-1 clockwise / +1 couter-clockwise)</param>
        public static void p_rotZ(Point3D P1, Point3D sincos, double Direction)
        {
            // x'= x*CosAlfa - y*SinAlfa;
            // y'= x*SinAlfa + y*CosAlfa;  where (x′, y′) are the co-ordinates of the point after rotation

            double sin, cos, x;
            sin = sincos.X;
            cos = sincos.Y;
            x = P1.X;
            P1.X = (x * cos - Direction * P1.Y * sin); // Calcs with the original coordinate values
            P1.Y = (Direction * x * sin + P1.Y * cos); // Calcs with the original coordinate values
        }

        /// <summary>
        /// Rotate points around Z axis with provided Sin and Cos
        /// This method directly edits the given points
        /// </summary>
        /// <param name="Points">List of points which are rotated</param>
        /// <param name="sincos">Sin and Cos Point3D object</param>
        /// <param name="Direction">Direction (-1 clockwise / +1 couter-clockwise)</param>
        public static void p_rotZ(List<Point3D> Points, Point3D sincos, double Direction)
        {
            for (int i = 0; i < Points.Count; i++)
                p_rotZ(Points[i], sincos, Direction);
        }
        #endregion

        #region p_scale
        public static void p_scaleX(Point3D P1, double MultValue)
        {
            P1.X *= MultValue;
        }

        public static void p_scaleY(Point3D P1, double MultValue)
        {
            P1.Y *= MultValue;
        }

        public static void p_scaleZ(Point3D P1, double MultValue)
        {
            P1.Z *= MultValue;
        }

        /// <summary>
        /// Multiply point's coordinates by given value
        /// </summary>
        /// <param name="P1">Point which is multiplied</param>
        /// <param name="MultValue">The value</param>
        /// <param name="ApplyZ">True or false, for applying the value to Z coordinate</param>
        /// <returns>Result from multiplying</returns>
        public static void p_mult(Point3D P1, double MultValue, bool ApplyZ)
        {
            p_scaleX(P1, MultValue);
            p_scaleY(P1, MultValue);
            if (ApplyZ)
                p_scaleZ(P1, MultValue);
        }

        /// <summary>
        /// Multiply point's coordinates by given value
        /// </summary>
        /// <param name="Objects">Objects with 3D points which are multiplied</param>
        /// <param name="MultValue">The value</param>
        /// <returns>Result from multiplying</returns>
        public static void p_mult(List<Point3D> Objects, double MultValue, bool ApplyZ)
        {
            for (int i = 0; i < Objects.Count; i++)
                p_mult(Objects[i], MultValue, ApplyZ);
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Create deep copy of the given list
        /// </summary>
        /// <param name="aList">Source list with points</param>
        /// <returns>New copy of the list</returns>
        public static List<Point3D> CloneList(List<Point3D> aList)
        {
            List<Point3D> newList = new List<Point3D>(
                (int)Math.Round(aList.Count * 1.4, 0)
            );

            foreach (Point3D item in aList)
            {
                newList.Add(item.Clone());
            }

            return newList;
        }

        /// <summary>
        /// Create deep copy of the given list
        /// </summary>
        /// <param name="aList">Source list with points</param>
        /// <returns>New copy of the list</returns>
        public static Point3D[] CloneList(Point3D[] aList)
        {
            Point3D[] newList = new Point3D[aList.Length];

            for (int i = 0; i < aList.Length; i++)
            {
                newList[i] = aList[i].Clone(); 
            }

            return newList;
        }

        private static List<Point3D> ReverseList(List<Point3D> aList)
        {
            List<Point3D> reversed = new List<Point3D>(aList);
            reversed.Reverse();
            return reversed;
        }

        public static void Translation(Point3D objectP, Point3D sourceP, Point3D destinationP)
        {
            double xDiff = destinationP.X - sourceP.X;
            double yDiff = destinationP.Y - sourceP.Y;
            double zDiff = destinationP.Z - sourceP.Z;
            objectP.X += xDiff;
            objectP.Y += yDiff;
            objectP.Z += zDiff;
        }

        /// <summary>
        /// Add or subtract two points
        /// This method directly edits the given point
        /// </summary>
        /// <param name="P1">First point</param>
        /// <param name="P2">Second point</param>
        /// <param name="Direction">+1 or -1</param>
        /// <returns>The result from adding or subtracting</returns>
        public static void p_add(Point3D P1, Point3D P2, double Direction)
        {
            P1.X += Direction * P2.X;
            P1.Y += Direction * P2.Y;
            P1.Z += Direction * P2.Z;
        }

        /// <summary>
        /// Add or subtract two points
        /// This method directly edits the given point
        /// </summary>
        /// <param name="P1">First point</param>
        /// <param name="P2">Second point</param>
        /// <param name="Direction">+1 or -1</param>
        /// <returns>The result from adding or subtracting</returns>
        public static void p_add(List<Point3D> P1, Point3D P2, double Direction)
        {
            for (int i = 0; i < P1.Count; i++)
                p_add(P1[i], P2, Direction);
        }

        /// <summary>
        /// Shrink point's X coordinate with a coefficient and direction
        /// This method directly edits the given point
        /// </summary>
        /// <param name="P1">Point which is shrink</param>
        /// <param name="Coef">Coefficient of shrinking</param>
        /// <param name="Direction">+1 or -1</param>
        /// <returns>Result from shrinking</returns>
        public static void p_shrinkX(Point3D P1, double Coef, double Direction)
        {
            P1.X = P1.X + (P1.Y * Direction * Coef);
        }

        public static List<Point3D> per2ln(Point3D pp, Point3D pt1, Point3D pt2)
        {
            Point3D[] sourceParams = align_calc_pars(new Point3D[] { pt1, pt2, pp });
            List<Point3D> lpp = new List<Point3D>(1);
            lpp.Add(pp.Clone());
            align(lpp, sourceParams, OriginAlignParametersOXY);
            List<Point3D> result = new List<Point3D>(2);
            result.Add(lpp[0]);
            result.Add(new Point3D(lpp[0].X, 0.0, 0.0));
            align(result, OriginAlignParametersOXY, sourceParams);
            return result;
        }

        /// <summary>
        /// Find the distance between two 3D points
        /// </summary>
        /// <param name="P1">First point</param>
        /// <param name="P2">Second point</param>
        /// <returns>The found distance</returns>
        public static double Distance(Point3D P1, Point3D P2)
        {
            return Math.Sqrt(Math.Pow(P1.X - P2.X, 2) +
                Math.Pow(P1.Y - P2.Y, 2) +
                Math.Pow(P1.Z - P2.Z, 2));
        }

        /// <summary>
        /// Calculate the intersection point between two lines
        /// or the distance between them
        /// The lines may be parallel, but should not lie in each other
        /// </summary>
        /// <param name="P11">First point from first line</param>
        /// <param name="P12">Second point from first line</param>
        /// <param name="P21">First point from second line</param>
        /// <param name="P22">Second point from second line</param>
        /// <param name="IsCrossed">True if the lines should be crossed, false for intersection</param>
        /// <returns>The intersecting point and distance between two lines, if any</returns>
        public static Point3D[] LineLineIntersect(Point3D P11, Point3D P12, Point3D P21, Point3D P22, bool IsCrossed)
        {
            Point3D[] sourceInput;
            Point3D P3 = new Point3D();
            double Zvalue;
            Point3D[] result = new Point3D[2];

            if (IsCrossed == true)
            {
                double deltaX = P21.X - P11.X;
                double deltaY = P21.Y - P11.Y;
                double deltaZ = P21.Z - P11.Z;
               // Point3D P21Copy = new Point3D(P21.X - deltaX, P21.Y - deltaY, P21.Z - deltaZ);
                Point3D P22Copy = new Point3D(P22.X - deltaX, P22.Y - deltaY, P22.Z - deltaZ);
                sourceInput = new Point3D[] { P11, P12, P22Copy };
            }
            else
               sourceInput = new Point3D[] { P11, P12, P21 };
           
            // Align the points from the second line in X0Y plane in the origin
            // of the coordinate system
            List<Point3D> alignSource = new List<Point3D>();
            alignSource.Add(P21);
            alignSource.Add(P22);
            // Clone the list, to prevent changing of the input parameters
            alignSource = CloneList(alignSource);
            Point3D[] orientationSource = align_calc_pars(sourceInput);
            align(alignSource, orientationSource, OriginAlignParametersOXY);

            Point3D pAl21 = alignSource[0];
            Point3D pAl22 = alignSource[1];

            // Check for crossed lines when in intersection mode
            if ((pAl21.Z != pAl22.Z) && !IsCrossed)
                throw new GeometryException("Lines are crossed");
           
            if (IsCrossed == true)
            {
                // Distance between the two lines
                result[1] = new Point3D(0, 0, pAl21.Z);
                Zvalue = pAl21.Z / 2;
                pAl21.Z = 0;
                pAl22.Z = 0;
                P3.Z = Zvalue;
            } 

            // Check for parallel lines
            double delimiter = pAl21.Y - pAl22.Y;
            if (Math.Abs(delimiter) < Math.Pow(Math.E, -15))
                delimiter = Math.Pow(Math.E, -15);

            // The intersection point before align back
            P3.X = (pAl22.X - pAl21.X) * (pAl21.Y / delimiter) + pAl21.X;

            // Align the intersection to its original location
            alignSource = new List<Point3D>();
            alignSource.Add(P3);
            align(alignSource, OriginAlignParametersOXY, orientationSource);

            // The aligned intersection point in its original position
            result[0] = alignSource[0];
            return result; // Return the intersection point and if lines are crossed the distance between them
        }

        /// <summary>
        ///  Intersection beetween line and plane
        /// </summary>
        /// <param name="Plane">Plane defined by 3 points </param>
        /// <param name="Line">Line defined by 2 points</param>
        /// <returns>Point3D of intersection</returns>
        public static Point3D LineToPlaneIntersect(List<Point3D> Plane, List<Point3D> Line)
        {
            List<Point3D> sourceOrientation = Geometry.align_calc_pars(Plane);
            List<Point3D> originOXYParameters = new List<Point3D>(OriginAlignParametersOXY.ToList<Point3D>());
            //Align plane and line with OXY
            Geometry.align(Line, sourceOrientation, originOXYParameters);

            Point3D result = new Point3D();

            double delimiter = Line[1].Z - Line[0].Z;
            // Check for parallel lines
            if (Math.Abs(delimiter) < Math.Pow(Math.E, -15))
                delimiter = Math.Pow(Math.E, -15);

            result.X = (Line[0].X - Line[1].X) * (Line[1].Z / delimiter) + Line[1].X;
            result.Y = (Line[0].Y - Line[1].Y) * (Line[1].Z / delimiter) + Line[1].Y;

            Line.Add(result);

            //Align back
            Geometry.align(Line, originOXYParameters, sourceOrientation);

            return Line[2];
        }

        /// <summary>
        /// Chech if point is inside polygon(or lie on it). Do not work for cross polygons!
        /// </summary>
        /// <param name="Point">Point</param>
        /// <param name="Polygon">Polygon</param>
        /// <returns>If point is inside or lie on polygon return True. If point is outside return False</returns>
        public static bool PointInsidePolygon(Point3D Point, List<Point3D> Polygon)
        {
            Geometry.p_add(Polygon, Point, -1);
            double sumAngle = 0;
            double currentAngle = 0;
            double previuosAngle = 0;
            Polygon.Add(Polygon[0]);
            double a = 0;
            Point3D zeroPoint = new Point3D(0, 0, 0);

            for (int i = 0; i < Polygon.Count; i++)
            {
                if (Polygon[i].Equals(zeroPoint)) // Point lie on polygon point
                    return true;

                Point3D currSinCos = Geometry.calc_rotZ(Polygon[i]);
                currentAngle = Geometry.CalculateAngle(currSinCos);

                a = Math.Abs(currentAngle - previuosAngle);
                if ((a > Math.PI) && (i != 0))
                    a = 2 * Math.PI - a;

                if (i != 0)
                    sumAngle += a;
                previuosAngle = currentAngle;

            }

            if (sumAngle == 2 * Math.PI)
            {
                return true;
            }
            else
                return false;

        }

        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        /// <param name="degrees">Angel value of degrees</param>
        /// <returns>Angle in radians</returns>
        public static double ConvertToRad(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        /// <param name="degrees">Angel value in radians</param>
        /// <returns>Angle in degree</returns>
        public static double ConvertToDeg(double radians)
        {
            return (Math.Round(radians * 180 / Math.PI));
        }

        /// <summary>
        /// Round Angle
        /// </summary>
        /// <param name="angle"> Angle in radians</param>
        /// <param name="degreeRound">Round Value</param>
        /// <returns>Return angle in Radians</returns>
        public static double AngleRound(double angle, int degreeRound)
        { 
            // Convert from radians to degrees
            angle = ConvertToDeg(angle);
            //Find the remainder
            int remainder = ((int)Math.Round(angle) % degreeRound);

            int difference = degreeRound - remainder;

            if (difference >= degreeRound / 2)
            {
                //Round to smallest value
                angle = ((int)angle / degreeRound) * degreeRound;
            }
            else
                //Round to bigger value
                angle = (((int)angle / degreeRound) + 1) * (degreeRound);

            // If angle equals 2*PI it is set to zero
            if (angle == 360)
                angle = 0;

            //convert back to radians
            return ConvertToRad(angle);
        }

        public static double ValueRound(double value, int valueRound)
        {         
           
            //Find the remainder
            int remainder = ((int)Math.Round(value) % valueRound);

            int difference = valueRound - remainder;

            if (difference >= valueRound / 2)
            {
                //Round to smallest value
                value = ((int)value / valueRound) * valueRound;
            }
            else
                //Round to bigger value
                value = (((int)value / valueRound) + 1) * (valueRound);
         
            return value;
        }



        /// <summary>
        /// Calculate angle with the provided Sin and Cos. From 0 to 2pi.
        /// </summary>
        /// <param name="SinCos">Sin and Cos of an angle</param>
        /// <returns>Angle in radians</returns>
        public static double CalculateAngle(Point3D SinCos)
        {
            //double sign = (SinCos.Y > 0) ? 1 : -1;
            //double angle =  Math.Asin(SinCos.X);
            //return angle;

            double angle;
            if (SinCos.X >= 0) // Sin > 0
            {
                if (SinCos.Y > 0) // Cos > 0
                {
                    angle = Math.Asin(SinCos.X); // 1 quadrant
                }
                else // Cos < 0
                {
                    angle = Math.PI - Math.Asin(SinCos.X); // 2 quadrant
                }
            }
            else // Sin < 0
            {
                if (SinCos.Y < 0) // Cos < 0
                {
                    angle = Math.PI - Math.Asin(SinCos.X); // 3 quadrant
                }
                else // Cos > 0
                {
                    angle = 2 * Math.PI + Math.Asin(SinCos.X); // 4 quadrant
                }
            }

            return angle;
        }

        /// <summary>
        /// Get the rotation angles by X, Y, Z axis of 4 points
        /// representing the coordinate system of an object
        /// </summary>
        /// <param name="sourceInput">Center, X, Y, Z points of the coordinate system of the object</param>
        /// <returns>Three Sin and Cos by X, Y, Z axis</returns>
        public static List<Point3D> GetRotationAngles(List<Point3D> sourceInput)
        {
            if (sourceInput.Count != 4)
                throw new GeometryException("Please provide exactly 4 Point3D objects for this calculation.");

            List<Point3D> source = Geometry.CloneList(sourceInput);
            Geometry.p_add(source, source[0].Clone(), -1);

            Point3D ySinCos = Geometry.calc_rotY(source[1]);
            Geometry.p_rotY(source, ySinCos, -1);

            Point3D zSinCos = Geometry.calc_rotZ(source[1]);
            Geometry.p_rotZ(source, zSinCos, -1);

            Point3D xSinCos = Geometry.calc_rotX(source[2]);
            Geometry.p_rotX(source, xSinCos, -1);

            List<Point3D> result = new List<Point3D>(3);
            result.Add(xSinCos);
            result.Add(ySinCos);
            result.Add(zSinCos);
            return result;
        }

        /// <summary>
        /// Get rotation angles of a vector (in result of that the vector will lay on X axis)
        /// </summary>
        /// <param name="vect">Given vector</param>
        /// <returns>Three Sin and Cos by X, Y, Z axis</returns>
        public static List<Point3D> GetRotationAngles(Point3D vect)
        {
            Point3D p = vect.Clone();
            Point3D ySinCos = Geometry.calc_rotY(p);
            Geometry.p_rotY(p, ySinCos, -1);
            Point3D zSinCos = Geometry.calc_rotZ(p);
            Geometry.p_rotZ(p, zSinCos, -1);
            Point3D xSinCos = Geometry.calc_rotX(p);
            Geometry.p_rotX(p, xSinCos, -1);

            List<Point3D> result = new List<Point3D>(3);
            result.Add(xSinCos);
            result.Add(ySinCos);
            result.Add(zSinCos);
            return result;
        }

        public static void RotateSinCos(Point3D vect, List<Point3D> sinCos, bool StraightOrder)
        {
            if (sinCos.Count != 3)
                throw new GeometryException("Please provide exactly three Sin and Cos values!");

            Point3D xSinCos = sinCos[0];
            Point3D ySinCos = sinCos[1];
            Point3D zSinCos = sinCos[2];
            switch (StraightOrder)
            {
                case true:
                    Geometry.p_rotY(vect, ySinCos, -1);
                    Geometry.p_rotZ(vect, zSinCos, -1);
                    Geometry.p_rotX(vect, xSinCos, -1);
                    break;
                case false:
                    Geometry.p_rotX(vect, xSinCos, 1);
                    Geometry.p_rotZ(vect, zSinCos, 1);
                    Geometry.p_rotY(vect, ySinCos, 1);
                    break;
                default:
                    break;
            }
        }

        public static T Max<T>(T x, T y, T z) where T: struct, IComparable<T>
        {
            T max = (x.CompareTo(y) > 0) ? x : y;
            return (z.CompareTo(max) > 0) ? z : max;
        }

        public static T Min<T>(T x, T y, T z) where T : struct, IComparable<T>
        {
            T max = (x.CompareTo(y) < 0) ? x : y;
            return (z.CompareTo(max) < 0) ? z : max;
        }

        public static byte Max(byte x, byte y, byte z)
        {
            return Max<byte>(x, y, z);
        }

        public static int Max(int x, int y, int z)
        {
            return Max<int>(x, y, z);
        }

        public static double Max(double x, double y, double z)
        {
            return Max<double>(x, y, z);
        }

        public static byte Min(byte x, byte y, byte z)
        {
            return Min<byte>(x, y, z);
        }

        public static int Min(int x, int y, int z)
        {
            return Min<int>(x, y, z);
        }

        public static double Min(double x, double y, double z)
        {
            return Min<double>(x, y, z);
        }
        #endregion

        #region Align
        /// <summary>Calculate orientation parameters for align</summary>
        /// <param name="PointsSource">
        /// Three points needed for calculating the orientation
        /// P1 - The origin where the point will be aligned
        /// P2 - The X axis orientation
        /// P3 - Defines the plane of aligning
        /// </param>
        /// <returns>Four orientation parameters - translation, rotation by X, Y, Z axis</returns>
        public static List<Point3D> align_calc_pars(List<Point3D> PointsSource)
        {
            return new List<Point3D>(align_calc_pars(PointsSource.ToArray()));
        }

        /// <summary>
        /// Calculate orientation parameters for align
        /// </summary>
        /// <param name="PointsSource">
        /// Three points needed for calculating the orientation
        /// P1 - The origin where the point will be aligned
        /// P2 - The X axis orientation
        /// P3 - Defines the plane of aligning
        /// </param>
        /// <returns>Four orientation parameters - translation, rotation by X, Y, Z axis</returns>
        public static Point3D[] align_calc_pars(Point3D[] PointsSource)
        {
            // PR0 is translation value
            Point3D PR0 = PointsSource[0].Clone();
            Point3D PR1, PR2, PR3;
            Point3D P0 = new Point3D(PointsSource[0]);
            Point3D P1 = new Point3D(PointsSource[1]);
            Point3D P2 = new Point3D(PointsSource[2]);

            if (PointsSource.Length != 3)
                throw new GeometryException("The count of source points for " +
                    "calculating align parameters should be 3");

            // 1st step P_ADD(Translation of points to origin)
            p_add(P0, PR0, -1d);
            p_add(P1, PR0, -1d);
            p_add(P2, PR0, -1d);

            // Calculate rotation around Z axis
            PR1 = calc_rotZ(P1);
            p_rotZ(P1, PR1, -1.0d);
            p_rotZ(P2, PR1, -1.0d);

            // Calculate rotation around Y axis
            PR2 = calc_rotY(P1);
            p_rotY(P1, PR2, -1.0d);
            p_rotY(P2, PR2, -1.0d);

            // Calculate rotation around X axis
            PR3 = calc_rotX(P2);
            p_rotX(P1, PR3, -1.0d);
            p_rotX(P2, PR3, -1.0d);
            // Now line P0P1 lie on X axis and P2 is in XY plane
            
            // Translation (X, Y, Z)
            // Sin and cos for Z axis rotation (Sin, Cos, 0)
            // Sin and cos for Y axis rotation
            // Sin and cos for X axis rotation
            Point3D[] PointsResult = new Point3D[] { PR0, PR1, PR2, PR3 };

            //Check the line is correct translate and rotate
            //PointsResult[0] = P0;
            //PointsResult[1] = P1;
            //PointsResult[2] = P2;

            //List of 3D points of orientation
            return PointsResult;
        }

        public static Point3D[] align_calc_pars(Point3D P1, Point3D P2, Point3D P3)
        {
            return align_calc_pars(new Point3D[] { P1, P2, P3 });
        }

        public static List<List<Point3D>> align_prepare(List<Point3D> PointsSource, List<Point3D> PointsDestination)
        {
            List<List<Point3D>> result = new List<List<Point3D>>();
            result.Add(align_calc_pars(PointsSource));
            result.Add(align_calc_pars(PointsDestination));
            return result;
        }

        /// <summary>
        /// Align 3D Points with provided source and destination orientation parameters
        /// </summary>
        /// <param name="Points">Object with points, which is aligned</param>
        /// <param name="SourceOrientation">Four source orientation parameters, calculated with align_calc_pars</param>
        /// <param name="DestinationOrientation">Four destination orientation parameters, calculated with align_calc_pars</param>
        public static void align(List<Point3D> Points, List<Point3D> SourceOrientation, List<Point3D> DestinationOrientation)
        {
            align(Points, SourceOrientation.ToArray(), DestinationOrientation.ToArray());
        }

        /// <summary>
        /// Align 3D Points with provided source and destination orientation parameters
        /// </summary>
        /// <param name="Points">Object with points, which is aligned</param>
        /// <param name="SourceOrientation">Four source orientation parameters, calculated with align_calc_pars</param>
        /// <param name="DestinationOrientation">Four destination orientation parameters, calculated with align_calc_pars</param>
        public static void align(List<Point3D> Points, Point3D[] SourceOrientation, Point3D[] DestinationOrientation)
        {
            List<Point3D> AlignPoints = new List<Point3D>();

            if ((SourceOrientation.Length != 4) || (DestinationOrientation.Length != 4))
                throw new GeometryException("The count of source or destination orientation points for " +
                    "align should be 4");

            Point3D PRS0 = SourceOrientation[0];
            Point3D PRS1 = SourceOrientation[1];
            Point3D PRS2 = SourceOrientation[2];
            Point3D PRS3 = SourceOrientation[3];

            Point3D PRD0 = DestinationOrientation[0];
            Point3D PRD1 = DestinationOrientation[1];
            Point3D PRD2 = DestinationOrientation[2];
            Point3D PRD3 = DestinationOrientation[3];

            for (int i = 0; i < Points.Count; i++)
            {
                Point3D pInternal = Points[i];

                // 1st step P_ADD(Translation of points to origin)
                p_add(pInternal, PRS0, -1d);
                p_rotZ(pInternal, PRS1, -1.0d);
                p_rotY(pInternal, PRS2, -1.0d);
                p_rotX(pInternal, PRS3, -1.0d);

                p_rotX(pInternal, PRD3, 1.0d);
                p_rotY(pInternal, PRD2, 1.0d);
                p_rotZ(pInternal, PRD1, 1.0d);
                p_add(pInternal, PRD0, 1d);
            }
        }

        /// <summary>
        /// Align List of objects with 3D points with provided source and destination orientation parameters
        /// </summary>
        /// <param name="Points">List of object of 3D points, which is aligned</param>
        /// <param name="SourceOrientation">Four source orientation parameters, calculated with align_calc_pars</param>
        /// <param name="DestinationOrientation">Four destination orientation parameters, calculated with align_calc_pars</param>
        public static void align(List<List<Point3D>> Points, List<Point3D> SourceOrientation, List<Point3D> DestinationOrientation)
        {
            for (int i = 0; i < Points.Count; i++)
               align(Points[i], SourceOrientation, DestinationOrientation);
        }

        /// <summary>
        /// Align List of objects with 3D points with provided source and destination orientation parameters
        /// </summary>
        /// <param name="Points">List of object of 3D points, which is aligned</param>
        /// <param name="SourceOrientation">Four source orientation parameters, calculated with align_calc_pars</param>
        /// <param name="DestinationOrientation">Four destination orientation parameters, calculated with align_calc_pars</param>
        public static void align(List<List<Point3D>> Points, Point3D[] SourceOrientation, Point3D[] DestinationOrientation)
        {
            for (int i = 0; i < Points.Count; i++)
                align(Points[i], SourceOrientation, DestinationOrientation);
        }
        #endregion

        #region Affine transformation
        /// <summary>
        /// Prepare parameters for Affine transformation: Translation (X, Y, Z),
        /// Sin and cos for X, Y, Z axis rotation
        /// </summary>                   
        /// <param name="PointsSource">Three points needed for calculating the orientation</param>
        /// <returns>Five orientation points - translation, rotation by X, Y, Z axis and shrink by X</returns>
        public static List<Point3D> affine_calc_pars(List<Point3D> PointsSource)
        {
            List<Point3D> PointsResult = new List<Point3D>();

            // PR0 is translation value
            Point3D PR0 = PointsSource[0];
            Point3D PR1, PR2, PR3;
            Point3D PR4 = new Point3D();
            Point3D P0 = new Point3D(PointsSource[0]);
            Point3D P1 = new Point3D(PointsSource[1]);
            Point3D P2 = new Point3D(PointsSource[2]);

            p_add(P0, PR0, -1);
            p_add(P1, PR0, -1);
            p_add(P2, PR0, -1);

            // Calculate rotation around Z axis
            PR1 = calc_rotZ(P1);
            p_rotZ(P1, PR1, -1.0d);
            p_rotZ(P2, PR1, -1.0d);

            // Calculate rotation around Y axis
            PR2 = calc_rotY(P1);
            p_rotY(P1, PR2, -1.0d);
            p_rotY(P2, PR2, -1.0d);

            // Calculate rotation around X axis
            PR3 = calc_rotX(P2);
            p_rotX(P1, PR3, -1.0d);
            p_rotX(P2, PR3, -1.0d);
            // Now line P0P1 lie on X axis and P2 is in XY plane

            PR4.X = P1.X;
            PR4.Y = P2.Y;

            p_mult(P1, (1d / PR4.X), false);
            p_mult(P2, (1d / PR4.X), false);
            p_scaleY(P2, (1d / PR4.Y));
            PR4.Z = P2.X;
            p_shrinkX(P2, PR4.Z, -1);

            PointsResult.Add(PR0); // Translation (X, Y, Z)
            PointsResult.Add(PR1); // Sin and cos for Z axis rotation (Sin, Cos, 0)
            PointsResult.Add(PR2); // Sin and cos for Y axis rotation
            PointsResult.Add(PR3); // Sin and cos for X axis rotation
            PointsResult.Add(PR4); // Shrink by X axis parameter

            //Check if the line is correct - translate and rotate
            //PointsResult.Add(P0);
            //PointsResult.Add(P1);
            //PointsResult.Add(P2);

            //List of 3D points of orientation
            return PointsResult;
        }

        public static List<Point3D> affine_calc_pars(Point3D P1, Point3D P2, Point3D P3)
        {
            List<Point3D> parameters = new List<Point3D>(3);
            parameters.Add(P1);
            parameters.Add(P2);
            parameters.Add(P3);
            return affine_calc_pars(parameters);
        }

        public static List<List<Point3D>> affine_prepare(List<Point3D> PointsSource, List<Point3D> PointsDestination)
        {
            List<List<Point3D>> result = new List<List<Point3D>>();
            result.Add(affine_calc_pars(PointsSource));
            result.Add(affine_calc_pars(PointsDestination));
            return result;
        }

        /// <summary>
        /// Calculate the affine transformation for the points
        /// </summary>
        /// <param name="Points">Source object (e.g. polyline)</param>
        /// <param name="SourceOrientation">Five source orientation parameters</param>
        /// <param name="DestinationOrientation">Five destination orientation parameters</param>
        /// <returns>The affine transformed source objects</returns>
        public static List<Point3D> affine_calc(List<Point3D> Points, List<Point3D> SourceOrientation, List<Point3D> DestinationOrientation)
        {
            List<Point3D> AffinePoints = new List<Point3D>();

            Point3D PRS0 = SourceOrientation[0];
            Point3D PRS1 = SourceOrientation[1];
            Point3D PRS2 = SourceOrientation[2];
            Point3D PRS3 = SourceOrientation[3];
            Point3D PRS4 = SourceOrientation[4];

            Point3D PRD0 = DestinationOrientation[0];
            Point3D PRD1 = DestinationOrientation[1];
            Point3D PRD2 = DestinationOrientation[2];
            Point3D PRD3 = DestinationOrientation[3];
            Point3D PRD4 = DestinationOrientation[4];

            foreach (Point3D currentP in Points)
            {
                Point3D pInternal = new Point3D(currentP);

                p_add(pInternal, PRS0, -1d); // 1st step P_ADD(Translation of points to origin)
                p_rotZ(pInternal, PRS1, -1.0d);
                p_rotY(pInternal, PRS2, -1.0d);
                p_rotX(pInternal, PRS3, -1.0d);

                pInternal.X *= (1d / PRS4.X);
                pInternal.Y *= (1d / PRS4.Y);
                p_shrinkX(pInternal, PRS4.Z, -1d);

                p_shrinkX(pInternal, PRD4.Z, +1d);
                pInternal.Y *= PRD4.Y;
                pInternal.X *= PRD4.X;

                p_rotX(pInternal, PRD3, +1.0d);
                p_rotY(pInternal, PRD2, +1.0d);
                p_rotZ(pInternal, PRD1, +1.0d);
                p_add(pInternal, PRD0, +1d);
                AffinePoints.Add(pInternal);
            }

            return AffinePoints;
        }
        #endregion

        #region 4-point homography
        /// <summary>
        /// Prepare parameters for 4-point transformation: Translation (X, Y, Z),
        /// Sin and cos for X, Y, Z axis rotation, Shrink by X
        /// </summary>                   
        /// <param name="PointsSource">Four base points needed for calculating the orientation</param>
        /// <returns>
        /// Eleven orientation parameters - translation, rotation by X, Y, Z axis and shrink by X
        /// </returns>
        public static List<Point3D> homography_calc_pars(List<Point3D> PointsSource)
        {
            List<Point3D> PointsResult = new List<Point3D>();

            // PR0 is translation value
            Point3D PR0 = PointsSource[0]; // ap1
            Point3D PR1, PR2, PR3, PR4, PR5, PR6;
            Point3D P0 = new Point3D(PointsSource[0]);
            Point3D P1 = new Point3D(PointsSource[1]);
            Point3D P2 = new Point3D(PointsSource[2]);
            Point3D P3 = new Point3D(PointsSource[3]);

            PR4 = new Point3D();
            PR5 = new Point3D();
            PR6 = new Point3D();

            p_add(P0, PR0, -1);
            p_add(P1, PR0, -1);
            p_add(P2, PR0, -1);
            p_add(P3, PR0, -1);

            // Calculate rotation around Z axis
            PR1 = calc_rotZ(P1);
            p_rotZ(P1, PR1, -1.0d);
            p_rotZ(P2, PR1, -1.0d);
            p_rotZ(P3, PR1, -1.0d);

            // Calculate rotation around Y axis
            PR2 = calc_rotY(P1);
            p_rotY(P1, PR2, -1.0d);
            p_rotY(P2, PR2, -1.0d);
            p_rotY(P3, PR2, -1.0d);

            // Calculate rotation around X axis
            PR3 = calc_rotX(P2);
            p_rotX(P1, PR3, -1.0d);
            p_rotX(P2, PR3, -1.0d);
            p_rotX(P3, PR3, -1.0d);
            // Now line P0P1 lie on X axis and P2 is in XY plane

            PR4.X = P1.X; // ak1
            p_mult(P1, (1d / PR4.X), false);
            p_mult(P2, (1d / PR4.X), false);
            p_mult(P3, (1d / PR4.X), false);

            PR4.Y = P2.X / P2.Y; // ak2
            p_shrinkX(P2, PR4.Y, -1);
            p_shrinkX(P3, PR4.Y, -1);
            
            PR4.Z = P2.Y; // ak3
            p_scaleY(P2, (1d / PR4.Z));
            p_scaleY(P3, (1d / PR4.Z));

            List<Point3D> PRrest = homography_calc_additional_pars(P0, P1, P2, P3);

            PointsResult.Add(PR0); // Translation (X, Y, Z)
            PointsResult.Add(PR1); // Sin and cos for Z axis rotation (Sin, Cos, 0)
            PointsResult.Add(PR2); // Sin and cos for Y axis rotation
            PointsResult.Add(PR3); // Sin and cos for X axis rotation
            PointsResult.Add(PR4); // Shrink by X parameter
            PointsResult.Add(PRrest[0]); // Sin cos for X - asc4 - PR5
            PointsResult.Add(PRrest[1]); // Sin cos for Y - asc5 - PR6
            PointsResult.Add(PRrest[2]); // Scale parameters for X - ak4, ak5 - PR7
            PointsResult.Add(PRrest[3]); // Scale parameters for Y - ak6, ak7 - PR8
            PointsResult.Add(PRrest[4]); // Parameter of translation - ap2 - PR9
            PointsResult.Add(PRrest[5]); // Parameter of translation - ap3 - PR10


            // Check if the line is correct - translate and rotate
            //PointsResult.Add(P0);
            //PointsResult.Add(P1);
            //PointsResult.Add(P2);
            //PointsResult.Add(P3);

            //List of 3D points of orientation
            return PointsResult;
        }

        /// <summary>
        /// Prepare parameters for 4-point transformation: Translation (X, Y, Z),
        /// Sin and cos for X, Y, Z axis rotation, Shrink by X
        /// </summary>                   
        /// <param name="PointsSource">Four base points needed for calculating the orientation</param>
        /// <returns>
        /// Eleven orientation parameters - translation, rotation by X, Y, Z axis and shrink by X
        /// </returns>
        public static Point3D[] homography_calc_pars(Point3D[] PointsSource)
        {
            return homography_calc_pars(new List<Point3D>(PointsSource)).ToArray();
        }

        /// <summary>
        /// Calculate additional sin and cos and scale parameters
        /// This method translates the 4th point to (1; 1)
        /// </summary>
        /// <param name="P0">Current point</param>
        /// <param name="P1">Current point</param>
        /// <param name="P2">Current point</param>
        /// <param name="P3">Current point</param>
        /// <returns>
        /// List of six parameters - (sin, cos, 0) for X and Y, (scale by X,
        /// scale by Y, 0) for X and Y axis and two translation parameters
        /// </returns>
        public static List<Point3D> homography_calc_additional_pars(Point3D P0, Point3D P1, Point3D P2, Point3D P3)
        {
            List<Point3D> result = new List<Point3D>();
            Point3D pt1 = new Point3D();
            Point3D pt2 = new Point3D();
            Point3D pt3 = new Point3D();
            Point3D pt4 = new Point3D();
            Point3D ap2 = new Point3D();
            Point3D ap3 = new Point3D();
            Point3D asc4 = new Point3D();
            Point3D asc5 = new Point3D();
            Point3D ptt1 = new Point3D();
            Point3D ptt2 = new Point3D();
            Point3D ptt3 = new Point3D();
            Point3D ptt4 = new Point3D();            

            double tz1, tz2, tz3, tz4, ak4, ak5, ak6, ak7;

            pt1.SetCoordinates(P0.X, P0.Y, 0);
            pt2.SetCoordinates(P1.X, P1.Y, 0);
            pt3.SetCoordinates(P2.X, P2.Y, 0);
            pt4.SetCoordinates(P3.X, P3.Y, 0);

            ptt4.Copy(pt4);
            p_add(ptt4, pt2, -1d);  //Translate P3 by P2 values
            asc4 = calc_rotZ(ptt4); // Calc sin cos of translated P4 point

            //Save the coordinate in ptt points
            ptt1.SetCoordinates(pt1.X, pt1.Y, pt1.Z);
            ptt2.SetCoordinates(pt2.X, pt2.Y, pt2.Z);
            ptt3.SetCoordinates(pt3.X, pt3.Y, pt3.Z);
            ptt4.SetCoordinates(pt4.X, pt4.Y, pt4.Z);
            
            //Set coordinates with X=0
            pt1.SetCoordinates(1d, pt1.Y, pt1.Z);
            pt2.SetCoordinates(1d, pt2.Y, pt2.Z);
            pt3.SetCoordinates(1d, pt3.Y, pt3.Z);
            pt4.SetCoordinates(1d, pt4.Y, pt4.Z);

            // Rotate points by Translated Sin Cos of point P1
            p_rotZ(pt1, asc4, 1d);
            p_rotZ(pt2, asc4, 1d);
            p_rotZ(pt3, asc4, 1d);
            p_rotZ(pt4, asc4, 1d);

            // Save point Y value in tz
            tz1 = pt1.Y;
            tz2 = pt2.Y;
            tz3 = pt3.Y;
            tz4 = pt4.Y;

            // Put original X value in pt.X,
            // put rotate X value in pt.Y,
            // put rotate Z value in pt.Z
            pt1.SetCoordinates(ptt1.X / tz1, pt1.X / tz1, pt1.Z);
            pt2.SetCoordinates(ptt2.X / tz2, pt2.X / tz2, pt2.Z);
            pt3.SetCoordinates(ptt3.X / tz3, pt3.X / tz3, pt3.Z);
            pt4.SetCoordinates(ptt4.X / tz4, pt4.X / tz4, pt4.Z);

            // Save translated XYZ value of first point(P0)
            ap2.SetCoordinates(pt1.X, pt1.Y, pt1.Z);
            
            // Rotate back to origin of coordinate system
            p_add(pt1, ap2, -1d);
            p_add(pt2, ap2, -1d);
            p_add(pt3, ap2, -1d);
            p_add(pt4, ap2, -1d);

            // first two parametars
            ak4 = 1d / pt2.X;
            ak5 = 1d / pt3.Y;

            //Multiply X and Y by the parametars
            pt1.SetCoordinates(pt1.X * ak4, pt1.Y * ak5, pt1.Z);
            pt2.SetCoordinates(pt2.X * ak4, pt2.Y * ak5, pt2.Z);
            pt3.SetCoordinates(pt3.X * ak4, pt3.Y * ak5, pt3.Z);
            pt4.SetCoordinates(pt4.X * ak4, pt4.Y * ak5, pt4.Z);

            //=============   Y   =============//

            //Set coordinates with Z=0
            pt1.SetCoordinates(pt1.Y, pt1.X, 0);
            pt2.SetCoordinates(pt2.Y, pt2.X, 0);
            pt3.SetCoordinates(pt3.Y, pt3.X, 0);
            pt4.SetCoordinates(pt4.Y, pt4.X, 0);

            ptt4.Copy(pt3);
            p_add(ptt4, pt4, -1d); //Translate P2 by P3 values
            asc5 = calc_rotZ(ptt4);      // Calc sin cos of translated P4 point

            //Save the coordinate in ptt points
            ptt1.SetCoordinates(pt1.X, pt1.Y, pt1.Z);
            ptt2.SetCoordinates(pt2.X, pt2.Y, pt2.Z);
            ptt3.SetCoordinates(pt3.X, pt3.Y, pt3.Z);
            ptt4.SetCoordinates(pt4.X, pt4.Y, pt4.Z);

            pt1.SetCoordinates(1d, pt1.Y, pt1.Z);
            pt2.SetCoordinates(1d, pt2.Y, pt2.Z);
            pt3.SetCoordinates(1d, pt3.Y, pt3.Z);
            pt4.SetCoordinates(1d, pt4.Y, pt4.Z);

            p_rotZ(pt1, asc5, 1d);
            p_rotZ(pt2, asc5, 1d);
            p_rotZ(pt3, asc5, 1d);
            p_rotZ(pt4, asc5, 1d);

            tz1 = pt1.Y;
            tz2 = pt2.Y;
            tz3 = pt3.Y;
            tz4 = pt4.Y;

            pt1.SetCoordinates(ptt1.X / tz1, pt1.X / tz1, pt1.Z);
            pt2.SetCoordinates(ptt2.X / tz2, pt2.X / tz2, pt2.Z);
            pt3.SetCoordinates(ptt3.X / tz3, pt3.X / tz3, pt3.Z);
            pt4.SetCoordinates(ptt4.X / tz4, pt4.X / tz4, pt4.Z);

            ap3.SetCoordinates(pt1.X, pt1.Y, pt1.Z);

            p_add(pt1, ap3, -1d);
            p_add(pt2, ap3, -1d);
            p_add(pt3, ap3, -1d);
            p_add(pt4, ap3, -1d);

            ak6 = 1d / pt3.X;
            ak7 = 1d / pt2.Y;

            pt1.SetCoordinates(pt1.X * ak6, pt1.Y * ak7, pt1.Z);
            pt2.SetCoordinates(pt2.X * ak6, pt2.Y * ak7, pt2.Z);
            pt3.SetCoordinates(pt3.X * ak6, pt3.Y * ak7, pt3.Z);
            pt4.SetCoordinates(pt4.X * ak6, pt4.Y * ak7, pt4.Z);

            pt1.SetCoordinates(pt1.Y, pt1.X, 0.0d);
            pt2.SetCoordinates(pt2.Y, pt2.X, 0.0d);
            pt3.SetCoordinates(pt3.Y, pt3.X, 0.0d);
            pt4.SetCoordinates(pt4.Y, pt4.X, 0.0d);

            Point3D ak45 = new Point3D(ak4, ak5, 0);
            Point3D ak67 = new Point3D(ak6, ak7, 0);

            result.Add(asc4); // Sin cos for X
            result.Add(asc5); // Sin cos for Y
            result.Add(ak45); // Scale parameters for X
            result.Add(ak67); // Scale parameters for Y
            result.Add(ap2); // Parameter of translation
            result.Add(ap3); // Parameter of translation

            // for test draw
            //result.Add(pt1);
            //result.Add(pt2);
            //result.Add(pt3);
            //result.Add(pt4);

            return result;
        }

        public static List<List<Point3D>> homography_prepare(List<Point3D> PointsSource, List<Point3D> PointsDestination)
        {
            List<List<Point3D>> result = new List<List<Point3D>>();
            result.Add(homography_calc_pars(PointsSource));
            result.Add(homography_calc_pars(PointsDestination));
            return result;
        }

        /// <summary>
        /// Calculate the 4-point transformation for the points
        /// </summary>
        /// <param name="Points">Source object (e.g. polyline)</param>
        /// <param name="SourceOrientation">Nine source orientation parameters</param>
        /// <param name="DestinationOrientation">Nine destination orientation parameters</param>
        /// <returns>The transformed source objects with 4-point method</returns>
        public static List<Point3D> homography_calc(List<Point3D> Points, List<Point3D> SourceOrientation, List<Point3D> DestinationOrientation)
        {
            List<Point3D> FourPResult = new List<Point3D>();

            Point3D PRS0 = SourceOrientation[0];    // PointsResult.Add(PR0);       // Translation (X, Y, Z)
            Point3D PRS1 = SourceOrientation[1];    // PointsResult.Add(PR1);       // Sin and cos for Z axis rotation (Sin, Cos, 0)
            Point3D PRS2 = SourceOrientation[2];    // PointsResult.Add(PR2);       // Sin and cos for Y axis rotation
            Point3D PRS3 = SourceOrientation[3];    // PointsResult.Add(PR3);       // Sin and cos for X axis rotation
            Point3D PRS4 = SourceOrientation[4];    // PointsResult.Add(PR4);       // Shrink by X parameter - ak....
            Point3D asc4 = SourceOrientation[5];    // PointsResult.Add(PRrest[0]); // Sin cos for X - asc4 - PR5
            Point3D asc5 = SourceOrientation[6];    // PointsResult.Add(PRrest[1]); // Sin cos for Y - asc5 - PR6
            Point3D PRS7 = SourceOrientation[7];    // PointsResult.Add(PRrest[2]); // Scale parameters for X - ak4, ak5 - PR7
            Point3D PRS8 = SourceOrientation[8];    // PointsResult.Add(PRrest[3]); // Scale parameters for Y - ak6, ak7 - PR8
            Point3D ap2 = SourceOrientation[9];     // PointsResult.Add(PRrest[4]); // Parameter of translation - ap2 - PR9
            Point3D ap3 = SourceOrientation[10];    // PointsResult.Add(PRrest[5]); // Parameter of translation - ap3 - PR10

            double ak4 = PRS7.X;
            double ak5 = PRS7.Y;
            double ak6 = PRS8.X;
            double ak7 = PRS8.Y;

            Point3D PRD0 = DestinationOrientation[0];  // Translation (X, Y, Z)
            Point3D PRD1 = DestinationOrientation[1];  // Sin and cos for Z axis rotation (Sin, Cos, 0)
            Point3D PRD2 = DestinationOrientation[2];  // Sin and cos for Y axis rotation
            Point3D PRD3 = DestinationOrientation[3];  // Sin and cos for X axis rotation
            Point3D PRD4 = DestinationOrientation[4];  // Shrink by X parameter // bk..
            Point3D bsc4 = DestinationOrientation[5];  // Sin cos for X - bsc4 - PR5
            Point3D bsc5 = DestinationOrientation[6];  // Sin cos for Y - bsc5 - PR6
            Point3D PRD7 = DestinationOrientation[7];  // Scale parameters for X - bk4, bk5 - PR7
            Point3D PRD8 = DestinationOrientation[8];  // Scale parameters for Y - bk6, bk7 - PR8
            Point3D bp2 = DestinationOrientation[9];   // Parameter of translation - bp2 - PR9
            Point3D bp3 = DestinationOrientation[10];  // Parameter of translation - bp3 - PR10 

            double bk4 = PRD7.X;
            double bk5 = PRD7.Y;
            double bk6 = PRD8.X;
            double bk7 = PRD8.Y;
            double tz;
            Point3D pt = new Point3D();
            Point3D ptt = new Point3D();

            foreach (Point3D currentP in Points)
            {
                pt.Copy(currentP);
                p_add(pt, PRS0, -1d); // 1st step P_ADD(Translation of points to origin)
                p_rotZ(pt, PRS1, -1.0d);
                p_rotY(pt, PRS2, -1.0d);
                p_rotX(pt, PRS3, -1.0d);

                p_mult(pt, (1d / PRS4.X), true);
                p_shrinkX(pt, PRS4.Y, -1d);
                p_scaleY(pt, (1d / PRS4.Z));

                // Additional actions for 4p transformation (in lisp sm_m002.lsp on Line 420)
                pt.SetCoordinates(pt.X, pt.Y, 0.0d);
                ptt.Copy(pt);
                pt.SetCoordinates(1.0d, pt.Y, pt.Z);
                p_rotZ(pt, asc4, 1.0d);
                tz = pt.Y;
                if (Math.Abs(tz) < Math.Pow(Math.E, -10))
                    tz = Math.Pow(Math.E, -10);

                pt.SetCoordinates(ptt.X / tz, pt.X / tz, pt.Z);
                p_add(pt, ap2, -1.0d);
                pt.SetCoordinates(pt.X * ak4, pt.Y * ak5, pt.Z);
                
                pt.SetCoordinates(pt.Y, pt.X, 0d);
                ptt.Copy(pt);
                pt.SetCoordinates(1d, pt.Y, pt.Z);
                p_rotZ(pt, asc5, 1d);
                tz = pt.Y;
                if (Math.Abs(tz) < Math.Pow(Math.E, -10))
                    tz = Math.Pow(Math.E, -10);

                pt.SetCoordinates(ptt.X / tz, pt.X / tz, pt.Z);
                p_add(pt, ap3, -1d);
                pt.SetCoordinates(pt.X * ak6, pt.Y * ak7 , pt.Z);
                pt.Z = 0;

                // Roll back with destination parameters
                // 4-point transformation part
                // The rollback part from line 450 to 475, sm_m002.lsp, calc_afin
                pt.SetCoordinates(pt.X / bk6, pt.Y / bk7, pt.Z);
                p_add(pt, bp3, 1d);
                ptt.Copy(pt);
                pt.SetCoordinates(1.0d, pt.Y, pt.Z);
                p_rotZ(pt, bsc5, 1d);
                tz = pt.Y;
                if (Math.Abs(tz) < Math.Pow(Math.E, -10))
                    tz = Math.Pow(Math.E, -10);

                pt.SetCoordinates(ptt.X / tz, pt.X / tz, pt.Z);
                pt.SetCoordinates(pt.Y, pt.X, 0.0d);
                if (Math.Abs(bk4) < Math.Pow(Math.E, -10))
                    bk4 = Math.Pow(Math.E, -10);

                pt.SetCoordinates(pt.X / bk4, pt.Y / bk5, pt.Z);
                p_add(pt, bp2, 1d);
                ptt.Copy(pt);
                pt.SetCoordinates(1d, pt.Y, pt.Z);
                p_rotZ(pt, bsc4, 1d);
                tz = pt.Y;
                if (Math.Abs(tz) < Math.Pow(Math.E, -10))
                    tz = Math.Pow(Math.E, -10);
                pt.SetCoordinates(ptt.X / tz, pt.X / tz, pt.Z);

                // Affine transformation back to destination
                p_scaleY(pt, PRD4.Z);
                p_shrinkX(pt, PRD4.Y, 1d);
                p_mult(pt, PRD4.X, true);

                p_rotX(pt, PRD3, 1.0d);
                p_rotY(pt, PRD2, 1.0d);
                p_rotZ(pt, PRD1, 1.0d);
                p_add(pt, PRD0, 1d);
                FourPResult.Add(new Point3D(pt));
            }

            return FourPResult;
        }

        /// <summary>
        /// Calculate the 4-point transformation for the points
        /// </summary>
        /// <param name="Points">Source object (e.g. polyline)</param>
        /// <param name="SourceOrientation">Nine source orientation parameters</param>
        /// <param name="DestinationOrientation">Nine destination orientation parameters</param>
        /// <returns>The transformed source objects with 4-point method</returns>
        public static List<Point3D> homography_calc(List<Point3D> Points, Point3D[] SourceOrientation, Point3D[] DestinationOrientation)
        {
            return homography_calc(Points, new List<Point3D>(SourceOrientation),
                new List<Point3D>(DestinationOrientation));
        }

        public static List<Point3D> homography_main(List<Point3D> Objects, Point3D[] SourcePoints, Point3D[] DestPoints)
        {
            Point3D[] hSourceParameters = homography_calc_pars(SourcePoints);
            Point3D[] hDestParameters = homography_calc_pars(DestPoints);
            return homography_calc(Objects, hSourceParameters, hDestParameters);
        }
        #endregion

        #region Perspective projection
        /// <summary>
        /// Create custom perspective projection from 3D model with the provided parameters
        /// </summary>
        /// <param name="SourceView">Three points representing the view point,
        /// center of desired projection and horizon X axis</param>
        /// <param name="DestView">Three points representing the center of destination
        /// projection and positive X and Y midpoints</param>
        /// <param name="Objects">Objects which needs to be projected</param>
        /// <returns>2D vector drawing of new projection placed at the destination center and X, Y midpoints</returns>
        public static void PerspectiveProjection(List<Point3D> SourceView, List<Point3D> DestView, List<List<Point3D>> Objects)
        {
            Point3D[] sourceViewCenter = new Point3D[] { SourceView[1], SourceView[0], SourceView[2] };
            Point3D[] sourceParam = align_calc_pars(sourceViewCenter);
            Point3D[] destParam;

            // Align Source View to Origin of coordinate system
            List<Point3D> alignedSourceView = CloneList(SourceView);
            align(alignedSourceView, sourceParam, OriginAlignParametersOYX);
            alignedSourceView[2].Y = alignedSourceView[2].Z = 0;
            alignedSourceView.Add(new Point3D(0, 0, -100));

            // Rotate the Aligned Source View points around Z axis with 90 degrees
            p_rotX(alignedSourceView, new Point3D(1, 0, 0), 1);

            // The viewpoint lying on Z axis
            double P0z = alignedSourceView[0].Z;
            //Scale factor for destination projection
            double scaleF = Distance(DestView[0], DestView[1]) / alignedSourceView[2].X;
            sourceParam = align_calc_pars(SourceView.ToArray());
            destParam = align_calc_pars(new Point3D[] { alignedSourceView[0], alignedSourceView[1], alignedSourceView[2] });

            // Align objects in the XOY origin
            align(Objects, sourceParam, destParam);

            // Project all object's points
            // Scale Current X and Y coordinate to destination projection
            Point3D currP;
            for (int i = 0; i < Objects.Count; i++)
            {
                for (int j = 0; j < Objects[i].Count; j++)
                {
                    currP = Objects[i][j];
                    if ((P0z - currP.Z) != 0)
                    {
                        currP.X = (P0z * currP.X) / ((P0z - currP.Z) * scaleF);
                        currP.Y = (P0z * currP.Y) / ((P0z - currP.Z) * scaleF);
                    }
                    else
                    {
                        currP.X = 0;
                        currP.Y = 0;
                    }
                    currP.Z = 0;
                }
            }

            // Align the projected objects to the destination plane
            sourceParam = align_calc_pars(new Point3D[] { alignedSourceView[1], alignedSourceView[2], alignedSourceView[3] });
            destParam = align_calc_pars(DestView.ToArray());

            // ***********************************************************
            // Currently the destination focus is not sended, if is needed, uncomment the following section
            // ***********************************************************
            // Add new list with focus distance(Z value must be different from 0)
            //List<Point3D> destFocus = new List<Point3D>();
            //Point3D destP0z = new Point3D(alignedSourceView[0].X, alignedSourceView[0].Y, alignedSourceView[0].Z * scaleF);
            //destFocus.Add(destP0z);
            //destFocus.Add(alignedSourceView[1]);
            //Objects.Add(destFocus);

            // Aling the objects to the new desired position
            align(Objects, sourceParam, destParam);
        }
        #endregion

        #region 6-point calibration
        private static List<Point3D> cal_6p_stuff(List<Point3D> pl1, List<Point3D> pl0, List<Point3D> pl2)
        {
            Point3D[] sourceOrientation;

            // Align the six points in projection from its (center, X, Y) points to X0Y plane
            sourceOrientation = align_calc_pars(pl0.ToArray());
            List<Point3D> pl21 = CloneList(pl2);
            align(pl21, sourceOrientation, OriginAlignParametersOXY);

            // Align the six points in the 3D object on X0Z plane
            sourceOrientation = align_calc_pars(new Point3D[] { pl1[1], pl1[0], pl1[2] });
          
            List<Point3D> pl11 = CloneList(pl1);
            align(pl11, sourceOrientation, OriginAlignParametersOZX);

            List<Point3D> pl1pr = new List<Point3D>(pl1.Count + 5);
            double fdist = pl11[0].Z;
            double k1;
            for (int i = 1; i < pl11.Count; i++)
            {
                k1 = fdist / (fdist - pl11[i].Z);
                pl1pr.Add(new Point3D(k1 * pl11[i].X, k1 * pl11[i].Y, 0));
            }

            // Homography
            List<Point3D> pl1pr1 = homography_main(pl1pr, 
                new Point3D[] { pl1pr[0], pl1pr[1], pl1pr[2], pl1pr[3] }, 
                new Point3D[] { pl21[1], pl21[2], pl21[3], pl21[4] }
            );
            List<Point3D> hUnnamed = homography_main(
                new List<Point3D>(new Point3D[] { pl1pr1[4] }),
                new Point3D[] { pl1pr1[0], pl1pr1[1], pl1pr1[2], pl1pr1[3] }, 
                new Point3D[] { pl1pr1[0], pl1pr1[1], pl1pr1[2], pl21[0] }
            );

            Point3D pt1 = pl1pr1[3];
            Point3D pt2 = hUnnamed[0];
            Point3D pt3 = LineLineIntersect(pl21[5], pl21[0], pl1pr1[4], pt2, false)[0];

            List<Point3D> pl1pr2 = homography_main(pl1pr1,
                new Point3D[] { pl1pr1[0], pl1pr1[1], pl1pr1[2], pl1pr1[4] },
                new Point3D[] { pl1pr1[0], pl1pr1[1], pl1pr1[2], pt3 }
            );

            List<Point3D> lpe0p = homography_main(
                new List<Point3D>(new Point3D[] { pl21[0], p000 }),
                new Point3D[] { pl1pr2[0], pl1pr2[1], pl1pr2[2], pl1pr2[3] },
                new Point3D[] { pl1pr[0], pl1pr[1], pl1pr[2], pl1pr[3] }
            );

            align(lpe0p, OriginAlignParametersOZX, sourceOrientation);
            lpe0p.Insert(0, pl1[0]); // ****************************************** must be optimized, because inserts the point at the begining of the list, and this causes all elements be moved with one index toward

            return lpe0p;
        }

        /// <summary>
        /// 6-Point calibration
        /// </summary>
        /// <param name="pl1">Six points from 3D object</param>
        /// <param name="pl0">Center of the projection, X and Y axis</param>
        /// <param name="pl2">The six points from the object on the projection</param>
        /// <returns></returns>
        public static List<List<Point3D>> cal_6p_trans(List<Point3D> pl1, List<Point3D> pl0, List<Point3D> pl2)
        {
            if ((pl1.Count != 6) || (pl2.Count != 6) || (pl0.Count != 3))
                throw new GeometryException("Wrong number of parameters passed to 6p transformation method!");

            List<Point3D> lp145_1 = cal_6p_stuff(pl1, pl0, pl2);
            List<Point3D> lp145_2 = cal_6p_stuff(ReverseList(pl1), 
                pl0, ReverseList(pl2));

            Point3D vp1 = LineLineIntersect(lp145_1[0], lp145_1[1], lp145_2[0], lp145_2[1], true)[0];
            Point3D[] orientSourceX0Y = align_calc_pars(new Point3D[] { vp1, lp145_1[1], lp145_1[2] });
            
            align(lp145_1, orientSourceX0Y, OriginAlignParametersOXY);
            align(lp145_2, orientSourceX0Y, OriginAlignParametersOXY);

            Point3D pt1 = lp145_2[1];
            Point3D pt2 = lp145_2[2];
            double k1 = pt1.Z / (pt1.Z - pt2.Z);
            double xt1 = pt1.X + k1 * (pt2.X - pt1.X);
            double yt1 = pt1.Y - k1 * (pt1.Y - pt2.Y);
            Point3D vp2 = new Point3D(xt1, yt1, 0.0);

            Point3D sc_z1 = calc_rotZ(vp2);
            p_rotZ(lp145_1, sc_z1, -1.0);
            p_rotZ(lp145_2, sc_z1, -1.0);

            k1 = Distance(pl0[0], pl2[0]) / Math.Abs(lp145_1[0].Y);
            Point3D[] l1 = new Point3D[3];
            l1[0] = new Point3D(k1 * lp145_1[0].X, 0.0, 0.0);
            l1[1] = new Point3D(k1 * lp145_1[0].X, k1 * lp145_1[0].Y, 0.0);
            l1[2] = new Point3D(k1 * lp145_1[0].X, 0.0, k1 * lp145_2[0].Z);

            Point3D[] orientSource = align_calc_pars(new Point3D[] { pl0[0], pl2[0], pl2[1] });
            Point3D[] orientDestin = align_calc_pars(l1);
            List<Point3D> pl01 = CloneList(pl0);
            align(pl01, orientSource, orientDestin);
            p_rotZ(pl01, sc_z1, 1.0);
            align(pl01, OriginAlignParametersOXY, orientSourceX0Y); // Align back
            orientSource = align_calc_pars(pl01.ToArray());
            orientDestin = align_calc_pars(pl0.ToArray());
            pl01.Insert(0, vp1); // ****************************************** must be optimized, because inserts the point at the begining of the list, and this causes all elements be moved with one index toward
            List<Point3D> pl02 = CloneList(pl01);
            align(pl02, orientSource, orientDestin);
            // Remove the third point, and add two new points to pl02
            pl02[2] = new Point3D(pl0[1]);
            pl02[3] = new Point3D(pl0[2]);
            pl01 = CloneList(pl02);
            align(pl01, orientDestin, orientSource);

            List<List<Point3D>> result = new List<List<Point3D>>();
            result.Add(pl01);
            result.Add(pl02);
            return result;
        }
        #endregion

        #region 4-point calibration
        /// <summary>
        /// 4-Point calibration
        /// </summary>
        /// <param name="pl1">Four coplanar points from 3D object</param>
        /// <param name="pl0">Center of the projection, X and Y axis</param>
        /// <param name="pl2">The four points from the object on the projection</param>
        /// <returns></returns>
        public static List<List<Point3D>> cal_4p_trans(List<Point3D> pl1, List<Point3D> pl0, List<Point3D> pl2, double Fdistparameter = -1)
        {
            if ((pl1.Count != 4) || (pl2.Count != 4) || (pl0.Count != 3))
                throw new GeometryException("Wrong number of parameters passed to 4p transformation method!");

            List<List<Point3D>> result = null;
            Point3D[] sourceOrientation = align_calc_pars(pl0.ToArray());
            Point3D[] destinationOrientation;
            List<Point3D> pl21 = CloneList(pl2);
            List<Point3D> pl01 = CloneList(pl0); 
            // Align pl2 and pl0 to X0Y plane with the center of projection in (0, 0)
            align(pl21, sourceOrientation, OriginAlignParametersOXY);
            align(pl01, sourceOrientation, OriginAlignParametersOXY);

            // Align pl1 to X0Y plane with first side lying on X axis
            sourceOrientation = align_calc_pars(new Point3D[] { pl1[0], pl1[1], pl1[2] });
            List<Point3D> pl11 = CloneList(pl1);
            align(pl11, sourceOrientation, OriginAlignParametersOXY);

            List<Point3D> sqrl1 = new List<Point3D>(4);
            sqrl1.Add(p000);
            sqrl1.Add(px00);
            sqrl1.Add(pxy0);
            sqrl1.Add(p0y0);

            List<Point3D> sqrl0 = homography_main(sqrl1, pl11.ToArray(), pl21.ToArray());

            Point3D pt1 = LineLineIntersect(sqrl0[0], sqrl0[1], sqrl0[2], sqrl0[3], false)[0];
            Point3D pt2 = LineLineIntersect(sqrl0[0], sqrl0[3], sqrl0[1], sqrl0[2], false)[0];

            double dAB, dAH, dBH;
            dAB = Distance(pt1, pt2);
            dAH = Distance(pt1, pl0[0]);
            dBH = Distance(pt2, pl0[0]);
            double h = Math.Sqrt(Math.Abs(Math.Pow(dAB, 2) - Math.Pow(dAH, 2) - Math.Pow(dBH, 2))) / Math.Sqrt(2);

            // Should be determined experimentally
            double d1 = Distance(pt1, pl0[0]);
            double d2 = Distance(pt2, pl0[0]);
            if ((d1 > 1e5) || (d2 > 1e5))
            {
                // TODO: should be continued
                return null;
            }
            else
            {
                List<Point3D> pl3 = per2ln(p000, pt1, pt2);
                List<Point3D> pl4 = per2ln(pt1, p000, pt2);
                Point3D pt3 = LineLineIntersect(pl3[0], pl3[1], pl4[0], pl4[1], false)[0];
                Point3D pt4 = pl3[1];
                List<Point3D> pl5 = new List<Point3D>(4);
                pl5.Add(pt4.Clone());
                pl5.Add(pl3[0]);
                pl5.Add(pt3);
                p_add(pl5, pt4, -1.0);
                Point3D sc2 = calc_rotZ(pl5[2]);
                //double testY = -1d * pl5[1].X * sc2.X + pl5[1].Y * sc2.Y;
                p_rotZ(pl5, sc2, -1.0);
                Point3D pt6 = new Point3D(pl5[2].X / 2.0, 0.0, 0.0);
                p_add(pl5, pt6, -1.0);
                double testY = pt6.X * pt6.X - pl5[1].X * pl5[1].X;
                if (testY < 0)
                    return null;
                Point3D pt7 = new Point3D(pl5[1].X, Math.Sqrt(testY), 0.0);
                pl5.Insert(0, new Point3D(pt7));
                p_rotX(pl5, px00, 1.0);

                p_add(pl5, pt6, 1.0);
                p_rotZ(pl5, sc2, 1.0);
                p_add(pl5, pt4, 1.0);

                List<Point3D> pl22 = CloneList(pl21);
                List<Point3D> pl02 = CloneList(pl01);
                List<Point3D> sqrl02 = CloneList(sqrl0);
                p_rotZ(pl22, sc2, -1.0);
                p_rotZ(pl02, sc2, -1.0);
                p_rotZ(sqrl02, sc2, -1.0);
                p_rotZ(pl5, sc2, -1.0);

                Point3D pt9 = new Point3D(0, 0, pl5[0].Z);
                p_add(pl22, pt9, -1.0);
                p_add(pl02, pt9, -1.0);
                p_add(sqrl02, pt9, -1.0);
                p_add(pl5, pt9, -1.0);

                Point3D sc3 = calc_rotY(new Point3D(-pl5[1].X, -pl5[1].Y, -pl5[1].Z));
                p_rotY(pl22, sc3, -1.0);
                p_rotY(pl02, sc3, -1.0);
                p_rotY(sqrl02, sc3, -1.0);
                p_rotY(pl5, sc3, -1.0);

                p_add(pl22, pt9, 1.0);
                p_add(pl02, pt9, 1.0);
                p_add(sqrl02, pt9, 1.0);
                p_add(pl5, pt9, 1.0);

                List<Point3D> pl1pr = new List<Point3D>(pl22.Count + 5);
                double fdist = pl5[0].Z;
                
                if ((Fdistparameter > 0) && (fdist>Fdistparameter) )
                    return null;                

                double k1;
                for (int i = 0; i < pl22.Count; i++)
                {
                    k1 = fdist / (fdist - pl22[i].Z);
                    pl1pr.Add(new Point3D(k1 * pl22[i].X, k1 * pl22[i].Y, 0.0));
                }

                List<Point3D> plspr = new List<Point3D>(sqrl02.Count + 5);
                for (int i = 0; i < pl22.Count; i++)
                {
                    k1 = fdist / (fdist - sqrl02[i].Z);
                    plspr.Add(new Point3D(k1 * sqrl02[i].X, k1 * sqrl02[i].Y, 0.0));
                }

                k1 = 1d / Distance(plspr[0], plspr[1]);
                Point3D pf1 = new Point3D(0.0, 0.0, fdist);
                p_add(pl1pr, pf1, -1.0);
                p_mult(pl1pr, k1, true);
                p_add(pl1pr, pf1, 1.0);

                sourceOrientation = align_calc_pars(pl02.ToArray());
                pl02.Insert(0, pl5[0]);
                destinationOrientation = align_calc_pars(pl0.ToArray());
                List<Point3D> pl00 = CloneList(pl02);
                align(pl00, sourceOrientation, destinationOrientation);

                sourceOrientation = align_calc_pars(new Point3D[] { pl1pr[0], pl1pr[1], pl1pr[2] });
                destinationOrientation = align_calc_pars(new Point3D[] { pl1[0], pl1[1], pl1[2] });
                align(pl02, sourceOrientation, destinationOrientation);

                if (double.IsNaN(pl02[1].X))
                    return null;

                result = new List<List<Point3D>>(2);
                result.Add(pl00);
                result.Add(pl02);
                double dist_res = Distance(pl02[0], pl02[1]);
                return result;
            }
        }
        #endregion
    } // end class

    public class GeometryException : System.ApplicationException
    {
        public GeometryException()
        {
        }

        public GeometryException(string message)
            : base(message)
        {
        }
    }
}
