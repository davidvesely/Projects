using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SixDoFMouse
{
    public static class AutoCADExtensions
    {
        /// <summary>
        /// Gets the transformation matrix from the current viewport Display Coordinate System (DCS)
        /// to the World Coordinate System (WCS).
        /// </summary>
        /// <param name="ed">The instance to which this method applies.</param>
        /// <returns>The DCS to WCS transformation matrix.</returns>
        public static Matrix3d DCS2WCS(this Editor ed)
        {
            Matrix3d retVal = new Matrix3d();
            bool tilemode = ed.Document.Database.TileMode;
            if (!tilemode)
                ed.SwitchToModelSpace();
            using (ViewTableRecord vtr = ed.GetCurrentView())
            {
                retVal =
                    Matrix3d.Rotation(-vtr.ViewTwist, vtr.ViewDirection, vtr.Target) *
                    Matrix3d.Displacement(vtr.Target - Point3d.Origin) *
                    Matrix3d.PlaneToWorld(vtr.ViewDirection);
            }
            if (!tilemode)
                ed.SwitchToPaperSpace();
            return retVal;
        }

        /// <summary>
        /// Gets the transformation matrix from the World Coordinate System (WCS)
        /// to the current viewport Display Coordinate System (DCS).
        /// </summary>
        /// <param name="ed">The instance to which this method applies.</param>
        /// <returns>The WCS to DCS transformation matrix.</returns>
        public static Matrix3d WCS2DCS(this Editor ed)
        {
            return ed.DCS2WCS().Inverse();
        }
    }
}
