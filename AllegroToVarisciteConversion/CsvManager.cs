using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
        private string CsvPath {  get; }
        /// <summary>
        /// Gets or sets the log path.
        /// </summary>
        /// <value>
        /// The log path.
        /// </value>
        private string LogPath { get; }
        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>
        /// The table.
        /// </value>
        private List<string[]> Table {  get; }
        /// <summary>
        /// Gets or sets the coords.
        /// </summary>
        /// <value>
        /// The coords.
        /// </value>
        private List<MyDictionary> Coords { get; }

        /*----------------- Class constructor ------------------*/

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvManager"/> class.
        /// </summary>
        /// <param name="csvPath">The CSV path.</param>
        /// <param name="logPath">The log path.</param>
        /// <param name="table">The table.</param>
        /// <param name="coords">The coords.</param>
        public CsvManager(string csvPath, string logPath, List<string[]> table, List<MyDictionary> coords)
        {
            this.CsvPath = csvPath;
            this.LogPath = logPath;
            this.Table = table;
            this.Coords = coords;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvManager"/> class.
        /// </summary>
        /// <param name="csvPath">The CSV path.</param>
        /// <param name="logPath">The log path.</param>
        /// <param name="coords">The coords.</param>
        public CsvManager(string csvPath, string logPath, List<MyDictionary> coords)
        {
            this.CsvPath = csvPath;
            this.LogPath = logPath;
            this.Coords = coords;
        }

        /*----------------- Main functions ------------------*/

        /// <summary>
        /// Saves the file.
        /// </summary>
        public void SaveFile()
        {
            var csv = new StringBuilder();
            var line = "";
            for (var i = 0; i < this.Table[0].Length; i++)
            {
                line += this.Table[0][i] + ",";
            }
            csv.AppendLine(line + "DXF");

            this.Table.Sort(CompareByFirstElement);

            for (var i = 1; i < this.Table.Count; i++)
            {
                line = "";
                for (var j = 0; j < this.Table[i].Length; j++)
                {
                    line += this.Table[i][j] + ",";
                }
                line += GetCoords(this.Table[i][0]);
                csv.AppendLine(line);
            }

            File.AppendAllText(CsvPath, csv.ToString());

            LogManager.SaveLogToFile(LogPath);
        }
        /// <summary>
        /// Saves the file using one file.
        /// </summary>
        public void SaveFileUsingOneFile()
        {
            var csv = new StringBuilder();
            csv.AppendLine("REFDES,SYM_X,SYM_Y,SYM_ROTATE,SYM_MIRROR,DFX");//Top line

            this.Coords.Sort((x, y) => CompareKeys(x.Key, y.Key));

            foreach (var line in from value in this.Coords let line = $"{value.Key},0,0,0,{value.Mirror}," select line + GetCoords(value.Key))
            {
                csv.AppendLine(line);
            }

            File.AppendAllText(CsvPath, csv.ToString());//Save file to pc
            LogManager.SaveLogToFile(LogPath);
        }

        /*----------------- Help functions ------------------*/

        /// <summary>
        /// Gets the coords.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private string GetCoords(string name)
        {
            var line = "";
            for (var i = 0; i < this.Coords.Count; i++)
            {
                if (!Coords[i].Key.Equals(name)) continue;
                var x = Coords[i];
                line += ":";
                foreach (var point in x.Value)
                {
                    if (!point.IsRegularPoint())
                    {
                        line += $"[{point.X};{point.Y};{point.Z}]";
                    }
                    else
                    {
                        line += $"[{point.X};{point.Y}]";
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
        private static int CompareByFirstElement(IReadOnlyList<string> x, IReadOnlyList<string> y)
        {
            return NaturalCompare(x[0], y[0]);
        }
        /// <summary>
        /// Compares the keys.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private static int CompareKeys(string x, string y)
        {
            return NaturalCompare(x, y);
        }
        /// <summary>
        /// Naturals the compare.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        private static int NaturalCompare(string x, string y)
        {
            var xParts = Regex.Split(x, @"(\d+)");
            var yParts = Regex.Split(y, @"(\d+)");

            for (var i = 0; i < Math.Min(xParts.Length, yParts.Length); i++)
            {
                if (i % 2 != 0)
                {
                    var xValue = int.Parse(xParts[i]);
                    var yValue = int.Parse(yParts[i]);

                    var result = xValue.CompareTo(yValue);

                    if (result != 0)
                    {
                        return result;
                    }
                }
                else
                {
                    var result = string.Compare(xParts[i], yParts[i], StringComparison.Ordinal);

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
