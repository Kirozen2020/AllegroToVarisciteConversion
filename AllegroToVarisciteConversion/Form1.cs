using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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

        public string full_patch_Place;
        public string full_patch_Coords;
        private List<MyDictionary> coords;

        private void btnOpenPlace_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.full_patch_Place = openFileDialog1.FileName;
            }
        }

        private void btnOpenCoords_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.full_patch_Coords = openFileDialog1.FileName;
            }

            this.coords = InitElementCoords();
        }
        
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

                        //Console.WriteLine($"({t1},{t2}) ({t3},{t4})");
                        temp.AddValue(int.Parse(t1), int.Parse(t2));
                        //temp.AddValue(int.Parse(t3), int.Parse(t4));

                    }
                }
            }
            coords.RemoveAt(0);
            return coords;
        }
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
        /*
        public static List<string> ConverToList(string[] arr)
        {
            List<string> strings = new List<string>();
            for (int i = 0; i < arr.Length; i++)
            {
                strings.Add(arr[i]);
            }

            strings.RemoveAll(string.IsNullOrEmpty);
            

            return strings;
        }*/
        /*
        public static bool EndOfItem(string[] line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Equals("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~"))
                {
                    return true;
                }
            }
            return false;
        }*/

        public static bool HasCoords(string[] line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Equals("segment:xy"))
                    return true;
            }
            return false;
        }
        public static bool HasName(string[] line)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Equals("RefDes:"))
                    return true;
            }
            return false;
        }
        /*
        private bool IsNameInList(string name, List<MyDictionary> coords)
        {
            for (int i = 0; i < coords.Count; i++)
            {
                if (coords[i].Key.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }*/

        private void btnSave_Click(object sender, EventArgs e)
        {
            

            MessageBox.Show("File Saved");
        }
    }
}
