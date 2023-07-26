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
        public List<Arc> Arcs { get; }

        public MyDictionary(string key) {
            this.Key = key;
            this.Value = new List<Point>();
            this.Arcs = new List<Arc>();
        }

        public void AddValue(double x, double y)
        {
            Point p = new Point((int)x, (int)y);
            this.Value.Add(p);
        }

        public void AddArc(Point center, Point start, Point end, bool isClockwise)
        {
            Arc arc = new Arc(isClockwise, center, end, start);
            this.Arcs.Add(arc);
        }

        public void AddArc(Arc arc)
        {
            if(arc != null)
            {
                Arcs.Add(arc);
            }
        }
    }
}
