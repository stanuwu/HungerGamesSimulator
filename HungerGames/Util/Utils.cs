using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PixelFormat = System.Windows.Media.PixelFormat;

namespace HungerGames.Util
{
    public static class Utils
    {
        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            PixelFormat format = bitmap.PixelFormat switch
            {
                System.Drawing.Imaging.PixelFormat.Format24bppRgb => PixelFormats.Bgr24,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb => PixelFormats.Bgra32,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb => PixelFormats.Bgr32,
                _ => PixelFormats.Default
            };
            BitmapSource source = BitmapSource.Create(data.Width, data.Height, bitmap.HorizontalResolution, bitmap.VerticalResolution, format, null,
                data.Scan0, data.Stride * data.Height, data.Stride);
            bitmap.UnlockBits(data);
            return source;
        }
    }
}