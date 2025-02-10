using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polufabrikkat.Core.Interfaces;
using static System.Net.Mime.MediaTypeNames;
using SkiaSharp;

namespace Polufabrikkat.Core.Utilities
{
    public class AiImageProcessor : IAiImageProcessor
    {
        public async Task<byte[]> AddTextToImageAsync(string imageUrl, string text)
        {
            using (HttpClient client = new HttpClient())
            {
                var imageData = await client.GetByteArrayAsync(imageUrl);

                using (var inputStream = new SKManagedStream(new MemoryStream(imageData)))
                using (var bitmap = SKBitmap.Decode(inputStream))
                using (var canvas = new SKCanvas(bitmap))
                {
                    string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "Montserrat-ExtraBold.ttf");
                    SKTypeface typeface = SKTypeface.FromFile(fontPath);

                    var paint = new SKPaint
                    {
                        Color = SKColors.White,
                        TextSize = 24,
                        IsAntialias = true,
                        Typeface = typeface,
                        TextAlign = SKTextAlign.Center,
                    };

                    float margin = 20;

                    float maxTextWidth = bitmap.Width - 2 * margin;

                    var lines = WrapText(text, paint, maxTextWidth);

                    float lineHeight = paint.FontSpacing;
                    float textBlockHeight = lines.Count * lineHeight;

                    float y = (bitmap.Height - textBlockHeight) / 2 + lineHeight / 2;

                    foreach (var line in lines)
                    {
                        float x = bitmap.Width / 2;

                        canvas.DrawText(line, x, y, paint);
                        y += lineHeight;
                    }

                    using (var image = SKImage.FromBitmap(bitmap))
                    using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 100))
                    {
                        return data.ToArray();
                    }
                }
            }
        }
        private List<string> WrapText(string text, SKPaint paint, float maxWidth)
        {
            var words = text.Split(' ');
            var lines = new List<string>();
            var currentLine = "";

            foreach (var word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                float textWidth = paint.MeasureText(testLine);

                if (textWidth > maxWidth)
                {
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        lines.Add(currentLine);
                        currentLine = word;
                    }
                    else
                    {
                        lines.Add(word);
                        currentLine = "";
                    }
                }
                else
                {
                    currentLine = testLine;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }

            return lines;
        }
    }
}
