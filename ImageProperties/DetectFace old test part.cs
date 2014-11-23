// ****************************************************
                List<Point3D> binarPlacesHomography = Geometry.homography_calc(
                    new List<Point3D>(dataMouse.binarPlacesBase), sourceBinPars, destBinPars);
                WeightCenter binFound = FindBinaryPlace(binarPlacesHomography, original);

                List<Point3D> binar8Codes = new List<Point3D>(Geometry.CloneList(dataMouse.binarSpotsSide01));
                // Rotate the 8 binary codes according the founded position
                Geometry.p_rotZ(binar8Codes, dataMouse.SinCosDescriptor[binFound.rowStart], 1);
                binar8Codes = Geometry.homography_calc(binar8Codes, sourceBinPars, destBinPars);

                if (checkBoxWriteDetected.Checked)
                {
                    using (StreamWriter str = new StreamWriter("homography.txt", true))
                    {
                        //List<Point3D> weightListConverted = Geometry.CloneList(weightsList);
                        //ConvertXY(weightListConverted, rows);
                        //foreach (Point3D pt in weightListConverted)
                        //{
                        //    str.WriteLine(pt.X.ToString() + "," + pt.Y.ToString() + "," + pt.Z.ToString());
                        //}
                        //str.WriteLine();

                        //foreach (Point3D pt in homographyMin)
                        //{
                        //    str.WriteLine(pt.X.ToString() + "," + pt.Y.ToString() + "," + pt.Z.ToString());
                        //}
                        //str.WriteLine();

                        List<Point3D> weightListConverted = Geometry.CloneList(weightListMin);
                        ConvertXY(weightListConverted, rows);
                        foreach (Point3D pt in weightListConverted)
                        {
                            str.WriteLine(pt.X.ToString() + "," + pt.Y.ToString() + "," + pt.Z.ToString());
                        }
                        str.WriteLine();
                    }
                }

                if (checkBoxVizDetectedSpots.Checked)
                {
                    //DrawPoints(data, weightListMin, rows, cols);
                    //ConvertYX(binarPlacesHomography, rows);
                    //DrawPoints(original, binarPlacesHomography, rows, cols);
                    //VisualizeWeightCenter(binFound, rows, cols, original);
                    DrawPoints(original, binar8Codes, rows, cols);
                }
                //List<List<Point3D>> result = new List<List<Point3D>>(3);
                //result.Add(weightListMin);