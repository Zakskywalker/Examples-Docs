using System.IO;
using SkiaSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

class Program
{
    static void Main()
    {
        int width = 800, height = 800;
        int frameCount = 30;

        using var gif = new Image<Rgba32>(width, height);
        // Configure metadata for endless loop
        var gifMetaData = gif.Metadata.GetGifMetadata();
        gifMetaData.RepeatCount = 0;

        for (int i = 0; i < frameCount; i++)
        {
            float progress = MathF.Min(1.0f, (float)i / (frameCount * 0.7f));
            float angle = (float)i / frameCount * 360f;

            // 1. Draw vector frame using SkiaSharp
            using var surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Rgba8888));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            float cx = width / 2f, cy = height / 2f;

            // Draw center rotating element
            using var hubPaint = new SKPaint { Color = SKColor.Parse("#2B6CB0"), IsAntialias = true };
            canvas.Save();
            canvas.RotateDegrees(angle, cx, cy);
            canvas.DrawCircle(cx, cy, 50, hubPaint);
            canvas.Restore();

            // Draw connectors
            using var linePaint = new SKPaint { 
                Color = SKColor.Parse("#CBD5E0"), 
                StrokeWidth = 4, 
                Style = SKPaintStyle.Stroke, 
                IsAntialias = true 
            };

            SKPoint target = new SKPoint(cx + 250, cy - 150);
            SKPoint currentTarget = new SKPoint(cx + (target.X - cx) * progress, cy + (target.Y - cy) * progress);

            // Draw bezier spline to give that modern mindmap curve
            using var path = new SKPath();
            path.MoveTo(cx, cy);
            path.QuadTo(cx + 100, cy, currentTarget.X, currentTarget.Y);
            canvas.DrawPath(path, linePaint);

            // Draw target node card
            if (progress >= 0.8f)
            {
                using var cardPaint = new SKPaint { Color = SKColor.Parse("#E53E3E"), IsAntialias = true };
                var rect = new SKRoundRect(new SKRect(target.X - 70, target.Y - 25, target.X + 70, target.Y + 25), 10);
                canvas.DrawRoundRect(rect, cardPaint);
            }

            // 2. Convert Skia frame into ImageSharp frame
            using var skImage = surface.Snapshot();
            using var data = skImage.Encode(SKEncodedImageFormat.Png, 100);
            using var ms = new MemoryStream(data.ToArray());
            using var frameImage = SixLabors.ImageSharp.Image.Load<Rgba32>(ms);

            frameImage.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay = 5; // 50ms delay
            gif.Frames.AddFrame(frameImage.Frames.RootFrame);
        }

        // Remove placeholder initial frame and save
        gif.Frames.RemoveFrame(0);
        gif.SaveAsGif("animated_tech_stack.gif");
        Console.WriteLine("GIF generated successfully!");
    }
}

