using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace BiBilet.Web.Utils
{
    public static class DiskUtils
    {
        /// <summary>
        /// Saves given stream as a jpeg file
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="outputFilename"></param>
        /// <returns></returns>
        public static bool SaveImage(Stream fileStream, string outputFilename)
        {
            Bitmap bmp = null;
            EncoderParameters encoderParameters = null;

            try
            {
                bmp = new Bitmap(fileStream);

                var imgCodecInfo = GetEncoderInfo("image/jpeg");

                var encoder = Encoder.Quality;
                encoderParameters = new EncoderParameters(1)
                {
                    Param = {[0] = new EncoderParameter(encoder, 90L)}
                };

                bmp.Save(outputFilename, imgCodecInfo, encoderParameters);

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                bmp?.Dispose();
                encoderParameters?.Dispose();
            }
        }

        /// <summary>
        /// Permanently deletes a directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static bool DeleteDirectory(string directory)
        {
            try
            {
                if (directory == null)
                    throw new ArgumentNullException(nameof(directory));

                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }

                return true;
            }
            catch
            {
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
                if (filePath == null)
                    throw new ArgumentNullException(nameof(filePath));

                if (System.IO.File.Exists(filePath))
                {
                    var fInfo = new FileInfo(filePath);
                    fInfo.Delete();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns an <see cref="ImageCodecInfo"/> for given mime type
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            var encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
}