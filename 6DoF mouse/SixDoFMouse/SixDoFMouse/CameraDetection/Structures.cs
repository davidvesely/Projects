using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SixDoFMouse.CameraDetection
{
    /// <summary>
    /// Coefficients used during filtration. IM1... and IM2... 
    /// coefficients must be set to 1 on initialization, otherwise RGB filtration will not work
    /// </summary>
    public struct Filteringcoefficients
    {
        public double IM_R_K;// IM1_G, /* <- should be set to 1 -> */ IM1_B;
        public double IM_G_K;// IM2_B, /* <- should be set to 1 -> */ IM2_R;
        public double IM_B_K;// IM3_R, /* <- should be set to 1 -> */ IM3_G;
        public double IM_Y_K;
        public int IM_R_PRAG, IM_R_ADD;
        public int IM_G_PRAG, IM_G_ADD;
        public int IM_B_PRAG, IM_B_ADD;
        public int IM_Y_PRAG, IM_Y_ADD;
    }

    public class HSVCoefficients
    {
        public byte HueDown, HueUp;
        public byte SatDown, SatUp;
        public byte ValueDown, ValueUp;

        public HSVCoefficients()
            : this(0, 0, 0, 0, 0, 0)
        {
        }

        public HSVCoefficients(byte hueDown, byte hueUp,
            byte satDown, byte satUp, byte valueDown, byte valueUp)
        {
            HueDown = hueDown;
            HueUp = hueUp;
            SatDown = satDown;
            SatUp = satUp;
            ValueDown = valueDown;
            ValueUp = valueUp;
        }
    }

    public struct ColorRatios
    {
        // Ratio between the colors
        public float RatioRG, RatioRB, RatioGB;
        // A border in which the ratio will be seek
        public float LimitRG, LimitRB, LimitGB;
    }

    public enum FilterColors { B = 0, G = 1, R = 2, Y = 3 };

    public struct PixelData
    {
        public int R, G, B;
        public int Sum;

        public PixelData(int aR, int aG, int aB)
        {
            R = aR;
            G = aG;
            B = aB;
            Sum = R + G + B;
        }

        public PixelData(int aR, int aG, int aB, int aSum)
        {
            R = aR;
            G = aG;
            B = aB;
            Sum = aSum;
        }

        public override string ToString()
        {
            return string.Format("Sum = {0}", Sum);
        }
    }

    public class PixelDataComparer : IComparer<PixelData>
    {
        public int Compare(PixelData pix1, PixelData pix2)
        {
            return pix1.Sum.CompareTo(pix2.Sum);
        }
    }

    public struct Strip
    {
        public int start;
        public int end;
        public Strip(int aStart, int aEnd)
        {
            start = aStart;
            end = aEnd;
        }
    }

    public struct WeightCenter
    {
        public int x, y;
        // Used for indicating on which place is
        // located the always black first spot
        public int rowStart;
        public WeightCenter(int aX, int aY, int aRowStart)
        {
            x = aX;
            y = aY;
            rowStart = aRowStart;
        }

        public override string ToString()
        {
            return String.Format("x: {0}, y: {1}", x, y);
        }

        public override bool Equals(object w)
        {
            return this.Equals((WeightCenter)w);
        }

        public bool Equals(WeightCenter w)
        {
            // Return true if the fields match:
            return (this.x == w.x) &&
                (this.x == w.y) &&
                (this.rowStart == w.rowStart);
        }

        public static bool operator ==(WeightCenter p1, WeightCenter p2)
        {
            // If one is null, but not both, return false.
            if (((object)p1 == null) || ((object)p2 == null))
            {
                return false;
            }

            // Return true if the fields match:
            return p1.Equals(p2);
        }

        public static bool operator !=(WeightCenter p1, WeightCenter p2)
        {
            return !(p1 == p2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static readonly WeightCenter Empty = new WeightCenter();
    }
}
