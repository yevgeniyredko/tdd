using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter
    {
        private readonly Point center;
        private Size FieldSize => new Size(center.X * 2, center.Y * 2);
        private readonly List<Rectangle> rectangles;
        private readonly IEnumerator<Point> spiralEnumerator;

        public CircularCloudLayouter(Point center)
        {
            if (center.X < 0 || center.Y < 0)
                throw new ArgumentException(nameof(center) + " coordinates must " +
                                            $"be nonnegative, but were {center}");

            this.center = center;
            this.rectangles = new List<Rectangle>();
            this.spiralEnumerator = CalculatePointsOnSpiral(center).GetEnumerator();
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Width > FieldSize.Width || rectangleSize.Height > FieldSize.Height)
                throw new ArgumentException(nameof(rectangleSize) + " must be " +
                                            $"smaller than field size {FieldSize}, " +
                                            $"but was {rectangleSize}");

            while (spiralEnumerator.MoveNext())
            {
                var rectangle = CreateRectangle(spiralEnumerator.Current, rectangleSize);

                if (rectangles.Any(r => r.IntersectsWith(rectangle)) 
                    || rectangle.Left < 0 || rectangle.Top < 0
                    || rectangle.Right > FieldSize.Width || rectangle.Bottom > FieldSize.Height)
                    continue;

                rectangles.Add(rectangle);
                return rectangle;
            }

            throw new InvalidOperationException("Rectangle can't be put on field");
        }

        private static IEnumerable<Point> CalculatePointsOnSpiral(
            Point center, 
            double coefficient = 0.5, 
            double angleDelta = 0.2)
        {
            var angle = 0.0;
            Point? previous = null;
            while (true)
            {
                var length = angle * coefficient;
                angle += angleDelta;

                var x = (int) (length * Math.Cos(angle)) + center.X;
                var y = (int) (length * Math.Sin(angle)) + center.Y;

                if (x < 0 || y < 0 || x > center.X * 2 || y > center.Y * 2)
                    yield break;
                if (previous != null && x == previous.Value.X && y == previous.Value.Y)
                    continue;

                var result = new Point(x, y);
                yield return result;
                previous = result;
            }
        }

        private static Rectangle CreateRectangle(Point center, Size size)
        {
            if (size.Width <= 0 || size.Height <= 0)
                throw new ArgumentException($"{nameof(size)} must be positive, but was {size}");

            var location = new Point(center.X - size.Width / 2, center.Y - size.Height / 2);
            return new Rectangle(location, size);
        }
    }
}