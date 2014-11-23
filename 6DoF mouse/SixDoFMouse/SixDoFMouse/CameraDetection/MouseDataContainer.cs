using CADBest.GeometryNamespace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixDoFMouse
{
    internal class MouseDataContainer
    {
        private static readonly MouseDataContainer instance = new MouseDataContainer();

        private MouseDataContainer() 
        {
            SinCosDescriptor[0] = new Point3D(0, 1, 0);
            SinCosDescriptor[1] = SinCos72;
            SinCosDescriptor[2] = SinCos144;
            SinCosDescriptor[3] = SinCos216;
            SinCosDescriptor[4] = SinCos288;

            ColorSpotSides[0] = ColorSpotsSide01;
            ColorSpotSides[1] = ColorSpotsSide02;
            ColorSpotSides[2] = ColorSpotsSide03;
            ColorSpotSides[3] = ColorSpotsSide04;
            ColorSpotSides[4] = ColorSpotsSide05;
            ColorSpotSides[5] = ColorSpotsSide06;
            ColorSpotSides[6] = ColorSpotsSide07;
            ColorSpotSides[7] = ColorSpotsSide08;
            ColorSpotSides[8] = ColorSpotsSide09;
            ColorSpotSides[9] = ColorSpotsSide10;
            ColorSpotSides[10] = ColorSpotsSide11;
            ColorSpotSides[11] = ColorSpotsSide12;

            OriginAlignParametersOXY = new Point3D[] { p000, p0y0, p0y0, p0y0 };
        }

        public static MouseDataContainer Instance
        {
            get { return instance; }
        }

        private void InitializeColors()
        {
            CustomColors = new List<Color>(600);
        }

        public List<Color> CustomColors;

        public readonly Point3D p000 = new Point3D(0.0, 0.0, 0.0);
        public readonly Point3D p00_1 = new Point3D(0.0, 0.0, -1.0);
        public readonly Point3D p10_1 = new Point3D(1.0, 0.0, -1.0);
        public readonly Point3D p001 = new Point3D(0.0, 0.0, 1.0);
        public readonly Point3D p101 = new Point3D(1.0, 0.0, 1.0);

        public readonly Point3D[] OriginAlignParametersOXY;
        private readonly Point3D p0y0 = new Point3D(0.0, 1.0, 0.0);

        public readonly int[,] indexWeightsDetect = {
            { 0, 1, 2, 3 },
            { 0, 1, 3, 2 },
            { 0, 2, 1, 3 },
            { 0, 2, 3, 1 },
            { 0, 3, 1, 2 },
            { 0, 3, 2, 1 }
        };

        public readonly int[,] indexBinaryPositionReorder = {
            { 0, 1, 2, 3, 4 },
            { 1, 2, 3, 4, 0 },
            { 2, 3, 4, 0, 1 }, 
            { 3, 4, 0, 1, 2 },
            { 4, 0, 1, 2, 3 }
        };

        private readonly Point3D SinCos72 = new Point3D(0.9510565163, 0.3090169944, 0);
        private readonly Point3D SinCos144 = new Point3D(0.5877852523, -0.809016994, 0);
        private readonly Point3D SinCos216 = new Point3D(-0.587785252, -0.809016994, 0);
        private readonly Point3D SinCos288 = new Point3D(-0.951056516, 0.309016994, 0);
        public readonly Point3D[] SinCosDescriptor = new Point3D[5];

        #region Side 1
        // Five probable places of first always black binar spot on Side 1
        public readonly Point3D[] binarPlacesBase = new Point3D[] {
            new Point3D(-3.7105, 16.8971, 31.4164),
            new Point3D(-17.2167, 1.6926, 31.4164),
            new Point3D(-6.9300, -15.8510, 31.4164),
            new Point3D(12.9338, -11.4890, 31.4164),
            new Point3D(14.9235, 8.7504, 31.4164)
        };

        public readonly Point3D[] binarSpotsSide01 = new Point3D[] {
            new Point3D(-3.7105, 16.8971, 31.4164),
            new Point3D(-1.2469, 16.9330, 31.4164),
            new Point3D(1.2200, 16.9688, 31.4164),
            new Point3D(3.6858, 17.0018, 31.4164),
            new Point3D(-3.6786, 14.4326, 31.4164),
            new Point3D(-1.2141, 14.4693, 31.4164),
            new Point3D(1.2503, 14.5043, 31.4164),
            new Point3D(3.7172, 14.5401, 31.4164)
        };
        #endregion

        public readonly Point3D[] ColorSpotsDescriptorSmall = new Point3D[] {
            new Point3D(-11.8513, 16.3120, 0.0),
            new Point3D(-19.1759, -6.2306, 0.0),
            new Point3D(0.0000, -20.1627, 0.0),
            new Point3D(19.1759, -6.2307, 0.0),
            new Point3D(11.8514, 16.3119, 0.0)
        };

        public readonly Point3D[] ColorSpotsSide01 = new Point3D[] {
            new Point3D(-11.8513, 16.3120, 31.4164),
            new Point3D(-19.1759, -6.2306, 31.4164),
            new Point3D(0.0000, -20.1627, 31.4164),
            new Point3D(19.1759, -6.2307, 31.4164),
            new Point3D(11.8514, 16.3119, 31.4164)
        };

        public readonly Point3D[] ColorSpotsSide02 = new Point3D[] {
            new Point3D(-35.3001, -8.6930, 8.4770),
            new Point3D(-23.4488, -4.8423, 28.6397),
            new Point3D(-16.1242, 17.7003, 28.6398),
            new Point3D(-23.4487, 27.7817, 8.4771),
            new Point3D(-35.3001, 11.4697, -3.9842)
        };

        public readonly Point3D[] ColorSpotsSide03 = new Point3D[] {
            new Point3D(2.6408, 36.2587, -8.4770),
            new Point3D(21.8167, 30.0281, 3.9843),
            new Point3D(33.6680, 13.7161, -8.4770),
            new Point3D(21.8167, 9.8654, -28.6397),
            new Point3D(2.6408, 23.7975, -28.6397)
        };

        public readonly Point3D[] ColorSpotsSide04 = new Point3D[] {
            new Point3D(2.6408, -23.7975, 28.6397),
            new Point3D(2.6408, -36.2587, 8.4770),
            new Point3D(21.8167, -30.0281, -3.9842),
            new Point3D(33.6680, -13.7161, 8.4770),
            new Point3D(21.8167, -9.8654, 28.6397)
        };

        public readonly Point3D[] ColorSpotsSide05 = new Point3D[] {
            new Point3D(-23.4487, -27.7817, -8.4770),
            new Point3D(-35.3001, -11.4697, 3.9842),
            new Point3D(-35.3001, 8.6930, -8.4770),
            new Point3D(-23.4488, 4.8423, -28.6397),
            new Point3D(-16.1242, -17.7003, -28.6397)
        };

        public readonly Point3D[] ColorSpotsSide06 = new Point3D[] {
            new Point3D(-11.8513, -16.3120, -31.4164), //6.1
            new Point3D(-19.1759, 6.2306, -31.4164),   //6.2
            new Point3D(0.0000, 20.1627, -31.4164),    //6.3
            new Point3D(19.1759, 6.2306, -31.4164),    //6.4
            new Point3D(11.8514, -16.3120, -31.4164)   //6.5
        };


        public readonly Point3D[] ColorSpotsSide07 = new Point3D[] {
            new Point3D(-2.6408, -23.7975, 28.6397),  //7.1
            new Point3D(-21.8167, -9.8654, 28.6397),  //7.2
            new Point3D(-33.6680, -13.7161, 8.4770),  //7.3
            new Point3D(-21.8167, -30.0281, -3.9842), //7.4
            new Point3D(-2.6408, -36.2587, 8.4770)    //7.5
        };

        public readonly Point3D[] ColorSpotsSide08 = new Point3D[] {                
            new Point3D(-21.8167, 30.0281, 3.9843  ),  //8.1
            new Point3D(-2.6408, 36.2587, -8.4769  ),  //8.2
            new Point3D(-2.6408, 23.7975, -28.6397 ),  //8.3
            new Point3D(-21.8166, 9.8654, -28.6397 ),  //8.4
            new Point3D(-33.6680, 13.7161, -8.4770 )   //8.5
        };

        public readonly Point3D[] ColorSpotsSide09 = new Point3D[] {  
            new Point3D(23.4488, 27.7817, 8.4771),  //9.1
            new Point3D(16.1242, 17.7003, 28.6398), //9.2
            new Point3D(23.4488, -4.8423, 28.6397), //9.3
            new Point3D(35.3001, -8.6930, 8.4770),  //9.4
            new Point3D(35.3001, 11.4697, -3.9842)  //9.5
        };

        public readonly Point3D[] ColorSpotsSide10 = new Point3D[] {
            new Point3D(-11.8513, 20.8047, 28.6398),
            new Point3D(11.8514, 20.8047, 28.6398),
            new Point3D(19.1759, 30.8861, 8.4771),
            new Point3D(0.0000, 37.1167, -3.9842),
            new Point3D(-19.1759, 30.8861, 8.4771)
        };

        public readonly Point3D[] ColorSpotsSide11 = new Point3D[] {
            new Point3D(35.3001, -11.4697, 3.9843),
            new Point3D(23.4488, -27.7817, -8.4770),
            new Point3D(16.1242, -17.7003, -28.6397),
            new Point3D(23.4488, 4.8423, -28.6397),
            new Point3D(35.3001, 8.6930, -8.4770)
        };

        public readonly Point3D[] ColorSpotsSide12 = new Point3D[] {
            new Point3D(19.1759, -30.8861, -8.4770),
            new Point3D(0.0000, -37.1167, 3.9842),
            new Point3D(-19.1759, -30.8861, -8.4770),
            new Point3D(-11.8513, -20.8047, -28.6397),
            new Point3D(11.8514, -20.8047, -28.6397)
        };

        public readonly Point3D[][] ColorSpotSides = new Point3D[12][];
    }
}
