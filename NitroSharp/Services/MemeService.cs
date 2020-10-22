using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Threading.Tasks;

namespace NitroSharp.Services
{
    public class MemeService : IDisposable
    {
        private static readonly Dictionary<string, int> FontIds = new Dictionary<string, int>
        {
            { "architect", 0 },
            { "bangers", 1 },
            { "apple", 2 },
            { "robotoblack", 3 },
            { "roboto", 4 },
            { "schoolbell", 5 }
        };

        private readonly PrivateFontCollection FontCollection = new PrivateFontCollection();
        private readonly FontFamily[] fonts;

        private Brush? defaultBrush;
        private Font? font;
        private Bitmap? map;
        private Graphics? img;

        public MemeService()
        {
            // TODO update paths to be Path.Join for OS specific releases.
            FontCollection.AddFontFile("./Resources/Fonts/ArchitectsDaughter-Regular.ttf");
            FontCollection.AddFontFile("./Resources/Fonts/Bangers-Regular.ttf");
            FontCollection.AddFontFile("./Resources/Fonts/HomemadeApple-Regular.ttf");
            FontCollection.AddFontFile("./Resources/Fonts/Roboto-Black.ttf");
            FontCollection.AddFontFile("./Resources/Fonts/Roboto-Regular.ttf");
            FontCollection.AddFontFile("./Resources/Fonts/Schoolbell-Regular.ttf");

            fonts = FontCollection.Families;
        }

        public async Task<MemoryStream?> BuildMemeAsync(byte[] image, Tuple<Rectangle, string, Brush?>[] captions, string? font = null, int fsize = 16, Brush? defaultBrush = null)
        {
            var a = RegisterBitmap(image);
            var b = RegisterFont(font, fsize);
            var c = RegisterBrush(defaultBrush);

            await a; await b; await c;

            foreach (var line in captions)
            {
                await AddText(line.Item1, line.Item2, line.Item3);
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
            if (FontIds.TryGetValue(font ?? "", out int id))
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
                this.defaultBrush = new SolidBrush(Color.Black);
            else
                this.defaultBrush = brush;

            return Task.CompletedTask;
        }

        public Task AddText(Rectangle position, string caption, Brush? brush)
        {
            if (this.img is null) throw new Exception("Bitmap is null. A Bitmap must be registered with MemeService.RegisterBitmap");

            // Register a font and brush if the saved item is null
            if (this.font is null) RegisterFont(null);
            if (this.defaultBrush is null) RegisterBrush(null);


            this.img.DrawString(caption, this.font, brush ?? this.defaultBrush, position);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            img?.Dispose();
            defaultBrush?.Dispose();
            font?.Dispose();
        }
    }
}
