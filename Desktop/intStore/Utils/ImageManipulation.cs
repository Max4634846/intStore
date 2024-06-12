using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace intStore.Utils
{
    public class ImageManipulation
    {
        public ImageSource GetPhotoFromDataBase(string photoPath)
        {
            if (!string.IsNullOrEmpty(photoPath))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(photoPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;
            }
            return null;
        }
    }
}
