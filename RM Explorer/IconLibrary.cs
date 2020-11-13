using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace IconLib
{
    public static class IconLibrary
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            MemoryStream memStream = new MemoryStream();
            bitmap.Save(memStream, ImageFormat.Png);
            memStream.Position = 0;
            // Create a new BitmapImage
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }
        public static BitmapImage ToBitmapImage(this Icon ico)
        {
            MemoryStream memStream = new MemoryStream();
            ico.ToBitmap().Save(memStream, ImageFormat.Png);
            memStream.Position = 0;
            // Create a new BitmapImage
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
