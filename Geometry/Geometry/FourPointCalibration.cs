using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using G = CADBest.GeometryNamespace.Geometry;

namespace CADBest.GeometryNamespace
{
    public class FourPointCalibration
    {
        private readonly int QueueLength;
        private List<List<Point3D>> HorizonLinesQueue;
        
        private static readonly Point3D p000 = new Point3D(0.0, 0.0, 0.0);
        private static readonly Point3D px00 = new Point3D(1.0, 0.0, 0.0);
        private static readonly Point3D p0y0 = new Point3D(0.0, 1.0, 0.0);
        private static readonly Point3D pxy0 = new Point3D(1.0, 1.0, 0.0);
        private static readonly Point3D[] OriginAlignParametersOXY =
            new Point3D[] { p000, p0y0, p0y0, p0y0 };

        // For drawing in AutoCAD purposes
        VisualizationACAD VisualizeObjects;

        public FourPointCalibration(int qLength)
        {
            QueueLength = qLength;
            HorizonLinesQueue = new List<List<Point3D>>(QueueLength);
            VisualizeObjects = new VisualizationACAD();
        }

        /// <summary>
        /// 4-Point calibration
        /// </summary>
        /// <param name="objectPointsOriginal">Five coplanar points from 3D object</param>
        /// <param name="centerProjectionOriginal">Center of the projection, X and Y axis</param>
        /// <param name="projectionPointsOriginal">Five points from the object on the projection</param>
        public List<Point3D> FourPointCalibrationMain(List<Point3D> objectPointsOriginal,
            List<Point3D> centerProjectionOriginal, List<Point3D> projectionPointsOriginal,
            double Fdistparameter = -1)
        {
            // pl0-Center projection, pl1-3D points, pl2-Projection points
            if ((objectPointsOriginal.Count != 5) ||
                (projectionPointsOriginal.Count != 5) ||
                (centerProjectionOriginal.Count != 3))
            {
                throw new GeometryException("Wrong number of parameters passed to 4p transformation method!");
            }

            List<Point3D> projectionPoints = Geometry.CloneList(projectionPointsOriginal);
            List<Point3D> objectPoints = Geometry.CloneList(objectPointsOriginal);
            List<Point3D> centerProjection = Geometry.CloneList(centerProjectionOriginal);

            // Align the lists to X0Y plane with center of the projection in (0, 0)
            Point3D[] sourceOrientation = Geometry.align_calc_pars(centerProjection.ToArray());
            Geometry.align(projectionPoints, sourceOrientation, OriginAlignParametersOXY);
            Geometry.align(centerProjection, sourceOrientation, OriginAlignParametersOXY);
            sourceOrientation = Geometry.align_calc_pars(objectPointsOriginal[0], objectPointsOriginal[1], objectPointsOriginal[2]);
            Geometry.align(objectPoints, sourceOrientation, OriginAlignParametersOXY);

            //ACADManager.DrawPolylines(projectionPoints, false, System.Drawing.Color.White);
            //ACADManager.DrawPolylines(objectPoints, false, System.Drawing.Color.White);

            List<Point3D> sqrl0 = null;
            // Find the five possible horizon lines with iterating
            // the order of the five object and projection points
            List<List<Point3D>> horizonLines = new List<List<Point3D>>();
            for (int i = 0; i < 5; i++)
            {
                horizonLines.Add(CalcHorizonLine(objectPoints, projectionPoints, ref sqrl0));
                // Changing the order of the point
                // the first goes at the end of the list
                objectPoints.Add(objectPoints[0]);
                objectPoints.RemoveAt(0);
                projectionPoints.Add(projectionPoints[0]);
                projectionPoints.RemoveAt(0);
            }

            // For drawing only
            //List<Point3D> horizonLineMiddleDel = MiddleHorizonLines(horizonLines);
            //ACADManager.DrawPolylines(horizonLineMiddleDel, false, System.Drawing.Color.Green);

            // Filter the most distant line of the five found ones
            FilterHorizonLines(horizonLines);
            // Middled horizon line with the excluded from previous filtering
            List<Point3D> horizonLineMiddle = MiddleHorizonLines(horizonLines);
            
            // Increment the queue with the current middled horizon line,
            // exit if there are not enough queue members,
            // and find the middled from the queue
            HorizonLinesQueue.Add(horizonLineMiddle);
            if (HorizonLinesQueue.Count > QueueLength)
                HorizonLinesQueue.RemoveAt(0);
            List<Point3D> horizonLineMiddleQueue = null;
            if (HorizonLinesQueue.Count == QueueLength)
                horizonLineMiddleQueue = MiddleHorizonLines(HorizonLinesQueue);
            else
                return null;

            List<Point3D> pl0 = centerProjectionOriginal;
            Point3D pt1 = horizonLineMiddleQueue[0];
            Point3D pt2 = horizonLineMiddleQueue[1];
            double dAB, dAH, dBH;
            dAB = G.Distance(pt1, pt2);
            dAH = G.Distance(pt1, pl0[0]);
            dBH = G.Distance(pt2, pl0[0]);
            double h = Math.Sqrt(Math.Abs(Math.Pow(dAB, 2) - Math.Pow(dAH, 2) - Math.Pow(dBH, 2))) / Math.Sqrt(2);

            // Should be determined experimentally
            if ((dAH > 1e5) || (dBH > 1e5))
            {
                // TODO: should be continued
                return null;
            }

            // Stop there
            return null;

            List<Point3D> pl3 = G.per2ln(p000, pt1, pt2);
            List<Point3D> pl4 = G.per2ln(pt1, p000, pt2);
            Point3D pt3 = G.LineLineIntersect(pl3[0], pl3[1], pl4[0], pl4[1], false)[0];
            Point3D pt4 = pl3[1];
            List<Point3D> pl5 = new List<Point3D>(4);
            pl5.Add(pt4.Clone());
            pl5.Add(pl3[0]);
            pl5.Add(pt3);
            G.p_add(pl5, pt4, -1.0);
            Point3D sc2 = G.calc_rotZ(pl5[2]);
            //double testY = -1d * pl5[1].X * sc2.X + pl5[1].Y * sc2.Y;
            G.p_rotZ(pl5, sc2, -1.0);
            Point3D pt6 = new Point3D(pl5[2].X / 2.0, 0.0, 0.0);
            G.p_add(pl5, pt6, -1.0);
            double testY = pt6.X * pt6.X - pl5[1].X * pl5[1].X;
            if (testY < 0)
                return null;
            Point3D pt7 = new Point3D(pl5[1].X, Math.Sqrt(testY), 0.0);
            pl5.Insert(0, new Point3D(pt7));
            G.p_rotX(pl5, px00, 1.0);

            G.p_add(pl5, pt6, 1.0);
            G.p_rotZ(pl5, sc2, 1.0);
            G.p_add(pl5, pt4, 1.0);

            List<Point3D> pl21 = projectionPoints;
            List<Point3D> pl01 = centerProjection;
            List<Point3D> pl22 = G.CloneList(pl21);
            List<Point3D> pl02 = G.CloneList(pl01);
            List<Point3D> sqrl02 = G.CloneList(sqrl0);
            G.p_rotZ(pl22, sc2, -1.0);
            G.p_rotZ(pl02, sc2, -1.0);
            G.p_rotZ(sqrl02, sc2, -1.0);
            G.p_rotZ(pl5, sc2, -1.0);

            Point3D pt9 = new Point3D(0, 0, pl5[0].Z);
            G.p_add(pl22, pt9, -1.0);
            G.p_add(pl02, pt9, -1.0);
            G.p_add(sqrl02, pt9, -1.0);
            G.p_add(pl5, pt9, -1.0);

            Point3D sc3 = G.calc_rotY(new Point3D(-pl5[1].X, -pl5[1].Y, -pl5[1].Z));
            G.p_rotY(pl22, sc3, -1.0);
            G.p_rotY(pl02, sc3, -1.0);
            G.p_rotY(sqrl02, sc3, -1.0);
            G.p_rotY(pl5, sc3, -1.0);

            G.p_add(pl22, pt9, 1.0);
            G.p_add(pl02, pt9, 1.0);
            G.p_add(sqrl02, pt9, 1.0);
            G.p_add(pl5, pt9, 1.0);

            List<Point3D> pl1pr = new List<Point3D>(pl22.Count + 5);
            double fdist = pl5[0].Z;

            if ((Fdistparameter > 0) && (fdist > Fdistparameter))
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

            k1 = 1d / G.Distance(plspr[0], plspr[1]);
            Point3D pf1 = new Point3D(0.0, 0.0, fdist);
            G.p_add(pl1pr, pf1, -1.0);
            G.p_mult(pl1pr, k1, true);
            G.p_add(pl1pr, pf1, 1.0);

            sourceOrientation = G.align_calc_pars(pl02.ToArray());
            pl02.Insert(0, pl5[0]);
            Point3D[] destinationOrientation = G.align_calc_pars(pl0.ToArray());
            List<Point3D> pl00 = G.CloneList(pl02);
            G.align(pl00, sourceOrientation, destinationOrientation);

            sourceOrientation = G.align_calc_pars(pl1pr[0], pl1pr[1], pl1pr[2]);
            List<Point3D> pl1 = objectPointsOriginal;
            destinationOrientation = G.align_calc_pars(pl1[0], pl1[1], pl1[2]);
            G.align(pl02, sourceOrientation, destinationOrientation);

            if (double.IsNaN(pl02[1].X))
                return null;
            //ACADManager.DrawPolylines(pl02, false, System.Drawing.Color.White);
            return pl02;
        }

        private List<Point3D> CalcHorizonLine(List<Point3D> objectPoints,
            List<Point3D> projectionPoints, ref List<Point3D> sqrl0Out)
        {
            List<Point3D> sqrl1 = new List<Point3D>(4);
            sqrl1.Add(p000);
            sqrl1.Add(px00);
            sqrl1.Add(pxy0);
            sqrl1.Add(p0y0);

            //Point3D[] sourcePars = Geometry.align_calc_pars(objectPoints.GetRange(0, 3).ToArray());
            //List<Point3D> destinPars = Geometry.align_calc_pars(objectPoints.GetRange(0, 3));
            //Geometry.align(objectPoints, sourcePars, OriginAlignParametersOXY);

            List<Point3D> sqrl0 = Geometry.homography_main(sqrl1, objectPoints.ToArray(), projectionPoints.ToArray());
            // Send the first sqrl0 for later processing
            if (sqrl0Out == null)
                sqrl0Out = sqrl0;
            VisualizeObjects.DrawPolylines(sqrl0, false, System.Drawing.Color.White);

            Point3D pt1 = Geometry.LineLineIntersect(sqrl0[0], sqrl0[1], sqrl0[2], sqrl0[3], false)[0];
            Point3D pt2 = Geometry.LineLineIntersect(sqrl0[0], sqrl0[3], sqrl0[1], sqrl0[2], false)[0];

            List<Point3D> result = new List<Point3D>(2);
            result.Add(pt1);
            result.Add(pt2);
            VisualizeObjects.DrawPolylines(result, false, System.Drawing.Color.White);
            return result;
        }

        private void FilterHorizonLines(List<List<Point3D>> horizonLines)
        {
            List<Point3D> horizonMiddle = MiddleHorizonLines(horizonLines);
            List<Point3D> maxHorizon = null;
            double maxDist0 = 0, maxDist1 = 0;
            foreach (List<Point3D> horizon in horizonLines)
            {
                double dist0, dist1;
                dist0 = Geometry.Distance(horizonMiddle[0], horizon[0]);
                dist1 = Geometry.Distance(horizonMiddle[1], horizon[1]);

                if ((dist0 > maxDist0) || (dist1 > maxDist1))
                {
                    maxHorizon = horizon;
                    maxDist0 = dist0;
                    maxDist1 = dist1;
                }
            }

            if (maxHorizon != null)
            {
                horizonLines.Remove(maxHorizon);
            }
        }

        private List<Point3D> MiddleHorizonLines(List<List<Point3D>> horizonLines)
        {
            Point3D horizonMiddle0 = new Point3D();
            Point3D horizonMiddle1 = new Point3D();
            int linesCount = 0;
            foreach (List<Point3D> horizon in horizonLines)
            {
                horizonMiddle0.Add(horizon[0]);
                horizonMiddle1.Add(horizon[1]);
                linesCount++;
            }
            if (linesCount == 0)
                return null;

            // Divide the point's coordinates by 5 (averaging)
            Geometry.p_mult(horizonMiddle0, 1d / (double)linesCount, true);
            Geometry.p_mult(horizonMiddle1, 1d / (double)linesCount, true);

            List<Point3D> result = new List<Point3D>(2);
            result.Add(horizonMiddle0);
            result.Add(horizonMiddle1);
            return result;
        }
    }
}
