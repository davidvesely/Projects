using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FreeImageAPI;

namespace ImageProperties
{
    class FreeImageWrapper
    {
        [DllImport("FreeImage.dll")]
        public static extern int FreeImage_Load(FREE_IMAGE_FORMAT format,
                       string filename, int flags);

        [DllImport("FreeImage.dll")]
        public static extern void FreeImage_Unload(int handle);

        [DllImport("FreeImage.dll")]
        public static extern bool FreeImage_Save(FREE_IMAGE_FORMAT format,
           int handle, string filename, int flags);


    }
}
