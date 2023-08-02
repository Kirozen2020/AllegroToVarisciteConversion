using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllegroToVarisciteConversion
{
    internal class Point3D
    {
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Point3D"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Point3D(int x, int y)
        {
            this.X = x.ToString();
            this.Y = y.ToString();
            this.Z = null;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Point3D"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public Point3D(int x, int y, int z) : this(x, y)
        {
            this.Z = z.ToString();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Point3D"/> class.
        /// </summary>
        public Point3D()
        {
        }
        /// <summary>
        /// Determines whether [is regular point].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is regular point]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRegularPoint()
        {
            return (this.Z == null);
        }
        /// <summary>
        /// Gets the regular point.
        /// </summary>
        /// <returns></returns>
        public Point GetRegularPoint()
        {
            return new Point(int.Parse(this.X), int.Parse(this.Y));
        }


    }
}
