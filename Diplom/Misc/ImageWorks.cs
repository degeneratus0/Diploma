using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Diplom.Misc
{
    static class ImageWorks
    {
        public static BitmapImage Display(byte[] image)
        {
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = new MemoryStream(image);
            img.EndInit();
            return img;
        }
    }
}
