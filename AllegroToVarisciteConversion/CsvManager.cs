using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllegroToVarisciteConversion
{
    internal class CsvManager
    {
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
        /// <summary>
        /// Gets or sets the log mode.
        /// </summary>
        /// <value>
        /// The log mode.
        /// </value>
        private string logMode { get; set; }
        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        private LogManager log {  get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvManager"/> class.
        /// </summary>
        /// <param name="csv_path">The CSV path.</param>
        /// <param name="log_path">The log path.</param>
        /// <param name="table">The table.</param>
        /// <param name="coords">The coords.</param>
        /// <param name="logMode">The log mode.</param>
        /// <param name="log">The log.</param>
        public CsvManager(string csv_path, string log_path, List<string[]> table, List<MyDictionary> coords, string logMode, LogManager log)
        {
            this.csv_path = csv_path;
            this.log_path = log_path;
            this.table = table;
            this.coords = coords;
            this.logMode = logMode;
            this.log = log;
        }


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

            switch (logMode)
            {
                case "error":

                    //this.logTextGlobal.Append(this.logTextErrorPlacementCoordinates.ToString());
                    //this.logTextGlobal.Append(this.logTextErrorPlacementReport.ToString());
                    //this.logTextGlobal.AppendLine("\nErrors count: " + this.errorCount);
                    //this.logTextGlobal.AppendLine("\nFile saved at " + outputString);

                    //File.WriteAllText(outputLogPath, this.logTextGlobal.ToString());
                    File.WriteAllText(log_path, this.log.GetFullLogMessage(2, log_path).ToString());
                    break;
                case "info":
                    //this.logTextGlobal.Append(this.logTextInfoPlacementCoordinates.ToString());
                    //this.logTextGlobal.AppendLine();
                    //this.logTextGlobal.Append(this.logTextInfoPlacementReport.ToString());
                    //this.logTextGlobal.AppendLine("\nErrors count: " + this.errorCount);
                    //this.logTextGlobal.AppendLine("\nFile saved at " + outputString);

                    //File.WriteAllText(outputLogPath, this.logTextGlobal.ToString());
                    File.WriteAllText(log_path, this.log.GetFullLogMessage(0, log_path).ToString());
                    break;
                case "debug":
                    //this.logTextGlobal.Append(this.logTextDebugPlacementCoordinates.ToString());
                    //this.logTextGlobal.AppendLine();
                    //this.logTextGlobal.Append(this.logTextDebugPlacementReport.ToString());
                    //this.logTextGlobal.AppendLine("\nErrors count: " + this.errorCount);
                    //this.logTextGlobal.AppendLine("\nFile saved at " + outputString);

                    //File.WriteAllText(outputLogPath, this.logTextGlobal.ToString());
                    File.WriteAllText(log_path, this.log.GetFullLogMessage(1, log_path).ToString());
                    break;
            }
            //CleanLogPlacementCoordinates();
            //CleanLogPlacementReport();
            this.log.ClearCoordsLog();
            this.log.ClearPlacementLog();
            this.log.error_counter = 0;
            //this.errorCount = 0;
            //this.filePath = null;
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

            for (int i = 0; i < this.coords.Count; i++)
            {
                line = $"{this.coords[i].Key},0,0,0,{this.coords[i].Mirror},";
                line += GetCoords(this.coords[i].Key);
                csv.AppendLine(line);
            }

            File.AppendAllText(csv_path, csv.ToString());//Save file to pc

            switch (logMode)
            {
                case "error":

                    //this.logTextGlobal.Append(this.logTextErrorPlacementCoordinates.ToString());
                    //this.logTextGlobal.Append(this.logTextErrorPlacementReport.ToString());
                    //this.logTextGlobal.AppendLine("\nErrors count: " + this.errorCount);
                    //this.logTextGlobal.AppendLine("\nFile saved at " + outputString);

                    //File.WriteAllText(outputLogPath, this.logTextGlobal.ToString());
                    File.WriteAllText(log_path, this.log.GetFullLogMessage(2, log_path).ToString());
                    break;
                case "info":
                    //this.logTextGlobal.Append(this.logTextInfoPlacementCoordinates.ToString());
                    //this.logTextGlobal.AppendLine();
                    //this.logTextGlobal.Append(this.logTextInfoPlacementReport.ToString());
                    //this.logTextGlobal.AppendLine("\nErrors count: " + this.errorCount);
                    //this.logTextGlobal.AppendLine("\nFile saved at " + outputString);

                    //File.WriteAllText(outputLogPath, this.logTextGlobal.ToString());
                    File.WriteAllText(log_path, this.log.GetFullLogMessage(0, log_path).ToString());
                    break;
                case "debug":
                    //this.logTextGlobal.Append(this.logTextDebugPlacementCoordinates.ToString());
                    //this.logTextGlobal.AppendLine();
                    //this.logTextGlobal.Append(this.logTextDebugPlacementReport.ToString());
                    //this.logTextGlobal.AppendLine("\nErrors count: " + this.errorCount);
                    //this.logTextGlobal.AppendLine("\nFile saved at " + outputString);

                    //File.WriteAllText(outputLogPath, this.logTextGlobal.ToString());
                    File.WriteAllText(log_path, this.log.GetFullLogMessage(1, log_path).ToString());
                    break;
            }
            //CleanLogPlacementCoordinates();
            //CleanLogPlacementReport();
            this.log.ClearCoordsLog();
            this.log.ClearPlacementLog();
            this.log.error_counter = 0;
            //this.errorCount = 0;
            //this.filePath = null;
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
    }
}
