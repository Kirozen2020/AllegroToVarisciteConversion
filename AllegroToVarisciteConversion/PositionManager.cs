using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllegroToVarisciteConversion
{
    internal class PositionManager
    {
        /*----------------- Variables ------------------*/

        /// <summary>
        /// Gets or sets the full path to placement report file.
        /// </summary>
        /// <value>
        /// The full path to placement report file.
        /// </value>
        public string full_path_to_placement_report_file {  get; set; }
        /// <summary>
        /// Gets or sets the VPC number.
        /// </summary>
        /// <value>
        /// The VPC number.
        /// </value>
        public int vpc_number { get; set; }
        /// <summary>
        /// The coords
        /// </summary>
        private List<MyDictionary> coords {  get; set; }
        /// <summary>
        /// Gets or sets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        private List<string> names { get; set; }

        /*----------------- Class constructor ------------------*/

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionManager"/> class.
        /// </summary>
        /// <param name="full_path_to_placement_report_file">The full path to placement report file.</param>
        /// <param name="error_counter">The error counter.</param>
        /// <param name="vpc_number">The VPC number.</param>
        public PositionManager(string full_path_to_placement_report_file)
        {
            this.full_path_to_placement_report_file = full_path_to_placement_report_file;
            this.vpc_number = 0;
            this.coords = InitElementCoords();
            this.names = InitNames();
        }

        /*----------------- Main functions ------------------*/

        /// <summary>
        /// Initializes the element coords.
        /// </summary>
        /// <returns></returns>
        private List<MyDictionary> InitElementCoords()
        {
            LogManager.AddCommentLine(LogManager.LogLevel.Informational,
                "Opening placement file " + this.full_path_to_placement_report_file);

            LogManager.AddCommentLine(LogManager.LogLevel.Informational,
                "Start converting placement report file");

            int index = 0;

            List<MyDictionary> coords = new List<MyDictionary>();

            MyDictionary temp = new MyDictionary("First element -> delete");

            using (var reader = new StreamReader(this.full_path_to_placement_report_file))
            {
                while (reader.EndOfStream == false)
                {
                    var line = reader.ReadLine().Split(' ');
                    if (HasValue(line, "RefDes:"))
                    {
                        if (coords.Count != 0)
                        {
                            LogManager.AddCommentLine(LogManager.LogLevel.Informational,
                                $"Refdes {temp.Key} is added with {temp.Value.Count} coordinates");
                        }
                        coords.Add(temp);
                        temp = new MyDictionary(line[12]);
                        LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                            "Reading line " + index.ToString() + " \"" +
                            string.Join(" ", line) +
                            "\" - Name line");
                    }
                    else if (VPCLine(line))
                    {
                        if (coords.Count != 0)
                        {
                            LogManager.AddCommentLine(LogManager.LogLevel.Informational,
                                $"Refdes {temp.Key} is added with {temp.Value.Count} coordinates");
                        }
                        coords.Add(temp);
                        temp = new MyDictionary("VPC" + this.vpc_number);
                        this.vpc_number++;
                        LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                            "Reading line " + index.ToString() + " \"" +
                            string.Join(" ", line) +
                            "\" - VPC line");
                    }
                    else if (HasValue(line, "subclass"))
                    {
                        LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                            "Reading line " + index.ToString() + " \"" +
                            string.Join(" ", line) +
                            "\" - Mirror line");

                        string[] subclass = line[13].Split('_');
                        if (subclass.Length > 2 && (subclass[2].Equals("TOP") || subclass[2].Equals("BOTTOM")))
                        {
                            temp.AddMirror(subclass[2].Equals("TOP") ? "NO" : "YES");
                        }
                    }
                    else if (HasValue(line, "~end-of-file~"))
                    {
                        coords.Add(temp);
                    }
                    else if (HasValue(line, "segment:xy"))
                    {
                        LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                            "Reading line " + index.ToString() + " \"" +
                            string.Join(" ", line) +
                            "\" - Coordinates line");

                        string xCoordinate = ExtractNumericValue(line,3, temp);
                        string yCoordinate = ExtractNumericValue(line, 4, temp);

                        if (xCoordinate != null && yCoordinate != null)
                        {
                            temp.AddValue(int.Parse(xCoordinate), int.Parse(yCoordinate));
                            LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                                $"Coordinates {xCoordinate},{yCoordinate} added to refdes {temp.Key}");
                        }
                        
                    }
                    else if (HasValue(line, "seg:xy"))
                    {
                        LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                            "Reading line " + index.ToString() + " \"" +
                            string.Join(" ", line) +
                            "\" - Coordinates line");

                        string x1 = ConvertLineToCoordinate(line, 4, temp, 2);
                        string y1 = ConvertLineToCoordinate(line, 5, temp, 2);
                        string x2 = ConvertLineToCoordinate(line, 7, temp, 2);
                        string y2 = ConvertLineToCoordinate(line, 8, temp, 2);

                        line = reader.ReadLine().Split(' ');
                        index++;

                        LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                            "Reading line " + index.ToString() + " \"" +
                            string.Join(" ", line) +
                            "\" - Coordinates line");

                        string centerX = ConvertLineToCoordinate(line, 6, temp, 2);
                        string centerY = ConvertLineToCoordinate(line, 7, temp, 2);

                        int radius = int.Parse(line[9].Split('.')[0].Trim('(', ')'));

                        string isClockwise = ConvertDirectionToClockwise(line[12], temp.Key);
                        
                        if(radius == 0)
                        {
                            temp.AddValue(int.Parse(x1), int.Parse(y1));
                            LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                                $"Start point of an arc: {x1},{y1} added to refdes {temp.Key}");
                            temp.AddValue(int.Parse(x2), int.Parse(y2));
                            LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                                $"End point of an arc: {x2},{y2} added to refdes {temp.Key}");
                        }
                        else if (x1 != null && y1 != null && x2 != null && y2 != null && centerX != null && centerY != null && isClockwise != null)
                        {
                            temp.AddValue(int.Parse(x1), int.Parse(y1));
                            LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                                $"Start point of an arc: {x1},{y1} added to refdes {temp.Key}");
                            temp.AddValue(int.Parse(centerX), int.Parse(centerY), int.Parse(isClockwise));
                            LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                                $"Center point of an arc: {centerX},{centerY} added to refdes {temp.Key}");
                            temp.AddValue(int.Parse(x2), int.Parse(y2));
                            LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                                $"End point of an arc: {x2},{y2} added to refdes {temp.Key}");
                        }
                    }
                    else
                    {
                        LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                            "Reading line " + index.ToString() + " \"" +
                            string.Join(" ", line) +
                            "\" - Dumping line");
                    }

                    index++;
                }
            }
            if (coords.Count > 0)
            {
                coords.RemoveAt(0);
            }
            else
            {
                MessageBox.Show("You opened a wrong file or an empty one", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (this.vpc_number > 2)
            {
                MessageBox.Show("You have in total more then 1 VPC element", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return coords;
        }

        /*----------------- Help functions ------------------*/

        /// <summary>
        /// Extracts the numeric value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="index">The index.</param>
        /// <param name="temp">The temporary.</param>
        /// <returns></returns>
        private string ExtractNumericValue(string[] line, int index, MyDictionary temp)
        {
            if (CanConvertToNumeric(line[index]))
            {
                string value = string.Concat(line[index].Where(char.IsDigit));
                return value.Substring(0, value.Length - 2);
            }
            else
            {
                string errorMessage = $"Error!!! Cannot convert {line[index].Substring(1)} to number in refdes {temp.Key}";
                LogManager.AddCommentLine(LogManager.LogLevel.Error, errorMessage);
                return null;
            }
        }
        /// <summary>
        /// Converts the direction to clockwise.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="refDes">The reference DES.</param>
        /// <returns></returns>
        private static string ConvertDirectionToClockwise(string direction, string refDes)
        {
            switch (direction)
            {
                case "CCW":
                    return "0";
                case "CW":
                    return "1";
                default:
                    LogManager.AddCommentLine(LogManager.LogLevel.Error,
                        $"Error!!! Cannot convert {direction} to number in refdes {refDes}");
                    return null;
            }
        }
        /// <summary>
        /// Converts the line to coordinate.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="index">The index.</param>
        /// <param name="temp">The temporary.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        private string ConvertLineToCoordinate(string[] line, int index, MyDictionary temp, int offset)
        {
            if (CanConvertToNumeric(line[index]))
            {
                string coordinate = string.Concat(line[index].Where(char.IsDigit));
                return coordinate.Substring(0, coordinate.Length - offset);
            }
            else
            {
                string t = line[index];
                t = t.Substring(offset == 1 ? 1 : 0);
                LogManager.AddCommentLine(LogManager.LogLevel.Error,
                    $"Error!!! Cannot convert {t} to number in refdes {temp.Key}");
                return null;
            }
        }
        /// <summary>
        /// Initializes the names.
        /// </summary>
        /// <returns></returns>
        private List<string> InitNames()
        {
            List<string> names = new List<string>();

            for (int i = 0; i < this.coords.Count; i++)
            {
                names.Add(this.coords[i].Key);
            }

            names.Sort(CustomStringComparer);

            return names;
        }
        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <returns></returns>
        public List<string> GetNames()
        {
            return this.names;
        }
        /// <summary>
        /// Gets the coords.
        /// </summary>
        /// <returns></returns>
        public List<MyDictionary> GetCoords()
        {
            return this.coords;
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
        /// <summary>
        /// Moves all elements.
        /// </summary>
        public void MoveAllElements()
        {
            List<MyDictionary> lst = this.coords;
            int deleyY = FindMaxOrMinXOrY('y', "min") - 10;
            int deleyX = FindMaxOrMinXOrY('x', "min") - 10;
            for (int i = 0; i < lst.Count; i++)
            {
                List<Point3D> coordinations = lst[i].Value;
                for (int j = 0; j < coordinations.Count; j++)
                {
                    Point3D point = coordinations[j];
                    point.Y = (int.Parse(point.Y) - deleyY + 20).ToString();
                    point.X = (int.Parse(point.X) - deleyX + 50).ToString();
                    coordinations[j] = point;
                }
            }
            this.coords = lst;
        }
        /// <summary>
        /// Flips the image.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public void FlipImage()
        {
            List<MyDictionary> list = this.coords;
            List<MyDictionary> result = new List<MyDictionary>();
            int maxY = FindMaxOrMinXOrY('y', "max") + 30;

            foreach (var dict in list)
            {
                MyDictionary newDict = new MyDictionary(dict.Key);
                newDict.AddMirror(dict.Mirror);

                foreach (var point in dict.Value)
                {
                    int newY = maxY - int.Parse(point.Y);
                    if (point.Z != "Empty")
                    {
                        newDict.Value.Add(new Point3D(int.Parse(point.X), newY, int.Parse(point.Z)));
                    }
                    else
                    {
                        newDict.Value.Add(new Point3D(int.Parse(point.X), newY));
                    }
                }
                result.Add(newDict);
            }
            this.coords = result;
        }
        /// <summary>
        /// Deletes the unnecessary coords.
        /// </summary>
        /// <returns></returns>
        public void DeleteUnnecessaryCoords()
        {
            List<MyDictionary> result = this.coords;
            List<MyDictionary> temp = this.coords;
            int count = 0;

            for (int i = 0; i < result.Count; i++)
            {
                for (int j = 0; j < result[i].Value.Count - 1; j++)
                {
                    if (result[i].Value[j].GetRegularPoint().Equals(result[i].Value[j + 1].GetRegularPoint()))
                    {
                        temp[i].Value.Remove(result[i].Value[j]);
                        count++;
                    }
                }
            }

            this.coords = temp;
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
        /// VPCs the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        private bool VPCLine(string[] line)
        {
            for (int i = 0; i < line.Length - 1; i++)
            {
                if (line[i].Equals("BOARD") && line[i + 1].Equals("GEOMETRY"))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Determines whether the specified line has value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified line has value; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasValue(string[] line, string value)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Determines whether this instance [can convert to numeric] the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can convert to numeric] the specified input; otherwise, <c>false</c>.
        /// </returns>
        private bool CanConvertToNumeric(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                {
                    if (c != '.' && c != '(' && c != ')')
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
