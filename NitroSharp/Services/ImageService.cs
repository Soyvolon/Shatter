using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;

namespace NitroSharp.Services
{
    public class ImageService
    {
        private const string ImgDir = "./Resources/Images";
        private Dictionary<string, string> FilePaths  = new Dictionary<string, string>()
        {
            {"burn", "burn_img.gif" }
        };

        public ImageService()
        {
            Dictionary<string, string> fullLengthPaths = new Dictionary<string, string>();
            // Get the full paths from the local paths of the images.
            foreach (var image in FilePaths)
            {
                fullLengthPaths.TryAdd(image.Key, Path.GetFullPath(Path.Combine(ImgDir, image.Value)));
            }

            FilePaths = fullLengthPaths;
        }

        /// <summary>
        /// Gets a new file stream for the selected image.
        /// </summary>
        /// <param name="image">Relative name of the image.</param>
        /// <returns>A FileStream to send to discord, or null if the image is not found.</returns>
        public FileStream? GetImageStream(string image)
        {
            if (FilePaths.TryGetValue(image, out string? path))
            {
                var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                return fs;
            }
            else return null;
        }
    }
}
