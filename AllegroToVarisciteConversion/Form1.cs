using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
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

            this.logTextGlobal.AppendLine("Program Start");
            infoToolStripMenuItem.CheckState = CheckState.Checked;
        }

        /// <summary>
        /// The full patch place
        /// </summary>
        public string placementReportPatch;
        /// <summary>
        /// The full patch coords
        /// </summary>
        public string placementCoordinatesPatch;
        /// <summary>
        /// The scheme drawing patch
        /// </summary>
        public string schemeDrawingPatch;
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
            this.logTextDebugPlacementCoordinates.AppendLine($"Opening coordinates file {this.placementCoordinatesPatch}");
            this.logTextInfoPlacementCoordinates.AppendLine($"Opening coordinates file {this.placementCoordinatesPatch}");

            this.logTextDebugPlacementCoordinates.AppendLine($"\nStart converting coordinates file to List<string[]> format\n");
            this.logTextInfoPlacementCoordinates.AppendLine($"\nStart converting coordinates file to List<string[]> format\n");

            List<string[]> tabel = new List<string[]>();
            int index = 0;
            using(var reader = new StreamReader(this.placementCoordinatesPatch))
            {
                while (reader.EndOfStream == false)
                {
                    var line = reader.ReadLine().Split('!');
                    for (int i = 0; i < line.Length; i++)
                    {
                        line[i] = RemoveWhiteSpaces(line[i]);
                    }
                    tabel.Add(line);

                    switch (index)
                    {
                        case 0:case 1:case 2:case 3:
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
                    if(index >= 6)
                    {
                        this.logTextDebugPlacementCoordinates.AppendLine("Reading line " + index + ": {" + ConvertListToString(line, index) + "}");
                        if (CanConvertToNumeric(line[1]) == false)//error
                        {
                            this.logTextDebugPlacementCoordinates.AppendLine($"ERROR!!! Cannot parse line {index} Refdes {line[0]} coordinate X has incorrect string value {line[1]} that cannot be parsed");
                            this.logTextErrorPlacementCoordinates.AppendLine($"ERROR!!! Cannot parse line {index} Refdes {line[0]} coordinate X has incorrect string value {line[1]} that cannot be parsed");
                            this.logTextInfoPlacementCoordinates.AppendLine($"ERROR!!! Cannot parse line {index} Refdes {line[0]} coordinate X has incorrect string value {line[1]} that cannot be parsed");
                        }
                        else if(CanConvertToNumeric(line[2]) == false)//error
                        {
                            this.logTextDebugPlacementCoordinates.AppendLine($"ERROR!!! Cannot parse line {index} Refdes {line[0]} coordinate Y has incorrect string value {line[2]} that cannot be parsed");
                            this.logTextErrorPlacementCoordinates.AppendLine($"ERROR!!! Cannot parse line {index} Refdes {line[0]} coordinate Y has incorrect string value {line[2]} that cannot be parsed");
                            this.logTextInfoPlacementCoordinates.AppendLine($"ERROR!!! Cannot parse line {index} Refdes {line[0]} coordinate Y has incorrect string value {line[2]} that cannot be parsed");

                        }
                        this.logTextDebugPlacementCoordinates.AppendLine($"Refdes {line[0]} is located at {line[1]},{line[2]}");
                        this.logTextInfoPlacementCoordinates.AppendLine($"Refdes {line[0]} is located at {line[1]},{line[2]}");
                    }
                    index++;
                }
            }
            tabel.RemoveAt(0);
            tabel.RemoveAt(0);
            tabel.RemoveAt(0);

            tabel.RemoveAt(1);
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
                    if(c != '.')
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

            this.logTextDebugPlacementReport.AppendLine("Opening placement file " + this.placementReportPatch);
            this.logTextInfoPlacementReport.AppendLine("Opening placement file " + this.placementReportPatch);

            this.logTextDebugPlacementReport.AppendLine("\nStart converting placement report file");
            this.logTextInfoPlacementReport.AppendLine("\nStart converting placement report file");

            int index = 0;

            List<MyDictionary> coords = new List<MyDictionary>();

            MyDictionary temp = new MyDictionary("First element -> delete");

            using (var reader = new StreamReader(this.placementReportPatch))
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
                        var t1 = string.Concat(line[3].Where(Char.IsDigit));
                        t1 = t1.Substring(0, t1.Length - 2);
                        var t2 = string.Concat(line[4].Where(Char.IsDigit));
                        t2 = t2.Substring(0, t2.Length - 2);

                        temp.AddValue(int.Parse(t1), int.Parse(t2));

                        this.logTextDebugPlacementReport.AppendLine($"Coordination {t1},{t2} added to refdes {temp.Key}");
                        //this.logTextInfo.AppendLine($"Coordination {t1},{t2} added to refdes {temp.Key}");
                    }
                    else
                    {
                        this.logTextDebugPlacementReport.AppendLine($"Reading line {index}: {ConvertToLinePlacementReport(line, 0)}");
                    }
                    
                    index++;
                }
            }
            coords.RemoveAt(0);
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
                str += "} coordination line";
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
                    for (int j = 0; j < x.Value.Count; j++)
                    {
                        line += $"[{x.Value[j].X};{x.Value[j].Y}]";
                    }
                }
            }
            return line;
        }
        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="outputString">The output string.</param>
        /// <param name="outputLogPatch">The output log patch.</param>
        private void SaveFile(string outputString, string outputLogPatch)
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
                csv.AppendLine(":"+line);
            }

            File.AppendAllText(outputString, csv.ToString());

            switch (logMode)
            {
                case "error":

                    this.logTextGlobal.Append(this.logTextErrorPlacementCoordinates.ToString());
                    this.logTextGlobal.Append(this.logTextErrorPlacementReport.ToString());
                    this.logTextGlobal.AppendLine("\nFile saved at " + outputString);

                    File.WriteAllText(outputLogPatch, this.logTextGlobal.ToString());
                    break;
                case "info":
                    this.logTextGlobal.Append(this.logTextInfoPlacementCoordinates.ToString());
                    this.logTextGlobal.Append(this.logTextInfoPlacementReport.ToString());
                    this.logTextGlobal.AppendLine("\nFile saved at " + outputString);

                    File.WriteAllText(outputLogPatch, this.logTextGlobal.ToString());
                    break;
                case "debug":
                    this.logTextGlobal.Append(this.logTextDebugPlacementCoordinates.ToString());
                    this.logTextGlobal.Append(this.logTextDebugPlacementReport.ToString());
                    this.logTextGlobal.AppendLine("\nFile saved at " + outputString);

                    File.WriteAllText(outputLogPatch, this.logTextGlobal.ToString());
                    break;
            }
            CleanLogPlacementCoordinates();
            CleanLogPlacementReport();
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
            openFileDialog.InitialDirectory = @"C:\";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.placementReportPatch = openFileDialog.FileName;
                
            }

            if(this.placementReportPatch != null)
            {
                this.coords = InitElementCoords();
            }
            
            if(this.coords != null)
            {
                MoveAllElements(ref this.coords);

                List<List<Point>> lst = ConvertToListOfListOfPoints();

                DrawPoints(pbSketch, lst);

                pbSketch.Image = Image.FromFile(@"../../Resources/image.png");
            }
            
        }
        /// <summary>
        /// Converts to list of list of points.
        /// </summary>
        /// <returns></returns>
        private List<List<Point>> ConvertToListOfListOfPoints()
        {
            List<List<Point>> lst = new List<List<Point>>();

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
        private void DrawPoints(PictureBox pb, List<List<Point>> pointLists)
        {
            this.logTextDebugPlacementReport.AppendLine("Start drawing scheme\n");
            this.logTextInfoPlacementReport.AppendLine("Start drawing scheme\n");

            if (this.motherBoardImage != null)
            {
                this.motherBoardImage.Dispose();
            }
            if (pb == null || pointLists == null)
                return;

            using (Bitmap bmp = new Bitmap(FindMaxX() + 20/*pb.Width*/, FindMaxY() + 20/*pb.Height*/))
            {
                
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // Clear the PictureBox
                    g.Clear(Color.White);

                    // Draw lines for each list of points
                    using (Pen pen = new Pen(Color.Black, 2))
                    {
                        foreach (List<Point> points in pointLists)
                        {
                            // Draw lines connecting the points
                            if (points.Count > 1)
                            {
                                for (int i = 0; i < points.Count - 1; i++)
                                {
                                    g.DrawLine(pen, points[i], points[i + 1]);
                                }

                                // Connect the last point with the first point to complete the figure
                                g.DrawLine(pen, points[points.Count - 1], points[0]);
                            }
                        }
                    }
                }

                // Assign the updated bitmap to the PictureBox
                if(!IsBitmapFormatCompatible(bmp))
                {
                    this.logTextErrorPlacementReport.AppendLine("Errpr!!! Bitmap format is wrong, cannot converte bitmap to image");
                    this.logTextDebugPlacementReport.AppendLine("Errpr!!! Bitmap format is wrong, cannot converte bitmap to image");
                    this.logTextInfoPlacementReport.AppendLine("Errpr!!! Bitmap format is wrong, cannot converte bitmap to image");
                    MessageBox.Show("BitMap format error", "Alert!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.motherBoardImage = bmp;
                AddText(ref this.motherBoardImage);
                pbSketch.Image = this.motherBoardImage;
            }
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
                    maxX = Math.Max(maxX, lst[i].Value[j].X);
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
                    maxY = Math.Max(maxY, lst[i].Value[j].Y);
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
            openFileDialog.InitialDirectory = @"C:\";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.placementCoordinatesPatch = openFileDialog.FileName;
            }

            if(this.placementCoordinatesPatch != null)
            {
                this.table = InitTabel();
            }
            
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
            }
            
        }
        /// <summary>
        /// Sums the points.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        private int SumPoints(List<Point> value, char v)
        {
            int sum = 0;
            if(v == 'x')
            {
                for (int i = 0; i < value.Count; i++)
                {
                    sum += value[i].X;
                }
            }
            else if(v == 'y')
            {
                for (int i = 0; i < value.Count; i++)
                {
                    sum += value[i].Y;
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
                    if (lst[i].Value[j].Y < num)
                    {
                        num = lst[i].Value[j].Y;
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
                    if (lst[i].Value[j].X < num)
                    {
                        num = lst[i].Value[j].X;
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
                List<Point> coordinations = lst[i].Value;
                for (int j = 0; j < coordinations.Count; j++)
                {
                    Point point = coordinations[j];
                    point.Y -= deleyY;
                    point.X -= deleyX;
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
            string savePatch = null;
            SaveFileDialog save = new SaveFileDialog();
            save.InitialDirectory = @"C:\";
            save.Filter = "CSV File (*.csv)|*.csv|All Files (*.*)|*.*";
            save.Title = "Save output file in...";
            save.DefaultExt = "csv";
            if (save.ShowDialog() == DialogResult.OK)
            {
                savePatch = save.FileName;
            }

            if (this.placementCoordinatesPatch != null && this.placementReportPatch != null)
            {
                SaveFile(savePatch, ChangeFileExtension(savePatch, ".log"));
                MessageBox.Show("File Saved", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("You need to chose files first!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
    }
}
