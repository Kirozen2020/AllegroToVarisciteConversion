using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AllegroToVarisciteConversion
{
    internal class ImageManager
    {
        /*----------------- Variables ------------------*/

        /// <summary>
        /// Gets or sets the bitmap.
        /// </summary>
        /// <value>
        /// The bitmap.
        /// </value>
        public Bitmap MotherBoardImage { get; set; }
        /// <summary>
        /// Gets or sets the coords.
        /// </summary>
        /// <value>
        /// The coords.
        /// </value>
        public List<MyDictionary> Coords { get; set; }
        /// <summary>
        /// Gets or sets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public List<string> Names { get; set; }
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public Image Image { get; set; }
        /// <summary>
        /// Gets or sets all points.
        /// </summary>
        /// <value>
        /// All points.
        /// </value>
        public List<List<Point3D>> AllPoints { get; set; }
        /// <summary>
        /// Gets or sets the red points.
        /// </summary>
        /// <value>
        /// The red points.
        /// </value>
        public List<List<Point3D>> RedPoints { get; set; }

        /*----------------- Class constructor ------------------*/

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageManager"/> class.
        /// </summary>
        /// <param name="coords">The coords.</param>
        public ImageManager(List<MyDictionary> coords)
        {
            Coords = coords;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageManager"/> class.
        /// </summary>
        /// <param name="coords">The coords.</param>
        /// <param name="allPoints">All points.</param>
        /// <param name="redPoints">The red points.</param>
        public ImageManager(List<MyDictionary> coords, List<List<Point3D>> allPoints, List<List<Point3D>> redPoints) : this(coords)
        {
            AllPoints = allPoints;
            RedPoints = redPoints;

            DrawPoints(allPoints, redPoints);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageManager"/> class.
        /// </summary>
        /// <param name="coords">The coords.</param>
        /// <param name="allPoints">All points.</param>
        public ImageManager(List<MyDictionary> coords, List<List<Point3D>> allPoints) : this(coords)
        {
            AllPoints = allPoints;
            RedPoints = new List<List<Point3D>>();

            DrawPoints(allPoints, RedPoints);
        }

        /*----------------- Main functions ------------------*/

        /// <summary>
        /// Draws the points.
        /// </summary>
        /// <param name="pointLists">The point lists.</param>
        /// <param name="redElements"></param>
        private void DrawPoints(IReadOnlyCollection<List<Point3D>> pointLists, IEnumerable<List<Point3D>> redElements)
        {
            LogManager.AddCommentLine(LogManager.LogLevel.Informational, "Start drawing scheme");

            if (pointLists == null)
                return;

            using (var bmp = new Bitmap(FindMaxOrMinXOrY('x', "max") + 30, FindMaxOrMinXOrY('y', "max") + 30))
            {

                using (var g = Graphics.FromImage(bmp))
                {
                    // Clear the PictureBox
                    g.Clear(Color.White);

                    // Draw lines for each list of points
                    using (var pen = new Pen(Color.Black, 2))
                    {
                        foreach (var points in pointLists.Where(points => points.Count > 1))
                        {
                            DrawArcs(g, pen, points);
                            // Connect the last point with the first point to complete the figure
                            g.DrawLine(pen, points[points.Count - 1].GetRegularPoint(), points[0].GetRegularPoint());
                        }
                    }

                    using (var pen = new Pen(Color.Red, 5))
                    {
                        foreach (var points in redElements.Where(points => points.Count > 1))
                        {
                            DrawArcs(g, pen, points);
                            // Connect the last point with the first point to complete the figure
                            g.DrawLine(pen, points[points.Count - 1].GetRegularPoint(), points[0].GetRegularPoint());
                        }
                    }
                }

                // Assign the updated bitmap to the PictureBox
                if (!IsBitmapFormatCompatible(bmp))
                {
                    LogManager.AddCommentLine(LogManager.LogLevel.Error,
                        "Error!!! Bitmap format is wrong, cannot convert bitmap to image");
                    MessageBox.Show(@"BitMap format error", @"Attention!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.MotherBoardImage = bmp;
                this.MotherBoardImage = AddText(this.MotherBoardImage);
                this.Image = Image.FromHbitmap(this.MotherBoardImage.GetHbitmap());

                this.Names = new List<string>();
                foreach (var item in this.Coords)
                {
                    this.Names.Add(item.Key);
                }

                this.Names.Sort(CustomStringComparer);

            }
        }

        /// <summary>
        /// Draws the arcs.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="points">The points.</param>
        private static void DrawArcs(Graphics g, Pen pen, IReadOnlyList<Point3D> points)
        {
            for (var i = 0; i < points.Count - 1; i++)
            {
                if (i + 1 < points.Count - 1)
                {

                    if (!points[i + 1].IsRegularPoint())
                    {
                        var start = points[i].GetRegularPoint();
                        var center = points[i + 1].GetRegularPoint();
                        var isClockwise = !points[i + 1].Z.Equals("0");
                        var end = points[i + 2].GetRegularPoint();

                        var radius = (int)Math.Sqrt(Math.Pow(center.X - start.X, 2) + Math.Pow(center.Y - start.Y, 2));
                        var x = center.X - radius;
                        var y = center.Y - radius;
                        var wigth = 2 * radius;
                        var height = 2 * radius;

                        var startAngle = (float)Math.Atan2(start.Y - center.Y, start.X - center.X) * 180 / (float)Math.PI;
                        var endAngle = (float)Math.Atan2(end.Y - center.Y, end.X - center.X) * 180 / (float)Math.PI;
                        var sweepAngle = endAngle - startAngle;

                        if (Math.Abs(sweepAngle) == 0)
                        {
                            g.DrawEllipse(pen, x, y, wigth, height);
                        }
                        else
                        {
                            switch (isClockwise)
                            {
                                case true when sweepAngle < 0:
                                    sweepAngle += 360;
                                    break;
                                case false when sweepAngle > 0:
                                    sweepAngle -= 360;
                                    break;
                            }

                            g.DrawArc(pen, x, y, wigth, height, startAngle, sweepAngle);
                        }
                        g.DrawLine(pen, start.X, start.Y, end.X, end.Y);
                    }

                }
                else
                {
                    g.DrawLine(pen, points[i].GetRegularPoint(), points[i + 1].GetRegularPoint());
                }
                g.DrawLine(pen, points[i].GetRegularPoint(), points[i + 1].GetRegularPoint());
            }
        }

        /*----------------- Help functions ------------------*/

        /// <summary>
        /// Finds the maximum or minimum x or y.
        /// </summary>
        /// <param name="xory">The xory.</param>
        /// <param name="maxormin">The maxormin.</param>
        /// <returns></returns>
        private int FindMaxOrMinXOrY(char xory, string maxormin)
        {
            var lst = this.Coords;
            var num = 0;
            switch (maxormin)
            {
                case "max":
                {
                    num = int.MinValue;
                    foreach (var point in lst.SelectMany(myDict => myDict.Value))
                    {
                        switch (xory)
                        {
                            case 'x':
                                num = Math.Max(num, int.Parse(point.X));
                                break;
                            case 'y':
                                num = Math.Max(num, int.Parse(point.Y));
                                break;
                        }
                    }

                    break;
                }
                case "min":
                {
                    num = int.MaxValue;
                    foreach (var point in lst.SelectMany(myDict => myDict.Value))
                    {
                        switch (xory)
                        {
                            case 'x':
                                num = Math.Min(num, int.Parse(point.X));
                                break;
                            case 'y':
                                num = Math.Min(num, int.Parse(point.Y));
                                break;
                        }
                    }

                    break;
                }
            }
            return num;
        }
        /// <summary>
        /// Adds the text.
        /// </summary>
        private Bitmap AddText(Bitmap bitmap)
        {
            try
            {
                var lst = this.Coords;
                using (var graphics = Graphics.FromImage(bitmap))
                using (var font = new Font("Arial", 15))
                using (var brush = new SolidBrush(Color.Black))
                {
                    foreach (var item in lst)
                    {
                        if (item == null || item.Value.Count <= 0) continue;
                        var x = (SumPoints(item.Value, 'x') / item.Value.Count) - 20;
                        var y = (SumPoints(item.Value, 'y') / item.Value.Count) - 5;
                        graphics.DrawString(item.Key, font, brush, x, y);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @"\nCannot add text to scheme", @"Error Text", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return bitmap;
        }
        /// <summary>
        /// Sums the points.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        private static int SumPoints(IEnumerable<Point3D> value, char v)
        {
            var sum = 0;
            switch (v)
            {
                case 'x':
                {
                    sum += value.Sum(t => int.Parse(t.X));

                    break;
                }
                case 'y':
                {
                    sum += value.Sum(t => int.Parse(t.Y));

                    break;
                }
            }
            return sum;
        }
        /// <summary>
        /// Determines whether [is bitmap format compatible] [the specified bitmap].
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>
        ///   <c>true</c> if [is bitmap format compatible] [the specified bitmap]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsBitmapFormatCompatible(Image bitmap)
        {
            // Get the pixel format of the bitmap
            var pixelFormat = bitmap.PixelFormat;

            // Check if the pixel format is compatible with PictureBox
            return pixelFormat == PixelFormat.Format32bppArgb || pixelFormat == PixelFormat.Format24bppRgb ||
                   pixelFormat == PixelFormat.Format8bppIndexed;
        }
        /// <summary>
        /// Customs the string comparer.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private static int CustomStringComparer(string x, string y)
        {
            IEnumerable<string> xParts = Regex.Split(x, "([0-9]+)");
            IEnumerable<string> yParts = Regex.Split(y, "([0-9]+)");

            using (IEnumerator<string> xEnum = xParts.GetEnumerator(), yEnum = yParts.GetEnumerator())
            {
                while (xEnum.MoveNext() && yEnum.MoveNext())
                {
                    if (xEnum.Current == yEnum.Current) continue;
                    if (int.TryParse(xEnum.Current, out var xNum) && int.TryParse(yEnum.Current, out var yNum))
                    {
                        return xNum.CompareTo(yNum);
                    }

                    if (xEnum.Current != null) return string.Compare(xEnum.Current, yEnum.Current, StringComparison.Ordinal);
                }

                return xParts.Count().CompareTo(yParts.Count());
            }
        }
    }
}
