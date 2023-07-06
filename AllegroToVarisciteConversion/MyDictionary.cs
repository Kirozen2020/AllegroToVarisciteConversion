using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllegroToVarisciteConversion
{
    internal class MyDictionary
    {
        public string Key { get; }
        public List<Point> Value { get; }

        public MyDictionary(string key) {
            this.Key = key;
            this.Value = new List<Point>();
        }

        public void AddValue(double x, double y)
        {
            Point p = new Point((int)x, (int)y);
            this.Value.Add(p);
        }
    }
}
