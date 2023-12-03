using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            this.Text = @"AllegroToVarisciteConversion " + Revision.revision;

            infoToolStripMenuItem.CheckState = CheckState.Checked;
            LogManager.Init(LogManager.LogLevel.Informational,LogManager.LogLevel.Error);
        }
        /// <summary>
        /// Gets or sets the text associated with this control.
        /// </summary>
        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        /*----------------- Variables ------------------*/

        /// <summary>
        /// Gets or sets the position manager.
        /// </summary>
        /// <value>
        /// The position manager.
        /// </value>
        private PositionManager PositionManager { get; set; }
        /// <summary>
        /// Gets or sets the polygon manager.
        /// </summary>
        /// <value>
        /// The polygon manager.
        /// </value>
        private PolygonManager PolygonManager { get; set; }
        /// <summary>
        /// Gets or sets the CSV manager.
        /// </summary>
        /// <value>
        /// The CSV manager.
        /// </value>
        private CsvManager CsvManager { get; set; }
        /// <summary>
        /// The file Path
        /// </summary>
        public string FilePath;
        /// <summary>
        /// The full Path place
        /// </summary>
        public string PlacementReportPath;
        /// <summary>
        /// The full Path coords
        /// </summary>
        public string PlacementCoordinatesPath;
        /// <summary>
        /// The last clicked item
        /// </summary>
        private ToolStripMenuItem _lastClickedItem;


        /*----------------- Click events ------------------*/

        /// <summary>
        /// Handles the Click event of the placementReportFileToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void PlacementReportFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = @"Text File (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.Title = @"Open Placement Report File...";
            openFileDialog.InitialDirectory = this.FilePath ?? @"C:\";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.PlacementReportPath = openFileDialog.FileName;
                this.FilePath = GetLastFilePath(openFileDialog.FileName);
            }

            if (this.PlacementReportPath != null)
            {
                this.PositionManager = new PositionManager(this.PlacementReportPath);
            }

            if (this.PositionManager.GetCoords() == null) return;
            if (this.PositionManager.GetCoords().Count > 0)
            {
                if (this.PolygonManager?.GetTable() != null)
                {
                    MessageBox.Show(GetEmptyRefDes(), @"Empty RefDes's", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.PositionManager.MoveAllElements();

                this.PositionManager.DeleteUnnecessaryCoords();

                this.PositionManager.FlipImage();

                var lst = ConvertToListOfListOfPoints();

                var img = new ImageManager(this.PositionManager.GetCoords(), lst);
                pbSketch.Image = img.Image;
            }
            FillListBoxWithNames();

        }
        /// <summary>
        /// Handles the Click event of the coordinatesReportFileToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void CoordinatesReportFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = @"Text File (*.txt)|*.txt|All Files (*.*)|*.*";
            openFileDialog.Title = @"Open Placement Coordinates File...";
            openFileDialog.InitialDirectory = this.FilePath ?? @"C:\";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.PlacementCoordinatesPath = openFileDialog.FileName;
                this.FilePath = GetLastFilePath(openFileDialog.FileName);
            }

            if (this.PlacementCoordinatesPath != null)
            {
                this.PolygonManager = new PolygonManager(this.PlacementCoordinatesPath);
            }

            if (this.PolygonManager.GetTable() == null || this.PositionManager.GetCoords() == null) return;
            if (this.PolygonManager.GetTable().Count > 0 && this.PositionManager.GetCoords().Count > 0)
            {
                MessageBox.Show(GetEmptyRefDes(), @"Empty RefDes's", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }



        }
        /// <summary>
        /// Handles the Click event of the exitToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ans = MessageBox.Show(@"Close Program?", @"Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ans == DialogResult.Yes)
            {
                Environment.Exit(1);
            }
        }
        /// <summary>
        /// Handles the Click event of the saveAsToolStripMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string savePath = null;
            var save = new SaveFileDialog();
            save.Filter = @"CSV File (*.csv)|*.csv|All Files (*.*)|*.*";
            save.Title = @"Save output file in...";
            save.DefaultExt = "csv";
            save.InitialDirectory = this.FilePath ?? @"C:\";
            if (save.ShowDialog() == DialogResult.OK)
            {
                savePath = save.FileName;
            }

            if (savePath == null) return;
            if (this.PlacementCoordinatesPath != null && this.PlacementReportPath != null)
            {
                this.CsvManager = new CsvManager(savePath, ChangeFileExtension(savePath, ".log"), this.PolygonManager.GetTable(), this.PositionManager.GetCoords());
                this.CsvManager.SaveFile();
                MessageBox.Show(@"File Saved", @"Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (this.PlacementReportPath != null && this.PlacementCoordinatesPath == null)
            {
                var ans = MessageBox.Show(@"You want to create output file using only Placement Report file?", @"Saving process", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (ans != DialogResult.OK) return;
                this.CsvManager = new CsvManager(savePath, ChangeFileExtension(savePath, ".log"), this.PositionManager.GetCoords());
                this.CsvManager.SaveFileUsingOneFile();
                MessageBox.Show(@"File saved", @"Congratulations", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (this.PlacementCoordinatesPath == null && this.PlacementReportPath == null)
            {
                MessageBox.Show(@"You need to chose files first!", @"Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        /// <summary>
        /// Logs the mode click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void LogModeClickEvent(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            UncheckAllMenuItems(menuStrip1.Items);
            item.Checked = !item.Checked;

            if (item.Checked)
            {
                item.CheckState = CheckState.Checked;
            }
            if (_lastClickedItem != null && _lastClickedItem != item)
            {
                _lastClickedItem.Checked = false;
                _lastClickedItem.CheckState = CheckState.Unchecked;
            }
            _lastClickedItem = item;

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
        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var allNames = new List<string>();

            if (this.PositionManager.GetCoords().Count > 1)
            {
                for (var i = 0; i < this.PositionManager.GetCoords().Count; i++)
                {
                    allNames.Add(this.PositionManager.GetCoords()[i].Key);
                }
            }

            var checkedCheckBoxNames = (from object selectedItem in listBox1.SelectedItems select selectedItem.ToString()).ToList();

            var lst = ConvertToListOfListOfPoints();
            var redElements = ConvertToListOfListOfPointsRed(checkedCheckBoxNames);

            var img = new ImageManager(this.PositionManager.GetCoords(), lst, redElements);
            pbSketch.Image = img.Image;
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
            for (var i = 0; i < this.PositionManager.GetCoords().Count; i++)
            {
                if (!this.PositionManager.GetCoords()[i].Key.Equals(name)) continue;
                var x = this.PositionManager.GetCoords()[i];
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
        /// Gets the empty reference Des's.
        /// </summary>
        /// <returns></returns>
        private string GetEmptyRefDes()
        {
            var names = new List<string>();
            var allNames = this.PolygonManager.GetTable();
            var result = "RefDes's that are not visible on the diagram:\n";

            for (var i = 1; i < allNames.Count; i++)
            {
                if (GetCoords(allNames[i][0]) == "")
                {
                    names.Add(allNames[i][0]);
                }
            }

            if(names.Count == 0)
            {
                return "there is no empty RefDes's";
            }
            else
            {
                result = names.Aggregate(result, (current, name) => current + (name + ", "));
            }

            return result.Remove(result.Length - 2);
        }
        /// <summary>
        /// Converts to list of points.
        /// </summary>
        /// <returns></returns>
        private List<List<Point3D>> ConvertToListOfListOfPoints()
        {
           var lst = new List<List<Point3D>>();

            for (var i = 0; i < this.PositionManager.GetCoords().Count; i++)
            {
                lst.Add(this.PositionManager.GetCoords()[i].Value);
            }

            return lst;
        }
        /// <summary>
        /// Fills the ListBox with names.
        /// </summary>
        private void FillListBoxWithNames()
        {
            listBox1.DataSource = this.PositionManager.GetNames();
            listBox1.Refresh();
            listBox1.SelectionMode = SelectionMode.MultiExtended;
        }
        /// <summary>
        /// Gets the last file Path.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private static string GetLastFilePath(string fileName)
        {
            string result = null;

            var split = fileName.Split('\\');

            for (var i = 0; i < split.Length-1; i++)
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
        private static string ChangeFileExtension(string filePath, string newExtension)
        {
            var directory = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            return directory != null ? Path.Combine(directory, fileName + newExtension) : null;
        }
        /// <summary>
        /// Unchecks all menu items.
        /// </summary>
        /// <param name="items">The items.</param>
        private static void UncheckAllMenuItems(ToolStripItemCollection items)
        {
            foreach (var item in items)
            {
                if (!(item is ToolStripMenuItem menuItem)) continue;
                menuItem.Checked = false;
                UncheckAllMenuItems(menuItem.DropDownItems);
            }
        }
        /// <summary>
        /// Converts to list of points red.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <returns></returns>
        private List<List<Point3D>> ConvertToListOfListOfPointsRed(IReadOnlyList<string> names)
        {
            var lst = new List<List<Point3D>>();

            for (var i = 0; i < this.PositionManager.GetCoords().Count; i++)
            {
                if (NameInList(this.PositionManager.GetCoords()[i].Key, names))
                {
                    lst.Add(this.PositionManager.GetCoords()[i].Value);
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
        private static bool NameInList(string key, IEnumerable<string> names)
        {
            return names.Any(name => name.Equals(key));
        }
    }
}
