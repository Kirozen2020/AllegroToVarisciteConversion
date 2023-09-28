using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AllegroToVarisciteConversion
{
    internal class CsvManager
    {
        /*----------------- Variables ------------------*/

        /// <summary>
        /// Gets or sets the CSV path.
        /// </summary>
        /// <value>
        /// The CSV path.
        /// </value>
        private string csv_path {  get; set; }
        /// <summary>
        /// Gets or sets the log path.
        /// </summary>
        /// <value>
        /// The log path.
        /// </value>
        private string log_path { get; set; }
        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>
        /// The table.
        /// </value>
        private List<string[]> table {  get; set; }
        /// <summary>
        /// Gets or sets the coords.
        /// </summary>
        /// <value>
        /// The coords.
        /// </value>
        private List<MyDictionary> coords { get; set; }

        /*----------------- Class constructor ------------------*/

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvManager"/> class.
        /// </summary>
        /// <param name="csv_path">The CSV path.</param>
        /// <param name="log_path">The log path.</param>
        /// <param name="table">The table.</param>
        /// <param name="coords">The coords.</param>
        /// <param name="log">The log.</param>
        public CsvManager(string csv_path, string log_path, List<string[]> table, List<MyDictionary> coords)
        {
            this.csv_path = csv_path;
            this.log_path = log_path;
            this.table = table;
            this.coords = coords;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvManager"/> class.
        /// </summary>
        /// <param name="csv_path">The CSV path.</param>
        /// <param name="log_path">The log path.</param>
        /// <param name="coords">The coords.</param>
        /// <param name="log">The log.</param>
        public CsvManager(string csv_path, string log_path, List<MyDictionary> coords)
        {
            this.csv_path = csv_path;
            this.log_path = log_path;
            this.coords = coords;
        }

        /*----------------- Main functions ------------------*/

        /// <summary>
        /// Saves the file.
        /// </summary>
        /// <param name="outputString">The output string.</param>
        /// <param name="outputLogPath">The output log Path.</param>
        public void SaveFile()
        {
            StringBuilder csv = new StringBuilder();
            string line = "";
            for (int i = 0; i < this.table[0].Length; i++)
            {
                line += this.table[0][i].ToString() + ",";
            }
            csv.AppendLine(line + "DXF");

            this.table.Sort((x,y) => CompareByFirstElement(x,y));

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

            File.AppendAllText(csv_path, csv.ToString());

            LogManager.SaveLogToFile(log_path);
        }
        /// <summary>
        /// Saves the file using one file.
        /// </summary>
        /// <param name="outputString">The output string.</param>
        /// <param name="outputLogPath">The output log path.</param>
        public void SaveFileUsingOneFile()
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine("REFDES,SYM_X,SYM_Y,SYM_ROTATE,SYM_MIRROR,DFX");//Top line
            string line = "";

            this.coords.Sort((x, y) => CompareKeys(x.Key, y.Key));

            for (int i = 0; i < this.coords.Count; i++)
            {
                line = $"{this.coords[i].Key},0,0,0,{this.coords[i].Mirror},";
                line += GetCoords(this.coords[i].Key);
                csv.AppendLine(line);
            }

            File.AppendAllText(csv_path, csv.ToString());//Save file to pc
            LogManager.SaveLogToFile(log_path);
        }

        /*----------------- Help functions ------------------*/

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
        /// Compares the by first element.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private int CompareByFirstElement(string[] x, string[] y)
        {
            return NaturalCompare(x[0], y[0]);
        }
        /// <summary>
        /// Compares the keys.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private int CompareKeys(string x, string y)
        {
            return NaturalCompare(x, y);
        }
        /// <summary>
        /// Naturals the compare.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        static int NaturalCompare(string x, string y)
        {
            string[] xParts = Regex.Split(x, @"(\d+)");
            string[] yParts = Regex.Split(y, @"(\d+)");

            for (int i = 0; i < Math.Min(xParts.Length, yParts.Length); i++)
            {
                if (i % 2 != 0)
                {
                    int xValue = int.Parse(xParts[i]);
                    int yValue = int.Parse(yParts[i]);

                    int result = xValue.CompareTo(yValue);

                    if (result != 0)
                    {
                        return result;
                    }
                }
                else
                {
                    int result = xParts[i].CompareTo(yParts[i]);

                    if (result != 0)
                    {
                        return result;
                    }
                }
            }

            return xParts.Length.CompareTo(yParts.Length);
        }
    }
}
