using Autodesk.AutoCAD.DatabaseServices;
using CADBest.GeometryNamespace;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SixDoFMouse.CameraDetection
{
    public class Calibration
    {
        // Orientation of first calibration frame
        public Point3D xSinCosRot, ySinCosRot, zSinCosRot;
        // Used for transformation between two coordinate systems
        public Point3D p0translation;
        public Point3D xSinCosAxis, ySinCosAxis, zSinCosAxis;
        // For transformation of coordinate systems
        public double xScale, yScale, zScale;
        private const int X_SCALE_K = 400, Y_SCALE_K = 300;

        public bool IsCalibrate = false;

        MouseDataContainer dataMouse = MouseDataContainer.Instance;

        // The storage for three descriptor orientations
        // (middle of our visual coordinate system, right X middle, and top Y middle)
        public List<List<Point3D>> CalibrationOrientation;
        public Point3D[] AlignParamCalibration;  

        public Calibration()
        {
            CalibrationOrientation = new List<List<Point3D>>();
        }

        /// <summary>
        /// Find the orientation of the mouse towards the camera
        /// </summary>
        /// <param name="ViewPointAverage">Detected orientation of the camera</param>
        /// <returns>Coordinate system of descriptor towards the camera</returns>
        public List<Point3D> RecalcOrientation(List<Point3D> ViewPointAverage)
        {
            Point3D center = new Point3D(0, 0, 0);
            Point3D xAxis = new Point3D(100, 0, 0);
            Point3D yAxis = new Point3D(0, 100, 0);
            Point3D zAxis = new Point3D(0, 0, 100);
            List<Point3D> coordSystemDescriptor = new List<Point3D>(4);
            coordSystemDescriptor.Add(center);
            coordSystemDescriptor.Add(xAxis);
            coordSystemDescriptor.Add(yAxis);
            coordSystemDescriptor.Add(zAxis);

            // Aligning 3 points from randomly chose side of descriptor (currently side 1)
            Point3D[] sourcePars = Geometry.align_calc_pars(
                ViewPointAverage[0],
                ViewPointAverage[1],
                ViewPointAverage[2]);
            Point3D[] destPars = Geometry.align_calc_pars(
                dataMouse.p000,   // 0, 0,  0
                dataMouse.p00_1,  // 0, 0, -1
                dataMouse.p10_1); // 1, 0, -1

            List<Point3D> DrawViewPoint =  Geometry.CloneList(ViewPointAverage);
            Geometry.align(DrawViewPoint, sourcePars, destPars);
            //if (DrawTestEvent != null)
                //DrawTestEvent(this, new DrawViewPointEventArgs(DrawViewPoint, true));

            List<Point3D> al1 = new List<Point3D>(4);
            al1.Add(dataMouse.ColorSpotsSide01[0].Clone());
            al1.Add(dataMouse.ColorSpotsSide01[1].Clone());
            al1.Add(dataMouse.ColorSpotsSide01[2].Clone());
            //al1.Add(dataMouse.ColorSpotsSide01[3].Clone());
            Geometry.align(al1, sourcePars, destPars);

            sourcePars = Geometry.align_calc_pars(
                dataMouse.ColorSpotsSide01[0],
                dataMouse.ColorSpotsSide01[1],
                dataMouse.ColorSpotsSide01[2]);
            destPars = Geometry.align_calc_pars(al1.ToArray());
            Geometry.align(coordSystemDescriptor, sourcePars, destPars);
                        
            return coordSystemDescriptor;
        }

        public void CalcCalibration(List<Point3D> ViewPoint, byte[, ,] ImgDataToFlash)
        {
            // If a new calibration is started
            if (CalibrationOrientation.Count == 3)
            {
                CalibrationOrientation = new List<List<Point3D>>();
                p0translation = null;
            }

            List<Point3D> orientationPlace = RecalcOrientation(ViewPoint);
            CalibrationOrientation.Add(orientationPlace);
            IsCalibrate = false;

            // For flash in the original image
            for (int i = 0; i < ImgDataToFlash.GetLength(0); i++)
                for (int j = 0; j < ImgDataToFlash.GetLength(1); j++)
                    ImgDataToFlash[i, j, 0] = ImgDataToFlash[i, j, 1] = ImgDataToFlash[i, j, 2] = 255;

            // If there are collected three calibration positions the parameters are calculated
            if (CalibrationOrientation.Count == 3)
            {
                CalibrationOrientation[2][0].X -= (CalibrationOrientation[1][0].X - CalibrationOrientation[0][0].X);
                CalibrationOrientation[2][0].Y -= (CalibrationOrientation[1][0].Y - CalibrationOrientation[0][0].Y);
                CalibrationOrientation[2][0].Z -= (CalibrationOrientation[1][0].Z - CalibrationOrientation[0][0].Z);
                CalibrationCalcParameters();
                System.Windows.Forms.MessageBox.Show("Calibration successful");
            }
        }

        //private void OnDrawTest(Point3D[] objects, Autodesk.AutoCAD.Colors.Color color)
        //{
        //    if ((DrawTestEvent != null) && (CalibrationOrientation.Count == 3))
        //    {
        //        DrawPolylineEventArgs e =
        //            new DrawPolylineEventArgs(Geometry.CloneList(new List<Point3D>(objects)), false, color);
        //        DrawTestEvent(this, e);
        //    }
        //}

        /// <summary>
        /// Calculate Align calibration parameters.
        /// Align the screen coordinate system to WCS, together with the orientation from first calibration frame.
        /// </summary>
        public void CalibrationCalcParameters()
        {
            // Transformation of coordinate systems
            p0translation = CalibrationOrientation[0][0].Clone();
            List<Point3D> customCoordinateSystem = new List<Point3D>();
            customCoordinateSystem.Add(CalibrationOrientation[0][0].Clone()); // [0] First frame center
            customCoordinateSystem.Add(CalibrationOrientation[1][0].Clone()); // [1] Second frame center
            customCoordinateSystem.Add(CalibrationOrientation[2][0].Clone()); // [2] Third frame center
            customCoordinateSystem.Add(CalibrationOrientation[0][1].Clone()); // [3] First frame X
            customCoordinateSystem.Add(CalibrationOrientation[0][2].Clone()); // [4] First frame Y
            customCoordinateSystem.Add(CalibrationOrientation[0][3].Clone()); // [5] First frame Z

            // Scale factors of X and Y axis, Z will be set randomly by us
            //xScale = p1Calibration.X / X_SCALE_K;

            //yScale = p2Calibration.Y / Y_SCALE_K;

            // Align the screen coordinate system to WCS,
            // together with the orientation from first calibration frame
            Geometry.p_add(customCoordinateSystem, p0translation, -1);
            ySinCosAxis = Geometry.calc_rotY(customCoordinateSystem[1]);
            Geometry.p_rotY(customCoordinateSystem, ySinCosAxis, -1);
            zSinCosAxis = Geometry.calc_rotZ(customCoordinateSystem[1]);
            Geometry.p_rotZ(customCoordinateSystem, zSinCosAxis, -1);
            xSinCosAxis = Geometry.calc_rotX(customCoordinateSystem[2]);
            Geometry.p_rotX(customCoordinateSystem, xSinCosAxis, -1);

            //Move calibration to origin and calculate align parameters
            List<Point3D> CopyCalib = Geometry.CloneList(CalibrationOrientation[0]);
            Geometry.p_add(CopyCalib, CopyCalib[0].Clone(), -1);
            AlignParamCalibration = Geometry.align_calc_pars(
            customCoordinateSystem[0],
            customCoordinateSystem[3],
            customCoordinateSystem[4]);

            // Determine the orientation of mouse in first calibration frame
            //List<Point3D> firstCaseCoordinates = new List<Point3D>();
            //firstCaseCoordinates.Add(customCoordinateSystem[0]);
            //firstCaseCoordinates.Add(customCoordinateSystem[3]);
            //firstCaseCoordinates.Add(customCoordinateSystem[4]);
            //firstCaseCoordinates.Add(customCoordinateSystem[5]);

            //List<Point3D> orientationCalibration =
            //    MouseRecognition.GetRotationAngles(firstCaseCoordinates);
            //xSinCosRot = orientationCalibration[0];
            //ySinCosRot = orientationCalibration[1];
            //zSinCosRot = orientationCalibration[2];

          

            // For testing
            //Geometry.p_rotY(firstCaseCoordinates, ySinCosRot, -1);
            //Geometry.p_rotZ(firstCaseCoordinates, zSinCosRot, -1);
            //Geometry.p_rotX(firstCaseCoordinates, xSinCosRot, -1);

            //OnDrawTest(new Point3D[] { firstCaseCoordinates[0], firstCaseCoordinates[1] }, Autodesk.AutoCAD.Colors.Color.FromRgb(255, 0, 0));
            //OnDrawTest(new Point3D[] { firstCaseCoordinates[0], firstCaseCoordinates[2] }, Autodesk.AutoCAD.Colors.Color.FromRgb(0, 255, 0));
            //OnDrawTest(new Point3D[] { firstCaseCoordinates[0], firstCaseCoordinates[3] }, Autodesk.AutoCAD.Colors.Color.FromRgb(0, 0, 255));

            //OnDrawTest(new Point3D[] { customCoordinateSystem[0], customCoordinateSystem[1] }, Autodesk.AutoCAD.Colors.Color.FromRgb(255, 255, 255));
            //OnDrawTest(new Point3D[] { customCoordinateSystem[0], customCoordinateSystem[2] }, Autodesk.AutoCAD.Colors.Color.FromRgb(255, 255, 255));
        }

        public void ReadRegistryCalibration(RegistryKey rk)
        {
            if ((string)rk.GetValue("Calibration00", "a") != "a")
            {
                CalibrationOrientation = new List<List<Point3D>>(3);
                for (int i = 0; i < 3; i++)
                {
                    List<Point3D> calPosition = new List<Point3D>(4);
                    for (int j = 0; j < 4; j++)
                    {
                        string pointStr = (string)rk.GetValue("Calibration" + i.ToString() + j.ToString());
                        pointStr = pointStr.Replace(" ", "");
                        string[] coordinates = pointStr.Split(',');
                        Point3D point = new Point3D();
                        point.X = double.Parse(coordinates[0]);
                        point.Y = double.Parse(coordinates[1]);
                        point.Z = double.Parse(coordinates[2]);
                        calPosition.Add(point);
                    }
                    CalibrationOrientation.Add(calPosition);
                }
                CalibrationCalcParameters();
            }
        }

        public void WriteRegistryCalibration(RegistryKey rk)
        {
            // Calibration orientation
            if (CalibrationOrientation != null)
                for (int i = 0; i < CalibrationOrientation.Count; i++)
                {
                    for (int j = 0; j < CalibrationOrientation[i].Count; j++)
                    {
                        rk.SetValue("Calibration" + i.ToString() + j.ToString(),
                            CalibrationOrientation[i][j].ToString());
                    }
                }
        }
    }
}