using System;

namespace CADBest.GeometryNamespace
{
    /// <summary>
    /// Point in 3D space representation
    /// </summary>
    public class Point3D
    {
        public double X;
        public double Y;
        public double Z;

        /// <summary>
        /// Construct point from three coordinates
        /// </summary>
        /// <param name="aX">X coordinate</param>
        /// <param name="aY">Y coordinate</param>
        /// <param name="aZ">Z coordinate</param>
        public Point3D(double aX, double aY, double aZ)
        {
            this.SetCoordinates(aX, aY, aZ);
        }

        /// <summary>
        /// Construct point from array with three coordinates
        /// </summary>
        /// <param name="xyz">Array with three coordinates</param>
        /// <exception cref="GeometryException">If the elements in the 
        /// provided array are with different count</exception>
        public Point3D(double[] xyz)
        {
            if (xyz.Length != 3)
                throw new GeometryException("Please provide array with three coordinates!");

            this.SetCoordinates(xyz[0], xyz[1], xyz[2]);
        }

        /// <summary>
        /// Construct point cloning an existing one
        /// </summary>
        /// <param name="p">Existing point</param>
        public Point3D(Point3D p)
        {
            this.Copy(p);
        }

        /// <summary>
        /// Construct empty point
        /// </summary>
        public Point3D()
            : this(0, 0, 0)
        {
        }

        /// <summary>
        /// Modify all of point's coordinates
        /// </summary>
        /// <param name="aX">X coordinate</param>
        /// <param name="aY">Y coordinate</param>
        /// <param name="aZ">Z coordinate</param>
        public void SetCoordinates(double aX, double aY, double aZ)
        {
            this.X = aX;
            this.Y = aY;
            this.Z = aZ;
        }

        /// <summary>
        /// Set coordinates of current point, copying them from other one
        /// </summary>
        /// <param name="p">The second point</param>
        public void Copy(Point3D p)
        {
            this.SetCoordinates(p.X, p.Y, p.Z);
        }

        /// <summary>
        /// Add X, Y, Z coordinates of given point to the current
        /// </summary>
        /// <param name="p">The added point</param>
        public void Add(Point3D p)
        {
            this.X += p.X;
            this.Y += p.Y;
            this.Z += p.Z;
        }

        /// <summary>
        /// Clone the current point and create new object with the same coordinates
        /// </summary>
        /// <returns>Newly created object</returns>
        public Point3D Clone()
        {
            return new Point3D(this.X, this.Y, this.Z);
        }

        /// <summary>
        /// Get double array of point's coordinates
        /// </summary>
        /// <returns>Array with three double values</returns>
        public double[] ToArray()
        {
            return new double[] { X, Y, Z };
        }

        #region Overrides
        /// <summary>
        /// Get the string representation of a point
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", Math.Round(X, 5), Math.Round(Y, 5), Math.Round(Z, 5));
        }

        /// <summary>
        /// Predefined operators for comparing two points
        /// </summary>
        /// <param name="obj">Object of any type</param>
        /// <returns>True if the object is equal to the current point</returns>
        public override bool Equals(Object obj)
        {
            // If parameter cannot be cast to ThreeDPoint return false:
            Point3D p2 = obj as Point3D;
            if ((object)p2 == null)
            {
                return false;
            }

            // Return true if the fields match:
            return this.Equals(p2);
        }

        public bool Equals(Point3D p2)
        {
            // Return true if the fields match:
            return (this.X == p2.X) && (this.Y == p2.Y) && (this.Z == p2.Z);
        }

        public static bool operator ==(Point3D p1, Point3D p2)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(p1, p2))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)p1 == null) || ((object)p2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return p1.Equals(p2);
        }

        public static bool operator !=(Point3D p1, Point3D p2)
        {
            return !(p1 == p2);
        }

        /// <summary>
        /// Add two point's coordinates
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <returns>New point with added coordinates</returns>
        public static Point3D operator +(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        /// <summary>
        /// Subtract two point's coordinates
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <returns>New point with subtracted coordinates</returns>
        public static Point3D operator -(Point3D p1, Point3D p2)
        {
            return new Point3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Statics
        public static Point3D Empty = new Point3D();

        /// <summary>
        /// Checks whether a Point is null or empty
        /// </summary>
        /// <param name="p">Desired point</param>
        /// <returns>True if is null or empty, otherwise false</returns>
        public static bool IsNullOrEmpty(Point3D p)
        {
            if ((p == null) || (p == Empty))
                return true;
            else
                return false;
        }
        #endregion
    }
}
