using System.Drawing;

namespace TagsCloudVisualization
{
    class Program
    {
        static void Main(string[] args)
        {
            var layouter = new CircularCloudLayouter(new Point(250, 250));
            var size = new Size(50, 40);
            const int count = 30;

            for (var i = 0; i < count; i++)
            {
                layouter.PutNextRectangle(size);
            }

            var bmp = layouter.DrawBitmap();
            bmp.Save($"cloud{count}.bmp");
        }
    }
}
