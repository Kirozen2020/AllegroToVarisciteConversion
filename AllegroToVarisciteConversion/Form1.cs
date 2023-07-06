using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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
        /// The coords
        /// </summary>
        private List<MyDictionary> coords;
        /// <summary>
        /// The table
        /// </summary>
        private List<string[]> table;

        

        /// <summary>
        /// Initializes the tabel.
        /// </summary>
        /// <returns></returns>
        private List<string[]> InitTabel()
        {
            List<string[]> tabel = new List<string[]>();
            
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
                }
            }
            tabel.RemoveAt(0);
            tabel.RemoveAt(0);
            tabel.RemoveAt(0);

            tabel.RemoveAt(1);
            return tabel;
        }
        /// <summary>
        /// Removes the white spaces.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        public static string RemoveWhiteSpaces(string line)
        {
            return new string(line.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
        /// <summary>
        /// Initializes the element coords.
        /// </summary>
        /// <returns></returns>
        private List<MyDictionary> InitElementCoords()
        {
            List<MyDictionary> coords = new List<MyDictionary>();

            MyDictionary temp = new MyDictionary("First element -> delete");

            using (var reader = new StreamReader(this.placementReportPatch))
            {
                while(reader.EndOfStream == false)
                {
                    var line = reader.ReadLine().Split(' ');
                    if (HasName(line))
                    {
                        coords.Add(temp);
                        temp = new MyDictionary(line[12]);
                    }
                    if (EndOfFile(line))
                    {
                        coords.Add(temp);
                    }

                    if (HasCoords(line))
                    {
                        var t1 = string.Concat(line[3].Where(Char.IsDigit));
                        t1 = t1.Substring(0, t1.Length - 2);
                        var t2 = string.Concat(line[4].Where(Char.IsDigit));
                        t2 = t2.Substring(0, t2.Length - 2);
                        var t3 = string.Concat(line[6].Where(Char.IsDigit));
                        t3 = t3.Substring(0, t3.Length - 2);
                        var t4 = string.Concat(line[7].Where(Char.IsDigit));
                        t4 = t4.Substring(0, t4.Length - 2);

                        temp.AddValue(int.Parse(t1), int.Parse(t2));

                    }
                }
            }
            coords.RemoveAt(0);
            return coords;
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
        private void SaveFile(string outputString)
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
            save.Title = "Save file in...";
            save.DefaultExt = "csv";
            if(save.ShowDialog() == DialogResult.OK)
            {
                savePatch = save.FileName;
            }

            if (this.placementCoordinatesPatch != null && this.placementReportPatch != null)
            {
                SaveFile(savePatch);
                MessageBox.Show("File Saved");
            }
            else
            {
                MessageBox.Show("you need to chose files first!", "attention", MessageBoxButtons.OK);
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
                List<Point> points = new List<Point>()
                {
                    new Point(50, 50),
                    new Point(100, 100),
                    new Point(150, 50),
                    new Point(100, 150),
                };


                PictureBox pictureBox = new PictureBox();
                pictureBox.Dock = DockStyle.Fill;

                Controls.Add(pictureBox);

                List<List<Point>> lst = ConvertToListOfListOfPoints();

                DrawPoints(pictureBox, lst);
            }
            
        }

        private List<List<Point>> ConvertToListOfListOfPoints()
        {
            List<List<Point>> lst = new List<List<Point>>();

            for (int i = 0; i < this.coords.Count; i++)
            {
                lst.Add(this.coords[i].Value);
            }

            return lst;
        }

        private void DrawPoints(PictureBox pb, List<List<Point>> pointLists)
        {
            if (pb == null || pointLists == null)
                return;

            using (Bitmap bmp = new Bitmap(pb.Width, pb.Height))
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
                pb.Image = bmp;
            }
        }

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
            DialogResult d = MessageBox.Show("Close Program?","Exit", MessageBoxButtons.YesNo);
            if (d == DialogResult.Yes)
            {
                System.Environment.Exit(1);
            }
        }

    }
}
