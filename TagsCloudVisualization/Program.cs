using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualization
{
    class Program
    {
        static void Main(string[] args)
        {
            var center = new Point(250, 250);
            var cloudLayouter = new CircularCloudLayouter(center);
            var rectangleSize = new Size(50, 40);
            var rectangles = new List<Rectangle>();

            const int count = 75;

            for (var i = 0; i < count; i++)
            {
                rectangles.Add(cloudLayouter.PutNextRectangle(rectangleSize));
            }

            var bitmapSize = new Size(center.X * 2, center.Y * 2);
            var bmp = BitmapDrawer.DrawRectangles(bitmapSize, Color.Yellow, rectangles);
            bmp.Save($"cloud{count}.bmp");
        }
    }
}
