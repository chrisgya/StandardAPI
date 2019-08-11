using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace StandardAPI.Common
{
    public static class Utility
    {

        public static IConfigurationRoot AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            return configurationBuilder.Build();
        }

        public static bool validateImage(string imagestring)
        {
            try
            {
                if (string.IsNullOrEmpty(imagestring))
                {
                    return false;
                }

                byte[] imgByte = Convert.FromBase64String(imagestring);

                using (MemoryStream ms = new MemoryStream(imgByte))
                {
                    using (Image img = Image.FromStream(ms))
                    {
                        if (img.RawFormat.Equals(ImageFormat.Bmp) ||
                            img.RawFormat.Equals(ImageFormat.Gif) ||
                            img.RawFormat.Equals(ImageFormat.Jpeg) ||
                            img.RawFormat.Equals(ImageFormat.Png))
                        {
                            return true;
                        }
                    }
                }


            }
            catch (Exception)
            {
            }

            return false;
        }


        public static DateTime GetLastDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }

    }
}
