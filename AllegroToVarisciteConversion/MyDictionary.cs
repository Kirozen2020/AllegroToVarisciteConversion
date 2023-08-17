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
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; }
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public List<Point3D> Value { get; }
        /// <summary>
        /// Gets the merror.
        /// </summary>
        /// <value>
        /// The merror.
        /// </value>
        public string Mirror { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MyDictionary"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public MyDictionary(string key) 
        {
            this.Key = key;
            this.Value = new List<Point3D>();
        }
        /// <summary>
        /// Adds the mirror.
        /// </summary>
        /// <param name="x">The x.</param>
        public void AddMirror(string x)
        {
            this.Mirror = x.ToString();
        }
        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public void AddValue(double x, double y)
        {
            Point3D p = new Point3D((int)x, (int)y);
            this.Value.Add(p);
        }
        /// <summary>
        /// Adds the value.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public void AddValue(double x, double y, double z)
        {
            Point3D p = new Point3D((int)x, (int)y, (int)z);
            this.Value.Add(p);
        }
    }
}
