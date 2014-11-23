using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.Win32;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.GraphicsSystem;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using CADBest.Geometry;
using SixDoFMouse.CameraDetection;

namespace SixDoFMouse
{
    public class AutoCADManager
    {
        #region Windows functions
        public static float ReadPrefScale()
        {
            RegistryKey sk1 = Registry.CurrentUser.OpenSubKey("Software\\CADBEST\\6DOF Mouse");
            float scale;
            if (sk1 != null)
            {
                scale = (float)Convert.ToDouble(sk1.GetValue("Scale"));
                if (scale == 0)
                    scale = 1;
            }
            else
                scale = 1;
            return scale;
        }

        public static Bitmap ResizeBitmap(float scale, Bitmap source)
        {
            int width, height;
            Bitmap bmp;
            Graphics graph;
            width = (int)(source.Width * scale);
            height = (int)(source.Height * scale);
            bmp = new Bitmap(width, height, source.PixelFormat);
            graph = Graphics.FromImage(bmp);
            graph.DrawImage(source, new Rectangle(0, 0, width, height));
            return bmp;
        }
        #endregion

        #region Thread safe optimization
        // This member is needed for transferring the processing of AutoCAD
        // environment from camera's thread to main thread of AutoCAD
        private static Control syncControl;

        // Delegates used for marshaling the call of corresponding method to main thread of AutoCAD
        private delegate void ModifyViewspaceDelegate(List<Point3D> ViewDesc);
        private delegate void DrawPolylinesDelegate(List<Point3D> Objects, bool ShouldDeletePrevious, Autodesk.AutoCAD.Colors.Color color);
        private delegate void DrawCoordinatesDelegate(List<List<Point3D>> Objects, bool ShouldDelete);

        private void InitializeSyncControl()
        {
            // The control created to help with marshaling
            // needs to be created on the main thread
            syncControl = new Control();
            syncControl.CreateControl();
        }
        #endregion

        #region AutoCAD functions
        // Used for saving the initial state of View
        private Vector3d _initialView;
        //private Document _doc;

        public FormVisualizer vizDataText;
        bool first = true;

        public AutoCADManager()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            ViewTableRecord vtr = doc.Editor.GetCurrentView();
            _initialView = vtr.ViewDirection;
            InitializeSyncControl();
        }

        private SelectionSet currentObjects;
        private double xAnglePrev = 0;
        private double yAnglePrev = 0;
        private double zAnglePrev = 0;

        public void SelectObjects()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            PromptSelectionResult selectSet = doc.Editor.GetSelection();
            if (selectSet.Status == PromptStatus.OK)
                currentObjects = selectSet.Value;
        }
       
        public void SelectionRotate(Document doc, Point3D xyzAngles)
        {
            //if (first == false)
            //{
            //xAnglePrev = xyzAngles.X;
            //yAnglePrev = xyzAngles.Y;
            //zAnglePrev = xyzAngles.Z;
                //first = true;
            //}

            Database currentDb = doc.Database;
            Matrix3d curUCSMatrix = doc.Editor.CurrentUserCoordinateSystem;
            CoordinateSystem3d curUCS = curUCSMatrix.CoordinateSystem3d;
            
            using (Transaction trans = currentDb.TransactionManager.StartTransaction())
            {
                if (currentObjects != null)
                {
                    foreach (SelectedObject curr in currentObjects)
                    {
                        if (curr != null)
                        {
                            Entity acEntity = trans.GetObject(curr.ObjectId, OpenMode.ForWrite) as Entity;

                            if (acEntity != null)
                            {
                                double xRot, yRot, zRot;
                                xRot = xAnglePrev - xyzAngles.X;
                                yRot = yAnglePrev - xyzAngles.Y;
                                zRot = zAnglePrev - xyzAngles.Z;

                                xAnglePrev = xyzAngles.X;
                                yAnglePrev = xyzAngles.Y;
                                zAnglePrev = xyzAngles.Z;

                                acEntity.TransformBy((Matrix3d.Rotation(
                                    yRot,                                                                         
                                    Vector3d.YAxis,
                                    Point3d.Origin)));
                                acEntity.TransformBy((Matrix3d.Rotation(
                                    xRot,
                                    Vector3d.XAxis,
                                    Point3d.Origin)));
                                acEntity.TransformBy((Matrix3d.Rotation(
                                    zRot,
                                    Vector3d.ZAxis,
                                    Point3d.Origin)));
                                trans.Commit();
                            }
                        }
                    }
                }
            }
        }

        public void ModifyViewspace(List<Point3D> ViewDesc)
        {
            // If ModifyViewspace method is called from different thread, InvokeRequired will be true
            if (syncControl.InvokeRequired)
            {
                syncControl.Invoke(new ModifyViewspaceDelegate(ModifyViewspace), new object[] { ViewDesc });
            }
            else
            {
                Document doc = AcadApp.DocumentManager.MdiActiveDocument;
                using (doc.LockDocument())
                {
                    ViewTableRecord vtr = doc.Editor.GetCurrentView();

                    if (GlobalProperties.ProcessObjects)
                        SelectionRotate(doc, ViewDesc[0]);
                    else
                    {
                        if (GlobalProperties.ViewPointMode)
                            RotateViewDest(ViewDesc, vtr);
                        else
                        {
                            //RotateView(ViewDesc[0], vtr);
                            if (ViewDesc.Count > 1)
                            {
                                MoveView(ViewDesc[1], vtr);
                            }
                        }
                    }

                    doc.Editor.SetCurrentView(vtr);
                }
            }
        }

        private void MoveView(Point3D Target, ViewTableRecord vtr)
        {
            if (Target != null)
                //vtr.Target = new Point3d(Target.X, Target.Y, Target.Z);
                vtr.CenterPoint = new Point2d(5 * Target.X, -5 * Target.Y);
        }

        public void RotateView(Document doc, ScreenAxisRotation axis)
        {

        }

        // Distance between camera and mouse coordinate system center
        double PreviousDistance = 0;
        double Width = 0;
        double Height = 0;

        private void RotateViewDest(List<Point3D> ViewDesc, ViewTableRecord vtr)
        {
            if (first == true)
            {
                Width = vtr.Width;
                Height = vtr.Height;
                first = false;
            }
           
            //Point3d view = new Point3d(ViewDesc[0].X, ViewDesc[0].Y, ViewDesc[0].Z);
            //Point3d target = new Point3d(ViewDesc[1].X, ViewDesc[1].Y, ViewDesc[1].Z);
            //vtr.Target = new Point3d(0, 0, 0);
            //vtr.Target = target; // camera around descriptor       
            //Vector3d viewvector = target.GetVectorTo(view);
            //vtr.ViewDirection = viewvector;
            //vtr.ViewDirection = vtr.Target.GetVectorTo(new Point3d(1.5*ViewDesc[0].X, 1.5*ViewDesc[0].Y, 1.5*ViewDesc[0].Z));
            //vtr.ViewDirection =  new Vector3d(ViewDesc[0].X, ViewDesc[0].Y, ViewDesc[0].Z);

            double zoomDist;
            double currentDist = Geometry.Distance(ViewDesc[3], new Point3D());
            //if (currentDist != null)
            //vizDataText.SetText(currentDist.ToString());

            if ((currentDist > 50) && (currentDist < 160))
            {
                vtr.Width = Width;
                vtr.Height = Height;
            }

            if ((currentDist > 160) && (currentDist < 200))
            {
                vtr.Width = 2 * Width;
                vtr.Height = 2 * Height;
            }
            if ((currentDist > 200) && (currentDist < 300))
            {
                vtr.Width = 4 * Width;
                vtr.Height = 4 * Height;
            }
            if (currentDist > 300) 
            {
                vtr.Width = 6 * Width;
                vtr.Height = 6 * Height;
            }
           
            //if (PreviousDistance != 0)
            //{
            //    zoomDist = PreviousDistance - currentDist;
            //    float zoomFactorIn = 0.7f;
            //    float zoomFactorOut = 1.5f;
            //    if (zoomDist < -5) // Zoom In
            //    {
            //        vtr.Width *= zoomFactorIn;
            //        vtr.Height *= zoomFactorIn;
            //    }
            //    else if (zoomDist > 5) // Zoom Out
            //    {
            //        vtr.Width *= zoomFactorOut;
            //        vtr.Height *= zoomFactorOut;
            //    }
            //}
            //PreviousDistance = currentDist;

            //vtr.CenterPoint = new Point2d(5*ViewDesc[3].X, -5*ViewDesc[3].Y);
        }

        private ObjectId PreviousViewPoint;

        public void DrawPolylines(List<Point3D> Objects, bool ShouldDeletePrevious, Autodesk.AutoCAD.Colors.Color color)
        {
            // If ModifyViewspace method is called from different thread, InvokeRequired will be true
            if (syncControl.InvokeRequired)
                syncControl.Invoke(new DrawPolylinesDelegate(DrawPolylines), new object[] { Objects, ShouldDeletePrevious, color });
            else
            {
                Document doc = AcadApp.DocumentManager.MdiActiveDocument;
                using (DocumentLock docLock = doc.LockDocument())
                {
                    Database currentDb = doc.Database;
                    using (Transaction trans = currentDb.TransactionManager.StartTransaction())
                    {
                        // Deleting the previous drawn object
                        if ((PreviousViewPoint != ObjectId.Null) && (ShouldDeletePrevious))
                        {
                            Entity ent = (Entity)trans.GetObject(PreviousViewPoint, OpenMode.ForWrite);
                            ent.Erase();
                            ent.Dispose();
                        }
                        // Open the Block table for read
                        BlockTable blockTable;
                        blockTable = trans.GetObject(currentDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                        // Open the Block table record Model space for write
                        BlockTableRecord blockTblRec =
                            trans.GetObject(blockTable[BlockTableRecord.ModelSpace],
                            OpenMode.ForWrite) as BlockTableRecord;

                        Point3dCollection plCollection = new Point3dCollection();
                        foreach (Point3D p in Objects)
                        {
                            plCollection.Add(new Point3d(p.X, p.Y, p.Z));
                        }
                        Polyline3d pl = new Polyline3d(Poly3dType.SimplePoly, plCollection, false);
                        pl.Color = color;

                        PreviousViewPoint = blockTblRec.AppendEntity(pl);
                        trans.AddNewlyCreatedDBObject(pl, true);
                        trans.Commit();
                        
                        // Updates the screen without flickering of view
                        AcadApp.UpdateScreen(); // or doc.Editor.Regen();
                    }
                }
            }
        }

        private List<ObjectId> PreviousCoordinates;

        public void DrawCoordinates(List<List<Point3D>> Objects, bool ShouldDelete)
        {
            // If ModifyViewspace method is called from different thread, InvokeRequired will be true
            if (syncControl.InvokeRequired)
                syncControl.Invoke(new DrawCoordinatesDelegate(DrawCoordinates), new object[] { Objects, ShouldDelete });
            else
            {
                Document doc = AcadApp.DocumentManager.MdiActiveDocument;
                using (DocumentLock docLock = doc.LockDocument())
                {
                    Database currentDb = doc.Database;
                    using (Transaction trans = currentDb.TransactionManager.StartTransaction())
                    {
                        if ((PreviousCoordinates != null) && (ShouldDelete))
                        {
                            foreach (ObjectId item in PreviousCoordinates)
                            {
                                Entity ent = (Entity)trans.GetObject(item, OpenMode.ForWrite);
                                ent.Erase();
                                ent.Dispose();
                            }
                        }
                        // Open the Block table for read
                        BlockTable blockTable;
                        blockTable = trans.GetObject(currentDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                        // Open the Block table record Model space for write
                        BlockTableRecord blockTblRec =
                            trans.GetObject(blockTable[BlockTableRecord.ModelSpace],
                            OpenMode.ForWrite) as BlockTableRecord;

                        Autodesk.AutoCAD.Colors.Color[] colors = new Autodesk.AutoCAD.Colors.Color[3];
                        colors[0] = Autodesk.AutoCAD.Colors.Color.FromRgb(255, 0, 0);
                        colors[1] = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 0);
                        colors[2] = Autodesk.AutoCAD.Colors.Color.FromRgb(0, 0, 255);
                        
                        int colorCounter = 0;
                        PreviousCoordinates = new List<ObjectId>();
                        foreach (List<Point3D> plList in Objects)
                        {
                            Point3dCollection plCollection = new Point3dCollection();
                            foreach (Point3D p in plList)
                            {
                                plCollection.Add(new Point3d(p.X, p.Y, p.Z));
                            }
                            Polyline3d pl = new Polyline3d(Poly3dType.SimplePoly, plCollection, false);
                            pl.Color = colors[colorCounter];
                            colorCounter++;

                            PreviousCoordinates.Add(blockTblRec.AppendEntity(pl));
                            trans.AddNewlyCreatedDBObject(pl, true);
                        }
                        trans.Commit();
                        // Updates the screen without flickering of view
                        //doc.Editor.Regen();
                        Autodesk.AutoCAD.ApplicationServices.Application.UpdateScreen();
                    }
                }
            }
        }
        #endregion
    }

    public enum ScreenAxisRotation
	{
        SideAxis,
        UpAxis,
        Twist
	}

    public class CalcDistanceException : System.ApplicationException
    {
        public CalcDistanceException()
        {
        }

        public CalcDistanceException(string message)
            : base(message)
        {
        }
    } 
}
