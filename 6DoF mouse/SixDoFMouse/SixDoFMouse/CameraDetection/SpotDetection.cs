using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SixDoFMouse.CameraDetection
{
    public class SpotDetection
    {
        /// <summary>
        /// Mark strips with valid pixels by rows or columns
        /// </summary>
        /// <param name="source">Image, which is checked for valid pixels</param>
        /// <param name="StripDirection">Count of pixels in the current direction (rows or cols)</param>
        /// <param name="elements">Count of pixels in the opposite direction (cols or rows)</param>
        /// <param name="color">Indicates which color is being processed</param>
        /// <param name="isRows">For processing in direction of rows, needs to be set true</param>
        /// <returns>List of founded strips</returns>
        public static List<Strip> FindStrips(byte[, ,] source, int StripDirection, int elements, FilterColors color, bool isRows)
        {
            List<Strip> strip = new List<Strip>(20);
            int j;
            int start, end;
            for (int i = 0; i < StripDirection; i++)
            {
                j = ScanPixels(source, elements, i, color, isRows);
                if (j < elements) //If a pixel is found
                {
                    start = i; //Mark its start
                    do //Find the last row with pixels
                    {
                        i++;
                        if (i == StripDirection)
                            break;
                        j = ScanPixels(source, elements, i, color, isRows);
                    } while (j < elements); // if j == cols -> therefore we've found an empty row
                    end = i - 1; //Mark the end of a strip with last row with pixels // strip[currentStrip].
                    if ((end - start) > 5) //If the strip is with normal width > 5 rows jump to next strip
                        strip.Add(new Strip(start, end));
                }
            }
            return strip;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="elements"></param>
        /// <param name="i"></param>
        /// <param name="color"></param>
        /// <param name="isRows"></param>
        /// <returns></returns>
        private static int ScanPixels(byte[, ,] source, int elements, int i, FilterColors color, bool isRows)
        {
            int j = 0;
            switch (isRows)
            {
                case true:
                    // Find pixels and stop until first founded on the current row
                    while ((j < elements) && (source[i, j, (int)color] != 255))
                        j++;
                    break;
                case false:
                    // Find pixels and stop until first founded on the current col
                    while ((j < elements) && (source[j, i, (int)color] != 255))
                        j++;
                    break;
            }
            return j;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="original"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static List<WeightCenter> FindSpots(byte[, ,] source, byte[, ,] original, FilterColors color,int FrameRows, int FrameCols)
        {
            int x1, y1, x2, y3;
            List<WeightCenter> weights = new List<WeightCenter>(20);
            WeightCenter w;
            List<Strip> RowStrips = FindStrips(source, FrameRows, FrameCols, color, true); //True for rows
            List<Strip> ColStrips = FindStrips(source, FrameCols, FrameRows, color, false); //False for cols      

            foreach (Strip row in RowStrips)
            {
                foreach (Strip col in ColStrips)
                {
                    x1 = col.start;
                    y1 = row.start;
                    x2 = col.end;
                    y3 = row.end;

                    w = CalculateWeightCenter(source, x1, y1, x2 - x1 + 1, y3 - y1 + 1, color);
                    if ((w.x != 0) && (w.y != 0))
                    {
                        weights.Add(w);
                    }
                }
            }

            //Visualization.VisualizeStrips(source, RowStrips, ColStrips, FrameRows, FrameCols);

            if (weights.Count == 5)
            {
                Visualization.VisualizeWeightCenter(weights, original, FrameRows, FrameCols); // Dobri visualization

                //if (FoundSpotsCheck.CheckState == CheckState.Checked)
                //Visualization.VisualizeWeightCenter(weights, imgOriginal.Data, FrameRows, FrameCols);
                return weights;
            }
            else
                return null;

            //VisualizeStrips(source, RowStrips, ColStrips, rows, cols);
            //VisualizeWeightCenter(weights, rows, cols, source);

        } // End FindSpots

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private static WeightCenter CalculateWeightCenter(byte[, ,] source, int x, int y,
            int width, int height, FilterColors color)
        {
            int sum_x = 0, sum_y = 0, n = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (source[i + y, j + x, (byte)color] == 255)
                    {
                        sum_x += j;
                        sum_y += i;
                        n++;
                    }
                }
            }

            WeightCenter w;
            if (n > 0)
            {
                w = new WeightCenter(
                    x + (int)Math.Round((double)(sum_x / n), 0),
                    y + (int)Math.Round((double)(sum_y / n), 0), y);
            }
            else
                w = WeightCenter.Empty;
            return w;
        }


    }
}
