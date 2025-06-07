using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Chrome_WPF.Converters
{
    public class Base64ToImageConverter : IValueConverter
    {
        private static readonly string DefaultImagePath = "pack://application:,,,/Chrome_WPF;component/Resources/img/dauchamhoi.jpg";
        private static BitmapImage? _defaultImage;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string base64 || string.IsNullOrWhiteSpace(base64))
            {
                Console.WriteLine("Base64 string is null or empty. Returning default image.");
                return GetDefaultImage();
            }

            Console.WriteLine($"Converting Base64. Length: {base64.Length}, First 50 chars: {base64.Substring(0, Math.Min(50, base64.Length))}...");
            try
            {
                byte[] binaryData = System.Convert.FromBase64String(base64);
                if (binaryData.Length == 0)
                {
                    Console.WriteLine("Decoded Base64 data is empty. Returning default image.");
                    return GetDefaultImage();
                }

                using var ms = new MemoryStream(binaryData);
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
                return image;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Invalid Base64 string: {ex.Message}. Input: {base64.Substring(0, Math.Min(50, base64.Length))}...");
                return GetDefaultImage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting Base64 to image: {ex.Message}");
                return GetDefaultImage();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Base64ToImageConverter is intended for one-way binding only.");
        }

        private static BitmapImage GetDefaultImage()
        {
            if (_defaultImage != null) return _defaultImage;

            lock (typeof(Base64ToImageConverter))
            {
                if (_defaultImage != null) return _defaultImage;

                try
                {
                    _defaultImage = new BitmapImage();
                    _defaultImage.BeginInit();
                    _defaultImage.UriSource = new Uri(DefaultImagePath, UriKind.Absolute);
                    _defaultImage.CacheOption = BitmapCacheOption.OnLoad;
                    _defaultImage.EndInit();
                    _defaultImage.Freeze();
                    return _defaultImage;
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Failed to load default image from {DefaultImagePath}: {ex.Message}");
                    return CreateEmptyBitmapImage();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error loading default image: {ex.Message}");
                    return CreateEmptyBitmapImage();
                }
            }
        }

        private static BitmapImage CreateEmptyBitmapImage()
        {
            var emptyImage = new BitmapImage();
            emptyImage.BeginInit();
            emptyImage.CreateOptions = BitmapCreateOptions.None;
            emptyImage.EndInit();
            emptyImage.Freeze();
            return emptyImage;
        }
    }
}