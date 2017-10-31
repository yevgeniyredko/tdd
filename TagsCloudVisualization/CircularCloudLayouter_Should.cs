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

        [SetUp]
        public void SetUp()
        {
            cloudLayouter = new CircularCloudLayouter(center);
        }

        [Test]
        public void Constructing_ShouldThrowOnNegativeCoordinates()
        {
            Action act = () => new CircularCloudLayouter(new Point(0, -1));
            act.ShouldThrow<ArgumentException>();
        }

        [Test]
        public void Center_ShouldBeCentralPointOfField()
        {
            var fieldCenter = new Point(
                cloudLayouter.FieldSize.Width / 2, 
                cloudLayouter.FieldSize.Height / 2);
            cloudLayouter.Center.Should().Be(fieldCenter);
        }

        [Test]
        public void PutNextRectangle_ShouldThrowOnRectanglesBiggerThanField()
        {
            var size = new Size(cloudLayouter.FieldSize.Width + 1, cloudLayouter.FieldSize.Height + 1);
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
        public void PutNextRectangle_RectanglesShouldNotIntersect(int maxWidth, int maxHeight, int rectanglesCount)
        {
            var sizes = GenerateRandomSizes(maxWidth, maxHeight, rectanglesCount);

            var rectangles = new List<Rectangle>();
            foreach (var size in sizes)
            {
                var rectangle = cloudLayouter.PutNextRectangle(size);
                rectangles.Add(rectangle);
            }

            foreach (var rectangle in rectangles)
            {
                rectangles.Count(r => r.IntersectsWith(rectangle)).Should().Be(1);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome == ResultState.Failure)
            {
                var bmp = cloudLayouter.DrawBitmap();
                var outputFilename = Path.Combine(
                    TestContext.CurrentContext.TestDirectory,
                    TestContext.CurrentContext.Test.Name + "_err.bmp");

                bmp.Save(outputFilename);
                Console.WriteLine($"Tag cloud visualization saved to file {outputFilename}");
            }
        }

        private static List<Size> GenerateRandomSizes(int maxWidth, int maxHeight, int count)
        {
            var result = new List<Size>();

            var rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                result.Add(new Size(rnd.Next(1, maxWidth + 1), rnd.Next(1, maxHeight + 1)));
            }

            return result;
        }
    }
}