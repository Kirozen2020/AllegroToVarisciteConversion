﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllegroToVarisciteConversion
{
    internal class ImageManager
    {
        /// <summary>
        /// Gets or sets the bitmap.
        /// </summary>
        /// <value>
        /// The bitmap.
        /// </value>
        public Bitmap motherBoardImage { get; set; }
        public List<MyDictionary> coords { get; set; }
        public List<string> names { get; set; }
        public Image image { get; set; }

        public LogManager log { get; set; }

        public ImageManager(List<MyDictionary> coords, LogManager log)
        {
            this.coords = coords;
            this.log = log;
        }




        /// <summary>
        /// Draws the points.
        /// </summary>
        /// <param name="pb">The pb.</param>
        /// <param name="pointLists">The point lists.</param>
        private void DrawPoints(PictureBox pb, List<List<Point3D>> pointLists)
        {
            //this.logTextDebugPlacementReport.AppendLine("Start drawing scheme\n");
            //this.logTextInfoPlacementReport.AppendLine("Start drawing scheme\n");
            log.AddComment("Start drawing scheme\n", new List<int> { 0, 1 }, "placement"); 

            if (pb == null || pointLists == null)
                return;

            using (Bitmap bmp = new Bitmap(FindMaxOrMinXOrY('x', "max") + 30, FindMaxOrMinXOrY('y', "max") + 30))
            {

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // Clear the PictureBox
                    g.Clear(Color.White);

                    // Draw lines for each list of points
                    using (Pen pen = new Pen(Color.Black, 2))
                    {
                        foreach (List<Point3D> points in pointLists)
                        {
                            // Draw lines connecting the points
                            if (points.Count > 1)
                            {
                                for (int i = 0; i < points.Count - 1; i++)
                                {
                                    if (i + 1 < points.Count - 1)
                                    {

                                        if (!points[i + 1].IsRegularPoint())
                                        {
                                            Point start = points[i].GetRegularPoint();
                                            Point center = points[i + 1].GetRegularPoint();
                                            bool isClockwise = false;
                                            if (!points[i + 1].Z.Equals("0"))
                                            {
                                                isClockwise = true;
                                            }
                                            Point end = points[i + 2].GetRegularPoint();

                                            int radius = (int)Math.Sqrt(Math.Pow(center.X - start.X, 2) + Math.Pow(center.Y - start.Y, 2));
                                            int x = center.X - radius;
                                            int y = center.Y - radius;
                                            int wigth = 2 * radius;
                                            int height = 2 * radius;

                                            float startAngle = (float)Math.Atan2(start.Y - center.Y, start.X - center.X) * 180 / (float)Math.PI;
                                            float endAngle = (float)Math.Atan2(end.Y - center.Y, end.X - center.X) * 180 / (float)Math.PI;
                                            float sweepAngle = endAngle - startAngle;

                                            if (Math.Abs(sweepAngle) == 0)
                                            {
                                                g.DrawEllipse(pen, x, y, wigth, height);
                                            }
                                            else
                                            {
                                                if (isClockwise && sweepAngle < 0)
                                                {
                                                    sweepAngle += 360;
                                                }
                                                else if (!isClockwise && sweepAngle > 0)
                                                {
                                                    sweepAngle -= 360;
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

                                // Connect the last point with the first point to complete the figure
                                g.DrawLine(pen, points[points.Count - 1].GetRegularPoint(), points[0].GetRegularPoint());
                            }
                        }
                    }
                }

                // Assign the updated bitmap to the PictureBox
                if (!IsBitmapFormatCompatible(bmp))
                {
                    //this.logTextErrorPlacementReport.AppendLine("Error!!! Bitmap format is wrong, cannot convert bitmap to image");
                    //this.logTextDebugPlacementReport.AppendLine("Error!!! Bitmap format is wrong, cannot convert bitmap to image");
                    //this.logTextInfoPlacementReport.AppendLine("Error!!! Bitmap format is wrong, cannot convert bitmap to image");
                    //this.errorCount++;
                    log.AddComment("Error!!! Bitmap format is wrong, cannot convert bitmap to image", new List<int> { 0, 1, 2 }, "placement");
                    MessageBox.Show("BitMap format error", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.motherBoardImage = bmp;
                this.motherBoardImage = AddText(this.motherBoardImage);
                this.image = Image.FromHbitmap(this.motherBoardImage.GetHbitmap());
                //pbSketch.Image = image;

                this.names = new List<string>();
                foreach (var item in this.coords)
                {
                    this.names.Add(item.Key);
                }

                this.names.Sort(CustomStringComparer);

                //listBox1.DataSource = names;
                //listBox1.SelectionMode = SelectionMode.MultiExtended;
            }
        }

        /// <summary>
        /// Finds the maximum or minimum x or y.
        /// </summary>
        /// <param name="xory">The xory.</param>
        /// <param name="maxormin">The maxormin.</param>
        /// <returns></returns>
        private int FindMaxOrMinXOrY(char xory, string maxormin)
        {
            List<MyDictionary> lst = this.coords;
            int num = 0;
            if (maxormin == "max")
            {
                num = int.MinValue;
                for (int i = 0; i < lst.Count; i++)
                {
                    for (int j = 0; j < lst[i].Value.Count; j++)
                    {
                        if (xory == 'x')
                        {
                            num = Math.Max(num, int.Parse(lst[i].Value[j].X));
                        }
                        else if (xory == 'y')
                        {
                            num = Math.Max(num, int.Parse(lst[i].Value[j].Y));
                        }
                    }
                }
            }
            else if (maxormin == "min")
            {
                num = int.MaxValue;
                for (int i = 0; i < lst.Count; i++)
                {
                    for (int j = 0; j < lst[i].Value.Count; j++)
                    {
                        if (xory == 'x')
                        {
                            num = Math.Min(num, int.Parse(lst[i].Value[j].X));
                        }
                        else if (xory == 'y')
                        {
                            num = Math.Min(num, int.Parse(lst[i].Value[j].Y));
                        }
                    }
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
                List<MyDictionary> lst = this.coords;
                for (int i = 0; i < lst.Count; i++)
                {
                    MyDictionary item = lst[i];

                    if (item != null)
                    {
                        int x = SumPoints(item.Value, 'x') / item.Value.Count - 20;
                        int y = SumPoints(item.Value, 'y') / item.Value.Count - 5;

                        Graphics graphics = Graphics.FromImage(bitmap);
                        Font font = new Font("Arial", 15);
                        Brush brush = new SolidBrush(Color.Black);

                        graphics.DrawString(item.Key, font, brush, x, y);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\nCannot add text to scheme", "Error Text", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //this.errorCount++;
                log.error_counter++;
            }
            return bitmap;
        }
        /// <summary>
        /// Sums the points.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        private int SumPoints(List<Point3D> value, char v)
        {
            int sum = 0;
            if (v == 'x')
            {
                for (int i = 0; i < value.Count; i++)
                {
                    sum += int.Parse(value[i].X);
                }
            }
            else if (v == 'y')
            {
                for (int i = 0; i < value.Count; i++)
                {
                    sum += int.Parse(value[i].Y);
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
        private bool IsBitmapFormatCompatible(Bitmap bitmap)
        {
            // Get the pixel format of the bitmap
            PixelFormat pixelFormat = bitmap.PixelFormat;

            // Check if the pixel format is compatible with PictureBox
            switch (pixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format8bppIndexed:
                    return true;
                default:
                    return false;
            }
        }
        /// <summary>
        /// Customs the string comparer.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        static int CustomStringComparer(string x, string y)
        {
            IEnumerable<string> xParts = Regex.Split(x, "([0-9]+)");
            IEnumerable<string> yParts = Regex.Split(y, "([0-9]+)");

            using (IEnumerator<string> xEnum = xParts.GetEnumerator(), yEnum = yParts.GetEnumerator())
            {
                while (xEnum.MoveNext() && yEnum.MoveNext())
                {
                    if (xEnum.Current != yEnum.Current)
                    {
                        if (int.TryParse(xEnum.Current, out int xNum) && int.TryParse(yEnum.Current, out int yNum))
                        {
                            return xNum.CompareTo(yNum);
                        }
                        return xEnum.Current.CompareTo(yEnum.Current);
                    }
                }

                return xParts.Count().CompareTo(yParts.Count());
            }
        }
    }
}