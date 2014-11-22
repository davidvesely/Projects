using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QuickStichNamespace
{
    public class FormStitchEngine
    {
        // Space between picture boxes, used during resizing
        // and repositioning the picture boxes
        private const int SpaceBetweenBoxes = 15;
        // Count of picture boxes which will be visible on the screen
        private const int CountPicBox = 2;
        private const int HeightSpaceBottom = 4;
        public ZoomPicBox MainImageBox;
        public List<ZoomPicBox> ImageBoxes;
        public bool IsStitchMode;
        public bool IsStitchPrepareMode;

        public FormStitchEngine()
        {
            ImageBoxes = new List<ZoomPicBox>();
            IsStitchMode = false;
            IsStitchPrepareMode = false;
        }

        public void AddImage(Panel panel, QuickStitch stitchModule, Side side)
        {
            string fileName = OpenImageFile();
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            Bitmap image = new Bitmap(fileName);
            ImageData imageData = new ImageData(fileName, image);
            ZoomPicBox picBox = CreatePictureBox(image);
            panel.Controls.Add(picBox);

            switch (side)
            {
                case Side.Left:
                    // Insert at beginning 
                    stitchModule.ImageDataCollection.Insert(0, imageData);
                    ImageBoxes.Insert(0, picBox);
                    ResizePicBoxes(panel);
                    break;
                case Side.Right:
                    // Insert at the end
                    stitchModule.ImageDataCollection.Add(imageData);
                    ImageBoxes.Add(picBox);
                    ResizePicBoxes(panel);
                    break;
                case Side.Main:
                    stitchModule.MainImageData = imageData;
                    stitchModule.ImageDataCollection.Add(imageData);
                    picBox.Size = panel.ClientSize;
                    MainImageBox = picBox;
                    ImageBoxes.Add(picBox);
                    break;
                default:
                    break;
            }

            panel.Refresh();
        }

        private ZoomPicBox CreatePictureBox(Bitmap image)
        {
            // New ZoomPicBox
            ZoomPicBox picBox = new ZoomPicBox();
            picBox.Image = image;
            picBox.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            picBox.Top = 0;
            picBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            picBox.MouseClick += PictureBox_ClickEvent;
            return picBox;
        }

        public void ResizePicBoxes(Panel panel)
        {
            // The width of single picture box
            int width = (panel.ClientRectangle.Width - SpaceBetweenBoxes * (CountPicBox - 1)) / CountPicBox;

            for (int i = 0; i < ImageBoxes.Count; i++)
            {
                if (i == 0)
                {
                    ImageBoxes[i].Left = 0;
                }
                else
                {
                    ImageBoxes[i].Left = ImageBoxes[i - 1].Right + SpaceBetweenBoxes;
                }
                ImageBoxes[i].Width = width;
            }

            // The second for is necessary because after repositioning of the controls
            // the height of client rectangle can be smaller because of scrollbars
            int height = panel.ClientRectangle.Height;
            for (int i = 0; i < ImageBoxes.Count; i++)
            {
                ImageBoxes[i].Height = height - HeightSpaceBottom;
            }
        }

        private string OpenImageFile()
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

        private void PictureBox_ClickEvent(object sender, MouseEventArgs e)
        {
            if (IsStitchPrepareMode)
            {
                MessageBox.Show("Test");
            }
        }
    }
}
