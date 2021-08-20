using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Everything_Daily
{
    public class RecordType
    {
        public string Name { get; set; }
        public Color Color { get; set; }

        public RecordType(string Name, Color Color)
        {
            this.Name = Name;
            this.Color = Color;
        }
    }
}
