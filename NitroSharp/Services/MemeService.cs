using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NitroSharp.Properties;

namespace NitroSharp.Services
{
    public class MemeService : IDisposable
    {
        private static readonly Dictionary<string, int> FontIds = new Dictionary<string, int>
        {
            { "arial", 0 },
            { "arialblack", 1 },
            { "comicsans", 2 },
            { "handwritten", 3 },
            { "sftoon", 4 }
        };

        private readonly PrivateFontCollection FontCollection = new PrivateFontCollection();
        private readonly FontFamily[] fonts;

        private Brush? brush;
        private Font? font;
        private Bitmap? map;
        private Graphics? img;

        public MemeService()
        {
            FontCollection.AddFontFile("./Resources/Fonts/arial.ttf");
            FontCollection.AddFontFile("./Resources/Fonts/arialblack.ttf");
            FontCollection.AddFontFile("./Resources/Fonts/comicsans.ttf");
            FontCollection.AddFontFile("./Resources/Fonts/handwritten.ttf");
            FontCollection.AddFontFile("./Resources/Fonts/sftoon.ttf");

            fonts = FontCollection.Families;
        }

        public async Task<MemoryStream?> BuildMemeAsync(byte[] image, Tuple<Rectangle, string>[] captions, string? font = null, int fsize = 16, Brush? brush = null)
        {
            var a = RegisterBitmap(image);
            var b = RegisterFont(font, fsize);
            var c = RegisterBrush(brush);

            await a; await b; await c;

            foreach (var line in captions)
            {
                await AddText(line.Item1, line.Item2);
            }

            var mem = new MemoryStream();

            this.map?.Save(mem, ImageFormat.Png);

            mem?.Seek(0, SeekOrigin.Begin);

            return mem;
        }

        public Task RegisterBitmap(byte[] image)
        {
            var memory = new MemoryStream(image);
            this.map = new Bitmap(memory);
            this.img = Graphics.FromImage(this.map);

            return Task.CompletedTask;
        }

        public Task RegisterFont(string? font, int size = 18)
        {
            Font? f = null;
            if(FontIds.TryGetValue(font, out int id))
            {
                try
                {
                    f = new Font(family: fonts[id], size);
                }
                catch { }
            }

            if (f is null)
                f = new Font("Arial", size);

            this.font = f;

            return Task.CompletedTask;
        }

        public Task RegisterBrush(Brush? brush = null)
        {
            if (brush is null)
                this.brush = new SolidBrush(Color.Black);
            else
                this.brush = brush;

            return Task.CompletedTask;
        }

        public Task AddText(Rectangle position, string caption)
        {
            if (this.img is null) throw new Exception("Bitmap is null. A Bitmap must be registered with MemeService.RegisterBitmap");

            // Register a font and brush if the saved item is null
            if (this.font is null) RegisterFont(null);
            if (this.brush is null) RegisterBrush(null);

            this.img.DrawString(caption, this.font, this.brush, position);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            img?.Dispose();
            brush?.Dispose();
            font?.Dispose();
        }
    }
}
