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

        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// containing full patch to the placement report file
        /// </summary>
        public string full_patch_Place;
        /// <summary>
        /// containing full patch to the placement coordinates file
        /// </summary>
        public string full_patch_Coords;
        /// <summary>
        /// containing all data from placement report file in list format
        /// </summary>
        private List<MyDictionary> coords;
        /// <summary>
        /// containing all data from placement coordinates file in list format
        /// </summary>
        private List<string[]> table;
        /// <summary>
        /// Open Placement Report file and getting his full patch
        /// </summary>
        private void btnOpenPlace_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.full_patch_Place = openFileDialog1.FileName;
            }
            this.coords = InitElementCoords();
        }
        /// <summary>
        /// Open Placement Coordinates file and getting his full patch
        /// </summary>
        private void btnOpenCoords_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.full_patch_Coords = openFileDialog1.FileName;
            }
            this.table = InitTabel();
        }
        /// <summary>
        /// Reading all data from placement report file, organize in one list
        /// </summary>
        /// <returns> one list with </returns>
        private List<string[]> InitTabel()
        {
            List<string[]> tabel = new List<string[]>();
            
            using(var reader = new StreamReader(this.full_patch_Place))
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
        /// Removing all space character from string
        /// </summary>
        /// <returns> string witout the space character </returns>
        public static string RemoveWhiteSpaces(string line)
        {
            return new string(line.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
        /// <summary>
        /// Reading all data from placement coordinates file, organize in one list
        /// </summary>
        /// <returns> List full of data from placement coordinates file</returns>
        private List<MyDictionary> InitElementCoords()
        {
            List<MyDictionary> coords = new List<MyDictionary>();

            MyDictionary temp = null;

            using (var reader = new StreamReader(this.full_patch_Coords))
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
        /// Function for determinate if the current line is the last line 
        /// </summary>
        /// <returns> true if the line is the last and false otherwise </returns>
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
        /// Function for determinate if the current line contains coordinates of an element
        /// </summary>
        /// <returns> true if the line has coords in it and false otherwise </returns>
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
        /// Function for determinate if the current line contains element name
        /// </summary>
        /// <returns> true if the line has name in it and false otherwise </returns>
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
        /// Button save click event, saving all data 
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFile();

            MessageBox.Show("File Saved");

            DialogResult d = MessageBox.Show("Close Program?", "Alert!", MessageBoxButtons.YesNo);
            if (d == DialogResult.Yes)
            {
                System.Environment.Exit(1);
            }
        }
        /// <summary>
        /// Function for convorting all coords of elemt to one string
        /// </summary>
        /// <returns> one string that containing all the coords value </returns>
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
                        line += x.Value[j].GetString();
                    }
                }
            }
            return line;
        }
        /// <summary>
        /// Function for creating full patch for output file
        /// </summary>
        /// <returns> string with full patch frot the output file </returns>
        private string GetOutputPatch()
        {
            string patch = null;

            var x = this.full_patch_Coords.Split('/');

            for (int i = 0; i < x.Length-1; i++)
            {
                patch += x[i] + "/";
            }

            return (patch + "Output.csv");
        }
        /// <summary>
        /// Save the file in the computer after combining all the data
        /// </summary>
        private void SaveFile()
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

            File.AppendAllText(GetOutputPatch(), csv.ToString());
        }
    }
}
