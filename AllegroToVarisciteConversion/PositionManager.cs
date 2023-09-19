using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllegroToVarisciteConversion
{
    internal class PositionManager
    {
        /// <summary>
        /// Gets or sets the full path to placement report file.
        /// </summary>
        /// <value>
        /// The full path to placement report file.
        /// </value>
        public string full_path_to_placement_report_file {  get; set; }
        /// <summary>
        /// Gets or sets the error counter.
        /// </summary>
        /// <value>
        /// The error counter.
        /// </value>
        public int error_counter { get; set; }
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
        List<MyDictionary> coords;

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionManager"/> class.
        /// </summary>
        /// <param name="full_path_to_placement_report_file">The full path to placement report file.</param>
        /// <param name="error_counter">The error counter.</param>
        /// <param name="vpc_number">The VPC number.</param>
        public PositionManager(string full_path_to_placement_report_file, int error_counter, int vpc_number)
        {
            this.full_path_to_placement_report_file = full_path_to_placement_report_file;
            this.error_counter = error_counter;
            this.vpc_number = vpc_number;

            coords = InitElementCoords();
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
        /// The log text information placement report
        /// </summary>
        private StringBuilder logTextInfoPlacementReport = new StringBuilder();
        /// <summary>
        /// The log text debug placement report
        /// </summary>
        private StringBuilder logTextDebugPlacementReport = new StringBuilder();
        /// <summary>
        /// The log text error placement report
        /// </summary>
        private StringBuilder logTextErrorPlacementReport = new StringBuilder();

        /// <summary>
        /// Cleans the log placement report.
        /// </summary>
        private void CleanLogPlacementReport()
        {
            this.logTextDebugPlacementReport.Clear();
            this.logTextErrorPlacementReport.Clear();
            this.logTextInfoPlacementReport.Clear();
        }


        /// <summary>
        /// Initializes the element coords.
        /// </summary>
        /// <returns></returns>
        private List<MyDictionary> InitElementCoords()
        {
            CleanLogPlacementReport();

            this.logTextDebugPlacementReport.AppendLine("Opening placement file " + this.full_path_to_placement_report_file);
            this.logTextInfoPlacementReport.AppendLine("Opening placement file " + this.full_path_to_placement_report_file);

            this.logTextDebugPlacementReport.AppendLine("\nStart converting placement report file");
            this.logTextInfoPlacementReport.AppendLine("\nStart converting placement report file");

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
                            this.logTextDebugPlacementReport.AppendLine($"Refdes {temp.Key} is added with {temp.Value.Count} coordinations");
                            this.logTextInfoPlacementReport.AppendLine($"Refdes {temp.Key} is added with {temp.Value.Count} coordinations");
                        }
                        coords.Add(temp);
                        temp = new MyDictionary(line[12]);
                        this.logTextDebugPlacementReport.AppendLine($"Reading line {index}: {ConvertToLinePlacementReport(line, 1)}");
                    }
                    else if (VPCLine(line))
                    {
                        if (coords.Count != 0)
                        {
                            this.logTextDebugPlacementReport.AppendLine($"Refdes {temp.Key} is added with {temp.Value.Count} coordinations");
                            this.logTextInfoPlacementReport.AppendLine($"Refdes {temp.Key} is added with {temp.Value.Count} coordinations");
                        }
                        coords.Add(temp);
                        temp = new MyDictionary("VPC" + this.vpc_number);
                        this.vpc_number++;
                        this.logTextDebugPlacementReport.AppendLine($"Reading line {index}: {ConvertToLinePlacementReport(line, 4)}");
                    }
                    else if (HasValue(line, "subclass"))
                    {
                        this.logTextDebugPlacementReport.AppendLine($"Reading line {index}: {ConvertToLinePlacementReport(line, 3)}");
                        string[] subclass = line[13].Split('_');
                        if (subclass.Length > 1)
                        {
                            if (subclass[2].Equals("TOP"))
                            {
                                temp.AddMirror("NO");
                            }
                            else if (subclass[2].Equals("BOTTOM"))
                            {
                                temp.AddMirror("YES");
                            }
                        }

                    }
                    else if (HasValue(line, "~end-of-file~"))
                    {
                        coords.Add(temp);
                    }
                    else if (HasValue(line, "segment:xy"))
                    {
                        this.logTextDebugPlacementReport.AppendLine($"Reading line {index}: {ConvertToLinePlacementReport(line, 2)}");
                        string t1 = null, t2 = null;
                        if (CanConvertToNumeric(line[3]))
                        {
                            t1 = string.Concat(line[3].Where(Char.IsDigit));
                            t1 = t1.Substring(0, t1.Length - 2);
                        }
                        else
                        {
                            string x = line[3];
                            x = x.Substring(1);
                            this.logTextErrorPlacementReport.AppendLine($"Error!!! Cannot convert {x} to number in refdes {temp.Key}");
                            this.logTextDebugPlacementReport.AppendLine($"Error!!! Cannot convert {x} to number in refdes {temp.Key}");
                            this.logTextInfoPlacementReport.AppendLine($"Error!!! Cannot convert {x} to number in refdes {temp.Key}");
                            this.error_counter++;
                        }
                        if (CanConvertToNumeric(line[4]))
                        {
                            t2 = string.Concat(line[4].Where(Char.IsDigit));
                            t2 = t2.Substring(0, t2.Length - 2);
                        }
                        else
                        {
                            string x = line[4];
                            x = x.Substring(0, x.Length - 1);
                            this.logTextErrorPlacementReport.AppendLine($"Error!!! Cannot convert {x} to number in refdes {temp.Key}");
                            this.logTextDebugPlacementReport.AppendLine($"Error!!! Cannot convert {x} to number in refdes {temp.Key}");
                            this.logTextInfoPlacementReport.AppendLine($"Error!!! Cannot convert {x} to number in refdes {temp.Key}");
                            this.error_counter++;
                        }

                        if (t1 != null && t2 != null)
                        {
                            temp.AddValue(int.Parse(t1), int.Parse(t2));
                            this.logTextDebugPlacementReport.AppendLine($"Coordinates {t1},{t2} added to refdes {temp.Key}");
                        }
                    }
                    else if (HasValue(line, "seg:xy"))
                    {
                        this.logTextDebugPlacementReport.AppendLine($"Reading line {index}: {ConvertToLinePlacementReport(line, 2)}");
                        string x1 = null, y1 = null, x2 = null, y2 = null, centerX = null, centerY = null, isClockwise = null;

                        if (CanConvertToNumeric(line[4]))
                        {
                            x1 = string.Concat(line[4].Where(char.IsDigit));
                            x1 = x1.Substring(0, x1.Length - 2);
                        }
                        else
                        {
                            string t1 = line[4];
                            t1 = t1.Substring(1);
                            this.logTextErrorPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextDebugPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextInfoPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.error_counter++;
                        }

                        if (CanConvertToNumeric(line[5]))
                        {
                            y1 = string.Concat(line[5].Where(char.IsDigit));
                            y1 = y1.Substring(0, y1.Length - 2);
                        }
                        else
                        {
                            string t1 = line[5];
                            t1 = t1.Substring(0, t1.Length - 1);
                            this.logTextErrorPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextDebugPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextInfoPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.error_counter++;
                        }

                        if (CanConvertToNumeric(line[7]))
                        {
                            x2 = string.Concat(line[7].Where(char.IsDigit));
                            x2 = x2.Substring(0, x2.Length - 2);
                        }
                        else
                        {
                            string t1 = line[7];
                            t1 = t1.Substring(1);
                            this.logTextErrorPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextDebugPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextInfoPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.error_counter++;
                        }

                        if (CanConvertToNumeric(line[8]))
                        {
                            y2 = string.Concat(line[8].Where(char.IsDigit));
                            y2 = y2.Substring(0, y2.Length - 2);
                        }
                        else
                        {
                            string t1 = line[8];
                            t1 = t1.Substring(0, t1.Length - 1);
                            this.logTextErrorPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextDebugPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextInfoPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.error_counter++;
                        }

                        line = reader.ReadLine().Split(' ');
                        index++;
                        this.logTextDebugPlacementReport.AppendLine($"Reading line {index}: {ConvertToLinePlacementReport(line, 2)}");
                        if (CanConvertToNumeric(line[6]))
                        {
                            centerX = string.Concat(line[6].Where(char.IsDigit));
                            centerX = centerX.Substring(0, centerX.Length - 2);
                        }
                        else
                        {
                            string t1 = line[6];
                            t1 = t1.Substring(1);
                            this.logTextErrorPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextDebugPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextInfoPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.error_counter++;
                        }

                        if (CanConvertToNumeric(line[7]))
                        {
                            centerY = string.Concat(line[7].Where(char.IsDigit));
                            centerY = centerY.Substring(0, centerY.Length - 2);
                        }
                        else
                        {
                            string t1 = line[7];
                            t1 = t1.Substring(0, t1.Length - 1);
                            this.logTextErrorPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextDebugPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextInfoPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.error_counter++;
                        }


                        if (line[12].Equals("CCW"))
                        {
                            isClockwise = "0";
                        }
                        else if (line[12].Equals("CW"))
                        {
                            isClockwise = "1";
                        }
                        else
                        {
                            string t1 = line[12];
                            this.logTextErrorPlacementReport.AppendLine($"Error!!! The arc data does not have \"CCW\" or \"CW\" parametar in refdef {temp.Key}");
                            this.logTextDebugPlacementReport.AppendLine($"Error!!! The arc data does not have \"CCW\" or \"CW\" parametar in refdes {temp.Key}");
                            this.logTextInfoPlacementReport.AppendLine($"Error!!! The arc data does not have \"CCW\" or \"CW\" parametar in refdes {temp.Key}");
                            this.error_counter++;
                        }

                        if (x1 != null && y1 != null && x2 != null && y2 != null && centerX != null && centerY != null && isClockwise != null)
                        {
                            temp.AddValue(int.Parse(x1), int.Parse(y1));
                            this.logTextDebugPlacementReport.AppendLine($"Start point fo arc: {x1},{y1} added to refdes {temp.Key}");
                            temp.AddValue(int.Parse(centerX), int.Parse(centerY), int.Parse(isClockwise));
                            this.logTextDebugPlacementReport.AppendLine($"Center point fo arc: {centerX},{centerY} added to refdes {temp.Key}");
                            temp.AddValue(int.Parse(x2), int.Parse(y2));
                            this.logTextDebugPlacementReport.AppendLine($"End point fo arc: {x2},{y2} added to refdes {temp.Key}");

                        }
                    }
                    else
                    {
                        this.logTextDebugPlacementReport.AppendLine($"Reading line {index}: {ConvertToLinePlacementReport(line, 0)}");
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
        /// Converts to line placement report.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        private string ConvertToLinePlacementReport(string[] line, int mode)
        {
            string str = "{";
            if (mode == 0)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    str += line[i] + "  ";
                }
                str += "} Dumping line";
            }
            if (mode == 1)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    str += line[i] + "  ";
                }
                str += "} Name line";
            }
            if (mode == 2)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    str += line[i] + "  ";
                }
                str += "} coordinates line";
            }
            if (mode == 3)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] != " ")
                    {
                        str += line[i] + " ";
                    }
                }
                str += "} Mirror line";
            }
            if (mode == 4)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    str += line[i] + "  ";
                }
                str += "} VPC line";
            }
            return str;
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
