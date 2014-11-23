using CADBest.AutoCADManagerNamespace;
using CADBest.GeometryNamespace;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;
using System.Drawing;

namespace SixDoFMouse.CameraDetection
{
    public class MouseRecognition
    {
        // The border between black and white binary spots
        private const int BW_LIMIT = 290;
        private const int DIST_CHECK_PIXELS = 10000;
        private MouseDataContainer dataMouse = MouseDataContainer.Instance;
        public FormVisualizer vizDataText;
        public AutoCADManager acadManager;
        private FourPointCalibration FourP;        

        #region Testing and Visualization
        // For testing
        //public FormVisualizer mainTextViz;
        //public Image<Bgr, byte> imgVisualizePoints;
        #endregion

        // Colors determined from old filtration of appropriate color spot
        public Color RedSpot, GreenSpot, BlueSpot, YellowSpot, BlackSpot, WhiteSpot;
        public Vector3D RedVector, GreenVector, BlueVector, YellowVector, BlackVector, WhiteVector;
        public readonly Color WhiteTheoretical;
        public Vector3D WhiteTVector;
        public bool UseHomographyAveraging = true;

        public MouseRecognition()
        {
            //RedSpot = Color.FromArgb(254, 77, 130);
            //GreenSpot = Color.FromArgb(141, 180, 74);
            //BlueSpot = Color.FromArgb(89, 179, 232);
            //YellowSpot = Color.FromArgb(253, 185, 6);
            //BlackSpot = Color.FromArgb(46, 26, 31);
            ////WhiteSpot = Color.FromArgb(216, 198, 222);
            //WhiteTheoretical = Color.FromArgb(240, 225, 243);
            acadManager = new AutoCADManager();
            FourP = new FourPointCalibration(10);           
        }

        public bool IsInitialized
        {
            get
            {
                return ((RedVector != null) &&
                    (GreenVector != null) &&
                    (BlueVector != null) &&
                    (YellowVector != null) &&
                    (WhiteVector != null));
            }
        }

        /// <summary>
        /// Detect the orientation of camera toward descriptor
        /// </summary>
        /// <param name="filtered">Filtered image array</param>
        /// <param name="original">Original image array</param>
        /// <param name="colorIndex">Define which color filter is used</param>
        /// <returns>Four points. Camera place, center 
        /// of projection and two orientations(X and Y)</returns>
        public List<Point3D> Detection(
            Image<Bgr, byte> imgContainer,
            byte[, ,] original, FilterColors colorIndex,
            int FrameRows, int FrameCols,
            List<Point3D> CenterProjXY)
        {
            FilterColors frameColor = colorIndex;
            // Correction is made for yellow color, which value is 4,
            // but a image array consists only of three color channels.
            // The color is used as index in that array
            if (colorIndex == FilterColors.Y)
                colorIndex = FilterColors.R;

            List<WeightCenter> sp = SpotDetection.FindSpots(imgContainer.Data, original, colorIndex, FrameRows, FrameCols);
            if (sp == null)
                return null;
           
            Point3D[][] orientation = DetectFace(sp, FrameRows);

            // In case five color spots are detected the processing of current frame continues
            if (orientation == null)
                return null;

            // Draw the corrected points on image
            Visualization.DrawPoints(original, new List<Point3D>(orientation[2]), FrameRows, FrameCols, 128);

            WeightCenter binFound;
            List<Point3D> bin8Location = CalcBinaryPlaces(original, orientation[0], orientation[1], out binFound);

            // Draw the exact 8 spots of binary code
            //Visualization.DrawPoints(original, bin8Location, FrameRows, FrameCols, 150);

            int sideNumber = ReadBinaryCode(original, bin8Location);
            if (sideNumber < 0)
                return null;

            #region Visualization of binary code location
            //if (imgVisualizePoints != null)
            //{
            //    int rows = original.GetLength(0);
            //    int cols = original.GetLength(1);
            //    Visualization.DrawPoints(imgVisualizePoints.Data, binar8Codes, rows, cols);
            //}                     
            //Number 3 has problem
            //if (color == FilterColors.Blue)
            //    mainTextViz.SetText(sideNumber.ToString());
            #endregion

            // Compare side number and color if they match correctly
            if (!CheckSideNumberColor(sideNumber, frameColor))
                return null;

            List<Point3D> fourPdescriptor = new List<Point3D>(5);
            fourPdescriptor.Add(dataMouse.ColorSpotsDescriptorSmall[0]);
            fourPdescriptor.Add(dataMouse.ColorSpotsDescriptorSmall[1]);
            fourPdescriptor.Add(dataMouse.ColorSpotsDescriptorSmall[2]);
            fourPdescriptor.Add(dataMouse.ColorSpotsDescriptorSmall[3]);
            fourPdescriptor.Add(dataMouse.ColorSpotsDescriptorSmall[4]);
            Point3D[] cameraProjectionPoints = orientation[2];

            int binPosition = binFound.rowStart;

            List<Point3D> fourPprojection = new List<Point3D>(4);
            // Order points in counter clock wise from binary code
            fourPprojection.Add(cameraProjectionPoints[dataMouse.indexBinaryPositionReorder[binPosition, 0]]);
            fourPprojection.Add(cameraProjectionPoints[dataMouse.indexBinaryPositionReorder[binPosition, 1]]);
            fourPprojection.Add(cameraProjectionPoints[dataMouse.indexBinaryPositionReorder[binPosition, 2]]);
            fourPprojection.Add(cameraProjectionPoints[dataMouse.indexBinaryPositionReorder[binPosition, 3]]);
            fourPprojection.Add(cameraProjectionPoints[dataMouse.indexBinaryPositionReorder[binPosition, 4]]);

            // For distance check
            List<Point3D> FoundSpots = Geometry.CloneList(fourPprojection);

            ImageProcessing.ConvertXY(fourPprojection, FrameRows);

            //FourP.ToDraw = true;
            //FourP.FourPointCalibrationMain(fourPdescriptor, CenterProjXY, fourPprojection);

            //FiveVariants(fourPprojection, CenterProjXY, fourPdescriptor);

            Point3D[] sourceViewPars = Geometry.align_calc_pars(new Point3D[] {
                dataMouse.ColorSpotsDescriptorSmall[0],
                dataMouse.ColorSpotsDescriptorSmall[1],
                dataMouse.ColorSpotsDescriptorSmall[2]
            });

            // sideNumber-1 is the index of the detected side
            Point3D[] destViewPars = Geometry.align_calc_pars(new Point3D[] {
                dataMouse.ColorSpotSides[sideNumber - 1][0],
                dataMouse.ColorSpotSides[sideNumber - 1][1],
                dataMouse.ColorSpotSides[sideNumber - 1][2]
            });

            // Averaging five result from 4-point transformation
            List<Point3D> fourP = new List<Point3D>();          
           
                int n = 0;
                List<List<Point3D>> fourPoint = new List<List<Point3D>>();
                //Rotate elements in list and calculate four point transform
                for (int i = 0; i < fourPprojection.Count; i++)
                {
                    List<List<Point3D>> pl1 = Geometry.cal_4p_trans(fourPdescriptor.GetRange(0, 4), CenterProjXY, fourPprojection.GetRange(0, 4));

                    if (pl1 != null)
                    {
                        fourPoint.Add(pl1[1]);

                        // Align camera toward descriptor
                        Geometry.align((fourPoint[n]), sourceViewPars, destViewPars);

                        //Add weight in fifth point in X value
                        fourPoint[n].Add(WeightDistance(fourPoint[n], FoundSpots, sideNumber, original, CenterProjXY, FrameRows, FrameCols, true));
                        n++;
                    }
                    //Add first in last position. Remove firs.
                    fourPdescriptor.Add(fourPdescriptor[0]);
                    fourPdescriptor.RemoveAt(0);

                    fourPprojection.Add(fourPprojection[0]);
                    fourPprojection.RemoveAt(0);
                }

                //Remove worst result             
                    double maxDist = -1;
                    double currDist = 0;
                    n = 0;
                    for (int i = 0; i < fourPoint.Count; i++)
                    {
                        currDist = fourPoint[i][4].X;
                        if (currDist > maxDist)
                        {
                            maxDist = currDist;
                            n = i;
                        }
                    }
                    if ((fourPoint.Count != 0) && (fourPoint.Count >= 2) && (fourPoint[n][4].X > 10))
                        fourPoint.RemoveAt(n);
                   
                    fourP = FormWebCamEmgu.AverageViewPointsWeight(false, fourPoint);                          

                if (fourP == null)
                    return null; 
            
            fourP.Add(WeightDistance(fourP, FoundSpots, sideNumber, original, CenterProjXY, FrameRows, FrameCols, false));
         
            return fourP;
        }

        private void RetrieveColorInformation(FilterColors color, byte[,,] original, 
            WeightCenter colorSpotLocation, Point3D whiteSpotLocation, Point3D blackSpotLocation)
        {
            if (WhiteSpot == Color.Empty)
            {
                WhiteSpot = ImageProcessing.GetColor(
                    original,
                    new WeightCenter((int)whiteSpotLocation.X, (int)whiteSpotLocation.Y, 0), 0);
            }

            if (BlackSpot == Color.Empty)
            {
                BlackSpot = ImageProcessing.GetColor(
                    original,
                    new WeightCenter((int)blackSpotLocation.X, (int)blackSpotLocation.Y, 0), 0);
            }

            Color colorSpot = ImageProcessing.GetColor(original, colorSpotLocation, 0);
            switch (color)
            {
                case FilterColors.B:
                    if (BlueSpot == Color.Empty)
                        BlueSpot = colorSpot;
                    break;
                case FilterColors.G:
                    if (GreenSpot == Color.Empty)
                        GreenSpot = colorSpot;
                    break;
                case FilterColors.R:
                    if (RedSpot == Color.Empty)
                        RedSpot = colorSpot;
                    break;
                case FilterColors.Y:
                    if (YellowSpot == Color.Empty)
                        YellowSpot = colorSpot;
                    break;
                default:
                    break;
            }

            RecalcVectors();
            RotateColorVectors();
        }

        public void RecalcVectors()
        {
            WhiteTVector = new Vector3D(WhiteTheoretical);
            WhiteTVector.Normalize();
            WhiteVector.Normalize();
            BlackVector = new Vector3D(BlackSpot);
            BlackVector.Normalize();
            BlueVector = new Vector3D(BlueSpot);
            BlueVector.Normalize();
            GreenVector = new Vector3D(GreenSpot);
            GreenVector.Normalize();
            RedVector = new Vector3D(RedSpot);
            RedVector.Normalize();
            YellowVector = new Vector3D(YellowSpot);
            YellowVector.Normalize();
        }

        public List<Point3D> RotateColorVectors()
        {
            Point3D white = WhiteVector.Clone();
            // Find the difference between theoretical white and current white
            // Find sin and cos of theoretical white vector
            List<Point3D> sinCosT = Geometry.GetRotationAngles(WhiteTVector);
            // Rotate the white vector with the sin cos from theoretical white
            Geometry.RotateSinCos(white, sinCosT, true);
            // Find the difference between theoretical and real white color vector
            List<Point3D> sinCosW = Geometry.GetRotationAngles(white);

            // Rotate the base color vectors with the white's sin and cos
            RotateSingleVector(RedVector, sinCosW);
            RotateSingleVector(GreenVector, sinCosW);
            RotateSingleVector(BlueVector, sinCosW);
            RotateSingleVector(YellowVector, sinCosW);

            return sinCosW;
        }

        public void RotateSingleVector(Vector3D vect, List<Point3D> sinCosW)
        {
            if (vect == null)
                return;

            // Rotate the current vector to (1, 0, 0)
            List<Point3D> sinCos = Geometry.GetRotationAngles(vect);
            Geometry.RotateSinCos(vect, sinCos, true);

            // Rotate with the sin and cos of white vector
            Geometry.RotateSinCos(vect, sinCosW, true);

            // Rotate back with its original sin and cos
            Geometry.RotateSinCos(vect, sinCos, false);
        }           

        /// <summary>
        /// Add Weight based on distance between found weight centers and projected centers 
        /// </summary>
        /// <param name="fourP">Four points. Camera place, center of projection and two orientations(X and Y)</param>
        /// <param name="FoundSpots">Found and Ordered points in counter clock wise from binary code</param>
        /// <param name="sideNumber">Side number</param>
        /// <param name="imageData">Image for visualization of cross</param>
        /// <param name="CenterProjXY">Center of projection. X and Y axis middle</param>
        /// <param name="FrameRows">Frame rows</param>
        /// <param name="FrameCols">Frame cols</param>
        /// <param name="crossViz">If true cross are visualize</param>
        /// <returns>Point3D with weight stored in X value</returns>
        private Point3D WeightDistance(List<Point3D> fourP, List<Point3D> FoundSpots, int sideNumber, 
            byte[,,] imageData, List<Point3D> CenterProjXY, int FrameRows, int FrameCols, bool crossViz)
        {
            List<List<Point3D>> ProjectedPointsOfDescriptor = new List<List<Point3D>>();
            ProjectedPointsOfDescriptor.Add(Visualization.DescriptorPointsOnImage(fourP, sideNumber, CenterProjXY, FrameRows));

            if(crossViz == true)
            Visualization.ProjectDescriptorPoints(fourP, sideNumber, CenterProjXY, imageData, FrameRows, FrameCols, 100);

            double Sumdist = 0;
            //Check the difference between Found spot and projected points
            for (int i = 0; i < 4; i++)
            {
                ProjectedPointsOfDescriptor[0][i].Z = 0;
                Sumdist += Geometry.Distance(ProjectedPointsOfDescriptor[0][i], FoundSpots[i]);
                //vizDataText.SetText(dist.ToString());
                //if (Sumdist > DIST_CHECK_PIXELS)
                //    return false;               
            }
            Point3D DistPoint = new Point3D(Sumdist, 0, 0);
            return DistPoint;
        }

        /// <summary>
        /// Improve accuracy of weight centers.
        /// </summary>
        /// <param name="sp">Weight centers</param>
        /// <param name="Descriptor"> Descriptor points</param>
        /// <returns>Average points</returns>
        private List<Point3D> HomgpaphyPoints(List<Point3D> projectionOriginal, List<Point3D> descriptorOriginal, int rows)
        {
            List<Point3D> projection = Geometry.CloneList(projectionOriginal);
            List<Point3D> descriptor = Geometry.CloneList(descriptorOriginal);
            ImageProcessing.ConvertXY(projection, rows);
            List<Point3D> newPoints = new List<Point3D>();

            //acadManager.DrawPolylines(projection, true, Color.White);
            List<Point3D> homography = new List<Point3D>();

            for (int i = 0; i < projection.Count; i++)
            {
                List<Point3D> projectionPars = Geometry.homography_calc_pars(projection.GetRange(1, 4));
                List<Point3D> descriptorPars = Geometry.homography_calc_pars(descriptor.GetRange(1, 4));
                List<Point3D> homographyPt = new List<Point3D>();
                homographyPt.Add(descriptor[0]);
                List<Point3D> homographyRes = Geometry.homography_calc(homographyPt, descriptorPars, projectionPars);
                homography.Add(homographyRes[0].Clone());

                double k = 5d;
                double x = (homographyRes[0].X + projection[0].X * (k - 1)) / k;
                double y = (homographyRes[0].Y + projection[0].Y * (k - 1)) / k;
                double z = (homographyRes[0].Z + projection[0].Z * (k - 1)) / k;
                newPoints.Add(new Point3D(x, y, z));

                projection.Add(projection[0]);
                projection.RemoveAt(0);

                descriptor.Add(descriptor[0]);
                descriptor.RemoveAt(0);

            }

            //acadManager.DrawPolylines(newPoints, false, Color.Red);
            //acadManager.DrawPolylines(homography, false, Color.Green);

            ImageProcessing.ConvertXY(newPoints, rows);
            return newPoints;
        }
       
        /// <summary>
        /// Find the correct order of detected color spots and calculate the orientation parameters
        /// </summary>
        /// <param name="weights">Weight centers of color spots, unordered</param>
        /// <returns>Orientation parameters of descriptor and projection from camera,
        /// and correctly reordered points from projection</returns>
        private Point3D[][] DetectFace(List<WeightCenter> weights, int FrameRows)
        {
            // Five points from the sides descriptor - destination for homography
            Point3D[] destP = dataMouse.ColorSpotsDescriptorSmall;
            Point3D[] destPars_fourthP = Geometry.homography_calc_pars(
                new Point3D[] { destP[0], destP[1], destP[2], destP[4] });
            Point3D[] destPars_fifthP = Geometry.homography_calc_pars(
                new Point3D[] { destP[0], destP[1], destP[2], destP[3] });
            Point3D[][] destPars = new Point3D[][] { destPars_fourthP, destPars_fifthP };
            double distance;
            double distanceMin = 10000;
            List<Point3D> homographyRes, weightListCurrent;
            List<Point3D> homographyMin = null, weightListMin = null;
            Point3D[] hmMinSourcePars = null, hmMinDestPars = null;

            // Convert the WeightCenter struct to List of Point3D
            List<Point3D> weightsList = new List<Point3D>(15);
            foreach (WeightCenter item in weights)
                weightsList.Add(new Point3D(item.x, item.y, 0));

            for (int i = 0; i < 6; i++)
            {
                weightListCurrent = new List<Point3D>(10);
                for (int j = 0; j < 4; j++)
                {
                    weightListCurrent.Add(weightsList[dataMouse.indexWeightsDetect[i, j]]);
                }
                Point3D[] sourcePars = Geometry.homography_calc_pars(weightListCurrent.ToArray());
                weightListCurrent.Add(weightsList[4]);

                for (int j = 0; j <= 1; j++)
                {
                    homographyRes = Geometry.homography_calc(weightListCurrent, sourcePars, destPars[j]);
                    distance = Geometry.Distance(homographyRes[4], destP[j + 3]);
                    if (distance < distanceMin)
                    {
                        distanceMin = distance;
                        homographyMin = homographyRes;
                        weightListMin = weightListCurrent;
                        hmMinSourcePars = sourcePars;
                        hmMinDestPars = destPars[j];
                        // For correct order of points
                        if (j == 0) // This should be fixed in future
                        {
                            Point3D pTemp = homographyMin[3];
                            homographyMin[3] = homographyMin[4];
                            homographyMin[4] = pTemp;

                            pTemp = weightListMin[3];
                            weightListMin[3] = weightListMin[4];
                            weightListMin[4] = pTemp;
                        }
                    }
                }
            }

            if (distanceMin < 5)
            {
                // If true is returned, recalculate parameters
                Point3D[] sourceBinPars, destBinPars;
                if (CheckOrder(weightListMin, FrameRows))
                {
                    sourceBinPars = destPars_fifthP;
                    destBinPars = Geometry.homography_calc_pars(
                        new Point3D[] { weightListMin[0], weightListMin[1], weightListMin[2], weightListMin[3] });
                }
                else
                {
                    sourceBinPars = hmMinDestPars;
                    destBinPars = hmMinSourcePars;
                }

                if (UseHomographyAveraging)
                {
                    //acadManager.DrawPolylines(weightListMin, true, Color.White);
                    List<Point3D> descriptor = new List<Point3D>(dataMouse.ColorSpotsDescriptorSmall);
                    for (int i = 0; i < 10; i++)
                    {
                        weightListMin = HomgpaphyPoints(weightListMin, descriptor, FrameRows);
                    }
                    //acadManager.DrawPolylines(weightListMin, false, Color.Green);
                    //List<Point3D> projectionPars = Geometry.homography_calc_pars(weightListMin.GetRange(0, 4));
                    //List<Point3D> descriptorPars = Geometry.homography_calc_pars(descriptor.GetRange(0, 4));
                    //List<Point3D> homography = Geometry.homography_calc(descriptor, descriptorPars, projectionPars);
                    //acadManager.DrawPolylines(homography, false, Color.Yellow);
                }
                
                Point3D[][] resultOrientation = new Point3D[][] { 
                    sourceBinPars, destBinPars, weightListMin.ToArray()
                };
                return resultOrientation;
            }
            else
                return null;
        }

        /// <summary>
        /// Create five variants of four point calibration
        /// </summary>
        /// <param name="FoundSpots">Found and ordered weight centers</param>
        /// <param name="CenterProjXY">Center of projection</param>
        /// <param name="DescriptorOriginal">Original descriptor coordinates</param>
        /// <returns>List with different solution based on same points calculated in different order</returns>
        public List<List<Point3D>> FiveVariants(List<Point3D> FoundSpots, List<Point3D> CenterProjXY, List<Point3D> DescriptorOriginal)
        {
            List<Point3D> Projection = Geometry.CloneList(FoundSpots);
            List<Point3D> Descriptor = Geometry.CloneList(DescriptorOriginal);
            List<List<Point3D>> ResultList = new List<List<Point3D>>();

            for (int i = 0; i < Projection.Count; i++)
            {
                List<List<Point3D>> fourPcurrent = Geometry.cal_4p_trans(Descriptor.GetRange(0, 4), CenterProjXY, Projection.GetRange(0, 4));
                if (fourPcurrent != null)
                    ResultList.Add(fourPcurrent[1]);

                Projection.Add(Projection[0]);
                Projection.RemoveAt(0);

                Descriptor.Add(Descriptor[0]);
                Descriptor.RemoveAt(0);

            }
            return ResultList;
        }

        /// <summary>
        /// Check for correct order of detected points from camera
        /// If the order is clockwise, it is reversed
        /// </summary>
        /// <param name="source">Points which are verified</param>
        /// <param name="rows">Height of the image</param>
        /// <returns>True if the parameters needs to be recalculated</returns>
        private bool CheckOrder(List<Point3D> source, int rows)
        {
            List<Point3D> weightListMin2 = Geometry.CloneList(source);
            ImageProcessing.ConvertXY(weightListMin2, rows);
            Point3D pMinus = new Point3D(weightListMin2[0]);
            Geometry.p_add(weightListMin2, pMinus, -1d);
            Point3D rotZsc = Geometry.calc_rotZ(weightListMin2[1]);
            Geometry.p_rotZ(weightListMin2, rotZsc, -1d);

            if (weightListMin2[2].Y < 0)
            {
                source.Reverse(1, source.Count - 1);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Find the location of the first always black spot
        /// </summary>
        /// <param name="binarPlaces">Five projected probable locations</param>
        /// <param name="original">The source image, where the spots will be searched</param>
        /// <returns>Location on the image and its number</returns>
        private WeightCenter FindBinaryPlace(byte[, ,] original, List<Point3D> binarPlaces)
        {
            int sumMin = 765; // 255 * 3 - the max sum possible
            int sum;
            byte R, G, B; // b0g1r2
            Point3D pMin = null;
            int foundedPosition = 0;
            for (int i = 0; i < 5; i++)
            {
                if ((binarPlaces[i].X < 0) || (binarPlaces[i].Y < 0) ||
                    (binarPlaces[i].X >= original.GetLength(1)) ||
                    (binarPlaces[i].Y >= original.GetLength(0)))
                {
                    return WeightCenter.Empty;
                }

                R = original[(int)(binarPlaces[i].Y), (int)(binarPlaces[i].X), 2];
                G = original[(int)(binarPlaces[i].Y), (int)(binarPlaces[i].X), 1];
                B = original[(int)(binarPlaces[i].Y), (int)(binarPlaces[i].X) ,0];
                sum = R + G + B;
                if (sum < sumMin) // Find the darkest spot
                {
                    foundedPosition = i;
                    sumMin = sum;
                    pMin = binarPlaces[i];
                }
            }
            WeightCenter w = new WeightCenter((int)pMin.X, (int)pMin.Y, foundedPosition);

            //Visualization.VisualizeWeightCenter(w, imgOriginal.Data, FrameRows, FrameCols, 100);
            //Visualization.VisualizeWeightCenter(w, imgR.Data, FrameRows, FrameCols);

            return new WeightCenter((int)pMin.X, (int)pMin.Y, foundedPosition);
        }

        /// <summary>
        /// Read the binary code with the formula: C1*1 + C2*2 + C5*4 + C6*8
        /// First point is always black, fifth point is always white
        /// </summary>
        /// <param name="original"></param>
        /// <param name="binLocations"></param>
        /// <returns>The number of detected side</returns>
        private int ReadBinaryCode(byte[, ,] original, List<Point3D> binLocations)
        {
            byte[] binaryCode = new byte[8];
            byte R, G, B; // b0g1r2
            int sum, sideNumber;

            // Read the binary code
            for (int i = 0; i < 8; i++)
            {
                if (binLocations == null)
                    return -1;
                if ((binLocations[i].Y < 0) || (binLocations[i].X < 0))
                    return -1;
                if ((binLocations[i].Y > original.GetLength(0)) ||
                    (binLocations[i].X > original.GetLength(1)))
                    return -1;

                R = original[(int)binLocations[i].Y, (int)binLocations[i].X, 2];
                G = original[(int)binLocations[i].Y, (int)binLocations[i].X, 1];
                B = original[(int)binLocations[i].Y, (int)binLocations[i].X, 0];
                sum = R + G + B;
                if (sum < BW_LIMIT) // The spot is black
                    binaryCode[i] = 1;
                else
                    binaryCode[i] = 0;
            }

            // Check for correct order of binary code
            if (!CheckBinaryControlSum(binaryCode))
                return -1;

            sideNumber = binaryCode[1] + binaryCode[2] * 2 +
                binaryCode[5] * 4 + binaryCode[6] * 8;
            if ((sideNumber < 1) || (sideNumber > 12))
            {
                sideNumber = -1; // Sometimes the binary code is read wrong
            }
            return sideNumber;
        }

        /// <summary>
        /// Calculate the exact 8 locations of binary code for reading
        /// </summary>
        /// <param name="original">Frame from which will be founded the always first black spot</param>
        /// <param name="sourceBinPars">Orientation parameters of descriptor</param>
        /// <param name="destBinPars">Orientation parameters of projection from camera</param>
        /// <returns>Eight positions of binary code according projection</returns>
        private List<Point3D> CalcBinaryPlaces(byte[, ,] original, Point3D[] sourceBinPars, Point3D[] destBinPars, out WeightCenter binFound)
        {
            List<Point3D> binarPlacesHomography = Geometry.homography_calc(
                    new List<Point3D>(dataMouse.binarPlacesBase), sourceBinPars, destBinPars);
            binFound = FindBinaryPlace(original, binarPlacesHomography);
            if (binFound == WeightCenter.Empty)
                return null;

            List<Point3D> binar8Codes = new List<Point3D>(Geometry.CloneList(dataMouse.binarSpotsSide01));
            // Rotate the 8 binary codes according the founded position
            Geometry.p_rotZ(binar8Codes, dataMouse.SinCosDescriptor[binFound.rowStart], 1);
            binar8Codes = Geometry.homography_calc(binar8Codes, sourceBinPars, destBinPars);

            return binar8Codes;
        }

        /// <summary>
        /// Check whether the sum of first three black spots on first and second row is even or odd
        /// In case of even sum, the 8th bit should be black, otherwise the 4th
        /// </summary>
        /// <param name="binaryCode">The binary code which is verified</param>
        /// <returns>True if the sum is correct, false if not</returns>
        private bool CheckBinaryControlSum(byte[] binaryCode)
        {
            byte sum = 0;
            for (int i = 0; i <= 2; i++)
            {
                if (binaryCode[i] == 1)
                    sum++;
                if (binaryCode[i + 4] == 1)
                    sum++;
            }

            if (sum % 2 == 0) // Even
            {
                if (binaryCode[7] != 1)
                    return false;
            }
            else // Odd
            {
                if (binaryCode[3] != 1)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Check if side number corresponds to the color.
        /// </summary>
        /// <param name="sideNumber">Side Number</param>
        /// <param name="color">Color</param>
        /// <returns>True if pass</returns>
        private bool CheckSideNumberColor(int sideNumber, FilterColors color)
        {
            // Check the number of side
            if (color == FilterColors.R) // Check for Red side
            {
                if ((sideNumber != 6) && (sideNumber != 7) && (sideNumber != 10))
                    return false;
            }

            if (color == FilterColors.Y) // Check for Yellow side
            {
                if ((sideNumber != 1) && (sideNumber != 5) && (sideNumber != 11))
                    return false;
            }

            if (color == FilterColors.G)
            {
                if ((sideNumber != 8) && (sideNumber != 9) && (sideNumber != 12))
                {
                    //MessageBox.Show("sidenumber = " + sideNumber.ToString() + " color " + color.ToString());
                    return false;
                }
            }

            if (color == FilterColors.B)
            {
                if ((sideNumber != 2) && (sideNumber != 3) && (sideNumber != 4))
                {
                    //MessageBox.Show("sidenumber = " + sideNumber.ToString() + " color " + color.ToString());
                    return false;
                }
            }

            return true;
        }
    }
}
