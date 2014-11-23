using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CADBest.GeometryNamespace
{
    public class GeometryTests
    {
        public static List<List<Point3D>> AlignTest(List<List<Point3D>> SourceObjects)
        {
            List<List<Point3D>> resultObjects = new List<List<Point3D>>();
            Point3D[] sourceOrientPoints = SourceObjects[0].ToArray();
            Point3D[] destinOrientPoints = SourceObjects[1].ToArray();
            Point3D[] sourceOrientation, destOrientation;

            sourceOrientation = Geometry.align_calc_pars(sourceOrientPoints);
            destOrientation = Geometry.align_calc_pars(destinOrientPoints);

            for (int i = 2; i < SourceObjects.Count; i++)
            {
                Geometry.align(SourceObjects[i], sourceOrientation, destOrientation);
                resultObjects.Add(SourceObjects[i]);
            }
            return resultObjects;
        }

        public static List<List<Point3D>> AffineTest(List<List<Point3D>> SourceObjects)
        {
            List<List<Point3D>> resultObjects = new List<List<Point3D>>();
            List<Point3D> sourceOrientPoints = SourceObjects[0];
            List<Point3D> destinOrientPoints = SourceObjects[1];
            List<Point3D> sourceOrientation = null, destOrientation = null;

            sourceOrientation = Geometry.affine_calc_pars(sourceOrientPoints);
            destOrientation = Geometry.affine_calc_pars(destinOrientPoints);

            for (int i = 2; i < SourceObjects.Count; i++)
            {
                resultObjects.Add(Geometry.affine_calc(SourceObjects[i], sourceOrientation, destOrientation));
            }
            return resultObjects;
        }

        public static List<List<Point3D>> FourPCalcParsTest(List<List<Point3D>> SourceObjects)
        {
            List<List<Point3D>> resultObjects = new List<List<Point3D>>();
            List<Point3D> sourceOrientPoints = SourceObjects[0];
            //List<Point3D> destinOrientPoints = SourceObjects[1];
            List<Point3D> sourceOrientation = null;

            sourceOrientation = Geometry.homography_calc_pars(sourceOrientPoints);

            resultObjects.Add(sourceOrientation);
            return resultObjects;
        }

        public static List<List<Point3D>> Homography(List<List<Point3D>> SourceObjects)
        {
            List<List<Point3D>> resultObjects = new List<List<Point3D>>();
            List<Point3D> sourceOrientPoints = SourceObjects[0];
            List<Point3D> destinOrientPoints = SourceObjects[1];
            List<Point3D> sourceOrientation = null, destOrientation = null;

            sourceOrientation = Geometry.homography_calc_pars(sourceOrientPoints);
            destOrientation = Geometry.homography_calc_pars(destinOrientPoints);


            for (int i = 2; i < SourceObjects.Count; i++)
            {
                resultObjects.Add(Geometry.homography_calc(SourceObjects[i], sourceOrientation, destOrientation));
            }
            return resultObjects;
        }
    }
}
