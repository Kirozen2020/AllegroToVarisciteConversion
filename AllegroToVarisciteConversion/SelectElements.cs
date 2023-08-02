using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllegroToVarisciteConversion
{
    public partial class SelectElements : Form
    {
        /// <summary>
        /// The names list
        /// </summary>
        private List<string> namesList;
        /// <summary>
        /// Gets the checked items list.
        /// </summary>
        /// <value>
        /// The checked items list.
        /// </value>
        public List<string> CheckedItemsList{ get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectElements"/> class.
        /// </summary>
        /// <param name="namesList">The names list.</param>
        public SelectElements(List<string> namesList)
        {
            InitializeComponent();
            this.namesList = SortAlphabetically(namesList);
            tableLayoutPanel1.Controls.Clear();

            // Set up the TableLayoutPanel properties
            tableLayoutPanel1.ColumnCount = 4;
            int rowCount = (int)Math.Ceiling(this.namesList.Count / 4.0);
            tableLayoutPanel1.RowCount = rowCount;

            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoScroll = true;
            for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
            {
                tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }

            for (int i = 0; i < rowCount; i++)
            {
                tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }
            //Create and add checkboxes based on the names list
            for (int i = 0; i < this.namesList.Count; i++)
            {
                CheckBox checkBox = new CheckBox
                {
                    Text = this.namesList[i]
                };

                // Add the checkbox to the TableLayoutPanel
                tableLayoutPanel1.Controls.Add(checkBox, i % tableLayoutPanel1.ColumnCount, i / tableLayoutPanel1.ColumnCount);
            }

            // Initialize the CheckedItemsList
            CheckedItemsList = new List<string>();
        }

        /// <summary>
        /// Sorts the alphabetically.
        /// </summary>
        /// <param name="unsortedList">The unsorted list.</param>
        /// <returns></returns>
        static List<string> SortAlphabetically(List<string> unsortedList)
        {
            // Clone the original list to avoid modifying it directly
            List<string> sortedList = new List<string>(unsortedList);

            // Use the Sort method to sort the list alphabetically
            sortedList.Sort();

            return sortedList;
        }

        /// <summary>
        /// Handles the Click event of the button1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.CheckedItemsList = new List<string>();
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                if (control is CheckBox checkBox && checkBox.Checked)
                {
                    CheckedItemsList.Add(checkBox.Text);
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Handles the Click event of the button2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
