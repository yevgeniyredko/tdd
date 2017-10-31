using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        public readonly Point Center;
        public Size FieldSize => new Size(Center.X * 2, Center.Y * 2);
        public readonly List<Rectangle> Rectangles;

        public CircularCloudLayouter(Point center)
        {
            if (center.X < 0 || center.Y < 0)
                throw new ArgumentException("Coordinate should be positive");

            this.Center = center;
            this.Rectangles = new List<Rectangle>();
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Width > FieldSize.Width || rectangleSize.Height > FieldSize.Height)
                throw new ArgumentException("Rectangle should be smaller than field");

            var rectangle = GetSpiralPoints(Center)
                .Select(p => CreateRectangleWithCenter(p, rectangleSize))
                .First(rect => !Rectangles.Any(r => r.IntersectsWith(rect)));
            Rectangles.Add(rectangle);

            return rectangle;
        }

        public Bitmap DrawBitmap()
        {
            var bmp = new Bitmap(FieldSize.Width, FieldSize.Height, PixelFormat.Format24bppRgb);

            using (var graphics = Graphics.FromImage(bmp))
            using (var brush = new SolidBrush(Color.Yellow))
            {
                foreach (var rectangle in Rectangles)
                {
                    graphics.FillRectangle(brush, rectangle);
                }
            }

            return bmp;
        }

        private static IEnumerable<Point> GetSpiralPoints(
            Point center, 
            double coefficient = 0.5, 
            double angleDelta = 0.2)
        {
            var angle = 0.0;
            while (true)
            {
                var length = angle * coefficient;

                var x = (int) (length * Math.Cos(angle)) + center.X;
                var y = (int) (length * Math.Sin(angle)) + center.Y;

                if (x < 0 || y < 0 || x > center.X * 2 || y > center.Y * 2)
                    yield break;

                yield return new Point(x, y);

                angle += angleDelta;
            }
        }

        private static Rectangle CreateRectangleWithCenter(Point center, Size size)
        {
            var location = new Point(center.X - size.Width / 2, center.Y - size.Height / 2);
            return new Rectangle(location, size);
        }
    }
}