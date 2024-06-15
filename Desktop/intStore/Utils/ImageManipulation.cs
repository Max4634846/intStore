using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace intStore.Utils
{
    public class ImageManipulation
    {
        /// <summary>
        /// Получает изображение из базы данных по указанному пути к файлу и возвращает его в виде объекта типа ImageSource.
        /// Используется для отображения данных в списке продуктов.
        /// </summary>
        /// <param name="photoPath">Путь к фотографии в базе данных.</param>
        /// <returns>Объект ImageSource, представляющий изображение, или null, если путь пуст или недопустим.</returns>
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
