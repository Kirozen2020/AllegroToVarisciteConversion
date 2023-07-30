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

        private List<string> namesList;
        public List<string> CheckedItemsList { get; private set; }

        public SelectElements(List<string> namesList)
        {
            InitializeComponent();
            this.namesList = namesList;

            // Create and add checkboxes based on the names list
            foreach (string name in namesList)
            {
                CheckBox checkBox = new CheckBox
                {
                    Text = name
                };

                // Add the checkbox to the FlowLayoutPanel
                flowLayoutPanel1.Controls.Add(checkBox);
            }
        }
        
        private void SelectElements_FormClosing(object sender, FormClosingEventArgs e)
        {
            CheckedItemsList.Clear();
            foreach (Control control in flowLayoutPanel1.Controls)
            {
                if (control is CheckBox checkBox && checkBox.Checked)
                {
                    CheckedItemsList.Add(checkBox.Text);
                }
            }
        }
    }
}
