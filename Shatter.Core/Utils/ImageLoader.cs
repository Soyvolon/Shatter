using System.Drawing;
using System.IO;
using System.Net;

namespace Shatter.Core.Utils
{
	public static class ImageLoader
    {
        public static Bitmap? GetBitmapFromUrl(string url)
        {
            using var client = new WebClient();
            var bytes = client.DownloadData(url);
            return new Bitmap(new MemoryStream(bytes));
        }
    }
}