using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CADBest.GeometryNamespace
{
    public static class ConicSections
    {
        // A value close to 0.
        private const double SMALL = 1E-8;
        private const int num_tests = 100;

        // We suppose that the source points are provided in X0Y plane
        public static List<List<Point3D>> DrawConicSection(double[] conicParameters, double xminInit, double xmaxInit)
        {
            // Get the X coordinate bounds
            double xmin = xminInit, xmax = xmaxInit;
            double ymin = xminInit / 3, ymax = xmaxInit * 3;

            double A, B, C, D, E, F;
            A = conicParameters[0];
            B = conicParameters[1];
            C = conicParameters[2];
            D = conicParameters[3];
            E = conicParameters[4];
            F = conicParameters[5];

            // Find the smallest X coordinate with a real value.
            for (double x = xminInit; x < xmaxInit; x += 0.1)
            {
                double y = G1(x, A, B, C, D, E, F, -1f);
                if (IsNumber(y))
                {
                    xmin = x;
                    break;
                }
            }

            // Find the largest X coordinate with a real value.
            for (double x = xmaxInit; x > xmin; x -= 0.1)
            {
                double y = G1(x, A, B, C, D, E, F, -1f);
                if (IsNumber(y))
                {
                    xmax = x;
                    break;
                }
            }

            // Get points for the negative root on the left.
            List<Point3D> ln_points = new List<Point3D>();
            double xmid1 = xmax;
            for (double x = xmin; x <= xmax; x++)
            {
                double y = G1(x, A, B, C, D, E, F, -1f);
                if (!IsNumber(y))
                {
                    xmid1 = x - 1;
                    break;
                }

                if ((y < ymin) || (y > ymax))
                    continue;
                ln_points.Add(new Point3D(x, y, 0));
            }

            // Get points for the positive root on the left.
            List<Point3D> lp_points = new List<Point3D>();
            for (double x = xmid1; x >= xmin; x--)
            {
                double y = G1(x, A, B, C, D, E, F, +1f);
                if (IsNumber(y))
                {
                    if ((y < ymin) || (y > ymax))
                        continue;
                    lp_points.Add(new Point3D(x, y, 0));
                }
            }

            // Make the curves on the right if needed.
            List<Point3D> rp_points = new List<Point3D>();
            List<Point3D> rn_points = new List<Point3D>();
            double xmid2 = xmax;
            if (xmid1 < xmax)
            {
                // Get points for the positive root on the right.
                for (double x = xmax; x >= xmid1; x--)
                {
                    double y = G1(x, A, B, C, D, E, F, +1f);
                    if (!IsNumber(y))
                    {
                        xmid2 = x + 1;
                        break;
                    }

                    if ((y < ymin) || (y > ymax))
                        continue;
                    rp_points.Add(new Point3D(x, y, 0));
                }

                // Get points for the negative root on the right.
                for (double x = xmid2; x <= xmax; x++)
                {
                    double y = G1(x, A, B, C, D, E, F, -1f);
                    if (IsNumber(y))
                    {
                        if ((y < ymin) || (y > ymax))
                            continue;
                        rn_points.Add(new Point3D(x, y, 0));
                    }
                }
            }

            // Connect curves if appropriate.
            // Connect the left curves on the left.
            if (xmin > xminInit)
                lp_points.Add(ln_points[0]);

            // Connect the left curves on the right.
            if (xmid1 < xmaxInit)
                ln_points.Add(lp_points[0]);

            // Make sure we have the right curves.
            if (rp_points.Count > 0)
            {
                // Connect the right curves on the left.
                rp_points.Add(rn_points[0]);

                // Connect the right curves on the right.
                if (xmax < xmaxInit)
                    rn_points.Add(rp_points[0]);
            }

            List<List<Point3D>> plotting = new List<List<Point3D>>();
            if (ln_points.Count > 0)
                plotting.Add(ln_points);
            if (lp_points.Count > 0)
                plotting.Add(lp_points);
            if (rp_points.Count > 0)
                plotting.Add(rp_points);
            if (rn_points.Count > 0)
                plotting.Add(rn_points);
            return plotting;
        }

        /// <summary>
        /// Find conic section from provided five points
        /// </summary>
        /// <param name="points">Coordinates of five points in the 3D space</param>
        public static double[] FindConicSection(List<Point3D> points)
        {
            if (points.Count != 5)
                return null;

            const int num_rows = 5;
            const int num_cols = 5;

            // Build the augmented matrix
            double[,] arr = new double[num_rows, num_cols + 2];
            for (int row = 0; row < num_rows; row++)
            {
                arr[row, 0] = points[row].X * points[row].X;
                arr[row, 1] = points[row].X * points[row].Y;
                arr[row, 2] = points[row].Y * points[row].Y;
                arr[row, 3] = points[row].X;
                arr[row, 4] = points[row].Y;
                arr[row, 5] = -1;
                arr[row, 6] = 0;
            }

            // Perform Gaussian elimination
            const float tiny = 0.001f;
            for (int r = 0; r < num_rows - 1; r++)
            {
                // Zero out all entries in column r after this row
                // See if this row has a non-zero entry in column r
                if (Math.Abs(arr[r, r]) < tiny)
                {
                    // Too close to zero. Try to swap with a later row
                    for (int r2 = r + 1; r2 < num_rows; r2++)
                    {
                        if (Math.Abs(arr[r2, r]) > tiny)
                        {
                            // This row will work. Swap them
                            for (int c = 0; c <= num_cols; c++)
                            {
                                double tmp = arr[r, c];
                                arr[r, c] = arr[r2, c];
                                arr[r2, c] = tmp;
                            }
                            break;
                        }
                    }
                }

                // If this row has a non-zero entry in column r, use it.
                if (Math.Abs(arr[r, r]) > tiny)
                {
                    // Zero out this column in later rows.
                    for (int r2 = r + 1; r2 < num_rows; r2++)
                    {
                        double factor = -arr[r2, r] / arr[r, r];
                        for (int c = r; c <= num_cols; c++)
                        {
                            arr[r2, c] = arr[r2, c] + factor * arr[r, c];
                        }
                    }
                }
            }

            // See if we have a solution.
            if (arr[num_rows - 1, num_cols - 1] == 0)
            {
                // We have no solution.
                // See if all of the entries in this row are 0.
                bool all_zeros = true;
                for (int c = 0; c <= num_cols + 1; c++)
                {
                    if (arr[num_rows - 1, c] != 0)
                    {
                        all_zeros = false;
                        break;
                    }
                }

                if (all_zeros)
                {
                    MessageBox.Show("The solution is not unique");
                }
                else
                {
                    MessageBox.Show("There is no solution");
                }

                // Return an array with six zero values
                return new double[6];
            }
            else
            {
                // Back-solve
                for (int r = num_rows - 1; r >= 0; r--)
                {
                    double tmp = arr[r, num_cols];
                    for (int r2 = r + 1; r2 < num_rows; r2++)
                    {
                        tmp -= arr[r, r2] * arr[r2, num_cols + 1];
                    }
                    arr[r, num_cols + 1] = tmp / arr[r, r];
                }

                // Save the results.
                double[] parameters = new double[6];
                parameters[0] = arr[0, num_cols + 1];
                parameters[1] = arr[1, num_cols + 1];
                parameters[2] = arr[2, num_cols + 1];
                parameters[3] = arr[3, num_cols + 1];
                parameters[4] = arr[4, num_cols + 1];
                parameters[5] = 1;
                return parameters;
            }
        }

        /// <summary>
        /// Find the intersection between two conic sections
        /// </summary>
        /// <param name="s1">Six parameters of the first conic</param>
        /// <param name="s2">Six parameters of the second conic</param>
        /// <returns>Points of intersection</returns>
        public static List<Point3D> IntersectionPoints(double[] s1, double[] s2, double xMin, double xMax)
        {
            if ((s1.Length != 6) && (s2.Length != 6))
                return null;

            List<Point3D> roots = new List<Point3D>();
            List<double> rootSign1 = new List<double>();
            List<double> rootSign2 = new List<double>();

            // Find roots for each of the difference equations
            double[] signs = { +1f, -1f };
            foreach (double sign1 in signs)
            {
                foreach (double sign2 in signs)
                {
                    List<Point3D> points = FindRootsUsingBinaryDivision(
                        xMin, xMax,
                        s1[0], s1[1], s1[2], s1[3], s1[4], s1[5], sign1,
                        s2[0], s2[1], s2[2], s2[3], s2[4], s2[5], sign2);
                    if (points.Count > 0)
                    {
                        roots.AddRange(points);
                        for (int i = 0; i < points.Count; i++)
                        {
                            rootSign1.Add(sign1);
                            rootSign2.Add(sign2);
                        }
                    }
                }
            }

            // Find corresponding points of intersection
            List<Point3D> PointsOfIntersection = new List<Point3D>();
            for (int i = 0; i < roots.Count; i++)
            {
                double y1 = G1(roots[i].X, s1[0], s1[1], s1[2], s1[3], s1[4], s1[5], rootSign1[i]);
                double y2 = G1(roots[i].X, s2[0], s2[1], s2[2], s2[3], s2[4], s2[5], rootSign2[i]);
                PointsOfIntersection.Add(new Point3D(roots[i].X, y1, 0));

                // Validation
                Debug.Assert(Math.Abs(y1 - y2) < SMALL);
            }

            return PointsOfIntersection;
        }

        // Find roots by using binary division
        private static List<Point3D> FindRootsUsingBinaryDivision(double xmin, double xmax,
            double A1, double B1, double C1, double D1, double E1, double F1, double sign1,
            double A2, double B2, double C2, double D2, double E2, double F2, double sign2)
        {
            List<Point3D> roots = new List<Point3D>();
            double delta_x = (xmax - xmin) / (num_tests - 1);

            // Loop over the possible x values looking for roots.
            double x0 = xmin;
            double x, y;
            for (int i = 0; i < num_tests; i++)
            {
                // Try to find a root in this range.
                UseBinaryDivision(x0, delta_x, out x, out y,
                    A1, B1, C1, D1, E1, F1, sign1,
                    A2, B2, C2, D2, E2, F2, sign2);

                // See if we have already found this root.
                if (IsNumber(y))
                {
                    bool is_new = true;
                    foreach (Point3D pt in roots)
                    {
                        if (Math.Abs(pt.X - x) < SMALL)
                        {
                            is_new = false;
                            break;
                        }
                    }

                    // If this is a new point, save it.
                    if (is_new)
                    {
                        roots.Add(new Point3D(x, y, 0));

                        // If we've found two roots, we won't find any more.
                        if (roots.Count > 1) return roots;
                    }
                }

                x0 += delta_x;
            }

            return roots;
        }

        // Find a root by using binary division
        private static void UseBinaryDivision(double x0, double delta_x,
            out double x, out double y,
            double A1, double B1, double C1, double D1, double E1, double F1, double sign1,
            double A2, double B2, double C2, double D2, double E2, double F2, double sign2)
        {
            const int num_trials = 200;
            const int sgn_nan = -2;

            // Get G(x) for the bounds.
            double xmin = x0;
            double g_xmin = G(xmin,
                A1, B1, C1, D1, E1, F1, sign1,
                A2, B2, C2, D2, E2, F2, sign2);
            if (Math.Abs(g_xmin) < SMALL)
            {
                x = xmin;
                y = g_xmin;
                return;
            }

            double xmax = xmin + delta_x;
            double g_xmax = G(xmax,
                A1, B1, C1, D1, E1, F1, sign1,
                A2, B2, C2, D2, E2, F2, sign2);
            if (Math.Abs(g_xmax) < SMALL)
            {
                x = xmax;
                y = g_xmax;
                return;
            }

            // Get the sign of the values.
            int sgn_min, sgn_max;
            if (IsNumber(g_xmin)) sgn_min = Math.Sign(g_xmin);
            else sgn_min = sgn_nan;
            if (IsNumber(g_xmax)) sgn_max = Math.Sign(g_xmax);
            else sgn_max = sgn_nan;

            // If the two values have the same sign,
            // then there is no root here.
            if (sgn_min == sgn_max)
            {
                x = 1;
                y = double.NaN;
                return;
            }

            // Use binary division to find the point of intersection.
            double xmid = 0, g_xmid = 0;
            int sgn_mid = 0;
            for (int i = 0; i < num_trials; i++)
            {
                // Get values for the midpoint.
                xmid = (xmin + xmax) / 2;
                g_xmid = G(xmid,
                    A1, B1, C1, D1, E1, F1, sign1,
                    A2, B2, C2, D2, E2, F2, sign2);
                if (IsNumber(g_xmid)) sgn_mid = Math.Sign(g_xmid);
                else sgn_mid = sgn_nan;

                // If sgn_mid is 0, gxmid is 0 so this is the root.
                if (sgn_mid == 0) break;

                // See which half contains the root.
                if (sgn_mid == sgn_min)
                {
                    // The min and mid values have the same sign.
                    // Search the right half.
                    xmin = xmid;
                    g_xmin = g_xmid;
                }
                else if (sgn_mid == sgn_max)
                {
                    // The max and mid values have the same sign.
                    // Search the left half.
                    xmax = xmid;
                    g_xmax = g_xmid;
                }
                else
                {
                    // The three values have different signs.
                    // Assume min or max is NaN.
                    if (sgn_min == sgn_nan)
                    {
                        // Value g_xmin is NaN. Use the right half.
                        xmin = xmid;
                        g_xmin = g_xmid;
                    }
                    else if (sgn_max == sgn_nan)
                    {
                        // Value g_xmax is NaN. Use the right half.
                        xmax = xmid;
                        g_xmax = g_xmid;
                    }
                    else
                    {
                        // This is a weird case. Just trap it.
                        //throw new InvalidOperationException(
                        //    "Unexpected difference curve. " +
                        //    "Cannot find a root between X = " +
                        //    xmin + " and X = " + xmax);
                    }
                }
            }

            if (IsNumber(g_xmid) && (Math.Abs(g_xmid) < SMALL))
            {
                x = xmid;
                y = g_xmid;
            }
            else if (IsNumber(g_xmin) && (Math.Abs(g_xmin) < SMALL))
            {
                x = xmin;
                y = g_xmin;
            }
            else if (IsNumber(g_xmax) && (Math.Abs(g_xmax) < SMALL))
            {
                x = xmax;
                y = g_xmax;
            }
            else
            {
                x = xmid;
                y = double.NaN;
            }
        }

        // Calculate G1(x).
        // root_sign is -1 or 1.
        private static double G1(double x, double A, double B, double C, double D, double E, double F, double root_sign)
        {
            double result = B * x + E;
            result = result * result;
            result = result - 4 * C * (A * x * x + D * x + F);
            result = root_sign * (double)Math.Sqrt(result);
            result = -(B * x + E) + result;
            result = result / 2 / C;

            return result;
        }

        // Calculate G1'(x).
        // root_sign is -1 or 1.
        private static double G1Prime(double x, double A, double B, double C, double D, double E, double F, double root_sign)
        {
            double numerator = 2 * (B * x + E) * B -
                4 * C * (2 * A * x + D);
            double denominator = 2 * (double)Math.Sqrt(
                (B * x + E) * (B * x + E) -
                4 * C * (A * x * x + D * x + F));
            double result = -B + root_sign * numerator / denominator;
            result = result / 2 / C;

            return result;
        }

        // Calculate G(x).
        private static double G(double x,
            double A1, double B1, double C1, double D1, double E1, double F1, double sign1,
            double A2, double B2, double C2, double D2, double E2, double F2, double sign2)
        {
            return
                G1(x, A1, B1, C1, D1, E1, F1, sign1) -
                G1(x, A2, B2, C2, D2, E2, F2, sign2);
        }

        // Calculate G'(x).
        private static double GPrime(double x,
            double A1, double B1, double C1, double D1, double E1, double F1, double sign1,
            double A2, double B2, double C2, double D2, double E2, double F2, double sign2)
        {
            return
                G1Prime(x, A1, B1, C1, D1, E1, F1, sign1) -
                G1Prime(x, A2, B2, C2, D2, E2, F2, sign2);
        }

        // Return true if the number is not infinity or NaN.
        private static bool IsNumber(double number)
        {
            return !(double.IsNaN(number) || double.IsInfinity(number));
        }

        public static void FindMinMaxX(List<Point3D> sourcePoints,
            out double xMin, out double xMax)
        {
            // Find min and max X coordinate
            xMin = sourcePoints[0].X;
            xMax = sourcePoints[0].X;
            for (int i = 1; i < sourcePoints.Count; i++)
            {
                xMin = Math.Min(xMin, sourcePoints[i].X);
                xMax = Math.Max(xMax, sourcePoints[i].X);
            }
        }
    }
}
