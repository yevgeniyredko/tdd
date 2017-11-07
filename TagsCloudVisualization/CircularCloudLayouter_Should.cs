using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace TagsCloudVisualization
{
    [TestFixture]
    public class CircularCloudLayouter_Should
    {
        private readonly Point center = new Point(500, 500);
        private CircularCloudLayouter cloudLayouter;
        private List<Rectangle> rectangles;

        [SetUp]
        public void SetUp()
        {
            cloudLayouter = new CircularCloudLayouter(center);
            rectangles = null;
        }

        [Test]
        public void Constructing_ShouldThrowOnNegativeCoordinates()
        {
            Action act = () => new CircularCloudLayouter(new Point(0, -1));
            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void PutNextRectangle_ShouldThrowOnRectanglesBiggerThanField()
        {
            var size = new Size(center.X * 2 + 1, center.Y * 2 + 1);
            Action act = () => cloudLayouter.PutNextRectangle(size);
            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void PutNextRectangle_ShouldPutFirstRectangleInCenter()
        {
            var rect = cloudLayouter.PutNextRectangle(new Size(10, 10));

            rect.Contains(center).Should().BeTrue();
        }

        [TestCase(10, 10)]
        [TestCase(11, 11)]
        public void PutNextRectangle_RectangleShouldHaveCorrectSize(int width, int height)
        {
            var size = new Size(width, height);
            var rect = cloudLayouter.PutNextRectangle(size);

            rect.Size.ShouldBeEquivalentTo(size);
        }

        [TestCase(50, 50, 2)]
        [TestCase(50, 50, 10)]
        [TestCase(50, 50, 20)]
        [TestCase(50, 50, 50)]
        [TestCase(50, 50, 70)]
        public void PutNextRectangle_RectanglesShouldNotIntersect(
            int width, int height, int rectanglesCount)
        {
            var sizes = Enumerable.Repeat(new Size(width, height), rectanglesCount);
            rectangles = sizes.Select(s => cloudLayouter.PutNextRectangle(s)).ToList();

            foreach (var rectangle in rectangles)
            {
                rectangles.Count(r => r.IntersectsWith(rectangle)).Should().Be(1);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome != ResultState.Failure 
                || rectangles == null) 
                return;

            var fieldSize = new Size(center.X * 2, center.Y * 2);
            var bitmap = BitmapDrawer.DrawRectangles(fieldSize, Color.Red, rectangles);
            var outputFilename = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                TestContext.CurrentContext.Test.Name + "_err.bmp");

            bitmap.Save(outputFilename);
            Console.WriteLine($"Tag cloud visualization saved to file {outputFilename}");
        }
    }
}