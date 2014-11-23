//private void Grayscale()
//{
//    //Bitmap bmp = new Bitmap(img);
//    Bitmap bmp = zoomPicBox1.Image;

//    int red, green, blue, c2i;
//    double gray;
//    Color c1, c2;
//    SaveFileDialog saveDlg = new SaveFileDialog();

//    for (int i = 0; i < img.Width; i++)
//    {
//        for (int j = 0; j < img.Height; j++)
//        {
//            c1 = bmp.GetPixel(i, j);
//            red = c1.R;
//            green = c1.G;
//            blue = c1.B;
//            gray = 0.2989 * (double)red + 0.5870 * (double)green + 0.1140 * (double)blue;
//            c2i = (int)Math.Round(gray);
//            c2 = Color.FromArgb(c2i, c2i, c2i);
//            bmp.SetPixel(i, j, c2);
//        }
//    }
//    saveDlg.DefaultExt = "jpg";
//    saveDlg.Filter = "bmp files|*.bmp|jpg files (*.jpg)|*.jpg|png files (*.png)|*.png|All files (*.*)|*.*";
//    saveDlg.FilterIndex = 2;
//    saveDlg.RestoreDirectory = true;
//    string fileName = "";

//    if (saveDlg.ShowDialog() == DialogResult.OK)
//        fileName = saveDlg.FileName;

//    if (fileName != String.Empty)
//        bmp.Save(fileName);
//}