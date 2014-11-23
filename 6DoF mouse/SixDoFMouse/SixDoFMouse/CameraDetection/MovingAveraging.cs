using CADBest.GeometryNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SixDoFMouse.CameraDetection
{
    public class MovingAveraging
    {
        //private const int TIMER_DISP_PARS_INTERVAL = 3000;
        //private const int LENGTH_TIME = 10;
        private const int LENGTH_MIDDLES = 2;

        // Interval of timer, which measures time between two frames
        private const int TIMER_INTERVAL = 100;
        private const int PERCENT_TOP = 2;

        private const double K_INIT = 1d;
        private const double K_DECR = 0.1d;

        private double Kfade;

        // Queue of times between successful frames and 6 parameters from mouse
        // (Angles by X, Y, Z axises and movement by these axes)
        //private List<double> TimeQueue = new List<double>(LENGTH_TIME);
        private List<Point3D> AngleQueue = new List<Point3D>(LENGTH_MIDDLES);
        private List<Point3D> TargetQueue = new List<Point3D>(LENGTH_MIDDLES);
        private Point3D AngleMiddle, TargetMiddle;
        private Point3D DeltaAngle, DeltaTarget;

        // Measures time between successful frames
        public TimerStopwatch stopwatch;

        // The interval of timer who dispatch parameters to AutoCAD
        public int IntervalTimerDispatch;

        // Time between two successful frames
        private double ElapsedTime = 0;
        
        // When TimerDispatch ticks it will send these angles to AutoCAD
        public Point3D CurrentAngle, CurrentTarget;

        public MovingAveraging(int interval)
        {
            stopwatch = new TimerStopwatch(TIMER_INTERVAL);
            //if (GlobalProperties.UseTimers)
                stopwatch.Enabled = true;
            IntervalTimerDispatch = interval;
            CurrentAngle = new Point3D();
            CurrentTarget = new Point3D();
            Kfade = K_INIT;
        }

        public bool UpdateStep()
        {
            if ((DeltaAngle != null) && (DeltaTarget != null) && (Kfade > 0))
            {
                double incX, incY, incZ;
                Point3D deltaAngleClone = DeltaAngle.Clone();
                // Angles
                incX = deltaAngleClone.X;
                incY = deltaAngleClone.Y;
                incZ = deltaAngleClone.Z;
                CurrentAngle.X += (incX * Kfade);
                CurrentAngle.Y += (incY * Kfade);
                CurrentAngle.Z += (incZ * Kfade);

                // Target
                Point3D deltaTargetClone = DeltaTarget.Clone();
                incX = deltaTargetClone.X;
                incY = deltaTargetClone.Y;
                incZ = deltaTargetClone.Z;
                CurrentTarget.X += (incX * Kfade);
                CurrentTarget.Y += (incY * Kfade);
                CurrentTarget.Z += (incZ * Kfade);

                Kfade -= K_DECR;
                return true;
            }
            else
                return false;
        }

        public double FrameTick(Point3D angles, Point3D target)
        {
            // Time between two successful frames,
            // which will be nulled on every success in ms
            ElapsedTime = stopwatch.ElapsedTime;
            // Add it to queue and get every parameter middled
            GatherPoints(AngleQueue, angles, ref AngleMiddle, true);
            GatherPoints(TargetQueue, target, ref TargetMiddle, true);

            if (AngleQueue.Count < LENGTH_MIDDLES)
                return 0;

            // Workaround for transition between 350 and 0 degrees
            double xDifference = Math.Abs(angles.X - AngleMiddle.X);
            double yDifference = Math.Abs(angles.Y - AngleMiddle.Y);
            double zDifference = Math.Abs(angles.Z - AngleMiddle.Z);
            double border = Math.PI / 2;
            if ((xDifference > border) ||
                (yDifference > border) ||
                (zDifference > border))
            {
                AngleMiddle = angles.Clone();
                AngleQueue.Clear();
                for (int i = 0; i < LENGTH_MIDDLES; i++)
                    AngleQueue.Add(angles);
                CurrentAngle = angles.Clone();
            }

            double time = ElapsedTime / (double)IntervalTimerDispatch;
            double dX, dY, dZ;
            // Angles
            dX = (AngleMiddle.X - CurrentAngle.X) / time;
            dY = (AngleMiddle.Y - CurrentAngle.Y) / time;
            dZ = (AngleMiddle.Z - CurrentAngle.Z) / time;
            DeltaAngle = new Point3D(dX, dY, dZ);
            
            // Target
            dX = (TargetMiddle.X - CurrentTarget.X) / time;
            dY = (TargetMiddle.Y - CurrentTarget.Y) / time;
            dZ = (TargetMiddle.Z - CurrentTarget.Z) / time;
            DeltaTarget = new Point3D(dX, dY, dZ);
            
            Kfade = K_INIT;

            return dY;
        }

        /// <summary>
        /// Calculate Average(Mean) point from sets of points
        /// </summary>
        /// <param name="ArrPoints">Add points to this list</param>
        /// <param name="P1"></param>
        /// <param name="Pmiddle">Previous average point</param>
        /// <param name="ShouldMiddle">If true calculate average points if false add points to ArrPoints</param>
        private void GatherPoints(List<Point3D> ArrPoints, Point3D P1, ref Point3D Pmiddle, bool ShouldMiddle)
        {
            if (P1 == null)
                return;

            ArrPoints.Add(P1);
            if (ArrPoints.Count > LENGTH_MIDDLES)
                ArrPoints.RemoveAt(0);

            if (ArrPoints.Count == LENGTH_MIDDLES)
            {
                if (ShouldMiddle)
                    MiddlePoints(ArrPoints, ref Pmiddle);
            }
        }

        private void MiddlePoints(List<Point3D> ArrPoints, ref Point3D Pmiddle)
        {
            double x = 0, y = 0, z = 0;
            foreach (Point3D p in ArrPoints)
            {
                x += p.X;
                y += p.Y;
                z += p.Z;
            }
            x /= LENGTH_MIDDLES;
            y /= LENGTH_MIDDLES;
            z /= LENGTH_MIDDLES;
            if (Pmiddle == null)
                Pmiddle = new Point3D();
            Pmiddle.SetCoordinates(x, y, z);
        }
    }
}
