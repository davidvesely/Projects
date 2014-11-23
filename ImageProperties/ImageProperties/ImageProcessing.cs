using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// ********************************************************************************************
//                                        width = cols
//                                       height = rows
// ********************************************************************************************

namespace ImageProperties
{
    public class Filters
    {
        #region data buffers

        byte[] original1, destination1;
        byte[,,] original3, destination3;

        enum ArrayRank { OneDim, ThreeDim };

        ArrayRank rank;

        #endregion

        #region Constructors

        public Filters(byte[] original, byte[] destination)
        {
            rank = ArrayRank.OneDim;
            original1 = original;
            destination1 = destination;
        }

        public Filters(byte[,,] original, byte[,,] destination)
        {
            rank = ArrayRank.ThreeDim;
            original3 = original;
            destination3 = destination;
        }

        public Filters(Bitmap original, Bitmap destination)
        {

        }

        #endregion

        #region enums
        public enum ColorChannels { Red, Green, Blue, RGB, ARGB, Gray };

        public enum Matrix3x3 { Bilinear, Sobel, Corner };
        #endregion

        #region Matrices
        public static readonly 
            int[,] gaussianMatrix = 
            new int[,] { { 1, 4, 6, 4, 1 }, 
                         { 4, 16, 24, 16, 4 },
                         { 6, 24, 36, 24, 6 },
                         { 4, 16, 24, 16, 4 },
                         { 1, 4, 6, 4, 1 } };

        public static readonly 
            int[,] bilinearMatrix = 
            new int[,] { { 1, 2, 1 }, 
                         { 2, 4, 2 },
                         { 1, 2, 1 } };

        public static readonly
            int[,] sobelMatrixVertical =
            new int[,] { { -1, 0, 1 }, 
                         { -2, 0, 2 },
                         { -1, 0, 1 } };

        public static readonly
            int[,] sobelMatrixHorizontal =
            new int[,] { {  1,  2,  1 }, 
                         {  0,  0,  0 },
                         { -1, -2, -1 } };

        public static readonly 
            int[,] cornerMatrix = 
            new int[,] { {  1, -2,  1 }, 
                         { -2,  4, -2 },
                         {  1, -2,  1 } };
        #endregion

        #region 4x4 matrix filters
        // This variation works with three dimensional arrays (used in OpenCV)
        public static void Gaussian(byte[, ,] original, 
            byte[, ,] destination, int rows, int cols, ColorChannels channels) 
        { 
            int sum_r, sum_g, sum_b;
            for (int i = 2; i < rows - 2; i++)
            {
                for (int j = 2; j < cols - 2; j++)
                {
                    sum_r = 0;
                    sum_g = 0;
                    sum_b = 0;
                    for (int ri = -2; ri <= 2; ri++)
                        for (int ci = -2; ci <= 2; ci++)
                        {
                            switch (channels)
                            {
                                case ColorChannels.Red:
                                    sum_r += original[i + ri, j + ci, 2] * gaussianMatrix[ri + 2, ci + 2];
                                    break;
                                case ColorChannels.Green:
                                    sum_g += original[i + ri, j + ci, 1] * gaussianMatrix[ri + 2, ci + 2];
                                    break;
                                case ColorChannels.Blue:
                                    sum_b += original[i + ri, j + ci, 0] * gaussianMatrix[ri + 2, ci + 2];
                                    break;
                                case ColorChannels.RGB:
                                case ColorChannels.ARGB:
                                    sum_r += original[i + ri, j + ci, 2] * gaussianMatrix[ri + 2, ci + 2];
                                    sum_g += original[i + ri, j + ci, 1] * gaussianMatrix[ri + 2, ci + 2];
                                    sum_b += original[i + ri, j + ci, 0] * gaussianMatrix[ri + 2, ci + 2];
                                    break;
                                case ColorChannels.Gray:
                                    sum_b += original[i + ri, j + ci, 0] * gaussianMatrix[ri + 2, ci + 2];
                                    break;
                                default:
                                    break;
                            } // End switch
                        } // End ci
                    // End ri
                    switch (channels)
                    {
                        case ColorChannels.Red:
                            destination[i, j, 2] = (byte)(sum_r / 256f);
                            break;
                        case ColorChannels.Green:
                            destination[i, j, 1] = (byte)(sum_g / 256f);
                            break;
                        case ColorChannels.Gray:
                        case ColorChannels.Blue:
                            destination[i, j, 0] = (byte)(sum_b / 256f);
                            break;
                        case ColorChannels.RGB:
                        case ColorChannels.ARGB:
                            destination[i, j, 2] = (byte)(sum_r / 256f);
                            destination[i, j, 1] = (byte)(sum_g / 256f);
                            destination[i, j, 0] = (byte)(sum_b / 256f);
                            break;
                        default:
                            break;
                    } // End switch
                } // End j
            } // End i
        } // End Gaussian Filter
        #endregion

        #region 3x3 matrix filters
        // This variation works with three dimensional arrays (used in OpenCV)
        public static void Filter3x3(byte[] original, byte[] destination,
            int cols, int rows, PixelFormat channels, Matrix3x3 matrixType)
        {
            int sum_r, sum_g, sum_b;
            int index;
            float divisor = 1;
            int[,] matrix = null;
            switch (matrixType)
            {
                case Matrix3x3.Bilinear:
                    matrix = bilinearMatrix;
                    divisor = 16;
                    break;
                case Matrix3x3.Sobel:
                    matrix = sobelMatrixVertical;
                    divisor = 8;
                    break;
                case Matrix3x3.Corner:
                    matrix = cornerMatrix;
                    divisor = 4;
                    break;
            }
            for (int i = 1; i < rows - 1; i++)
            {
                for (int j = 1; j < cols - 1; j++)
                {
                    sum_r = 0;
                    sum_g = 0;
                    sum_b = 0;
                    for (int hw = -1; hw <= 1; hw++)
                        for (int wi = -1; wi <= 1; wi++)
                        {
                            index = ((i + wi) * cols) + (j + hw);
                            switch (channels)
                            {
                                case PixelFormat.Format24bppRgb:
                                    index *= 3;
                                    break;
                                case PixelFormat.Format32bppArgb:
                                    index *= 4;
                                    break;
                                default:
                                    break;
                            } // End switch
                            switch (channels)
                            {
                                case PixelFormat.Format24bppRgb:
                                case PixelFormat.Format32bppArgb:
                                    sum_r += original[index + 2] * matrix[hw + 1, wi + 1];
                                    sum_g += original[index + 1] * matrix[hw + 1, wi + 1];
                                    sum_b += original[index + 0] * matrix[hw + 1, wi + 1];
                                    break;
                                case PixelFormat.Format8bppIndexed:
                                    sum_b += original[index] * matrix[hw + 1, wi + 1];
                                    break;
                                default:
                                    break;
                            } // End switch
                        } // End ci
                    // End ri
                    index = (i * cols) + j;
                    switch (channels)
                    {
                        case PixelFormat.Format24bppRgb:
                            index *= 3;
                            break;
                        case PixelFormat.Format32bppArgb:
                            index *= 4;
                            break;
                        default:
                            break;
                    } // End switch
                    switch (channels)
                    {
                        case PixelFormat.Format24bppRgb:
                        case PixelFormat.Format32bppArgb:
                            destination[index + 2] = (byte)(sum_r / divisor);
                            destination[index + 1] = (byte)(sum_g / divisor);
                            destination[index + 0] = (byte)(sum_b / divisor);
                            break;
                        case PixelFormat.Format8bppIndexed:
                            destination[index] = (byte)(sum_b / divisor);
                            break;
                        default:
                            break;
                    } // End switch
                } // End j
            } // End i
        } // End Corner Filter
        #endregion
    } // End class Filters

    public class ImageProcessing
    {
        public static string OpenImageFile()
        {
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                string strFileName = string.Empty;
                //diag.InitialDirectory = "c:\\";
                diag.Filter = "bmp files|*.bmp|jpg files (*.jpg)|*.jpg|png files (*.png)|*.png|All files (*.*)|*.*";
                diag.FilterIndex = 4;
                diag.RestoreDirectory = true;

                if (diag.ShowDialog() == DialogResult.OK)
                    strFileName = diag.FileName;

                //if (strFileName == String.Empty)
                //    return; //user didn't select a file to open
                return strFileName;
            }
        }
        
        public static Color InvertColor(Color c)
        {
            return Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
        }

        public static void Sobel(byte [] original, byte[] destination, int width, int height)
        {         
            // Sobel Matrix
            int[,] gx = Filters.sobelMatrixVertical;
            int[,] gy = Filters.sobelMatrixHorizontal;

            int limit = 128 * 128;         

            int new_rx = 0, new_ry = 0;
            int new_gx = 0, new_gy = 0;
            int new_bx = 0, new_by = 0;
            int rc, gc, bc;

            int l;
            int k;

            try
            {
                for (int i = 1; i < height - 1; i++)
                {
                    for (int j = 1; j < width - 1; j++)
                    {
                        new_rx = 0;
                        new_ry = 0;
                        new_gx = 0;
                        new_gy = 0;
                        new_bx = 0;
                        new_by = 0;
                        rc = 0;
                        gc = 0;
                        bc = 0;

                        for (int hw = -1; hw < 2; hw++)
                        {
                            for (int wi = -1; wi < 2; wi++)
                            {
                                k = (((i + wi) * width) + (j + hw)) * 3;
                                rc = original[k + 2];

                                new_rx += gx[hw + 1, wi + 1] * rc;
                                new_ry += gy[hw + 1, wi + 1] * rc;

                                gc = original[k + 1];
                                new_gx += gx[hw + 1, wi + 1] * gc;
                                new_gy += gy[hw + 1, wi + 1] * gc;

                                bc = original[k + 0];
                                new_bx += gx[hw + 1, wi + 1] * bc;
                                new_by += gy[hw + 1, wi + 1] * bc;
                            }
                        }
                        l = (i * width) + j;
                        if (new_rx * new_rx + new_ry * new_ry > limit || new_gx * new_gx + new_gy * new_gy > limit || new_bx * new_bx + new_by * new_by > limit)
                            destination[l] = 255;
                        else
                            destination[l] = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Sobel method");
            }
           
        }

        public static void SobelPointers(Bitmap original, Bitmap destination, int Width, int Height)
        {
            BitmapData bitmapDataOriginal = original.LockBits(
                new Rectangle(0, 0, Width, Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            BitmapData bitmapDataDestination = destination.LockBits(
                new Rectangle(0, 0, Width, Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format8bppIndexed);
            // Sobel Matrix
            int[,] gx = Filters.sobelMatrixVertical;
            int[,] gy = Filters.sobelMatrixHorizontal;

            int limit = 16384; //128 * 128

            int new_rx = 0, new_ry = 0;
            int new_gx = 0, new_gy = 0;
            int new_bx = 0, new_by = 0;
            int rc, gc, bc;

            int l;
            int k;
            //int wi = 0, hw = 0;

            unsafe
            {
                byte* ptrOriginal = (byte*)bitmapDataOriginal.Scan0.ToPointer();
                byte* ptrDestination = (byte*)bitmapDataDestination.Scan0.ToPointer();

                //byte* ptrOriginal1 = (byte*)bitmapDataOriginal.Scan0;
                //byte* ptrDestination1 = (byte*)bitmapDataDestination.Scan0;

                for (int i = 1; i < Height - 1; i++)
                {
                    for (int j = 1; j < Width - 1; j++)
                    {
                        new_rx = 0;
                        new_ry = 0;
                        new_gx = 0;
                        new_gy = 0;
                        new_bx = 0;
                        new_by = 0;
                        rc = 0;
                        gc = 0;
                        bc = 0;

                        for (int hw = -1; hw < 2; hw++)
                        {
                            for (int wi = -1; wi < 2; wi++)
                            {
                                k = (((i + wi) * Width) + (j + hw)) * 3;
                                //rc = allPixR[j + hw, i + wi];
                                rc = ptrOriginal[k + 2];

                                new_rx += gx[hw + 1, wi + 1] * rc;
                                new_ry += gy[hw + 1, wi + 1] * rc;

                                //gc = allPixG[j + hw, i + wi];
                                //gc = original[k + 1];
                                gc = ptrOriginal[k + 1];
                                new_gx += gx[hw + 1, wi + 1] * gc;
                                new_gy += gy[hw + 1, wi + 1] * gc;

                                //bc = allPixB[j + hw, i + wi];
                                //bc = original[k + 0];
                                bc = ptrOriginal[k + 0];
                                new_bx += gx[hw + 1, wi + 1] * bc;
                                new_by += gy[hw + 1, wi + 1] * bc;
                            }
                        }
                        l = (i * Width) + j;
                        //ptrDestination = ptrDestination1 + l;
                        if (new_rx * new_rx + new_ry * new_ry > limit || new_gx * new_gx + new_gy * new_gy > limit || new_bx * new_bx + new_by * new_by > limit)
                            ptrDestination[l] = 255;
                        else
                            ptrDestination[l] = 0;
                        //ptrOriginal+=3;
                        //ptrDestination++;
                    }//j
                }//i
            } //unsafe
            original.UnlockBits(bitmapDataOriginal);
            destination.UnlockBits(bitmapDataDestination);
        }

        public static Bitmap FilterImage(Bitmap original, Filters.Matrix3x3 matrix)
        {
            //PixelFormat format = PixelFormat.Format8bppIndexed;
            PixelFormat format = original.PixelFormat;
            int pixelSize = GetPixelInfo(format);
            int width = original.Width;
            int height = original.Height;
            byte[] grayArr = new byte[width * height * pixelSize];
            byte[] destArr = new byte[width * height * pixelSize];
            Bitmap destination = new Bitmap(original.Width, original.Height, format);
            Bitmap bmpGray = null;
            BitmapData originalGrayData = null;
            BitmapData destinationData = null;
            try
            {
                bmpGray = Grayscale(original);
                originalGrayData = bmpGray.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, format);
                destinationData = destination.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, format);
                ReadBmpData(originalGrayData, grayArr, width, height, pixelSize);
                Filters.Filter3x3(grayArr, destArr, width, height, format, matrix);
                WriteBmpData(destinationData, destArr, width, height, pixelSize);
            }
            finally
            {
                if (originalGrayData != null)
                    bmpGray.UnlockBits(originalGrayData);
                if (destinationData != null)
                    destination.UnlockBits(destinationData);
                if (bmpGray != null)
                    bmpGray.Dispose();
            }
            return destination;
        } // ReadBmpData and WriteBmpData (should be tried with pointers) its also checked with Array class, which cause the process 10x times slower execution

        public static Bitmap Grayscale(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][] 
            {
                new float[] {.3f, .3f, .3f, 0, 0},
                new float[] {.59f, .59f, .59f, 0, 0},
                new float[] {.11f, .11f, .11f, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        } // Very fast with ColorMatrix

        public static Bitmap InvertColor(Bitmap original)
        {

            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //create a blank bitmap the same size as original
            // Bitmap newBitmap = new Bitmap(bmp.Width, bmp.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
                  new float[][]
                {
                   new float[] {-1, 0, 0, 0, 0},
                   new float[] {0, -1, 0, 0, 0},
                   new float[] {0, 0, -1, 0, 0},
                   new float[] {0, 0, 0, 1, 0},
                   new float[] {1, 1, 1, 0, 1}
                });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the invert color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();          

            return newBitmap;

        } // Very fast with ColorMatrix

        public static Bitmap MiddleColor(Bitmap original)
        {
            byte red, green, blue;
            Color c1, c2;
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            FastBitmap originalFast = new FastBitmap(original);
            FastBitmap destinationFast = new FastBitmap(newBitmap);
            originalFast.LockImage();
            destinationFast.LockImage();


            for (int i = 0; i < original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    c1 = originalFast.GetPixel(i, j);

                    red = c1.R;
                    green = c1.G;
                    blue = c1.B;
                    if (red <= green && red <= blue)
                    {
                        c2 = Color.FromArgb(red, green - red, blue - red);
                    }
                    else if (green <= red && green <= blue)
                    {
                        c2 = Color.FromArgb(red - green, green, blue - green);
                    }
                    else if (blue <= red && blue <= green)
                    {
                        c2 = Color.FromArgb(red - blue, green - blue, blue);
                    }
                    else
                    {
                        c2 = Color.FromArgb(red, green, blue);
                    }

                    // c2 = Color.FromArgb((byte)~c1.R, (byte)~c1.G, (byte)~c1.B);

                    destinationFast.SetPixel(i, j, c2);
                }
            }
            originalFast.UnlockImage();
            destinationFast.UnlockImage();
            return newBitmap;
        } // slow

        public static void ReadBmpData(
            BitmapData bmpDataSource,
            byte[] buffer,
            int width,
            int height,
            int pixelSize)
        {
            // Get unmanaged data start address
            int addrStart = bmpDataSource.Scan0.ToInt32();

            for (int i = 0; i < height; i++)
            {
                // Get address of next row
                IntPtr realByteAddr = new IntPtr(addrStart +
                    System.Convert.ToInt32(i * bmpDataSource.Stride));

                // Perform copy from unmanaged memory
                // to managed buffer
                Marshal.Copy(
                    realByteAddr,
                    buffer,
                    (int)(i * width * pixelSize),
                    (int)(width * pixelSize)
                );
            }
        }

        private static int GetPixelInfo(PixelFormat format)
        {
            int pixelSize = 0;
            switch (format)
            {
                case PixelFormat.Format24bppRgb:
                    pixelSize = 3;
                    break;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    pixelSize = 4;
                    break;
                case PixelFormat.Format8bppIndexed:
                    pixelSize = 1;
                    break;
                default:
                    pixelSize = 0;
                    break;
            }
            return pixelSize;
        }

        public static void WriteBmpData(
            BitmapData bmpDataDest,
            byte[] destBuffer,
            int imageWidth,
            int imageHeight,
            int pixelSize)
        {
            // Get unmanaged data start address
            int addrStart = bmpDataDest.Scan0.ToInt32();

            for (int i = 0; i < imageHeight; i++)
            {
                // Get address of next row
                IntPtr realByteAddr = new IntPtr(addrStart +
                    System.Convert.ToInt32(i * bmpDataDest.Stride));

                // Perform copy from managed buffer
                // to unmanaged memory
                Marshal.Copy(
                    destBuffer,
                    i * imageWidth * pixelSize,
                    realByteAddr,
                    imageWidth * pixelSize
                );
            }
        }

        public static Color GetColor(Bitmap imgSource, Point p, int borderPixels)
        {
            int RSum = 0, GSum = 0, BSum = 0;
            int pixelCounter = 0;
            
            for (int i = p.Y - borderPixels; i <= p.Y + borderPixels; i++)
            {
                for (int j = p.X - borderPixels; j <= p.X + borderPixels; j++)
                {
                    Color c = imgSource.GetPixel(j, i);
                    RSum += c.R;
                    GSum += c.G;
                    BSum += c.B;
                    pixelCounter++;
                }
            }
            byte resultR = (byte)(RSum / pixelCounter);
            byte resultG = (byte)(GSum / pixelCounter);
            byte resultB = (byte)(BSum / pixelCounter);
            return Color.FromArgb(resultR, resultG, resultB);
        }
    }
}
