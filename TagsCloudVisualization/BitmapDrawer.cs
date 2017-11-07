using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace TagsCloudVisualization
{
    public class BitmapDrawer
    {
        public static Bitmap DrawRectangles(
            Size bitmapSize, Color rectangleColor, IEnumerable<Rectangle> rectangles)
        {
            var bitmap = new Bitmap(bitmapSize.Width, bitmapSize.Height, PixelFormat.Format24bppRgb);

            using (var graphics = Graphics.FromImage(bitmap))
            using (var brush = new SolidBrush(rectangleColor))
            {
                foreach (var rectangle in rectangles)
                {
                    graphics.FillRectangle(brush, rectangle);
                }
            }

            return bitmap;
        }
    }
}