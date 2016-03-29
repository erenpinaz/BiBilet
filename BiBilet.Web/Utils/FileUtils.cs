using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace BiBilet.Web.Utils
{
    public static class FileUtils
    {
        /// <summary>
        /// Crops and saves cropped image as jpeg
        /// </summary>
        /// <param name="content"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="outputFilename"></param>
        /// <returns></returns>
        public static bool CropSaveImage(Stream content, int x, int y, int width, int height, string outputFilename)
        {
            try
            {
                var cropRect = new Rectangle(x, y, width, height);

                using (var sourceBitmap = new Bitmap(content))
                using (var newBitMap = new Bitmap(cropRect.Width, cropRect.Height))
                using (var graphics = Graphics.FromImage(newBitMap))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;

                    graphics.DrawImage(sourceBitmap, new Rectangle(0, 0, newBitMap.Width, newBitMap.Height),
                        cropRect, GraphicsUnit.Pixel);

                    var imgCodecInfo = GetEncoderInfo("image/jpeg");

                    var encoder = Encoder.Quality;
                    var encoderParameters = new EncoderParameters(1)
                    {
                        Param = {[0] = new EncoderParameter(encoder, 75L)}
                    };

                    newBitMap.Save(outputFilename, imgCodecInfo, encoderParameters);

                    return true;
                }
            }
            catch
            {
                //TODO: Log error
                return false;
            }
        }

        /// <summary>
        /// Permanently deletes a directory
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static bool DeleteDirectory(string directoryPath)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
                }

                return true;
            }
            catch
            {
                //TODO: Log error
                return false;
            }
        }

        /// <summary>
        /// Permanently deletes a file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var fInfo = new FileInfo(filePath);
                    fInfo.Delete();
                }

                return true;
            }
            catch
            {
                //TODO: Log error
                return false;
            }
        }

        /// <summary>
        /// Creates unique file name
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GenerateFileName(string prefix, string extension)
        {
            return string.Format(@"{0}-{1}.{2}", prefix, Guid.NewGuid().ToString("N"), extension);
        }

        /// <summary>
        /// Returns an <see cref="ImageCodecInfo" /> for given mime type
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var encoders = ImageCodecInfo.GetImageEncoders();

            return encoders.FirstOrDefault(t => t.MimeType == mimeType);
        }
    }
}