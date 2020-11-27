using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Svg;

namespace Shatter.Core.Utils
{
	public static class SvgHandler
    {
        private static readonly HttpClient avatarClient = new HttpClient();

        public static async Task<MemoryStream?> GetWelcomeImage(bool welcome, string username, string pfpUrl)
        {
            try
            {
                // Load path
                var path = Path.GetFullPath(Path.Join("Resources", "Images", "welcome.svg"));
                // Load XML to File Stream
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                using var sr = new StreamReader(fs);

                var content = await sr.ReadToEndAsync();

                content = content.Replace("${title}", welcome ? "Welcome" : "Goodbye");
                content = content.Replace("${username}", username);

                // Create the new SVG Document
                var svg = SvgDocument.FromSvg<SvgDocument>(content);

                // Get the font size
                float font = username.Length <= 12 ? 44f : 44f - ((username.Length - 12f) * 1.1f);

                // Set the SVG properties to fill with custom content instead of placeholdres
                var msg = svg.GetElementById("notice_message");
                msg.FontSize = new SvgUnit(font);

                // Get the graphics and Memory Stream objects
                using var map = new Bitmap(1000, 300);
                using var graphic = Graphics.FromImage(map);

                // Draw the SVG to the graphics object
                svg.Draw(graphic);

                // Get the PFP Uri with an adjusted size.
                var uri = new Uri(pfpUrl);

                // Get the byte[] of the pfp
                var bytes = await avatarClient.GetByteArrayAsync(uri);

                // Load the graphics object for the data.
                using MemoryStream iconStream = new MemoryStream(bytes);
                using Bitmap iconMap = new Bitmap(iconStream);

                using var circleRes = ClipToCircle(iconMap, new PointF(iconMap.Width / 2f, iconMap.Height / 2f), iconMap.Width / 2f, Color.Transparent);

                using Bitmap resize = new Bitmap(circleRes, new Size(256, 256));

                // Draw the icon over the rest of the welcome image.
                graphic.DrawImage(resize, new Point(22, 22));

                // Save Graphic to new memory stream for sending purposes.
                MemoryStream? mem = new MemoryStream();
                map?.Save(mem, ImageFormat.Png);

                // Seek to beginning of file.
                mem?.Seek(0, SeekOrigin.Begin);

                return mem;
            }
            catch (Exception ex)
            {
                CoreUtils.Logger.LogError(ex, "SVG Handler Failed");
                return null;
            }
        }

        // makes nice round ellipse/circle images from rectangle images
        public static Image ClipToCircle(Image srcImage, PointF center, float radius, Color backGround)
        {
            Image dstImage = new Bitmap(srcImage.Width, srcImage.Height, srcImage.PixelFormat);

            using (Graphics g = Graphics.FromImage(dstImage))
            {
                RectangleF r = new RectangleF(center.X - radius, center.Y - radius,
                                                         radius * 2, radius * 2);

                // enables smoothing of the edge of the circle (less pixelated)
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // fills background color
                using (Brush br = new SolidBrush(backGround))
                {
                    g.FillRectangle(br, 0, 0, dstImage.Width, dstImage.Height);
                }

                // adds the new ellipse & draws the image again 
                GraphicsPath path = new GraphicsPath();
                path.AddEllipse(r);
                g.SetClip(path);
                g.DrawImage(srcImage, 0, 0);

                return dstImage;
            }
        }
    }
}
