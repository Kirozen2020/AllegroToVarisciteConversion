using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AllegroToVarisciteConversion
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            this.Text = "AllegroToVarisciteConversion " + Revision.revision;

            this.logTextGlobal.AppendLine("Program Start");
            infoToolStripMenuItem.CheckState = CheckState.Checked;
        }

        /// <summary>
        /// The file Path
        /// </summary>
        public string filePath = null;
        /// <summary>
        /// The full Path place
        /// </summary>
        public string placementReportPath;
        /// <summary>
        /// The full Path coords
        /// </summary>
        public string placementCoordinatesPath;
        /// <summary>
        /// The scheme drawing Path
        /// </summary>
        public string schemeDrawingPath;
        /// <summary>
        /// The coords
        /// </summary>
        private List<MyDictionary> coords;
        /// <summary>
        /// The table
        /// </summary>
        private List<string[]> table;
        /// <summary>
        /// The mother board image
        /// </summary>
        private Bitmap motherBoardImage;
        /// <summary>
        /// The last clicked item
        /// </summary>
        private ToolStripMenuItem lastClickedItem;
        /// <summary>
        /// The log mode
        /// </summary>
        private string logMode = "info";
        /// <summary>
        /// The error count
        /// </summary>
        private int errorCount = 0;

        /// <summary>
        /// The log text
        /// </summary>
        private StringBuilder logTextInfoPlacementCoordinates = new StringBuilder();
        /// <summary>
        /// The log text information placement report
        /// </summary>
        private StringBuilder logTextInfoPlacementReport = new StringBuilder();
        /// <summary>
        /// The log text debug
        /// </summary>
        private StringBuilder logTextDebugPlacementCoordinates = new StringBuilder();
        /// <summary>
        /// The log text debug placement report
        /// </summary>
        private StringBuilder logTextDebugPlacementReport = new StringBuilder();
        /// <summary>
        /// The log text error
        /// </summary>
        private StringBuilder logTextErrorPlacementCoordinates = new StringBuilder();
        /// <summary>
        /// The log text error placement report
        /// </summary>
        private StringBuilder logTextErrorPlacementReport = new StringBuilder();

        /// <summary>
        /// The log text global
        /// </summary>
        private StringBuilder logTextGlobal = new StringBuilder();

        /// <summary>
        /// Initializes the tabel.
        /// </summary>
        /// <returns></returns>
        private List<string[]> InitTabel()
        {
            CleanLogPlacementCoordinates();
            //Add lines to log file
            this.logTextDebugPlacementCoordinates.AppendLine($"Opening coordinates file {this.placementCoordinatesPath}");
            this.logTextInfoPlacementCoordinates.AppendLine($"Opening coordinates file {this.placementCoordinatesPath}");

            this.logTextDebugPlacementCoordinates.AppendLine($"\nStart converting coordinates file to List<string[]> format\n");
            this.logTextInfoPlacementCoordinates.AppendLine($"\nStart converting coordinates file to List<string[]> format\n");

            List<string[]> tabel = new List<string[]>();
            int index = 0;
            using(var reader = new StreamReader(this.placementCoordinatesPath))
            {
                while (reader.EndOfStream == false)
                {
                    var line = reader.ReadLine().Split('!');
                    for (int i = 0; i < line.Length; i++)
                    {
                        line[i] = RemoveWhiteSpaces(line[i]);
                    }
                    tabel.Add(line);
                    try
                    {
                        switch (index)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                this.logTextDebugPlacementCoordinates.AppendLine("Reading line " + index + ": {" + ConvertListToString(line, index) + "} Dumping line");
                                break;
                            case 4:
                                this.logTextDebugPlacementCoordinates.AppendLine("Reading line " + index + ": {" + ConvertListToString(line, index) + "}");
                                this.logTextDebugPlacementCoordinates.AppendLine("Getting Refdes is in column 0, X coordinate in column 1, Y coordinate in column 2");
                                this.logTextInfoPlacementCoordinates.AppendLine("Getting Refdes is in column 0, X coordinate in column 1, Y coordinate in column 2");
                                break;
                            case 5:
                                this.logTextDebugPlacementCoordinates.AppendLine("Reading line " + index + ": {" + ConvertListToString(line, index) + "} Dumping line");
                                break;
                        }
                        if (index >= 6)
                        {
                            this.logTextDebugPlacementCoordinates.AppendLine("Reading line " + index + ": {" + ConvertListToString(line, index) + "}");
                            if (CanConvertToNumeric(line[1]) == false)//error
                            {
                                this.errorCount++;
                                this.logTextDebugPlacementCoordinates.AppendLine($"ERROR!!! Cannot parse line {index} Refdes {line[0]} coordinate X has incorrect string value {line[1]} that cannot be parsed");
                                this.logTextErrorPlacementCoordinates.AppendLine($"ERROR!!! Cannot parse line {index} Refdes {line[0]} coordinate X has incorrect string value {line[1]} that cannot be parsed");
                                this.logTextInfoPlacementCoordinates.AppendLine($"ERROR!!! Cannot parse line {index} Refdes {line[0]} coordinate X has incorrect string value {line[1]} that cannot be parsed");
                            }
                            else if (CanConvertToNumeric(line[2]) == false)//error
                            {
                                this.errorCount++;
                                this.logTextDebugPlacementCoordinates.AppendLine($"ERROR!!! Cannot parse line {index} Refdes {line[0]} coordinate Y has incorrect string value {line[2]} that cannot be parsed");
                                this.logTextErrorPlacementCoordinates.AppendLine($"ERROR!!! Cannot parse line {index} Refdes {line[0]} coordinate Y has incorrect string value {line[2]} that cannot be parsed");
                                this.logTextInfoPlacementCoordinates.AppendLine($"ERROR!!! Cannot parse line {index} Refdes {line[0]} coordinate Y has incorrect string value {line[2]} that cannot be parsed");

                            }
                            else
                            {
                                this.logTextDebugPlacementCoordinates.AppendLine($"Refdes {line[0]} is located at {line[1]},{line[2]}");
                                this.logTextInfoPlacementCoordinates.AppendLine($"Refdes {line[0]} is located at {line[1]},{line[2]}");
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show("The file you opening is invalid or empty\nPlease choose another file", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }
                    index++;
                }
            }
            if(tabel.Count > 0)
            {
                tabel.RemoveAt(0);
                tabel.RemoveAt(0);
                tabel.RemoveAt(0);

                tabel.RemoveAt(1);
            }
            else
            {
                MessageBox.Show("The file you opening is invalid or empty\nPlease choose another file", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                
            }
            return tabel;
        }
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
        /// Cleans the log placement coordinates.
        /// </summary>
        private void CleanLogPlacementCoordinates()
        {
            this.logTextInfoPlacementCoordinates.Clear();
            this.logTextErrorPlacementCoordinates.Clear();
            this.logTextDebugPlacementCoordinates.Clear();
        }
        /// <summary>
        /// Converts the list to string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private string ConvertListToString(string[] str, int index)
        {
            string line = "";
            for (int i = 0; i < str.Length; i++)
            {
                if(index == 0 || index == 1 || index == 2 || index == 4)
                {
                    line += str[i]+" ";
                }
                else
                {
                    line += "  "+str[i] + "\t !";
                }
            }

            return line.Substring(0, line.Length-1);
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
            foreach(char c in input)
            {
                if(!char.IsDigit(c))
                {
                    if (c != '.' && c != '(' && c != ')')
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Removes the white spaces.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        private string RemoveWhiteSpaces(string line)
        {
            return new string(line.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
        /// <summary>
        /// Initializes the element coords.
        /// </summary>
        /// <returns></returns>
        private List<MyDictionary> InitElementCoords()
        {
            CleanLogPlacementReport();

            this.logTextDebugPlacementReport.AppendLine("Opening placement file " + this.placementReportPath);
            this.logTextInfoPlacementReport.AppendLine("Opening placement file " + this.placementReportPath);

            this.logTextDebugPlacementReport.AppendLine("\nStart converting placement report file");
            this.logTextInfoPlacementReport.AppendLine("\nStart converting placement report file");

            int index = 0;


            List<MyDictionary> coords = new List<MyDictionary>();

            MyDictionary temp = new MyDictionary("First element -> delete");

            using (var reader = new StreamReader(this.placementReportPath))
            {
                while(reader.EndOfStream == false)
                {
                    var line = reader.ReadLine().Split(' ');
                    if (HasName(line))
                    {
                        if(coords.Count != 0)
                        {
                            this.logTextDebugPlacementReport.AppendLine($"Refdes {temp.Key} is added with {temp.Value.Count} coordinations");
                            this.logTextInfoPlacementReport.AppendLine($"Refdes {temp.Key} is added with {temp.Value.Count} coordinations");
                        }
                        coords.Add(temp);
                        temp = new MyDictionary(line[12]);
                        this.logTextDebugPlacementReport.AppendLine($"Reading line {index}: {ConvertToLinePlacementReport(line, 1)}");
                    }
                    else if (EndOfFile(line))
                    {
                        coords.Add(temp);
                    }
                    else if (HasCoords(line))
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
                            this.errorCount++;
                        }
                        if (CanConvertToNumeric(line[4]))
                        {
                            t2 = string.Concat(line[4].Where(Char.IsDigit));
                            t2 = t2.Substring(0, t2.Length - 2);
                        }
                        else
                        {
                            string x = line[4];
                            x = x.Substring(0, x.Length-1);
                            this.logTextErrorPlacementReport.AppendLine($"Error!!! Cannot convert {x} to number in refdes {temp.Key}");
                            this.logTextDebugPlacementReport.AppendLine($"Error!!! Cannot convert {x} to number in refdes {temp.Key}");
                            this.logTextInfoPlacementReport.AppendLine($"Error!!! Cannot convert {x} to number in refdes {temp.Key}");
                            this.errorCount++;
                        }
                        
                        if(t1!=null && t2 != null)
                        {
                            temp.AddValue(int.Parse(t1), int.Parse(t2));
                            this.logTextDebugPlacementReport.AppendLine($"Coordinates {t1},{t2} added to refdes {temp.Key}");
                        }
                    }
                    else if (HasArc(line))
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
                            this.errorCount++;
                        }

                        if (CanConvertToNumeric(line[5]))
                        {
                            y1 = string.Concat(line[5].Where(char.IsDigit));
                            y1 = y1.Substring(0,y1.Length - 2);
                        }
                        else
                        {
                            string t1 = line[5];
                            t1 = t1.Substring(0, t1.Length - 1);
                            this.logTextErrorPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextDebugPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.logTextInfoPlacementReport.AppendLine($"Error!!! Cannot convert {t1} to number in refdes {temp.Key}");
                            this.errorCount++;
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
                            this.errorCount++;
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
                            this.errorCount++;
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
                            this.errorCount++;
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
                            this.errorCount++;
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
                            this.errorCount++;
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
            if(coords.Count > 0)
            {
                coords.RemoveAt(0);
            }
            else
            {
                MessageBox.Show("You opened a wrong file or an empty one", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return coords;
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
            if(mode == 0)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    str += line[i] + "  ";
                }
                str += "} Dumping line";
            }
            if(mode == 1)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    str += line[i] + "  ";
                }
                str += "} Name line";
            }
            if(mode == 2)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    str += line[i] + "  ";
                }
                str += "} coordinates line";
            }
            

            return str;
        }
        /// <summary>
        /// Ends the of file.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        public static bool EndOfFile(string[] line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Equals("~end-of-file~"))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Determines whether the specified line has coords.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>
        ///   <c>true</c> if the specified line has coords; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasCoords(string[] line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Equals("segment:xy"))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Determines whether the specified line has name.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>
        ///   <c>true</c> if the specified line has name; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasName(string[] line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Equals("RefDes:"))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Determines whether the specified line has arc.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>
        ///   <c>true</c> if the specified line has arc; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasArc(string[] line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Equals("seg:xy"))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Determines whether [has centar point] [the specified line].
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>
        ///   <c>true</c> if [has centar point] [the specified line]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasCentarPoint(string[] line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Equals("center-xy:"))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Gets the coords.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string GetCoords(string name)
        {
            string line = "";
            for (int i = 0; i < this.coords.Count; i++)
            {
                if (coords[i].Key.Equals(name))
                {
                    MyDictionary x = coords[i];
                    line += ":";
                    for (int j = 0; j < x.Value.Count; j++)
                    {
                        if (!x.Value[j].IsRegularPoint())
                        {
                            line += $"[{x.Value[j].X};{x.Value[j].Y};{x.Value[j].Z}]";
                        }
                        else
                        {
                            line += $"[{x.Value[j].X};{x.Value[j].Y}]";
                        }
                    }
                }
            }
            return line;
        }
        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="outputString">The output string.</param>
        /// <param name="outputLogPath">The output log Path.</param>
        private void SaveFile(string outputString, string outputLogPath)
        {
            StringBuilder csv = new StringBuilder();
            string line = "";
            for (int i = 0; i < this.table[0].Length; i++)
            {
                line += this.table[0][i].ToString() + ",";
            }
            csv.AppendLine(line + "DXF");
            for (int i = 1; i < this.table.Count; i++)
            {
                line = "";
                for (int j = 0; j < this.table[i].Length; j++)
                {
                    line += this.table[i][j].ToString() + ",";
                }
                line += GetCoords(this.table[i][0]);
                csv.AppendLine(line);
            }

            File.AppendAllText(outputString, csv.ToString());

            switch (logMode)
            {
                case "error":

                    this.logTextGlobal.Append(this.logTextErrorPlacementCoordinates.ToString());
                    this.logTextGlobal.Append(this.logTextErrorPlacementReport.ToString());
                    this.logTextGlobal.AppendLine("\nErrors count: " + this.errorCount);
                    this.logTextGlobal.AppendLine("\nFile saved at " + outputString);

                    File.WriteAllText(outputLogPath, this.logTextGlobal.ToString());
                    break;
                case "info":
                    this.logTextGlobal.Append(this.logTextInfoPlacementCoordinates.ToString());
                    this.logTextGlobal.AppendLine();
                    this.logTextGlobal.Append(this.logTextInfoPlacementReport.ToString());
                    this.logTextGlobal.AppendLine("\nErrors count: " + this.errorCount);
                    this.logTextGlobal.AppendLine("\nFile saved at " + outputString);

                    File.WriteAllText(outputLogPath, this.logTextGlobal.ToString());
                    break;
                case "debug":
                    this.logTextGlobal.Append(this.logTextDebugPlacementCoordinates.ToString());
                    this.logTextGlobal.AppendLine();
                    this.logTextGlobal.Append(this.logTextDebugPlacementReport.ToString());
                    this.logTextGlobal.AppendLine("\nErrors count: " + this.errorCount);
                    this.logTextGlobal.AppendLine("\nFile saved at " + outputString);

                    File.WriteAllText(outputLogPath, this.logTextGlobal.ToString());
                    break;
            }
            CleanLogPlacementCoordinates();
            CleanLogPlacementReport();
            this.errorCount = 0;
            this.filePath = null;
        }
        private void SaveFileUsingOneFile(string outputString, string outputLogPath)
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("REFDES,SYM_X,SYM_Y,SYM_ROTATE,SYM_MIRROR,DFX");
            string line = "";

            for (int i = 0; i < this.coords.Count; i++)
            {
                line = this.coords[i].Key + $",0,0,0,{this.coords[i].Mirror}";
                line += GetCoords(this.coords[i].Key);
                csv.AppendLine(line);
            }

        }
        /// <summary>
        /// Handles the Click event of the placementReportFileToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void placementReportFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.Title = "Open Placement Report File...";
            if (this.filePath != null)
            {
                openFileDialog.InitialDirectory = this.filePath;
            }
            else
            {
                openFileDialog.InitialDirectory = @"C:\";
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.placementReportPath = openFileDialog.FileName;
                this.filePath = GetLastFilePath(openFileDialog.FileName);
            }

            if(this.placementReportPath != null)
            {
                this.coords = InitElementCoords();
            }

            if (this.coords != null)
            {
                if (this.coords.Count > 0)
                {
                    if(this.table!= null)
                    {
                        MessageBox.Show(GetEmptyRefDeses(), "Empty RefDeses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    MoveAllElements(ref this.coords);

                    this.coords = DeleteUnnecessaryCoords();

                    this.coords = FlipImage(this.coords);
                    
                    List<List<Point3D>> lst = ConvertToListOfListOfPoints();

                    DrawPoints(pbSketch, lst);
                }
            }

        }
        /// <summary>
        /// Deletes the unnecessary coords.
        /// </summary>
        /// <returns></returns>
        private List<MyDictionary> DeleteUnnecessaryCoords()
        {
            List<MyDictionary> result = this.coords;
            List<MyDictionary> temp = this.coords;
            int count = 0;

            for (int i = 0; i < result.Count; i++)
            {
                for (int j = 0; j < result[i].Value.Count-1; j++)
                {
                    if (result[i].Value[j].GetRegularPoint().Equals(result[i].Value[j + 1].GetRegularPoint()))
                    {
                        temp[i].Value.Remove(result[i].Value[j]);
                        count++;
                    }
                }
            }

            return temp;
        }
        /// <summary>
        /// Gets the empty reference deses.
        /// </summary>
        /// <returns></returns>
        private string GetEmptyRefDeses()
        {
            List<string> names = new List<string>();
            List<string[]> allnames = this.table;
            string result = "RefDeses that are not visible on the diagram:\n";

            for (int i = 1; i < allnames.Count; i++)
            {
                if (GetCoords(allnames[i][0]) == "")
                {
                    names.Add(allnames[i][0]);
                }
            }

            if(names.Count == 0)
            {
                return "there is no empty RefDeses";
            }
            else
            {
                for (int i = 0; i < names.Count; i++)
                {
                    result += names[i] + ", ";
                }
            }

            return result.Remove(result.Length - 2);
        }

        /// <summary>
        /// Converts to list of list of points.
        /// </summary>
        /// <returns></returns>
        private List<List<Point3D>> ConvertToListOfListOfPoints()
        {
            List<List<Point3D>> lst = new List<List<Point3D>>();

            for (int i = 0; i < this.coords.Count; i++)
            {
                lst.Add(this.coords[i].Value);
            }

            return lst;
        }
        /// <summary>
        /// Draws the points.
        /// </summary>
        /// <param name="pb">The pb.</param>
        /// <param name="pointLists">The point lists.</param>
        private void DrawPoints(PictureBox pb, List<List<Point3D>> pointLists)
        {
            this.logTextDebugPlacementReport.AppendLine("Start drawing scheme\n");
            this.logTextInfoPlacementReport.AppendLine("Start drawing scheme\n");

            if (pb == null || pointLists == null)
                return;

            using (Bitmap bmp = new Bitmap(FindMaxX() + 30, FindMaxY() + 30))
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
                                    if (i+1 < points.Count-1)
                                    {
                                        
                                        if (!points[i+1].IsRegularPoint())
                                        {
                                            Point start = points[i].GetRegularPoint();
                                            Point center = points[i+1].GetRegularPoint();
                                            bool isClockwise = false;
                                            if (!points[i + 1].Z.Equals("0"))
                                            {
                                                isClockwise = true;
                                            }
                                            Point end = points[i+2].GetRegularPoint();

                                            int radius = (int)Math.Sqrt(Math.Pow(center.X - start.X, 2) + Math.Pow(center.Y - start.Y, 2));
                                            int x = center.X - radius;
                                            int y = center.Y - radius;
                                            int wigth = 2 * radius;
                                            int height = 2 * radius;

                                            float startAngle = (float)Math.Atan2(start.Y - center.Y, start.X - center.X) * 180 / (float)Math.PI;
                                            float endAngle = (float)Math.Atan2(end.Y - center.Y, end.X - center.X) * 180 / (float)Math.PI;
                                            float sweepAngle = endAngle - startAngle;

                                            if(Math.Abs(sweepAngle) == 0)
                                            {
                                                g.DrawEllipse(pen, x, y, wigth, height);
                                            }
                                            else
                                            {
                                                if(isClockwise && sweepAngle < 0)
                                                {
                                                    sweepAngle += 360;
                                                }
                                                else if(!isClockwise && sweepAngle > 0)
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
                if(!IsBitmapFormatCompatible(bmp))
                {
                    this.logTextErrorPlacementReport.AppendLine("Error!!! Bitmap format is wrong, cannot convert bitmap to image");
                    this.logTextDebugPlacementReport.AppendLine("Error!!! Bitmap format is wrong, cannot convert bitmap to image");
                    this.logTextInfoPlacementReport.AppendLine("Error!!! Bitmap format is wrong, cannot convert bitmap to image");
                    this.errorCount++;
                    MessageBox.Show("BitMap format error", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.motherBoardImage = bmp;
                AddText(ref this.motherBoardImage);
                Image image = Image.FromHbitmap(this.motherBoardImage.GetHbitmap());
                pbSketch.Image = image;

                List<string> names = new List<string>();
                foreach (var item in this.coords)
                {
                    names.Add(item.Key);
                }

                names.Sort(CustomStringComparer);

                listBox1.DataSource = names;
                listBox1.SelectionMode = SelectionMode.MultiExtended;
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
        /// <summary>
        /// Flips the image.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        private List<MyDictionary> FlipImage(List<MyDictionary> list)
        {
            List<MyDictionary> result = new List<MyDictionary>();
            int maxY = FindMaxY() + 30;

            foreach (var dict in list)
            {
                MyDictionary newDict = new MyDictionary(dict.Key);

                foreach (var point in dict.Value)
                {
                    int newY = maxY - int.Parse(point.Y);
                    if(point.Z != "Empty")
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
            return result;
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
        /// Finds the maximum x.
        /// </summary>
        /// <returns></returns>
        private int FindMaxX()
        {
            List<MyDictionary> lst = this.coords;

            int maxX = int.MinValue;

            for (int i = 0; i < lst.Count; i++)
            {
                for (int j = 0; j < lst[i].Value.Count; j++)
                {
                    maxX = Math.Max(maxX, int.Parse(lst[i].Value[j].X));
                }
            }

            return maxX;
        }
        /// <summary>
        /// Finds the maximum y.
        /// </summary>
        /// <returns></returns>
        private int FindMaxY()
        {
            List<MyDictionary> lst = this.coords;

            int maxY = int.MinValue;

            for (int i = 0; i < lst.Count; i++)
            {
                for (int j = 0; j < lst[i].Value.Count; j++)
                {
                    maxY = Math.Max(maxY, int.Parse(lst[i].Value[j].Y));
                }
            }

            return maxY;
        }
        /// <summary>
        /// Handles the Click event of the coordinatesReportFileToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void coordinatesReportFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.Title = "Open Placement Coordinates File...";
            if(this.filePath != null)
            {
                openFileDialog.InitialDirectory = this.filePath;
            }
            else
            {

                openFileDialog.InitialDirectory = @"C:\";
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.placementCoordinatesPath = openFileDialog.FileName;
                this.filePath = GetLastFilePath(openFileDialog.FileName);
            }

            if(this.placementCoordinatesPath != null)
            {
                this.table = InitTabel();
            }

            if (this.coords != null && this.table != null)
            {
                if (this.coords.Count > 0 && this.table.Count > 0)
                {
                    MessageBox.Show(GetEmptyRefDeses(), "Empty RefDeses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        /// <summary>
        /// Gets the last file Path.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private string GetLastFilePath(string fileName)
        {
            string result = null;

            string[] split = fileName.Split('\\');

            for (int i = 0; i < split.Length-1; i++)
            {
                result += split[i] + "\\";
            }
            return result;
        }

        /// <summary>
        /// Handles the Click event of the exitToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ans = MessageBox.Show("Close Program?","Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans == DialogResult.Yes)
            {
                this.logTextGlobal.AppendLine("Exit program");
                System.Environment.Exit(1);
            }
        }
        
        /// <summary>
        /// Adds the text.
        /// </summary>
        private void AddText(ref Bitmap bitmap)
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message+"\nCannot add text to scheme", "Error Text", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.errorCount++;
            }
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
            if(v == 'x')
            {
                for (int i = 0; i < value.Count; i++)
                {
                    sum += int.Parse(value[i].X);
                }
            }
            else if(v == 'y')
            {
                for (int i = 0; i < value.Count; i++)
                {
                    sum += int.Parse(value[i].Y);
                }
            }
            return sum;
        }

        /// <summary>
        /// Gets the minimum y coordination.
        /// </summary>
        /// <returns></returns>
        private int GetMinYCoordination()
        {
            List<MyDictionary> lst = this.coords;
            int num = int.MaxValue;
            for (int i = 0; i < lst.Count; i++)
            {
                for (int j = 0; j < lst[i].Value.Count; j++)
                {
                    if (int.Parse(lst[i].Value[j].Y) < num)
                    {
                        num = int.Parse(lst[i].Value[j].Y);
                    }
                }
            }
            return num;
        }
        /// <summary>
        /// Gets the minimum x coordination.
        /// </summary>
        /// <returns></returns>
        private int GetMinXCoordination()
        {
            List<MyDictionary> lst = this.coords;
            int num = int.MaxValue;
            for (int i = 0; i < lst.Count; i++)
            {
                for (int j = 0; j < lst[i].Value.Count; j++)
                {
                    if (int.Parse(lst[i].Value[j].X) < num)
                    {
                        num = int.Parse(lst[i].Value[j].X);
                    }
                }
            }
            return num;
        }
        /// <summary>
        /// Moves all elements.
        /// </summary>
        private void MoveAllElements(ref List<MyDictionary> lst)
        {
            int deleyY = GetMinYCoordination()-10;
            int deleyX = GetMinXCoordination() - 10;
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
        /// Handles the Click event of the saveAsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string savePath = null;
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "CSV File (*.csv)|*.csv|All Files (*.*)|*.*";
            save.Title = "Save output file in...";
            save.DefaultExt = "csv";
            if(this.filePath != null)
            {
                save.InitialDirectory = this.filePath;
            }
            else
            {
                save.InitialDirectory = @"C:\";
            }
            if (save.ShowDialog() == DialogResult.OK)
            {
                savePath = save.FileName;
            }
            if(savePath != null)
            {
                if (this.placementCoordinatesPath != null && this.placementReportPath != null)
                {
                    SaveFile(savePath, ChangeFileExtension(savePath, ".log"));
                    MessageBox.Show("File Saved", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (this.placementReportPath != null && this.placementCoordinatesPath == null)
                {
                    DialogResult ans = MessageBox.Show("You want to savw the file using only Placement Report file?", "Saving process", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if(ans == DialogResult.OK)
                    {
                        SaveFileUsingOneFile(savePath, ChangeFileExtension(savePath, ".log"));
                        MessageBox.Show("File saved", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if(this.placementCoordinatesPath == null && this.placementReportPath == null)
                {
                    MessageBox.Show("You need to chose files first!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            
        }
        /// <summary>
        /// Changes the file extension.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="newExtension">The new extension.</param>
        /// <returns></returns>
        private string ChangeFileExtension(string filePath, string newExtension)
        {
            string directory = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            return Path.Combine(directory, fileName + newExtension);
        }
        /// <summary>
        /// Handles the Click event of the errorToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void errorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            UncheckAllMenuItems(menuStrip1.Items);
            item.Checked = !item.Checked;

            if(item.Checked)
            {
                item.CheckState = CheckState.Checked;
            }
            if (lastClickedItem != null && lastClickedItem != item)
            {
                lastClickedItem.Checked = false;
                lastClickedItem.CheckState = CheckState.Unchecked;
            }
            lastClickedItem = item;
            logMode = "error";
        }
        /// <summary>
        /// Handles the Click event of the infoToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = !item.Checked;

            if (item.Checked)
            {
                item.CheckState = CheckState.Checked;
            }
            if (lastClickedItem != null && lastClickedItem != item)
            {
                lastClickedItem.Checked = false;
                lastClickedItem.CheckState = CheckState.Unchecked;
            }
            lastClickedItem = item;
            logMode = "info";
        }
        /// <summary>
        /// Handles the Click event of the debugToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            UncheckAllMenuItems(menuStrip1.Items);
            item.Checked = !item.Checked;

            if (item.Checked)
            {
                item.CheckState = CheckState.Checked;
            }
            if (lastClickedItem != null && lastClickedItem != item)
            {
                lastClickedItem.Checked = false;
                lastClickedItem.CheckState = CheckState.Unchecked;
            }
            lastClickedItem = item;
            logMode = "debug";
        }
        /// <summary>
        /// Unchecks all menu items.
        /// </summary>
        /// <param name="items">The items.</param>
        private void UncheckAllMenuItems(ToolStripItemCollection items)
        {
            foreach (var item in items)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    menuItem.Checked = false;
                    // If the subMenuItem has sub-items, uncheck them as well (recursive call).
                    UncheckAllMenuItems(menuItem.DropDownItems);
                }
            }
        }

        /// <summary>
        /// Converts to list of list of points red.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        private List<List<Point3D>> ConvertToListOfListOfPointsRED(List<string> names)
        {
            List<List<Point3D>> lst = new List<List<Point3D>>();

            for (int i = 0; i < this.coords.Count; i++)
            {
                if (NameInList(this.coords[i].Key, names))
                {
                    lst.Add(this.coords[i].Value);
                }
            }
            return lst;
        }
        /// <summary>
        /// Names the in list.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        private bool NameInList(string key, List<string> names)
        {
            for (int i = 0; i < names.Count; i++)
            {
                if (names[i].Equals(key))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Draws the points.
        /// </summary>
        /// <param name="pb">The pb.</param>
        /// <param name="pointLists">The point lists.</param>
        /// <param name="redElements">The red elements.</param>
        private void DrawPoints(PictureBox pb, List<List<Point3D>> pointLists, List<List<Point3D>> redElements)
        {
            this.logTextDebugPlacementReport.AppendLine("Start drawing scheme\n");
            this.logTextInfoPlacementReport.AppendLine("Start drawing scheme\n");

            if (pb == null || pointLists == null)
                return;

            using (Bitmap bmp = new Bitmap(FindMaxX() + 30, FindMaxY() + 30))
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

                    // Draw lines for each list of points
                    using (Pen pen = new Pen(Color.Red, 5))
                    {
                        foreach (List<Point3D> points in redElements)
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
                    this.logTextErrorPlacementReport.AppendLine("Error!!! Bitmap format is wrong, cannot convert bitmap to image");
                    this.logTextDebugPlacementReport.AppendLine("Error!!! Bitmap format is wrong, cannot convert bitmap to image");
                    this.logTextInfoPlacementReport.AppendLine("Error!!! Bitmap format is wrong, cannot convert bitmap to image");
                    this.errorCount++;
                    MessageBox.Show("BitMap format error", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.motherBoardImage = bmp;
                AddText(ref this.motherBoardImage);
                Image image = Image.FromHbitmap(this.motherBoardImage.GetHbitmap());
                pbSketch.Image = image;
            }
        }
        /// <summary>
        /// Handles the Resize event of the Form1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            listBox1.Height = this.Height - 66;
            pbSketch.Height = this.Height - 70;
            pbSketch.Width = this.Width - listBox1.Width - 30;
        }
        /// <summary>
        /// Handles the SelectedIndexChanged event of the listBox1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> allNames = new List<string>();

            if (this.coords.Count > 1)
            {
                for (int i = 0; i < this.coords.Count; i++)
                {
                    allNames.Add(this.coords[i].Key);
                }
            }

            List<string> checkedCheckBoxNames = new List<string>();

            foreach (var selectedItem in listBox1.SelectedItems)
            {
                checkedCheckBoxNames.Add(selectedItem.ToString());
            }

            List<List<Point3D>> lst = ConvertToListOfListOfPoints();
            List<List<Point3D>> redElements = ConvertToListOfListOfPointsRED(checkedCheckBoxNames);
            DrawPoints(pbSketch, lst, redElements);
        }
    }
}
