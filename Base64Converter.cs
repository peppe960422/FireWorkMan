using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireWorkMan
{
    public static class ImageBase64Converter
    {
        public static string ImageFileToBase64(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                throw new ArgumentNullException(nameof(imagePath));

            if (!File.Exists(imagePath))
                throw new FileNotFoundException($"File non trovato: {imagePath}");

            using (var image = Image.FromFile(imagePath))
            {

                string extension = Path.GetExtension(imagePath).ToLower();
                ImageFormat format = GetImageFormatFromExtension(extension);

                return ImageToBase64(image, format);
            }
        }


        public static string ImageToBase64(Image image, ImageFormat format = null)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            format ??= ImageFormat.Jpeg;

            using (var ms = new MemoryStream())
            {

                image.Save(ms, format);


                byte[] imageBytes = ms.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);

                return base64String;
            }
        }
        private static string CleanBase64String(string base64String)
        {
            if (base64String.Contains("base64,"))
            {
                // Rimuove il prefisso "data:image/format;base64,"
                int base64Index = base64String.IndexOf("base64,", StringComparison.Ordinal) + 7;
                return base64String[base64Index..];
            }

            return base64String;
        }

        private static ImageFormat GetImageFormatFromExtension(string extension)
        {
            return extension switch
            {
                ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                ".png" => ImageFormat.Png,
                ".bmp" => ImageFormat.Bmp,
                ".gif" => ImageFormat.Gif,
                ".tiff" or ".tif" => ImageFormat.Tiff,
                ".ico" => ImageFormat.Icon,
                _ => ImageFormat.Jpeg // Default
            };
        }

        public static Image Base64ToImage(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                throw new ArgumentNullException(nameof(base64String));

            string cleanBase64 = CleanBase64String(base64String);


            byte[] imageBytes = Convert.FromBase64String(cleanBase64);


            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);


                Image image = Image.FromStream(ms, true);
                return new Bitmap(image);
            }
        }

    }
}
