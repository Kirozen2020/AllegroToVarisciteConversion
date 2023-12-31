﻿using System;
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
        public string FullPathToPlacementReportFile {  get; set; }
        /// <summary>
        /// Gets or sets the VPC number.
        /// </summary>
        /// <value>
        /// The VPC number.
        /// </value>
        public int VpcNumber { get; set; }
        /// <summary>
        /// The coords
        /// </summary>
        private List<MyDictionary> Coords {  get; set; }
        /// <summary>
        /// Gets or sets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        private List<string> Names { get; set; }

        /*----------------- Class constructor ------------------*/

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionManager"/> class.
        /// </summary>
        /// <param name="fullPathToPlacementReportFile">The full path to placement report file.</param>
        public PositionManager(string fullPathToPlacementReportFile)
        {
            this.FullPathToPlacementReportFile = fullPathToPlacementReportFile;
            this.VpcNumber = 0;
            this.Coords = InitElementCoords();
            this.Names = InitNames();
        }

        /*----------------- Main functions ------------------*/

        /// <summary>
        /// Initializes the element coords.
        /// </summary>
        /// <returns></returns>
        private List<MyDictionary> InitElementCoords()
        {
            LogManager.AddCommentLine(LogManager.LogLevel.Informational,
                "Opening placement file " + this.FullPathToPlacementReportFile);

            LogManager.AddCommentLine(LogManager.LogLevel.Informational,
                "Start converting placement report file");

            var index = 0;

            var coords = new List<MyDictionary>();

            var temp = new MyDictionary("First element -> delete");

            using (var reader = new StreamReader(this.FullPathToPlacementReportFile))
            {
                while (reader.EndOfStream == false)
                {
                    var line = reader.ReadLine()?.Split(' ');
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
                    else if (VpcLine(line))
                    {
                        if (coords.Count != 0)
                        {
                            LogManager.AddCommentLine(LogManager.LogLevel.Informational,
                                $"Refdes {temp.Key} is added with {temp.Value.Count} coordinates");
                        }
                        coords.Add(temp);
                        temp = new MyDictionary("VPC" + this.VpcNumber);
                        this.VpcNumber++;
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
                        if (line != null)
                        {
                            LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                                "Reading line " + index.ToString() + " \"" +
                                string.Join(" ", line) +
                                "\" - Coordinates line");

                            var x1 = ConvertLineToCoordinate(line, 4, temp, 2);
                            var y1 = ConvertLineToCoordinate(line, 5, temp, 2);
                            var x2 = ConvertLineToCoordinate(line, 7, temp, 2);
                            var y2 = ConvertLineToCoordinate(line, 8, temp, 2);

                            line = reader.ReadLine()?.Split(' ');
                            index++;

                            if (line != null)
                            {
                                LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                                    "Reading line " + index.ToString() + " \"" +
                                    string.Join(" ", line) +
                                    "\" - Coordinates line");

                                var centerX = ConvertLineToCoordinate(line, 6, temp, 2);
                                var centerY = ConvertLineToCoordinate(line, 7, temp, 2);

                                var radius = int.Parse(line[9].Split('.')[0].Trim('(', ')'));

                                var isClockwise = ConvertDirectionToClockwise(line[12], temp.Key);

                                if (radius == 0 && IfAllNotNull(new List<string>()
                                        { x1, y1, x2, y2, centerX, centerY, isClockwise }))
                                {
                                    temp.AddValue(int.Parse(x1), int.Parse(y1));
                                    LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                                        $"Start point of an arc: {x1},{y1} added to refdes {temp.Key}");
                                    temp.AddValue(int.Parse(x2), int.Parse(y2));
                                    LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                                        $"End point of an arc: {x2},{y2} added to refdes {temp.Key}");
                                }
                                else if (IfAllNotNull(new List<string>()
                                             { x1, y1, x2, y2, centerX, centerY, isClockwise }))
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
                        }
                    }
                    else
                    {
                        if (line != null)
                            LogManager.AddCommentLine(LogManager.LogLevel.Debug,
                                "Reading line " + index + " \"" +
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
                MessageBox.Show(@"You opened a wrong file or an empty one", @"Attention!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (this.VpcNumber > 2)
            {
                MessageBox.Show(@"You have in total more then 1 VPC element", @"Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return coords;
        }

        /*----------------- Help functions ------------------*/

        /// <summary>Ifs all not null.</summary>
        /// <param name="values">The values.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        private static bool IfAllNotNull(IEnumerable<string> values)
        {
            return values.All(value => value != null);
        }
        /// <summary>
        /// Extracts the numeric value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="index">The index.</param>
        /// <param name="temp">The temporary.</param>
        /// <returns></returns>
        private static string ExtractNumericValue(IReadOnlyList<string> line, int index, MyDictionary temp)
        {
            if (CanConvertToNumeric(line[index]))
            {
                var value = string.Concat(line[index].Where(char.IsDigit));
                return value.Substring(0, value.Length - 2);
            }
            else
            {
                var errorMessage = $"Error!!! Cannot convert {line[index].Substring(1)} to number in refdes {temp.Key}";
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
        private static string ConvertLineToCoordinate(IReadOnlyList<string> line, int index, MyDictionary temp, int offset)
        {
            if (CanConvertToNumeric(line[index]))
            {
                var coordinate = string.Concat(line[index].Where(char.IsDigit));
                return coordinate.Substring(0, coordinate.Length - offset);
            }
            else
            {
                var t = line[index];
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
            var names = this.Coords.Select(t => t.Key).ToList();

            names.Sort(CustomStringComparer);

            return names;
        }
        /// <summary>
        /// Gets the names.
        /// </summary>
        /// <returns></returns>
        public List<string> GetNames()
        {
            return Names;
        }
        /// <summary>
        /// Gets the coords.
        /// </summary>
        /// <returns></returns>
        public List<MyDictionary> GetCoords()
        {
            return Coords;
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
        /// <summary>
        /// Moves all elements.
        /// </summary>
        public void MoveAllElements()
        {
            var lst = this.Coords;
            var delayY = FindMaxOrMinXOrY('y', "min") - 10;
            var delayX = FindMaxOrMinXOrY('x', "min") - 10;
            foreach (var coordination in lst.Select(dict => dict.Value))
            {
                for (var j = 0; j < coordination.Count; j++)
                {
                    var point = coordination[j];
                    point.Y = (int.Parse(point.Y) - delayY + 20).ToString();
                    point.X = (int.Parse(point.X) - delayX + 50).ToString();
                    coordination[j] = point;
                }
            }
            this.Coords = lst;
        }
        /// <summary>
        /// Flips the image.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public void FlipImage()
        {
            var list = this.Coords;
            var result = new List<MyDictionary>();
            var maxY = FindMaxOrMinXOrY('y', "max") + 30;

            foreach (var dict in list)
            {
                var newDict = new MyDictionary(dict.Key);
                newDict.AddMirror(dict.Mirror);

                foreach (var point in dict.Value)
                {
                    var newY = maxY - int.Parse(point.Y);
                    newDict.Value.Add(point.Z != "Empty"
                        ? new Point3D(int.Parse(point.X), newY, int.Parse(point.Z))
                        : new Point3D(int.Parse(point.X), newY));
                }
                result.Add(newDict);
            }
            this.Coords = result;
        }
        /// <summary>
        /// Deletes the unnecessary coords.
        /// </summary>
        /// <returns></returns>
        public void DeleteUnnecessaryCoords()
        {
            var result = this.Coords;
            var temp = this.Coords;

            for (var i = 0; i < result.Count; i++)
            {
                for (var j = 0; j < result[i].Value.Count - 1; j++)
                {
                    if (!result[i].Value[j].GetRegularPoint().Equals(result[i].Value[j + 1].GetRegularPoint()))
                        continue;
                    temp[i].Value.Remove(result[i].Value[j]);
                }
            }

            this.Coords = temp;
        }
        /// <summary>
        /// Finds the maximum or minimum x or y.
        /// </summary>
        /// <param name="XorY">The XorY.</param>
        /// <param name="MaxOrMin">The MaxOrMin.</param>
        /// <returns></returns>
        private int FindMaxOrMinXOrY(char XorY, string MaxOrMin)
        {
            var lst = this.Coords;
            var num = 0;
            switch (MaxOrMin)
            {
                case "max":
                {
                    num = int.MinValue;
                    foreach (var point in lst.SelectMany(myDict => myDict.Value))
                    {
                        switch (XorY)
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
                        switch (XorY)
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
        /// VPCs the line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        private static bool VpcLine(IReadOnlyList<string> line)
        {
            for (var i = 0; i < line.Count - 1; i++)
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
            return line.Any(t => t.Equals(value));
        }
        /// <summary>
        /// Determines whether this instance [can convert to numeric] the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can convert to numeric] the specified input; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanConvertToNumeric(string input)
        {
            return input.Where(c => !char.IsDigit(c)).All(c => c == '.' || c == '(' || c == ')');
        }
    }
}
