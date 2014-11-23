//First version of RGB filter
static readonly int IM1_R = 1, IM1_G = 1, IM1_B = 1, IM1_ADD = 512;
static readonly int IM2_G = 1, IM2_R = 1, IM2_B = 1, IM2_ADD = 512;
static readonly int IM3_B = 1, IM3_R = 1, IM3_G = 1, IM3_ADD = 512;
static readonly int IM_R_PRAG = 512;
static readonly int IM_G_PRAG = 512;
static readonly int IM_B_PRAG = 512;

public static void BlackWhite2(byte[] original, byte[] im1, byte[] im2, byte[] im3, int Width, int Height)
{
	byte red, green, blue;
	float im;
	int k, l; // Color index in the byte array
	if (original != null)
	{
		for (int i = 0; i < Height; i++)
		{
			for (int j = 0; j < Width; j++)
			{
				try
				{
					//GetPixel
					l = (i * Width) + j;
					k = l * 3; // For original color image (and skipping 54 bytes because of header)
					//if (k > original.Length - 4)
					//    System.Windows.Forms.MessageBox.Show("Out of boundaries");
					//throw new IndexOutOfRangeException();
					blue = original[k];
					green = original[k + 1];
					red = original[k + 2];
					//End Getting Pixel

					im = red * IM1_R - green * IM1_G - blue * IM1_B + IM1_ADD;
					if (im >= IM_R_PRAG)
						im1[l] = 0;
					else
						im1[l] = 255;

					im = green * IM2_G - red * IM2_R - blue * IM2_B + IM2_ADD;
					if (im >= IM_G_PRAG)
						im2[l] = 0;
					else
						im2[l] = 255;

					im = blue * IM3_B - green * IM3_G - red * IM3_R + IM3_ADD;
					if (im >= IM_B_PRAG)
						im3[l] = 0;
					else
						im3[l] = 255;
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message);
				}
			} //end j
		} //end i
	}
} //end BlackWhite2

//Changed to work only with one dimensional arrays
public static void Filter3x3(byte[, ,] original, byte[, ,] destination, 
	int rows, int cols, ColorChannels channels, Matrix3x3 matrixType)
{
	int sum_r, sum_g, sum_b;
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
			for (int ri = -1; ri <= 1; ri++)
				for (int ci = -1; ci <= 1; ci++)
				{
					switch (channels)
					{
						case ColorChannels.Red:
							sum_r += original[i + ri, j + ci, 2] * matrix[ri + 1, ci + 1];
							break;
						case ColorChannels.Green:
							sum_g += original[i + ri, j + ci, 1] * matrix[ri + 1, ci + 1];
							break;
						case ColorChannels.Blue:
							sum_b += original[i + ri, j + ci, 0] * matrix[ri + 1, ci + 1];
							break;
						case ColorChannels.RGB:
						case ColorChannels.ARGB:
							sum_r += original[i + ri, j + ci, 2] * matrix[ri + 1, ci + 1];
							sum_g += original[i + ri, j + ci, 1] * matrix[ri + 1, ci + 1];
							sum_b += original[i + ri, j + ci, 0] * matrix[ri + 1, ci + 1];
							break;
						case ColorChannels.Gray:
							sum_b += original[i + ri, j + ci, 0] * matrix[ri + 1, ci + 1];
							break;
					} // End switch
				} // End ci
			// End ri
			switch (channels)
			{
				case ColorChannels.Red:
					destination[i, j, 2] = (byte)(sum_r / divisor);
					break;
				case ColorChannels.Green:
					destination[i, j, 1] = (byte)(sum_g / divisor);
					break;
				case ColorChannels.Gray:
				case ColorChannels.Blue:
					destination[i, j, 0] = (byte)(sum_b / divisor);
					break;
				case ColorChannels.RGB:
				case ColorChannels.ARGB:
					destination[i, j, 2] = (byte)(sum_r / divisor);
					destination[i, j, 1] = (byte)(sum_g / divisor);
					destination[i, j, 0] = (byte)(sum_b / divisor);
					break;
			} // End switch
		} // End j
	} // End i
} // End Corner Filter

//Visualize the founded strips
//while (RowStrips[st_row].start != 0)
//{
//    for (int j = 0; j < cols; j++)
//    {
//        source[RowStrips[st_row].start, j, 1] = 255;
//        source[RowStrips[st_row].end, j, 1] = 255;
//    }
//    st_row++;
//}

//while (ColStrips[st_col].start != 0)
//{
//    for (int j = 0; j < rows; j++)
//    {
//        source[j, ColStrips[st_col].start, 0] = 255;
//        source[j, ColStrips[st_col].end, 0] = 255;
//    }
//    st_col++;
//}

//Visualize the squares
//for (int i = x1; i < x2; i++)
//{
//    destination[y1, i, (int)color] = 255;
//    destination[y3, i, (int)color] = 255;
//}

//for (int i = y1; i < y3; i++)
//{
//    destination[i, x1, (int)color] = 255;
//    destination[i, x2, (int)color] = 255;
//}