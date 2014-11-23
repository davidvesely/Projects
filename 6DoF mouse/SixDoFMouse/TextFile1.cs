private void RGBFilter()
{
    double im;
    byte min;
    byte red, new_red;
    byte green, new_green;
    byte blue, new_blue;

    byte[, ,] data1 = imgR.Data;
    byte[, ,] data2 = imgG.Data;
    byte[, ,] data3 = imgB.Data;
    byte[, ,] dataYellow = imgY.Data;
    byte[, ,] dataFiltered = imgFiltered.Data;

    for (int i = imgFiltered.Rows - 1; i >= 0; i--)
    {
        for (int j = imgFiltered.Cols - 1; j >= 0; j--)
        {
            red = dataFiltered[i, j, 2];
            green = dataFiltered[i, j, 1];
            blue = dataFiltered[i, j, 0];

            // Subtract the Min of R G B colors from the remaining two
            min = red;
            if (min > green)
                min = green;
            if (min > blue)
                min = blue;
            red = (byte)(red - min);
            green = (byte)(green - min);
            blue = (byte)(blue - min);

            dataFiltered[i, j, 2] = red;
            dataFiltered[i, j, 1] = green;
            dataFiltered[i, j, 0] = blue;

            if (USE_RED)
            {
                if (checkBox1.Checked)
                {
                    im = red * Coef.IM_R_K - green * Coef.IM1_G - blue * Coef.IM1_B + Coef.IM_R_ADD;
                    if (im >= Coef.IM_R_PRAG)
                        new_red = 255;
                    else
                        new_red = 0;
                }
                else
                    new_red = red;
                data1[i, j, 2] = new_red;
                data1[i, j, 1] = 0;
                data1[i, j, 0] = 0;
            }

            if (USE_GREEN)
            {
                if (checkBox1.Checked)
                {
                    im = green * Coef.IM_G_K - red * Coef.IM2_R - blue * Coef.IM2_B + Coef.IM_G_ADD;
                    if (im >= Coef.IM_G_PRAG)
                        new_green = 255;
                    else
                        new_green = 0;
                }
                else
                    new_green = green;
                data2[i, j, 2] = 0;
                data2[i, j, 1] = new_green;
                data2[i, j, 0] = 0;
            }

            if (USE_BLUE)
            {
                if (checkBox1.Checked)
                {
                    im = blue * Coef.IM_B_K - green * Coef.IM3_G - red * Coef.IM3_R + Coef.IM_B_ADD;
                    if (im >= Coef.IM_B_PRAG)
                        new_blue = 255;
                    else
                        new_blue = 0;
                }
                else
                    new_blue = blue;
                data3[i, j, 2] = 0;
                data3[i, j, 1] = 0;
                data3[i, j, 0] = new_blue;
            }

            if (USE_YELLOW)
            {
                if (checkBox1.Checked)
                {
                    im = Coef.IM_Y_K * (0.8 * red + green) - 2 * blue + Coef.IM_Y_ADD;
                    if (im >= Coef.IM_Y_PRAG)
                    {
                        new_red = 255;
                        new_green = 255;
                    }
                    else
                    {
                        new_red = 0;
                        new_green = 0;
                    }
                }
                else
                {
                    new_red = red;
                    new_green = green;
                }
                dataYellow[i, j, 2] = new_red;
                dataYellow[i, j, 1] = new_green;
                dataYellow[i, j, 0] = 0;
            }
        }
    }
}