using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllegroToVarisciteConversion
{
    internal class Coords
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coords(double x, double y) 
        {
            this.X = (int)x;
            this.Y = (int)y;
        }

        public override string ToString()
        {
            return $"[{this.X};{this.Y}]";
        }
    }
}
