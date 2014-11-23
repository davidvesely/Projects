using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using CADBest.GeometryNamespace;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Collections.Generic;
using System.Windows;
using SixDoFMouse.CameraDetection;
using System.Drawing;
using System.Threading;
using CADBest.AutoCADManagerNamespace;

[assembly: CommandClass(typeof(SixDoFMouse.MyCommands))]

namespace SixDoFMouse
{
    public class MyCommands
    {
        [CommandMethod("PANEL6DOF", CommandFlags.Modal)]
        public void MyCommand()
        {
            FormControls form = new FormControls();
            form.Show();
        }

// These commands and methods are necessary only in debug mode
#if DEBUG
        //[CommandMethod("TESTDIFFERENCE", CommandFlags.Modal)]
        //public void TestDifferentAngles()
        //{
        //    Point3D th = new Point3D(211.3091, 453.1539, 0.0);
        //    Point3D wh = new Point3D(353.5534, 353.5534, 0.0);

        //    List<Point3D> sinCosT = Geometry.GetRotationAngles(th);
        //    double angleT = Geometry.ConvertToDeg(Geometry.CalculateAngle(sinCosT[2]));
        //    Geometry.RotateSinCos(wh, sinCosT, true);
        //    List<Point3D> sinCosW = Geometry.GetRotationAngles(wh);
        //    double angle = Geometry.ConvertToDeg(Geometry.CalculateAngle(sinCosW[2]));
        //}

        [CommandMethod("TESTAVERAGE", CommandFlags.Modal)]
        public void TestAverage()
        {

        }

        [CommandMethod("TESTLOADFRAME", CommandFlags.Modal)]
        public void TestLoadFrame()
        {
            System.Windows.Forms.MessageBox.Show("Select the original image and then the filtered.");
            string fileName = ImageProcessing.OpenImageFile();
            string fileNameFiltered = ImageProcessing.OpenImageFile();
            if ((fileName != string.Empty) && (fileNameFiltered != string.Empty))
            {
                FormStaticHomography staticFrameForm = new FormStaticHomography(fileName, fileNameFiltered);
                staticFrameForm.Show();
            }
        }

        [CommandMethod("TESTLOADFRAME_OLD", CommandFlags.Modal)]
        public void TestLoadFrameOld()
        {
            string fileName = ImageProcessing.OpenImageFile();
            if (fileName != string.Empty)
            {
                FormStaticFrame staticFrameForm = new FormStaticFrame();
                staticFrameForm.SetPicture(fileName);
                staticFrameForm.Show();
            }
        }

        [CommandMethod("TEST_VIEWDIR_X", CommandFlags.Modal)]
        public void TestViewDirX()
        {
            TestViewDir(ScreenAxisRotation.SideAxis);
        }

        [CommandMethod("TEST_VIEWDIR_Y", CommandFlags.Modal)]
        public void TestViewDirY()
        {
            TestViewDir(ScreenAxisRotation.UpAxis);
        }

        [CommandMethod("TEST_VIEWTWIST", CommandFlags.Modal)]
        public void TestViewTwist()
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            ViewTableRecord vtr = doc.Editor.GetCurrentView();

            double angle5deg = 0.0872664626;
            double twist = vtr.ViewTwist;
            while (twist < 2 * Math.PI)
            {
                Thread.Sleep(50);
                twist += angle5deg;
                vtr.ViewTwist = twist;
                doc.Editor.SetCurrentView(vtr);
                doc.Editor.Regen();
            }
        }

        public void TestViewDir(ScreenAxisRotation axis)
        {
            Document doc = AcadApp.DocumentManager.MdiActiveDocument;
            ViewTableRecord vtr = doc.Editor.GetCurrentView();

            Vector3d dirVectorInitial = vtr.ViewDirection;
            Vector3d sideVectorAxis = dirVectorInitial.GetPerpendicularVector();
            // If there is existing ViewTwist angle
            // rotate the side vector with that angle
            // so it will coincide with the side vector
            if (vtr.ViewTwist > 1e-6)
                sideVectorAxis = sideVectorAxis.RotateBy(-vtr.ViewTwist, dirVectorInitial);
            Vector3d upVectorAxis = dirVectorInitial.CrossProduct(sideVectorAxis).GetNormal();

            Vector3d axisRotation = new Vector3d();
            switch (axis)
            {
                case ScreenAxisRotation.SideAxis:
                    axisRotation = sideVectorAxis;
                    break;
                case ScreenAxisRotation.UpAxis:
                    axisRotation = upVectorAxis;
                    break;
                case ScreenAxisRotation.Twist:
                    break;
                default:
                    break;
            }

            double angle5deg = 0.0872664626;

            for (int i = 0; i < 72; i++)
            {
                Thread.Sleep(50);

                // By X axis
                vtr.ViewDirection = vtr.ViewDirection.RotateBy(angle5deg, sideVectorAxis);
                upVectorAxis = upVectorAxis.RotateBy(angle5deg, sideVectorAxis);
                vtr.ViewTwist = AutoCADManager.CalcViewTwist(doc, vtr);
                doc.Editor.SetCurrentView(vtr);

                // By Y axis
                vtr.ViewDirection = vtr.ViewDirection.RotateBy(angle5deg, upVectorAxis);
                sideVectorAxis = sideVectorAxis.RotateBy(angle5deg, upVectorAxis);
                vtr.ViewTwist = AutoCADManager.CalcViewTwist(doc, vtr);
                doc.Editor.SetCurrentView(vtr);

                doc.Editor.Regen();
            }

            angle5deg *= -1;
            for (int i = 72; i > 0; i--)
            {
                Thread.Sleep(50);

                // By Y axis
                vtr.ViewDirection = vtr.ViewDirection.RotateBy(angle5deg, upVectorAxis);
                sideVectorAxis = sideVectorAxis.RotateBy(angle5deg, upVectorAxis);
                vtr.ViewTwist = AutoCADManager.CalcViewTwist(doc, vtr);
                doc.Editor.SetCurrentView(vtr);

                // By X axis
                vtr.ViewDirection = vtr.ViewDirection.RotateBy(angle5deg, sideVectorAxis);
                upVectorAxis = upVectorAxis.RotateBy(angle5deg, sideVectorAxis);
                vtr.ViewTwist = AutoCADManager.CalcViewTwist(doc, vtr);
                doc.Editor.SetCurrentView(vtr);

                doc.Editor.Regen();
            }
        }
#endif
    }
}