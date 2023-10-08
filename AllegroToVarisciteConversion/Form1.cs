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

            infoToolStripMenuItem.CheckState = CheckState.Checked;
            LogManager.Init(LogManager.LogLevel.Informational,LogManager.LogLevel.Error);
        }

        /*----------------- Variables ------------------*/

        /// <summary>
        /// Gets or sets the position manager.
        /// </summary>
        /// <value>
        /// The position manager.
        /// </value>
        private PositionManager positionManager { get; set; }
        /// <summary>
        /// Gets or sets the polygon manager.
        /// </summary>
        /// <value>
        /// The polygon manager.
        /// </value>
        private PolygonManager polygonManager { get; set; }
        /// <summary>
        /// Gets or sets the CSV manager.
        /// </summary>
        /// <value>
        /// The CSV manager.
        /// </value>
        private CsvManager csvManager { get; set; }
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
        /// The last clicked item
        /// </summary>
        private ToolStripMenuItem lastClickedItem;


        /*----------------- Click events ------------------*/

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

            if (this.placementReportPath != null)
            {
                this.positionManager = new PositionManager(this.placementReportPath);
            }

            if (this.positionManager.GetCoords() != null)
            {
                if (this.positionManager.GetCoords().Count > 0)
                {
                    if (this.polygonManager != null)
                    {
                        if (this.polygonManager.GetTable() != null)
                        {
                            MessageBox.Show(GetEmptyRefDeses(), "Empty RefDeses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    this.positionManager.MoveAllElements();

                    this.positionManager.DeleteUnnecessaryCoords();

                    this.positionManager.FlipImage();

                    List<List<Point3D>> lst = ConvertToListOfListOfPoints();

                    ImageManager img = new ImageManager(this.positionManager.GetCoords(), lst);
                    pbSketch.Image = img.image;
                }
                FillListBoxWithNames();
            }

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
                this.placementCoordinatesPath = openFileDialog.FileName;
                this.filePath = GetLastFilePath(openFileDialog.FileName);
            }

            if (this.placementCoordinatesPath != null)
            {
                this.polygonManager = new PolygonManager(this.placementCoordinatesPath);
            }

            if (this.polygonManager.GetTable() != null && this.positionManager.GetCoords() != null)
            {
                if (this.polygonManager.GetTable().Count > 0 && this.positionManager.GetCoords().Count > 0)
                {
                    MessageBox.Show(GetEmptyRefDeses(), "Empty RefDeses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }



        }
        /// <summary>
        /// Handles the Click event of the exitToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult ans = MessageBox.Show("Close Program?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans == DialogResult.Yes)
            {
                System.Environment.Exit(1);
            }
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
            if (this.filePath != null)
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
            if (savePath != null)
            {
                if (this.placementCoordinatesPath != null && this.placementReportPath != null)
                {
                    this.csvManager = new CsvManager(savePath, ChangeFileExtension(savePath, ".log"), this.polygonManager.GetTable(), this.positionManager.GetCoords());
                    this.csvManager.SaveFile();
                    MessageBox.Show("File Saved", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (this.placementReportPath != null && this.placementCoordinatesPath == null)
                {
                    DialogResult ans = MessageBox.Show("You want to create output file using only Placement Report file?", "Saving process", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (ans == DialogResult.OK)
                    {
                        this.csvManager = new CsvManager(savePath, ChangeFileExtension(savePath, ".log"), this.positionManager.GetCoords());
                        this.csvManager.SaveFileUsingOneFile();
                        MessageBox.Show("File saved", "Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else if (this.placementCoordinatesPath == null && this.placementReportPath == null)
                {
                    MessageBox.Show("You need to chose files first!", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }
        /// <summary>
        /// Logs the mode click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void LogModeClickEvent(object sender, EventArgs e)
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

            switch (item.Text)
            {
                case "Emergency":
                    LogManager.Init(LogManager.LogLevel.Emergency, LogManager.LogLevel.Error);
                    break;
                case "Alert":
                    LogManager.Init(LogManager.LogLevel.Alert, LogManager.LogLevel.Error);
                    break;
                case "Critical":
                    LogManager.Init(LogManager.LogLevel.Critical, LogManager.LogLevel.Error);
                    break;
                case "Error":
                    LogManager.Init(LogManager.LogLevel.Error, LogManager.LogLevel.Error);
                    break;
                case "Warning":
                    LogManager.Init(LogManager.LogLevel.Warning, LogManager.LogLevel.Error);
                    break;
                case "Notice":
                    LogManager.Init(LogManager.LogLevel.Notice, LogManager.LogLevel.Error);
                    break;
                case "Informational":
                    LogManager.Init(LogManager.LogLevel.Informational, LogManager.LogLevel.Error);
                    break;
                case "Debug":
                    LogManager.Init(LogManager.LogLevel.Debug, LogManager.LogLevel.Error);
                    break;
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

            if (this.positionManager.GetCoords().Count > 1)
            {
                for (int i = 0; i < this.positionManager.GetCoords().Count; i++)
                {
                    allNames.Add(this.positionManager.GetCoords()[i].Key);
                }
            }

            List<string> checkedCheckBoxNames = new List<string>();

            foreach (var selectedItem in listBox1.SelectedItems)
            {
                checkedCheckBoxNames.Add(selectedItem.ToString());
            }

            List<List<Point3D>> lst = ConvertToListOfListOfPoints();
            List<List<Point3D>> redElements = ConvertToListOfListOfPointsRED(checkedCheckBoxNames);

            ImageManager img = new ImageManager(this.positionManager.GetCoords(), lst, redElements);
            pbSketch.Image = img.image;
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
            for (int i = 0; i < this.positionManager.GetCoords().Count; i++)
            {
                if (this.positionManager.GetCoords()[i].Key.Equals(name))
                {
                    MyDictionary x = this.positionManager.GetCoords()[i];
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
        /// Gets the empty reference deses.
        /// </summary>
        /// <returns></returns>
        private string GetEmptyRefDeses()
        {
            List<string> names = new List<string>();
            List<string[]> allnames = this.polygonManager.GetTable();
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

            for (int i = 0; i < this.positionManager.GetCoords().Count; i++)
            {
                lst.Add(this.positionManager.GetCoords()[i].Value);
            }

            return lst;
        }
        /// <summary>
        /// Fills the ListBox with names.
        /// </summary>
        private void FillListBoxWithNames()
        {
            listBox1.DataSource = this.positionManager.GetNames();
            listBox1.Refresh();
            listBox1.SelectionMode = SelectionMode.MultiExtended;
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

            for (int i = 0; i < this.positionManager.GetCoords().Count; i++)
            {
                if (NameInList(this.positionManager.GetCoords()[i].Key, names))
                {
                    lst.Add(this.positionManager.GetCoords()[i].Value);
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
        }    }
}
