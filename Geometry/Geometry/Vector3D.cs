using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CADBest.GeometryNamespace
{
    /// <summary>
    /// Class representation of a vector in 3D space
    /// </summary>
    public class Vector3D : Point3D
    {
        /// <summary>
        /// Create an empty vector
        /// </summary>
        public Vector3D()
            : base()
        {
        }


        public Vector3D(double aX, double aY, double aZ)
            : base(aX, aY, aZ)
        {
        }

        /// <summary>
        /// Create vector from RGB color
        /// </summary>
        /// <param name="c"></param>
        public Vector3D(Color c)
            : base(c.R, c.G, c.B)
        {
        }

        public Vector3D(double[] xyz)
            : base(xyz)
        {
        }

        /// <summary>
        /// Length of the vector
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(Math.Pow(this.X, 2) +
                    Math.Pow(this.Y, 2) +
                    Math.Pow(this.Z, 2));
            }
        }

        /// <summary>
        /// Normalization of the vector
        /// </summary>
        public void Normalize()
        {
            double length = this.Length;
            if (length == 0)
                length = 1e-15;
            this.X /= length;
            this.Y /= length;
            this.Z /= length;
        }

        /// <summary>
        /// Add two point's coordinates
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <returns>New point with added coordinates</returns>
        public static Vector3D operator +(Vector3D p1, Vector3D p2)
        {
            return new Vector3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }

        /// <summary>
        /// Subtract two point's coordinates
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <returns>New point with subtracted coordinates</returns>
        public static Vector3D operator -(Vector3D p1, Vector3D p2)
        {
            return new Vector3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }

        /// <summary>
        /// Multiply two vectors
        /// </summary>
        /// <param name="Vector1">First vector</param>
        /// <param name="Vector2">Second vector</param>
        /// <returns>Returns a new vector with the contents multiplied together.</returns>
        public static Vector3D operator *(Vector3D Vector1, Vector3D Vector2)
        {
            if (Vector1 == null)
                throw new ArgumentNullException("vector1 is null");
            if (Vector2 == null)
                throw new ArgumentNullException("vector2 is null");

            return new Vector3D(Vector1.X * Vector2.X, Vector1.Y * Vector2.Y, Vector1.Z * Vector2.Z);
        }

        /// <summary>
        /// Returns a vector representing the cross product of the current vector and the given one
        /// </summary>        
        /// <param name="Vector">Second Vector</param>
        /// <returns>Vector3D</returns>
        public Vector3D CrossProduct(Vector3D Vector)
        {
            if (Vector == null)
                throw new ArgumentNullException("Vector is null");
            return new Vector3D(((Y * Vector.Z) - (Z * Vector.Y)),
                                 (Z * Vector.X) - (X * Vector.Z),
                                 (X * Vector.Y) - (Y * Vector.X));
        }
     
        /// <summary>
        /// Returns the dot product of the current vector and the given one
        /// </summary>
        /// <param name="Vector">Second Vector</param>
        /// <returns>DotProduct value</returns>
        public double DotProduct(Vector3D Vector)
        {
            if (Vector == null)
                throw new ArgumentNullException("Vector is null");
            return (X * Vector.X) + (Y * Vector.Y) + (Z * Vector.Z);
        }

        /// <summary>
        /// Multiply current vector with given one
        /// </summary>
        /// <param name="Vector">second Vector</param>
        /// <returns>Multiplied vector</returns>
        public Vector3D Multiply(Vector3D Vector)
        {
            if (Vector == null)
                throw new ArgumentNullException("Vector is null");
            return new Vector3D((X * Vector.X), (Y * Vector.Y), (Z * Vector.Z));
        }

  
    }
}
