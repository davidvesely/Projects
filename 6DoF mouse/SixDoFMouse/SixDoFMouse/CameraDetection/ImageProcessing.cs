using CADBest.GeometryNamespace;
using SixDoFMouse.CameraDetection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

// ******************************************************************************************** //
//                                        width = cols  x                                       //
//                                       height = rows  y                                       //
// ******************************************************************************************** //

namespace SixDoFMouse
{
    public class ImageProcessing
    {
        private ColorRatios[] Ratios;
        private Color[] BaseColors;

        public HSVCoefficients RedHSVCoef, GreenHSVCoef, BlueHSVCoef, YellowHSVCoef;

        public ImageProcessing()
        {
            byte Ri = (int)FilterColors.R;
            byte Gi = (int)FilterColors.G;
            byte Bi = (int)FilterColors.B;
            byte Yi = (int)FilterColors.Y;
            
            BaseColors = new Color[4];
            BaseColors[Ri] = Color.FromArgb(237, 120, 188);
            BaseColors[Gi] = Color.FromArgb(99, 221, 135);
            BaseColors[Bi] = Color.FromArgb(44, 207, 254);
            BaseColors[Yi] = Color.FromArgb(215, 234, 112);

            Ratios = new ColorRatios[4];
            for (int i = 0; i < 4; i++)
            {
                Ratios[i].RatioRG = (float)BaseColors[i].R / (float)BaseColors[i].G;
                Ratios[i].RatioRB = (float)BaseColors[i].R / (float)BaseColors[i].B;
                Ratios[i].RatioGB = (float)BaseColors[i].G / (float)BaseColors[i].B;
                Ratios[i].LimitRG = 0.5f;
                Ratios[i].LimitRB = 0.5f;
                Ratios[i].LimitGB = 0.5f;
            }

            // Default settings
            RedHSVCoef = new HSVCoefficients(154, 255, 106, 255, 160, 255);
            GreenHSVCoef = new HSVCoefficients(39, 75, 111, 255, 100, 255);
            BlueHSVCoef = new HSVCoefficients(75, 122, 173, 255, 186, 255);
            YellowHSVCoef = new HSVCoefficients(20, 43, 27, 255, 181, 255);
        }

        public static string OpenImageFile()
        {
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                string strFileName = string.Empty;
                diag.Filter = "bmp files|*.bmp|jpg files (*.jpg)|*.jpg|png files (*.png)|*.png|All files (*.*)|*.*";
                diag.FilterIndex = 4;
                diag.RestoreDirectory = true;
                if (diag.ShowDialog() == DialogResult.OK)
                    strFileName = diag.FileName;
                return strFileName;
            }
        }
        
        public static Color InvertColor(Color c)
        {
            return Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
        }

        public static void ConvertXY(List<List<Point3D>> source, int rows)
        {
            for (int i = 0; i < source.Count; i++)
                ConvertXY(source[i], rows);
        }

        public static void ConvertXY(List<Point3D> source, int rows)
        {
            for (int i = 0; i < source.Count; i++)
                ConvertXY(source[i], rows);
        }

        public static void ConvertXY(Point3D sourceP, int rows)
        {
            sourceP.Y = rows - sourceP.Y;
        }

        private static bool IsBetween<T>(T number, T start, T end) where T : struct, IComparable<T>
        {
            if ((number.CompareTo(start) >= 0) && (number.CompareTo(end) <= 0))
                return true;
            else
                return false;
        }

        public static Color GetColor(byte[, ,] imgSource, WeightCenter p, int borderPixels)
        {
            int RSum = 0, GSum = 0, BSum = 0;
            int pixelCounter = 0;
            for (int i = p.y - borderPixels; i <= p.y + borderPixels; i++)
            {
                for (int j = p.x - borderPixels; j <= p.x + borderPixels; j++)
                {
                    RSum += imgSource[i, j, 2];
                    GSum += imgSource[i, j, 1];
                    BSum += imgSource[i, j, 0];
                    pixelCounter++;
                }
            }
            byte resultR = (byte)(RSum / pixelCounter);
            byte resultG = (byte)(GSum / pixelCounter);
            byte resultB = (byte)(BSum / pixelCounter);
            return Color.FromArgb(resultR, resultG, resultB);
        }

        private const int MAX_MIN_SUMS = 10;

        /// <summary>
        /// Distance between two points without square rooting
        /// </summary>
        /// <param name="P1">First point</param>
        /// <param name="P2">Second point</param>
        /// <returns>Distance without square root</returns>
        public static double CustomDistance(Point3D P1, Point3D P2)
        {
            return Pow2(P1.X - P2.X) +
                Pow2(P1.Y - P2.Y) +
                Pow2(P1.Z - P2.Z);
        }

        private static double Pow2(double n)
        {
            return n * n;
        }

        private const int WHITE_COUNT = 10;

        public static Vector3D GetWhiteColor(byte[, ,] dataSource, int rows, int cols)
        {
            List<PixelData> whiteColors = new List<PixelData>(WHITE_COUNT);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    byte R = dataSource[i, j, 2];
                    byte G = dataSource[i, j, 1];
                    byte B = dataSource[i, j, 0];
                    int sum = R + G + B;
                    int index = whiteColors.Count - 1;
                    if ((whiteColors.Count == 0) || (whiteColors[index].Sum <= sum))
                    {
                        whiteColors.Add(new PixelData(R, G, B, sum));
                        if (index == (WHITE_COUNT - 1))
                        {
                            whiteColors.RemoveAt(0);
                        }
                    }
                }
            }

            double redSum = 0, greenSum = 0, blueSum = 0;
            foreach (PixelData pixel in whiteColors)
            {
                redSum += pixel.R;
                greenSum += pixel.G;
                blueSum += pixel.B;
            }

            redSum /= WHITE_COUNT;
            greenSum /= WHITE_COUNT;
            blueSum /= WHITE_COUNT;

            Vector3D result = new Vector3D(redSum, greenSum, blueSum);
            return result;
        }

        public void ImageProcessHSV(byte[, ,] dataOriginal, int rows, int cols)
        {
            //HSVCoefficients[] coefficients = new HSVCoefficients[] {
            //    RedHSVCoef, GreenHSVCoef, BlueHSVCoef, YellowHSVCoef
            //};

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    byte H = dataOriginal[i, j, 0];
                    byte S = dataOriginal[i, j, 1];
                    byte V = dataOriginal[i, j, 2];

                    if (IsBetween<byte>(H, RedHSVCoef.HueDown, RedHSVCoef.HueUp) &&
                        IsBetween<byte>(S, RedHSVCoef.SatDown, RedHSVCoef.SatUp) &&
                        IsBetween<byte>(V, RedHSVCoef.ValueDown, RedHSVCoef.ValueUp))
                    {
                        continue;
                    }
                    else if (IsBetween<byte>(H, GreenHSVCoef.HueDown, GreenHSVCoef.HueUp) &&
                      IsBetween<byte>(S, GreenHSVCoef.SatDown, GreenHSVCoef.SatUp) &&
                      IsBetween<byte>(V, GreenHSVCoef.ValueDown, GreenHSVCoef.ValueUp))
                    {
                        continue;
                    }
                    else if (IsBetween<byte>(H, BlueHSVCoef.HueDown, BlueHSVCoef.HueUp) &&
                      IsBetween<byte>(S, BlueHSVCoef.SatDown, BlueHSVCoef.SatUp) &&
                      IsBetween<byte>(V, BlueHSVCoef.ValueDown, BlueHSVCoef.ValueUp))
                    {
                        continue;
                    }
                    else if (IsBetween<byte>(H, YellowHSVCoef.HueDown, YellowHSVCoef.HueUp) &&
                      IsBetween<byte>(S, YellowHSVCoef.SatDown, YellowHSVCoef.SatUp) &&
                      IsBetween<byte>(V, YellowHSVCoef.ValueDown, YellowHSVCoef.ValueUp))
                    {
                        continue;
                    }
                    else
                    {
                        dataOriginal[i, j, 2] = 0; // Value is set to black
                    }
                }
            }
        }

        // Images that will be visualized in the main form
        // They are linked on initializing the main form
        public byte[, ,] DataR, DataG, DataB, DataY;

        public void ImageProcess(byte[,,] dataOriginal, int rows, int cols, int downLevel, int upLevel)
        {
            if ((DataR == null) || (DataG == null) ||
                (DataB == null) || (DataY == null))
                return;

            for (int i = 0; i < rows; i++) // Rows
            {
                for (int j = 0; j < cols; j++) // Cols
                {
                    byte R = CheckForZero(dataOriginal[i, j, 2]);
                    byte G = CheckForZero(dataOriginal[i, j, 1]);
                    byte B = CheckForZero(dataOriginal[i, j, 0]);

                    //PixelNormalization(ref R, ref G, ref B, downLevel, upLevel);
                    Differences(ref R, ref G, ref B);
                    GrayFilter(ref R, ref G, ref B, 48, 96);
                    PixelFilter(ref R, ref G, ref B, downLevel, upLevel);
                    

                    //float RatioRG = (float)R / (float)G;
                    //float RatioRB = (float)R / (float)B;
                    //float RatioGB = (float)G / (float)B;

                    dataOriginal[i, j, 2] = R;
                    dataOriginal[i, j, 1] = G;
                    dataOriginal[i, j, 0] = B;

                    //RatioSeparation(dataOriginal, DataR, i, j, RatioRG, RatioRB, RatioGB, FilterColors.R);
                    //RatioSeparation(dataOriginal, DataG, i, j, RatioRG, RatioRB, RatioGB, FilterColors.G);
                    //RatioSeparation(dataOriginal, DataB, i, j, RatioRG, RatioRB, RatioGB, FilterColors.B);
                    //RatioSeparation(dataOriginal, DataY, i, j, RatioRG, RatioRB, RatioGB, FilterColors.Y);
                }
            }
        }

        Vector3D NormalizationVector = new Vector3D();

        private void PixelNormalization(ref byte R, ref byte G,
            ref byte B, int downLevel, int upLevel)
        {
            Vector3D vect = NormalizationVector;
            int sum = R + G + B;

            if ((sum < downLevel) || (sum > upLevel))
            {
                R = G = B = 0;
            }
            else
            {
                vect.SetCoordinates(R, G, B);
                vect.Normalize();

                R = (byte)Math.Round(vect.X * 128);
                G = (byte)Math.Round(vect.Y * 128);
                B = (byte)Math.Round(vect.Z * 128);
            }
        }

        private void PixelFilter(ref byte R, ref byte G,
            ref byte B, int downLevel, int upLevel)
        {
            if ((R > upLevel) && (G > upLevel) && (B > upLevel))
            {
                R = 255;
                G = 255;
                B = 255;
            }
            if ((R < downLevel) && (G < downLevel) && (B < downLevel))
            {
                R = 0;
                G = 0;
                B = 0;
            }
        }

        private void GrayFilter(ref byte R, ref byte G,
            ref byte B, int downLevel, int upLevel)
        {
            if ((R > downLevel) && (R < upLevel) &&
                (G > downLevel) && (G < upLevel) &&
                (B > downLevel) && (B < upLevel))
            {
                R = 0;
                G = 0;
                B = 0;
            }
        }

        private void Differences(ref byte R, ref byte G, ref byte B)
        {
            double RG = Math.Abs(R - G);
            double RB = Math.Abs(R - B);
            double GB = Math.Abs(G - B);

            if ((RG < 20) && (RB < 20) && (GB < 20))
            {
                R = 0;
                G = 0;
                B = 0;
            }

            int a = R - G;
            int b = G - B;
            if (((Math.Abs(a - b) < 20) && (R > G) && (R > 10)))
            {
                R = 0;
                G = 0;
                B = 0;
            }
        }

        private void RatioSeparation(byte[,,] sourceData, byte[, ,] destData, int row, int col, float RatioRG, float RatioRB, float RatioGB, FilterColors color)
        {
            if ((CompareRatios(RatioRG, Ratios[(int)color].RatioRG, Ratios[(int)color].LimitRG)) &&
                (CompareRatios(RatioRB, Ratios[(int)color].RatioRB, Ratios[(int)color].LimitRB)) &&
                (CompareRatios(RatioGB, Ratios[(int)color].RatioGB, Ratios[(int)color].LimitGB)))
            {
                destData[row, col, 2] = sourceData[row, col, 2];
                destData[row, col, 1] = sourceData[row, col, 1];
                destData[row, col, 0] = sourceData[row, col, 0];
            }
            else
            {
                destData[row, col, 2] = 0;
                destData[row, col, 1] = 0;
                destData[row, col, 0] = 0;
            }
        }

        private bool CompareRatios(float ratio, float ratioBase, float ratioLimit)
        {
            return IsBetween<float>(ratio, ratioBase - ratioLimit, ratioBase + ratioLimit);
        }

        // Add 1 to every R, G, B component if such is equal to zero
        private byte CheckForZero(byte c)
        {
            if (c == 0)
                return 1;
            else
                return c;
        }
    } // End class
} // End namespace
