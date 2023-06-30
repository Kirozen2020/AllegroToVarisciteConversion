using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllegroToVarisciteConversion
{
    internal class MyDictionary
    {
        public string Key { get; }
        public List<Coords> Value { get; }

        public MyDictionary(string key) {
            this.Key = key;
            this.Value = new List<Coords>();
        }

        public void AddValue(double x, double y)
        {
            Coords c = new Coords(x, y);
            this.Value.Add(c);
        }
    }
}
