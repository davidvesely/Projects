using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.GraphicsSystem;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadColor = Autodesk.AutoCAD.Colors.Color;

namespace CADBest.GeometryNamespace
{
    public class VisualizationACAD
    {
        private delegate void DrawPolylinesDelegate(List<Point3D> Objects, bool ShouldFlush, Color color);
        private delegate void FlushPolylinesDelegate();

        public static Control SyncControl;

        public VisualizationACAD()
        {
            SyncControl = new Control();
            SyncControl.CreateControl();
        }

        private List<ObjectId> DrawnObjectsId = new List<ObjectId>();

        public void FlushPolylinesQueue()
        {
            // If this method is called from different thread, InvokeRequired will be true
            if (SyncControl.InvokeRequired)
                SyncControl.Invoke(new FlushPolylinesDelegate(FlushPolylinesQueue), new object[] { });
            else
            {
                Document doc = AcadApp.DocumentManager.MdiActiveDocument;
                using (DocumentLock docLock = doc.LockDocument())
                {
                    Database currentDb = doc.Database;
                    using (Transaction trans = currentDb.TransactionManager.StartTransaction())
                    {
                        foreach (ObjectId objectId in DrawnObjectsId)
                        {
                            Entity ent = (Entity)trans.GetObject(objectId, OpenMode.ForWrite);
                            ent.Erase();
                            ent.Dispose();
                        }
                        DrawnObjectsId.Clear();
                        trans.Commit();
                    }
                }
            }
        }

        public void DrawPolylines(List<Point3D> Objects, bool ShouldFlush, Color color)
        {
            if (SyncControl.InvokeRequired)
                SyncControl.Invoke(new DrawPolylinesDelegate(DrawPolylines), new object[] { Objects, ShouldFlush, color });
            else
            {
                Document doc = AcadApp.DocumentManager.MdiActiveDocument;
                using (DocumentLock docLock = doc.LockDocument())
                {
                    Database currentDb = doc.Database;
                    using (Transaction trans = currentDb.TransactionManager.StartTransaction())
                    {
                        // Deleting the previous drawn objects
                        if (ShouldFlush)
                            FlushPolylinesQueue();

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
                        pl.Color = AcadColor.FromColor(color);

                        blockTblRec.AppendEntity(pl);
                        trans.AddNewlyCreatedDBObject(pl, true);
                        trans.Commit();

                        // Updates the screen without flickering of view
                        AcadApp.UpdateScreen(); // or doc.Editor.Regen();
                    }
                }
            }
        }
    }
}
