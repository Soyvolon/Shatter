using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace Shatter.Core.Utils
{
    public static class ImageUtils
    {
        public static Task Negate(Bitmap image)
        {
            Bitmap clone = (Bitmap)image.Clone();

            using Graphics g = Graphics.FromImage(image);

            // negation ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
                new float[][]
                    {
                            new float[] {-1, 0, 0, 0, 0},
                            new float[] {0, -1, 0, 0, 0},
                            new float[] {0, 0, -1, 0, 0},
                            new float[] {0, 0, 0, 1, 0},
                            new float[] {1, 1, 1, 0, 1}
                    });

            ImageAttributes attributes = new ImageAttributes();

            attributes.SetColorMatrix(colorMatrix);

            g.DrawImage(clone, new Rectangle(0, 0, clone.Width, clone.Height),
                        0, 0, clone.Width, clone.Height, GraphicsUnit.Pixel, attributes);

            return Task.CompletedTask;
        }

        public static Task<Bitmap> Pixelate(Bitmap image)
        {
            using var downscaled = new Bitmap(image, new Size((int)Math.Floor(image.Width * .25), (int)Math.Floor(image.Height * .25)));
            var scaled = new Bitmap(downscaled, new Size(image.Width, image.Height));

            return Task.FromResult(scaled);
        }

        public static Task<Bitmap> Swirl(Bitmap image, double swirlTwists)
        {
            double swirlX = (image.Width - 1) / 2, swirlY = (image.Height - 1) / 2;

            double swirlRadius;
            if (image.Height > image.Width)
                swirlRadius = image.Width * .75f;
            else
                swirlRadius = image.Height * .75f;

            Bitmap swirled = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < (swirled.Height - 1); y++)
            {
                for (int x = 0; x < (swirled.Width - 1); x++)
                {
                    // Compute the distance and angle from the swirl center.
                    double pixelX = x - swirlX;
                    double pixelY = y - swirlY;
                    double pixelDistance = Math.Sqrt(Math.Pow(pixelX, 2) + Math.Pow(pixelY, 2));
                    double pixelAngle = Math.Atan2(pixelX, pixelY);

                    double swirlAmount = 1.0f - (pixelDistance / swirlRadius);
                    if (swirlAmount > 0.0f)
                    {
                        double twistAngle = swirlTwists * swirlAmount * Math.PI * 2.0;

                        // adjust the pixel angle and computer the adjusted pixel co-ordinates:
                        pixelAngle += twistAngle;
                        pixelX = Math.Cos(pixelAngle) * pixelDistance;
                        pixelY = Math.Sin(pixelAngle) * pixelDistance;
                    }

                    // read and write the pixel
                    Color src;
                    try
                    {
                        var xComp = (int)(swirlX + pixelX);
                        var oI = xComp < 0 || xComp >= swirled.Width ? x : xComp;

                        var yComp = (int)(swirlY + pixelY);
                        var oY = yComp < 0 || yComp >= swirled.Height ? y : yComp;

                        src = image.GetPixel(oI, oY);
                    }
                    catch
                    {
                        src = Color.Black;
                    }

                    swirled.SetPixel(x, y, src);
                }
            }

            return Task.FromResult(swirled);
        }
    }
}
