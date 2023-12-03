using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AllegroToVarisciteConversion
{
    internal class PolygonManager
    {
        /*----------------- Variables ------------------*/

        /// <summary>
        /// Gets or sets the full path to placement coordinates file.
        /// </summary>
        /// <value>
        /// The full path to placement coordinates file.
        /// </value>
        public string FullPathToPlacementCoordinatesFile {  get; set; }
        /// <summary>
        /// The table
        /// </summary>
        private readonly List<string[]> _table;

        /*----------------- Class constructor ------------------*/

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonManager"/> class.
        /// </summary>
        /// <param name="fullPathToPlacementCoordinatesFile">The full path to placement coordinates file.</param>
        public PolygonManager(string fullPathToPlacementCoordinatesFile)
        {
            this.FullPathToPlacementCoordinatesFile = fullPathToPlacementCoordinatesFile;
            this._table = InitTable();
        }

        /*----------------- Main functions ------------------*/

        /// <summary>
        /// Initializes the tabel.
        /// </summary>
        /// <returns></returns>
        private List<string[]> InitTable()
        {
            LogManager.AddCommentLine(LogManager.LogLevel.Informational, "Opening coordinates file: "+ FullPathToPlacementCoordinatesFile);
            LogManager.AddCommentLine(LogManager.LogLevel.Informational, "Start converting coordinates file to List<string[]> format");

            var table = new List<string[]>();
            var index = 0;
            using (var reader = new StreamReader(this.FullPathToPlacementCoordinatesFile))
            {
                while (reader.EndOfStream == false)
                {
                    var line = reader.ReadLine()?.Split('!');

                    LogManager.AddComment(LogManager.LogLevel.Informational,
                        "Reading line " + index + ": {" + ConvertListToString(line, index) + "} ");

                    if (line != null)
                        for (var i = 0; i < line.Length; i++)
                        {
                            line[i] = RemoveWhiteSpaces(line[i]);
                        }

                    table.Add(line);
                    try
                    {
                        switch (index)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                LogManager.AddCommentLine(LogManager.LogLevel.Informational, " Dumping line");
                                break;
                            case 4:
                                LogManager.AddCommentLine(LogManager.LogLevel.Informational, 
                                    "Getting Refdes is in column 0, X coordinate in column 1, Y coordinate in column 2");
                                break;
                            case 5:
                                LogManager.AddCommentLine(LogManager.LogLevel.Informational, " Dumping line");
                                break;
                        }
                        if (index >= 6)
                        {
                            if (line != null && CanConvertToNumeric(line[1]) == false)//error
                            {
                                LogManager.AddCommentLine(LogManager.LogLevel.Error, 
                                    "ERROR!!! Cannot parse line " + index + 
                                    "Refdes " + line[0] + 
                                    " coordinate X has incorrect string value {" + line[1] + "}");
                            }
                            else if (line != null && CanConvertToNumeric(line[2]) == false)//error
                            {
                                LogManager.AddCommentLine(LogManager.LogLevel.Error,
                                    "ERROR!!! Cannot parse line " + index +
                                    "Refdes " + line[0] +
                                    " coordinate Y has incorrect string value {" + line[2] + "}");
                            }
                            else
                            {
                                if (line != null)
                                    LogManager.AddCommentLine(LogManager.LogLevel.Informational,
                                        $"Refdes {line[0]} is located at {line[1]},{line[2]}");
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show(@"The file you opening is invalid or empty\nPlease choose another file", @"Attention!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }
                    index++;
                }
            }
            if (table.Count > 0)
            {
                table.RemoveAt(0);
                table.RemoveAt(0);
                table.RemoveAt(0);

                table.RemoveAt(1);
            }
            else
            {
                MessageBox.Show(@"The file you opening is invalid or empty\nPlease choose another file", @"Attention!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return table;
        }

        /*----------------- Help functions ------------------*/

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <returns></returns>
        public List<string[]> GetTable()
        {
            return _table;
        }
        /// <summary>
        /// Removes the white spaces.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        private static string RemoveWhiteSpaces(string line)
        {
            return new string(line.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());
        }
        /// <summary>
        /// Converts the list to string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static string ConvertListToString(IEnumerable<string> str, int index)
        {
            var line = "";
            foreach (var value in str)
            {
                if (index == 0 || index == 1 || index == 2 || index == 4)
                {
                    line += value + " ";
                }
                else
                {
                    line += "  " + value + "\t !";
                }
            }

            return line.Substring(0, line.Length - 1);
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
