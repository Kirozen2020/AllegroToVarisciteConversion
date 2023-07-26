using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllegroToVarisciteConversion
{
    internal class Arc
    {
        public Arc(bool isClockWise, Point center, Point endPoint, Point startPoint)
        {
            this.isClockWise = isClockWise;
            this.center = center;
            this.endPoint = endPoint;
            this.startPoint = startPoint;
        }

        public bool isClockWise { get; }
        public Point center { get; }
        public Point endPoint { get; }
        public Point startPoint { get; }
    }
}
