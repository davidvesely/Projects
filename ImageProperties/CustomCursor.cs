using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ImageProperties
{
    //Custom cursor use
    //IntPtr iCursHandle = CustomCursor.InterOpCursorCreate(20);

    class CustomCursor
    {
        //InterOp declaration -- put in your Form-Derived Class:
        [DllImport("user32.dll")]
        public static extern IntPtr CreateCursor(
            IntPtr hInst,
            int xHotSpot,
            int yHotSpot,
            int nWidth,
            int nHeight,
            byte[] pvANDPlane,
            byte[] pvXORPlane
        );

        // Cursor creation function -- put in your Form-Derived Class:
        public static IntPtr InterOpCursorCreate(int iFontHeightInPixels)
        {
            int i;

            // Cursor height and width scale from the font height:
            int iHeight = Convert.ToInt32(0.9F * iFontHeightInPixels);

            int iWidth = 1 + 2 * Convert.ToInt32(0.15F * iFontHeightInPixels);

            // This helps simplify the code by keeping the cursor's
            // centerline in the 1st byte of each mask row:
            if (iWidth > 15) iWidth = 15;

            // We must honor the driver's capabilities:
            int iAllowableWidth = SystemInformation.CursorSize.Width;
            int iAllowableHeight = SystemInformation.CursorSize.Height;

            // Limit the dimensions:
            if (iHeight > iAllowableHeight)
                iHeight = iAllowableHeight;

            if (iWidth > iAllowableWidth)
            {
                iWidth = iAllowableWidth;
                if ((iWidth % 2) != 0)
                    iWidth--; // Enforce odd width
            }
            int iHalfWidth = iWidth / 2;

            // Create byte buffer for masks (one bit for each pixel):
            int iArrayLen = iAllowableWidth * iAllowableHeight / 8;
            byte[] byANDmask = new byte[iArrayLen];
            byte[] byXORmask = new byte[iArrayLen];

            // Clear all XOR mask bits initially:
            for (i = 0; i < iArrayLen; i++)
            {
                // Set all the AND mask bits so there will be no opacity:
                byANDmask[i] = 0xFF;

                // Clear all the XOR mask bits initially:
                byXORmask[i] = 0x00;
            }

            // Calculate number of bytes in one mask row:
            int iNumBytesPerRow = iAllowableWidth / 8;

            // Selectively set the XOR mask bits.
            // First, do cursor's center column, while avoiding serifs.
            // In center column, there's only one pixel "on" per row:
            byte by = (byte)(1 << (7 - iHalfWidth));
            for (i = iHeight - 2; i > 0; i--)
                byXORmask[iNumBytesPerRow * i] = by;

            // Trim the serif pattern to length:
            uint uiSerif = 0xFFFE;
            // Widest serif has width = 15
            for (i = iWidth; i < 15; i++)
                uiSerif <<= 1;

            // Calculate where to put notch in the serifs:
            uint uiNotchMask = 0x0100;
            for (i = iHalfWidth; i < 7; i++)
                uiNotchMask <<= 1;

            // Punch hole in serif pattern, for notch:
            uiSerif ^= uiNotchMask;

            // Extract left and right bytes of serif pattern:
            uint uiSerifLeft = (uiSerif & 0xFF00) >> 8;
            uint uiSerifRight = uiSerif & 0x00FF;

            // Calculate bottom serif's postion in XOR mask array:
            int iBottomSerifOffset = iNumBytesPerRow * (iHeight - 1);

            // Now, set both halves of top serif in the XOR mask:
            byXORmask[0] = (byte)uiSerifLeft;
            byXORmask[1] = (byte)uiSerifRight;

            // Ditto for bottom serif:
            byXORmask[iBottomSerifOffset] = (byte)uiSerifLeft;
            byXORmask[iBottomSerifOffset + 1] = (byte)uiSerifRight;

            // Create cursor via InterOp:
            IntPtr cursorHandle = CreateCursor(
                Process.GetCurrentProcess().Handle, // app. instance
                iHalfWidth,       // hot spot X pos
                iHeight / 2,      // hot spot Y pos
                iAllowableWidth,  // cursor width acceptable to driver
                iAllowableHeight, // cursor height acceptable to driver
                byANDmask,        // AND mask
                byXORmask         // XOR mask
            );

            // The caller needs the new handle:
            return cursorHandle;
        }
    }
}
