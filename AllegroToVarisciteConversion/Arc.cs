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
        public Arc()
        {
            
        }
        public bool isClockWise { get; set; }
        public Point center { get; set; }
        public Point endPoint { get; set; }
        public Point startPoint { get; set; }
    }
}
