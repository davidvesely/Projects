using CADBest.GeometryNamespace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixDoFMouse
{
    public class GlobalProperties
    {
        public static float[] zoomFactor = { 0.01f, 0.03f, 0.05f, 0.08f, 0.09f, .1f, .2f, .3f, .4f, .5f, .7f, .8f, 
                                       1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f, 5f, 6f, 8f, 
                                       10f, 12f, 15f, 20f, 35f, 40f, 45f, 50f, 55f, 60f };

        public static readonly bool ViewPointMode = false;
        // Process Objects or ViewSpace
        public static readonly bool ProcessObjects = false;
        public static readonly bool UseTimers = true;
    }

    //public delegate void CameraEventHandler(object sender, EventArgs e);
    public delegate void DrawPolylineHandler(object sender, DrawPolylineEventArgs e);
    public delegate void CameraSendOrientationHandler(object sender, SendOrientationEventArgs e);

    public class DrawPolylineEventArgs : EventArgs
    {
        public List<Point3D> Polyline;
        public bool ShouldFlush;
        public Color PolylineColor;

        public DrawPolylineEventArgs(List<Point3D> list, bool shouldFlush, Color color)
            : base()
        {
            Polyline = list;
            ShouldFlush = shouldFlush;
            PolylineColor = color;
        }

        public DrawPolylineEventArgs(List<Point3D> list, bool ShouldFlush)
            : this(list, ShouldFlush, Color.White)
        {
        }
    }

    public class SendOrientationEventArgs : EventArgs
    {
        // This list should contain two Point3D objects
        // The first represent the three rotation angles
        // Second is the translation
        public List<Point3D> OrientationParameters;

        public SendOrientationEventArgs()
            : this(new List<Point3D>())
        {
        }

        public SendOrientationEventArgs(List<Point3D> list)
            : base()
        {
            OrientationParameters = list;
        }
    }
}
