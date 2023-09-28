using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string full_path_to_placement_coordinates_file {  get; set; }
        /// <summary>
        /// The table
        /// </summary>
        List<string[]> table;

        /*----------------- Class constructor ------------------*/

        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonManager"/> class.
        /// </summary>
        /// <param name="full_path_to_placement_coordinates_file">The full path to placement coordinates file.</param>
        public PolygonManager(string full_path_to_placement_coordinates_file)
        {
            this.full_path_to_placement_coordinates_file = full_path_to_placement_coordinates_file;
            this.table = InitTabel();
        }

        /*----------------- Main functions ------------------*/

        /// <summary>
        /// Initializes the tabel.
        /// </summary>
        /// <returns></returns>
        private List<string[]> InitTabel()
        {
            LogManager.AddCommentLine(LogManager.LogLevel.Informational, "Opening coordinates file: "+ full_path_to_placement_coordinates_file);
            LogManager.AddCommentLine(LogManager.LogLevel.Informational, "Start converting coordinates file to List<string[]> format");

            List<string[]> tabel = new List<string[]>();
            int index = 0;
            using (var reader = new StreamReader(this.full_path_to_placement_coordinates_file))
            {
                while (reader.EndOfStream == false)
                {
                    var line = reader.ReadLine().Split('!');

                    LogManager.AddComment(LogManager.LogLevel.Informational,
                        "Reading line " + index + ": {" + ConvertListToString(line, index) + "} ");

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
                            if (CanConvertToNumeric(line[1]) == false)//error
                            {
                                LogManager.AddCommentLine(LogManager.LogLevel.Error, 
                                    "ERROR!!! Cannot parse line " + index.ToString() + 
                                    "Refdes " + line[0] + 
                                    " coordinate X has incorrect string value {" + line[1] + "}");
                            }
                            else if (CanConvertToNumeric(line[2]) == false)//error
                            {
                                LogManager.AddCommentLine(LogManager.LogLevel.Error,
                                    "ERROR!!! Cannot parse line " + index.ToString() +
                                    "Refdes " + line[0] +
                                    " coordinate Y has incorrect string value {" + line[2] + "}");
                            }
                            else
                            {
                                LogManager.AddCommentLine(LogManager.LogLevel.Informational, 
                                    $"Refdes {line[0]} is located at {line[1]},{line[2]}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("The file you opening is invalid or empty\nPlease choose another file", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }
                    index++;
                }
            }
            if (tabel.Count > 0)
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

        /*----------------- Help functions ------------------*/

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <returns></returns>
        public List<string[]> GetTable()
        {
            return this.table;
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
                if (index == 0 || index == 1 || index == 2 || index == 4)
                {
                    line += str[i] + " ";
                }
                else
                {
                    line += "  " + str[i] + "\t !";
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
